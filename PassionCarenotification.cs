using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;

namespace PassionCareNotification
{
    public partial class PassionCarenotification : Form
    {
        public HubConnection _connection;
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
                .WithAutomaticReconnect()
                .Build();

            _connection.StartAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {

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

            _connection.Closed += async (exception) =>
            {
                await Task.Delay(5 * 60000);
                await _connection.StartAsync();
            };
        }

        public static string GetExeLocation()
        {
            try
            {
                string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
                return Path.GetDirectoryName(exePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
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
            string destinationFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string downloadFolder = "";
            downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
            string latestFilename = LatestFileUserInfoPassionCare(downloadFolder);

            if (Directory.Exists(Path.Combine(destinationFolder, "PassionCareNotification")))
            {
                if (Directory.GetFiles(Path.Combine(destinationFolder, "PassionCareNotification")).Length > 0)
                {
                    string lastFile = LatestFileUserInfoPassionCare(Path.Combine(destinationFolder, "PassionCareNotification"));
                    string user = ReadValueFrom(lastFile);
                    if (user != null)
                    {
                        return user;
                    }
                }
            }
            else if (File.Exists(latestFilename))
            {
                downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
                string sourceFilePath = latestFilename;
                if (File.Exists(sourceFilePath))
                {
                    string destinationFilePath = Path.Combine(destinationFolder, "PassionCareNotification");
                    try
                    {
                        Directory.CreateDirectory(destinationFilePath);
                        if (Directory.GetFiles(destinationFilePath).Length > 0)
                        {
                            if (File.Exists(Path.Combine(destinationFilePath, Path.GetFileName(sourceFilePath))))
                            {
                                DeleteFilesInFolder(Path.Combine(destinationFilePath, Path.GetFileName(sourceFilePath)));
                            }
                        }
                        File.Copy(sourceFilePath, Path.Combine(destinationFilePath, Path.GetFileName(latestFilename)));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                downloadFolder = GetExeLocation();
                latestFilename = LatestFileUserInfoPassionCare(downloadFolder);
                string sourceFilePath = latestFilename;
                if (File.Exists(sourceFilePath))
                {
                    string destinationFilePath = Path.Combine(destinationFolder, "PassionCareNotification");
                    try
                    {
                        Directory.CreateDirectory(destinationFilePath);
                        if (File.Exists(Path.Combine(destinationFilePath, Path.GetFileName(sourceFilePath))))
                        {
                            DeleteFilesInFolder(Path.Combine(destinationFilePath, Path.GetFileName(sourceFilePath)));
                        }
                        File.Copy(sourceFilePath, Path.Combine(destinationFilePath, Path.GetFileName(latestFilename)));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
                else
                {
                    return "0";
                }
            }

            string destinationFilePatha = Path.Combine(destinationFolder, "PassionCareNotification");
            if (File.Exists(LatestFileUserInfoPassionCare(destinationFilePatha)))
            {
                string userId = ReadValueFrom(LatestFileUserInfoPassionCare(destinationFilePatha));
                if (userId != null)
                {
                    return userId;
                }
            }
            return "0";
        }

        private string LatestFileUserInfoPassionCare(string downloadFolder)
        {
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
            return lastFilePath;
        }

        public static void DeleteFilesInFolder(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private string ReadValueFrom(string lastFilePath)
        {
            int columnIndexToRead = 0;
            try
            {
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

            }
            catch (Exception)
            {
                return "0";

            }
            return "0";
        }

        public void ReceiveMessageToUser(string message)
        {
            PassionCareNotify.Icon = new System.Drawing.Icon(Path.GetFullPath("bell-icon2.ico"));
            PassionCareNotify.Text = "Notification By PassionCare.";
            PassionCareNotify.Visible = true;
            PassionCareNotify.BalloonTipTitle = message.ToString();
            PassionCareNotify.BalloonTipText = "Notification By PassionCare.";
            PassionCareNotify.ShowBalloonTip(100);
        }
    }
}
