using IWshRuntimeLibrary;
namespace PassionCareNotification
{
    public static class Program
    {

        [STAThread]
        static void Main()
        {
            //string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            //string shortcutPath = Path.Combine(startupFolderPath, "PassionCareNotification.lnk");

            //if (!System.IO.File.Exists(shortcutPath))
            //{
            //    CreateShortcut(shortcutPath);
            //}

            // Move SetCompatibleTextRenderingDefault to the beginning
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PassionCarenotification());

        }

        //private static void CreateShortcut(string shortcutPath)
        //{
        //    WshShell shell = new WshShell();
        //    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
        //    shortcut.TargetPath = Application.ExecutablePath;
        //    shortcut.Description = "Notification Passion Care Users New Messages";
        //    shortcut.WorkingDirectory = Application.StartupPath;
        //    shortcut.Save();
        //}
    }
}