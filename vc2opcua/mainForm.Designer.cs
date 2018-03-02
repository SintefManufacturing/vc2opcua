namespace vc2opcua
{
    partial class mainForm
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
            this.StartButton = new System.Windows.Forms.Button();
            this.ServerBox = new System.Windows.Forms.TextBox();
            this.PortBox = new System.Windows.Forms.TextBox();
            this.StopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(97, 184);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(170, 23);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            // 
            // ServerBox
            // 
            this.ServerBox.Location = new System.Drawing.Point(97, 35);
            this.ServerBox.Name = "ServerBox";
            this.ServerBox.Size = new System.Drawing.Size(170, 20);
            this.ServerBox.TabIndex = 1;
            // 
            // PortBox
            // 
            this.PortBox.Location = new System.Drawing.Point(97, 77);
            this.PortBox.Name = "PortBox";
            this.PortBox.Size = new System.Drawing.Size(170, 20);
            this.PortBox.TabIndex = 2;
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(97, 233);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(170, 23);
            this.StopButton.TabIndex = 3;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 483);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.PortBox);
            this.Controls.Add(this.ServerBox);
            this.Controls.Add(this.StartButton);
            this.Name = "mainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.TextBox ServerBox;
        private System.Windows.Forms.TextBox PortBox;
        private System.Windows.Forms.Button StopButton;
    }
}

