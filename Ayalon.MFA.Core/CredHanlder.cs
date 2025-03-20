using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Ayalon.MFA.Core
{
    public class CredHandler
    {
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CredRead(string target, int type, int reservedFlag, out IntPtr credentialPtr);

        [DllImport("Advapi32.dll", SetLastError = true)]
        private static extern void CredFree(IntPtr buffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CREDENTIAL
        {
            public uint Flags;
            public uint Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }

        public string GetLoggedInUser()
        {
            IntPtr credPtr;
            if (CredRead("TargetName", 1, 0, out credPtr))
            {
                if (credPtr != IntPtr.Zero)
                {
                    CREDENTIAL? cred = Marshal.PtrToStructure<CREDENTIAL>(credPtr);
                    if (cred.HasValue)
                    {
                        string userName = Marshal.PtrToStringUni(cred.Value.UserName) ?? "Unknown User";
                        CredFree(credPtr);
                        return userName;
                    }
                }
                else
                {
                    throw new Exception("Failed to read credential.");
                }
            }
            return "Unknown User";
        }
    }
}
