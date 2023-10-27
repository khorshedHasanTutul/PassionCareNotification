using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;

namespace PassionCareNotification
{
    public partial class PassionCarenotification : Form
    {
        public  HubConnection _connection;
        public PassionCarenotification()
        {
            InitializeComponent();
            InitializeSignalRConnection();
        }

        public void InitializeSignalRConnection()
        {
            var appsettings = ConfigurationManager.AppSettings;
            this.ShowInTaskbar = false;

            _connection = new HubConnectionBuilder()
                .WithUrl(appsettings["BasePath"])
                .Build();

            _connection.StartAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _connection.Closed += async (exception) =>
                    {
                        await Task.Delay(2000000);
                        await _connection.StartAsync();
                    };
                }
                else
                {
                    
                }
            });
            _connection.InvokeAsync("SetUserId", UserIdentity());

            _connection.On<string>("ReceiveNotification", message =>
            {
                ReceiveMessageToUser(message);
            });

        }

        public string UserIdentity()
        {
            #region oldCode
            //var appSettings = ConfigurationManager.AppSettings;
            //string FilePathName = appSettings["UserIdentityLocation"];
            //if (FilePathName != null)
            //{
            //    try
            //    {
            //        if (File.Exists(FilePathName))
            //        {
            //            string fileAllText = File.ReadAllText(FilePathName);
            //            return fileAllText;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"{ex.Message}");
            //        return "0";
            //    }
            //}
            //return "0";
            #endregion

            string downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
            //string filePath = downloadFolder + "\\UserInfoPassionCare.csv";
            int columnIndexToRead = 0;
            string pattern = "UserInfoPassionCare";
            string lastFileName = "";

            string[] files = Directory.GetFiles(downloadFolder)
            .Where(x => Path.GetFileName(x).StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
            .ToArray();

            var indexes = files.Select(file =>
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                int startIndex = fileName.IndexOf('(');
                int endIndex = fileName.LastIndexOf(')');

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    string indexStr = fileName.Substring(startIndex + 1, endIndex - startIndex - 1);
                    if (int.TryParse(indexStr, out int index))
                    {
                        return index;
                    }
                }
            return -1;
            
            }).Where(index => index >= 0).ToList();

            if (indexes.Count == 0)
            {
                lastFileName = "UserInfoPassionCare.csv";
            }
            else if (indexes.Count == -1)
            {
                lastFileName = "UserInfoPassionCare.csv";
            }
            else
            {
                int maxIndex = indexes.Max();
                lastFileName = $"{pattern} ({maxIndex}).csv";
            }

            string lastFilePath = Path.Combine(downloadFolder, lastFileName);

            using (TextFieldParser parser = new TextFieldParser(lastFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    if (columnIndexToRead >= 0 && columnIndexToRead < fields.Length)
                    {
                        string columnValue = fields[columnIndexToRead];
                        return columnValue;
                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }

        public void ReceiveMessageToUser(string message)
        {
            notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("bell-icon.ico"));
            notifyIcon1.Text = "Notification By PassionCare.";
            notifyIcon1.Visible = true;
            notifyIcon1.BalloonTipTitle = message.ToString();
            notifyIcon1.BalloonTipText = "Notification By PassionCare.";
            notifyIcon1.ShowBalloonTip(100);
        }
    }
}
