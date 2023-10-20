using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;

namespace PassionCareNotification
{
    public partial class PassionCarenotification : Form
    {
        public readonly HubConnection _connection;
        public PassionCarenotification()
        {
            InitializeComponent();
            //System.IO.File.WriteAllText("D:\\HHAXCookies\\hhaxCookies.txt", DateTime.Now.ToString());
            var appsettings = ConfigurationManager.AppSettings;
            this.ShowInTaskbar = false;
            string userId = "0";

            _connection = new HubConnectionBuilder()
                .WithUrl(appsettings["BasePath"])
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5) })
                .Build();

            var refreshInterval = TimeSpan.FromSeconds(5);

            var timer = new System.Threading.Timer(async _ =>
            {
                //Console.WriteLine("Refreshing the SignalR connection...");
                try
                {
                    //await _connection.StopAsync();
                    await _connection.StartAsync();
                    await _connection.InvokeAsync("SetUserId", UserIdentity());
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"Error while refreshing the connection: {ex.Message}");
                    //System.IO.File.AppendAllText("D:\\HHAXCookies\\hhaxCookies.txt", ex.Message.ToString());
                }
            }, null, TimeSpan.Zero, refreshInterval);

            _connection.On<string>("ReceiveNotification", message =>
            {
                ReceiveMessageToUser(message);
            });

            _connection.StartAsync();
            // Set the user ID for the connection
            _connection.InvokeAsync("SetUserId", UserIdentity());

            _connection.Closed += async (exception) =>
            {
                await Task.Delay(2000);
                await _connection.StartAsync();
            };
        }

        public string UserIdentity()
        {
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


            // Specify the path to your CSV file

            string downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
            string filePath = downloadFolder + "\\UserInfo.csv";
            int columnIndexToRead = 0;

            using (TextFieldParser parser = new TextFieldParser(filePath))
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
