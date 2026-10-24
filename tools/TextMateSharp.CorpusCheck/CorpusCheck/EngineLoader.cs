using System.Reflection;
using System.Runtime.Loader;

namespace TextMateSharp.CorpusCheck;

// Loads an adapter (and the TextMateSharp it was built against) in its own AssemblyLoadContext, so the official
// TextMateSharp and this branch's, which share an assembly identity, can run side by side.
public static class EngineLoader
{
    public static Func<IEngine> Load(string adapterPath)
    {
        if (!File.Exists(adapterPath))
        {
            throw new FileNotFoundException($"Adapter not found; build the CorpusCheck project to publish it.", adapterPath);
        }

        var context = new AdapterLoadContext(adapterPath);
        Assembly assembly = context.LoadFromAssemblyPath(adapterPath);
        Type type = assembly.GetType("TextMateSharp.CorpusCheck.Engine", throwOnError: true)!;
        return () => (IEngine)Activator.CreateInstance(type)!;
    }

    private sealed class AdapterLoadContext(string adapterPath) : AssemblyLoadContext(Path.GetFileNameWithoutExtension(adapterPath))
    {
        private static readonly string ContractName = typeof(IEngine).Assembly.GetName().Name!;
        private readonly AssemblyDependencyResolver _resolver = new(adapterPath);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // The contract is shared with the checker; everything else (TextMateSharp, Onigwrap, GregsEx) is the adapter's own.
            if (assemblyName.Name == ContractName)
            {
                return null;
            }

            string? path = _resolver.ResolveAssemblyToPath(assemblyName);
            return path == null ? null : LoadFromAssemblyPath(path);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            // The official TextMateSharp's native onigwrap library, from runtimes/<rid>/native.
            string? path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            return path == null ? IntPtr.Zero : LoadUnmanagedDllFromPath(path);
        }
    }
}
