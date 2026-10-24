using System.Reflection;
using System.Runtime.Loader;

namespace TextMateSharp.Fuzz;

// One build of the runner (branch or official) loaded in its own AssemblyLoadContext, so both TextMateSharp versions,
// which share assembly names, live in the same process.
public sealed class Engine
{
    private readonly Type _session;

    public Engine(string name, string directory)
    {
        Name = name;
        string path = Path.Combine(directory, "TextMateSharp.Fuzz.Runner.dll");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The {name} runner is missing; build tools/TextMateSharp.Fuzz first.", path);
        }

        var context = new RunnerContext(name, path);
        Assembly assembly = context.LoadFromAssemblyPath(path);
        _session = assembly.GetType("TextMateSharp.Fuzz.Runner.Session", throwOnError: true)!;
        Version = (string)_session.GetProperty("Version")!.GetValue(null)!;
    }

    public string Name { get; }

    public string Version { get; }

    // A tokenizer session: its own registry and grammar cache, used by one thread at a time.
    public Func<string, string[], int, string> CreateSession()
    {
        object session = Activator.CreateInstance(_session)!;
        return _session.GetMethod("Run")!.CreateDelegate<Func<string, string[], int, string>>(session);
    }

    private sealed class RunnerContext(string name, string path) : AssemblyLoadContext(name)
    {
        private readonly AssemblyDependencyResolver _resolver = new(path);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            string? resolved = _resolver.ResolveAssemblyToPath(assemblyName);
            return resolved == null ? null : LoadFromAssemblyPath(resolved);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? resolved = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            return resolved == null ? IntPtr.Zero : LoadUnmanagedDllFromPath(resolved);
        }
    }
}
