using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace LauncherFinal.Helper
{
    public static class SecureStringExtensions
    {
        public static string ConvertToString(this SecureString secure)
        {
            if (secure == null || secure.Length == 0)
            {
                return string.Empty;
            }

            var pointer = IntPtr.Zero;

            try
            {
                pointer = Marshal.SecureStringToBSTR(secure);
                return Marshal.PtrToStringBSTR(pointer);
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return string.Empty;
            }
            finally
            {
                Marshal.FreeBSTR(pointer);
            }

        }
    }
}
