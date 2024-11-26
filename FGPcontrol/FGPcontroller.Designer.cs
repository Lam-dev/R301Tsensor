namespace FGPcontrol
{
    partial class FGPcontroller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.timer_getFGPfeature = new System.Windows.Forms.Timer(this.components);
            this.timer_showFGPimageAnimation = new System.Windows.Forms.Timer(this.components);
            this.label_notificationText = new System.Windows.Forms.Label();
            this.pictureBox_notificationIcon = new System.Windows.Forms.PictureBox();
            this.pictureBox_showFGPicon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_notificationIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_showFGPicon)).BeginInit();
            this.SuspendLayout();
            // 
            // serialPort
            // 
            this.serialPort.BaudRate = 56700;
            this.serialPort.PortName = "COM7";
            // 
            // timer_getFGPfeature
            // 
            this.timer_getFGPfeature.Interval = 700;
            this.timer_getFGPfeature.Tick += new System.EventHandler(this.timer_getFGPfeature_Tick);
            // 
            // timer_showFGPimageAnimation
            // 
            this.timer_showFGPimageAnimation.Tick += new System.EventHandler(this.timer_showFGPimageAnimation_Tick);
            // 
            // label_notificationText
            // 
            this.label_notificationText.AutoSize = true;
            this.label_notificationText.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_notificationText.Location = new System.Drawing.Point(74, 151);
            this.label_notificationText.Name = "label_notificationText";
            this.label_notificationText.Size = new System.Drawing.Size(326, 29);
            this.label_notificationText.TabIndex = 2;
            this.label_notificationText.Text = "Nhấc tay ra khỏi cảm biến. ";
            // 
            // pictureBox_notificationIcon
            // 
            this.pictureBox_notificationIcon.InitialImage = global::FGPcontrol.Icon.put55;
            this.pictureBox_notificationIcon.Location = new System.Drawing.Point(7, 137);
            this.pictureBox_notificationIcon.Name = "pictureBox_notificationIcon";
            this.pictureBox_notificationIcon.Size = new System.Drawing.Size(61, 57);
            this.pictureBox_notificationIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_notificationIcon.TabIndex = 1;
            this.pictureBox_notificationIcon.TabStop = false;
            // 
            // pictureBox_showFGPicon
            // 
            this.pictureBox_showFGPicon.InitialImage = global::FGPcontrol.Icon._13;
            this.pictureBox_showFGPicon.Location = new System.Drawing.Point(158, 12);
            this.pictureBox_showFGPicon.Name = "pictureBox_showFGPicon";
            this.pictureBox_showFGPicon.Size = new System.Drawing.Size(128, 113);
            this.pictureBox_showFGPicon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_showFGPicon.TabIndex = 0;
            this.pictureBox_showFGPicon.TabStop = false;
            // 
            // FGPcontroller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_notificationText);
            this.Controls.Add(this.pictureBox_notificationIcon);
            this.Controls.Add(this.pictureBox_showFGPicon);
            this.Name = "FGPcontroller";
            this.Size = new System.Drawing.Size(463, 210);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_notificationIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_showFGPicon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.Timer timer_getFGPfeature;
        private System.Windows.Forms.PictureBox pictureBox_showFGPicon;
        private System.Windows.Forms.Timer timer_showFGPimageAnimation;
        private System.Windows.Forms.PictureBox pictureBox_notificationIcon;
        private System.Windows.Forms.Label label_notificationText;
    }
}
