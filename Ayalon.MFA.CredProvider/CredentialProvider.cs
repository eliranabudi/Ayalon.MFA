using Ayalon.MFA.Core;
using CredProvider.NET.Interop2;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DrawingPoint = System.Drawing.Point;


namespace Ayalon.MFA.CredProvider
{
    [ComVisible(true)]
    [Guid("AC790E48-D761-42F5-8D08-E2B9823557B0")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Ayalon.MFA.CredProvider")]
    public class CredentialProvider : CredentialProviderBase
    {
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        public static extern IntPtr CreateWindowEx(
           int dwExStyle, string lpClassName, string lpWindowName, int dwStyle, int x, int y, int nWidth,
           int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

        private enum GetWindowType : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        const string AppName = "Ayalon MFA";

        public static CredentialView NotActive;
        private TwoFactorAuth twoFactorAuth;
        private CredHandler credHandler;
        private QrCodeGenerator qrCodeGenerator;

        private readonly string appDataPath;
        private readonly string secretKeyPath;

        public CredentialProvider()
        {
            twoFactorAuth = new TwoFactorAuth();
            credHandler = new CredHandler();
            qrCodeGenerator = new QrCodeGenerator();

            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            secretKeyPath = Path.Combine(appDataPath, AppName, "secret.key");
        }

        protected override CredentialView Initialize(_CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus, uint dwFlags)
        {
            var flags = (CredentialFlag)dwFlags;

            Logger.Write($"cpus: {cpus}; dwFlags: {flags}");

            var isSupported = IsSupportedScenario(cpus);

            if (!isSupported)
            {
                if (NotActive == null) NotActive = new CredentialView(this) { Active = false };
                return NotActive;
            }

            var view = new CredentialView(this) { Active = true };
            var userNameState = (cpus == _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON) ?
                    _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE : _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_HIDDEN;
            var confirmPasswordState = (cpus == _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CHANGE_PASSWORD) ?
                    _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_BOTH : _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_HIDDEN;

            view.AddField(
                cpft: _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_TILE_IMAGE,
                pszLabel: "Icon",
                state: _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_BOTH,
                guidFieldType: Guid.Parse(CredentialView.CPFG_CREDENTIAL_PROVIDER_LOGO)
            );

            view.AddField(
                cpft: _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_EDIT_TEXT,
                pszLabel: "Username",
                state: userNameState
            );

            view.AddField(
                cpft: _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_PASSWORD_TEXT,
                pszLabel: "Password",
                state: _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE,
                guidFieldType: Guid.Parse(CredentialView.CPFG_LOGON_PASSWORD_GUID)
            );

            // הוספת שדה OTP ושמירת מזהה השדה
            var otpFieldId = view.AddField(
                cpft: _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_EDIT_TEXT,
                pszLabel: "OTP",
                state: _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_HIDDEN // בהתחלה מוסתר
            );

            var validateButtonId = view.AddField(
                        cpft: _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SUBMIT_BUTTON,
                        pszLabel: "Validate OTP",
                        state: _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_DESELECTED_TILE
            );

            string secretKey = GetSecretKey();

            if (File.Exists(secretKeyPath))
            {
                if (!string.IsNullOrEmpty(secretKey))
                {
                    twoFactorAuth.SetSecretKey(secretKey);
                    // הצגת שדה ה-OTP
                    view.SetFieldState(otpFieldId, _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE);
                    view.SetFieldState(validateButtonId, _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE);
                }
                else
                {
                    ShowQrCodeForSync();
                    twoFactorAuth.SetSecretKey(secretKey);
                    view.SetFieldState(otpFieldId, _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE);
                    view.SetFieldState(validateButtonId, _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE);
                }
            }
            else
            {
                ShowQrCodeForSync();
                twoFactorAuth.SetSecretKey(secretKey);
                view.SetFieldState(otpFieldId, _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE);
                view.SetFieldState(validateButtonId, _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE);
            }

            // הגדרת האירוע ללחיצה על הכפתור
            view.SetButtonCallback(validateButtonId, () => OnValidateButtonClick(view, otpFieldId));

            return view;
        }

        private string GetSecretKey()
        {
            if (!File.Exists(secretKeyPath))
            {
                return null;
            }

            string encryptedSecretKey = File.ReadAllText(secretKeyPath);

            if (string.IsNullOrEmpty(encryptedSecretKey))
            {
                return null;
            }

            string secretKey = twoFactorAuth.DecryptSecretKey(encryptedSecretKey);
            return secretKey;
        }

        private static bool IsSupportedScenario(_CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus)
        {
            switch (cpus)
            {
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CREDUI:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_UNLOCK_WORKSTATION:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CHANGE_PASSWORD:
                    return true;

                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_PLAP:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_INVALID:
                default:
                    return false;
            }
        }

        private void ShowQrCodeForSync()
        {
            // יצירת והצגת קוד QR לסינכרון
            string issuer = AppName;
            string userName = credHandler.GetLoggedInUser();
            string secretKey = twoFactorAuth.GenerateSecretKey();

            var qrCodeBitmap = qrCodeGenerator.GenerateQrCode(secretKey, userName, issuer);

            //שמירת המפתח הסודי
            string encryptedSecretKey = twoFactorAuth.EncryptSecretKey(secretKey);
            Directory.CreateDirectory(Path.GetDirectoryName(secretKeyPath));
            File.WriteAllText(secretKeyPath, encryptedSecretKey);

            // הצגת קוד ה-QR למשתמש
            IntPtr mainWindowHandle = GetForegroundWindow();
            using (Graphics g = Graphics.FromHwnd(mainWindowHandle))
            {
                g.DrawImage(qrCodeBitmap, new DrawingPoint(200, 200)); // מיקום לדוגמה
            }
        }

        public bool ValidateOtpCode(CredentialView view, int otpFieldId)
        {
            string otpCode = view.GetValue(otpFieldId);
            return twoFactorAuth.ValidateCode(otpCode);
        }

        public void OnValidateButtonClick(CredentialView view, int otpFieldId)
        {
            if (ValidateOtpCode(view, otpFieldId))
            {
                Logger.Write("The entered OTP code is correct.");
            }
            else
            {
                Logger.Write("The entered OTP code is incorrect.");
            }
        }

        public CredentialView TestInitialize(_CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus, uint dwFlags)
        {
            return Initialize(cpus, dwFlags);
        }
    }
}
