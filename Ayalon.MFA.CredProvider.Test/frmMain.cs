using System;
using System.Windows.Forms;
using Ayalon.MFA.Core;
using Ayalon.MFA.CredProvider;
using CredProvider.NET.Interop2;

namespace Ayalon.MFA.CredProvider.Test
{
    public partial class frmMain : Form
    {
        private CredentialProvider credentialProvider;
        private TwoFactorAuth twoFactorAuth;
        private CredHandler credHandler;
        private QrCodeGenerator qrCodeGenerator;
        private string secretKey;
        private readonly string appDataPath;
        private readonly string secretKeyPath;

        public frmMain()
        {
            InitializeComponent();
            credentialProvider = new CredentialProvider();
            twoFactorAuth = new TwoFactorAuth();
            credHandler = new CredHandler();
            qrCodeGenerator = new QrCodeGenerator();
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            secretKeyPath = Path.Combine(appDataPath, "Ayalon MFA", "secret.key");
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAuthenticate_Click(object sender, EventArgs e)
        {

            var view = credentialProvider.TestInitialize(
                _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON,
                0);

            if (view.Active)
            {
                lblStatus.Text = "CredentialProvider is active.";
            }
            else
            {
                lblStatus.Text = "CredentialProvider is not active.";
            }
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string secretKeyPath = System.IO.Path.Combine(appDataPath, "Ayalon MFA", "secret.key");

                if (System.IO.File.Exists(secretKeyPath))
                {
                    string encryptedSecretKey = System.IO.File.ReadAllText(secretKeyPath);
                    string secretKey = twoFactorAuth.DecryptSecretKey(encryptedSecretKey);

                    if (!string.IsNullOrEmpty(secretKey))
                    {
                        string otpCode = txtOtp.Text;

                        if (twoFactorAuth.ValidateCode(otpCode))
                        {
                            MessageBox.Show("Two-factor authentication succeeded.");
                        }
                        else
                        {
                            MessageBox.Show("Two-factor authentication failed.");
                        }
                    }
                    else
                    {
                        ShowQrCodeForSync();
                    }
                }
                else
                {
                    ShowQrCodeForSync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ShowQrCodeForSync()
        {
            //MessageBox.Show(credHandler.GetLoggedInUser());
            string issuer = "Ayalon MFA";
            string userName = txtUserName.Text;
            secretKey = twoFactorAuth.GenerateSecretKey(); // שמירת המפתח הסודי שנוצר
            var qrCodeBitmap = qrCodeGenerator.GenerateQrCode(secretKey, userName, issuer);

            string encryptedSecretKey = twoFactorAuth.EncryptSecretKey(secretKey);
            Directory.CreateDirectory(Path.GetDirectoryName(secretKeyPath));
            File.WriteAllText(secretKeyPath, encryptedSecretKey);

            picQrCode.Image = qrCodeBitmap;
            picQrCode.Visible = true;
        }


        private void btnValidateOTP_Click(object sender, EventArgs e)
        {
            twoFactorAuth.SetSecretKey(secretKey); // הגדרת המפתח הסודי שנשמר
            if (twoFactorAuth.ValidateCode(txtOtp.Text))
            {
                MessageBox.Show("The entered code is right.");
                txtOtp.Clear();
            }
            else
            {
                MessageBox.Show("The entered code is wrong.");
                txtOtp.Clear();
            }
        }
    }
}

