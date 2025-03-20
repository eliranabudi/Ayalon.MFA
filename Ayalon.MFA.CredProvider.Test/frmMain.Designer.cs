namespace Ayalon.MFA.CredProvider.Test
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblUserName = new Label();
            txtUserName = new TextBox();
            txtPassword = new TextBox();
            lblPassword = new Label();
            txtOtp = new TextBox();
            lblOtp = new Label();
            btnAuthenticate = new Button();
            picQrCode = new PictureBox();
            lblStatus = new Label();
            btnValidateOTP = new Button();
            ((System.ComponentModel.ISupportInitialize)picQrCode).BeginInit();
            SuspendLayout();
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.Location = new Point(12, 25);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(63, 15);
            lblUserName.TabIndex = 0;
            lblUserName.Text = "Username:";
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(12, 43);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(100, 23);
            txtUserName.TabIndex = 1;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(12, 111);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(100, 23);
            txtPassword.TabIndex = 3;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(12, 93);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(60, 15);
            lblPassword.TabIndex = 2;
            lblPassword.Text = "Password:";
            // 
            // txtOtp
            // 
            txtOtp.Location = new Point(12, 194);
            txtOtp.Name = "txtOtp";
            txtOtp.Size = new Size(100, 23);
            txtOtp.TabIndex = 4;
            // 
            // lblOtp
            // 
            lblOtp.AutoSize = true;
            lblOtp.Location = new Point(15, 176);
            lblOtp.Name = "lblOtp";
            lblOtp.Size = new Size(319, 15);
            lblOtp.TabIndex = 5;
            lblOtp.Text = "Please type the OTP recieved from Microsoft Authenticator:";
            // 
            // btnAuthenticate
            // 
            btnAuthenticate.Location = new Point(12, 243);
            btnAuthenticate.Name = "btnAuthenticate";
            btnAuthenticate.Size = new Size(84, 23);
            btnAuthenticate.TabIndex = 6;
            btnAuthenticate.Text = "Authenticate";
            btnAuthenticate.UseVisualStyleBackColor = true;
            btnAuthenticate.Click += btnAuthenticate_Click;
            // 
            // picQrCode
            // 
            picQrCode.Location = new Point(507, 33);
            picQrCode.Name = "picQrCode";
            picQrCode.Size = new Size(291, 233);
            picQrCode.TabIndex = 7;
            picQrCode.TabStop = false;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 320);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(319, 15);
            lblStatus.TabIndex = 10;
            lblStatus.Text = "Please type the OTP recieved from Microsoft Authenticator:";
            // 
            // btnValidateOTP
            // 
            btnValidateOTP.Location = new Point(102, 243);
            btnValidateOTP.Name = "btnValidateOTP";
            btnValidateOTP.Size = new Size(84, 23);
            btnValidateOTP.TabIndex = 8;
            btnValidateOTP.Text = "Validate OTP";
            btnValidateOTP.UseVisualStyleBackColor = true;
            btnValidateOTP.Click += btnValidateOTP_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnValidateOTP);
            Controls.Add(picQrCode);
            Controls.Add(btnAuthenticate);
            Controls.Add(lblOtp);
            Controls.Add(txtOtp);
            Controls.Add(txtPassword);
            Controls.Add(lblPassword);
            Controls.Add(txtUserName);
            Controls.Add(lblUserName);
            Name = "frmMain";
            Text = "Form1";
            Load += frmMain_Load;
            ((System.ComponentModel.ISupportInitialize)picQrCode).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblUserName;
        private TextBox txtUserName;
        private TextBox txtPassword;
        private Label lblPassword;
        private TextBox txtOtp;
        private Label lblOtp;
        private Button btnAuthenticate;
        private PictureBox picQrCode;
        private Label lblStatus;
        private Button btnValidateOTP;
    }
}
