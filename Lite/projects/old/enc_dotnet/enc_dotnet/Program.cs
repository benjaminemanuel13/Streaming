
    using System.Runtime.InteropServices;

    [DllImport("encode", CallingConvention = CallingConvention.Cdecl)]
    static extern int start(int argCount, string launcher, string filename);

    int argCount = Environment.GetCommandLineArgs().Count();

    string[] argumnents;

    if (argCount > 1)
    {
        argumnents = new string[2] {
            Environment.GetCommandLineArgs()[0],
            Environment.GetCommandLineArgs()[1]
        };
    }
    else
    {
        argumnents = new string[1] {
            Environment.GetCommandLineArgs()[0]
        };
    }

    string launcher = argumnents[0];
    
    if (launcher.Contains("\\"))
    {
        launcher = launcher.Substring(launcher.LastIndexOf("\\") + 1, launcher.Length - 1 - launcher.LastIndexOf("\\"));
    }
        
    string filename = argumnents.Count() > 1 ? argumnents[1] : "nofile.fil";

    int value = start(argumnents.Count(), launcher, filename);