using IWshRuntimeLibrary;
namespace PassionCareNotification
{
    public static class Program
    {
        public static PassionCarenotification notificationForm;

        [STAThread]
        static void Main()
        {
            string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startupFolderPath, "PassionCareNotification.lnk");

            if (!System.IO.File.Exists(shortcutPath))
            {
                CreateShortcut(shortcutPath);
            }

            // Move SetCompatibleTextRenderingDefault to the beginning
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create the NotificationForm but don't show it yet
            notificationForm = new PassionCarenotification();

            // Start a timer to show the form every 10 seconds
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = 10000; // 10 seconds in milliseconds
            timer.Start();

            Application.Run();


        }

        static void TimerTick(object sender, EventArgs e)
        {
            // Close the previous NotificationForm if it's open
            if (notificationForm != null && !notificationForm.IsDisposed)
            {
                notificationForm.Close();
                notificationForm.Dispose();
            }

            // Create and show a new NotificationForm
            notificationForm = new PassionCarenotification();
            notificationForm.Show();
        }

        private static void CreateShortcut(string shortcutPath)
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.Description = "Notification Passion Care Users New Messages";
            shortcut.WorkingDirectory = Application.StartupPath;
            shortcut.Save();
        }
    }
}