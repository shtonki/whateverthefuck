namespace whateverthefuck.src.util
{
    using System.IO;

    public static class ContextHandler
    {
        public static void SetupUnifiedContext()
        {
            if (!Directory.GetCurrentDirectory().EndsWith("Debug"))
            {
                Directory.SetCurrentDirectory(System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("whateverthefuck.exe", string.Empty));
            }
        }
    }
}
