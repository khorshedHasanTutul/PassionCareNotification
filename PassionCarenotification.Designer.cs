namespace PassionCareNotification
{
    partial class PassionCarenotification
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PassionCarenotification));
            PassionCareNotify = new NotifyIcon(components);
            SuspendLayout();
            // 
            // PassionCareNotify
            // 
            PassionCareNotify.BalloonTipIcon = ToolTipIcon.Info;
            PassionCareNotify.Icon = (Icon)resources.GetObject("PassionCareNotify.Icon");
            PassionCareNotify.Text = "PassionCareNotify";
            PassionCareNotify.Visible = true;
            // 
            // PassionCarenotification
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PassionCarenotification";
            Opacity = 0D;
            Text = "PassionCarenotification";
            ResumeLayout(false);
        }

        #endregion
        private NotifyIcon PassionCareNotify;
    }
}