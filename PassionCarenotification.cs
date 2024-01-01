using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
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

            _connection.On<string, string, string , string, string, string>("ReceiveNotification", (message, twilioMessageId, contactId, twilioContactId , name, phoneNumber) =>
            {
                ReceiveMessageToUser(message, twilioMessageId,contactId, twilioContactId, name , phoneNumber);
            });

            _connection.Closed += async (exception) =>
            {
                await Task.Delay(5 * 60000);
                await _connection.StartAsync();
            };
        }


        public string UserIdentity()
        {
            string userName = string.Empty, password = string.Empty;
            var appsettings = ConfigurationManager.AppSettings;
            string loginApi = appsettings["loginapi"];
            // Specify the path to your text file
            string UsernamefilePath = "C:\\PassionCareNotification\\Username.txt";
            // string PasswordfilePath = "C:\\PassionCareNotification\\Password.txt";

            if (File.Exists(UsernamefilePath))
            {
                using (StreamReader reader = new StreamReader(UsernamefilePath))
                {
                    userName = reader.ReadLine();
                }
                return userName.Trim();

                //using (StreamReader reader = new StreamReader(PasswordfilePath))
                //{
                //    password = reader.ReadLine();
                //}
                //bool response = await loginMethod(userName, password, loginApi);
                //if (response)
                //{
                //    return userName;
                //}
            }
            return "NullFakeBuddy";
        }

        public void ReceiveMessageToUser(string message,string twillioMessageId, string contactId, string twilioContactId, string name , string phoneNumber)
        {
            var appsettings = ConfigurationManager.AppSettings;
            string redirectlink = appsettings["redirectlink"];
            PassionCareNotify.Icon = new System.Drawing.Icon(Path.GetFullPath("bell-icon2.ico"));
            //PassionCareNotify.Text = "Notification By PassionCare.";
            PassionCareNotify.Visible = true;
            PassionCareNotify.BalloonTipTitle ="From:"+name +"-"+phoneNumber;
            PassionCareNotify.BalloonTipText = message.ToString();
            PassionCareNotify.Tag = redirectlink+ twillioMessageId+"/"+contactId+"/"+twilioContactId;
            PassionCareNotify.BalloonTipClicked -= PassionCareNotify_Click;
            PassionCareNotify.BalloonTipClicked += PassionCareNotify_Click;
            PassionCareNotify.ShowBalloonTip(100);
        }

        private async Task<bool> loginMethod(string userName, string password, string loginApi)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Prepare the data to be sent in the request body
                var data = new
                {
                    UserName = userName,
                    Password = password,
                    Organization = "Datavanced"
                };
                string jsonData = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync(loginApi, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        ResponseMessage response1 = JsonConvert.DeserializeObject<ResponseMessage>(jsonContent);

                        if (response1.ResponseCode == 200)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return false;
        }

        private void PassionCareNotify_Click(object sender, EventArgs e)
        {
            string link = PassionCareNotify.Tag as string;

            if (!string.IsNullOrEmpty(link))
            {
                // Implement the logic to navigate to the desired page
                // For example, you can open a new form, navigate to a URL, etc.
                OpenDesiredPage(link);
                PassionCareNotify.BalloonTipClicked -= PassionCareNotify_Click;
            }
        }
        private void OpenDesiredPage(string link)
        {
            try
            {
                if (!string.IsNullOrEmpty(link))
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = link,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                else
                {
                    MessageBox.Show("Invalid link. Unable to open the desired page.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening the link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
