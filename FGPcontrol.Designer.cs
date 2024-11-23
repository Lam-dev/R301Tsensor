namespace SmartCard
{
    partial class FGPcontrol
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
            pictureBox_showFGPicon = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox_showFGPicon).BeginInit();
            SuspendLayout();
            // 
            // pictureBox_showFGPicon
            // 
            pictureBox_showFGPicon.Location = new Point(3, 3);
            pictureBox_showFGPicon.Name = "pictureBox_showFGPicon";
            pictureBox_showFGPicon.Size = new Size(93, 84);
            pictureBox_showFGPicon.TabIndex = 0;
            pictureBox_showFGPicon.TabStop = false;
            // 
            // FGPcontrol
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pictureBox_showFGPicon);
            Name = "FGPcontrol";
            Size = new Size(479, 251);
            ((System.ComponentModel.ISupportInitialize)pictureBox_showFGPicon).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox_showFGPicon;
    }
}
