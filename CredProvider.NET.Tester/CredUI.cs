using System;
using System.Runtime.InteropServices;

namespace CredProvider.NET.Tester
{
    public static class CredUI
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        public static extern int CredUIPromptForWindowsCredentials(
            ref CREDUI_INFO uiInfo,
            int authError,
            ref uint authPackage,
            IntPtr InAuthBuffer,
            uint InAuthBufferSize,
            out IntPtr refOutAuthBuffer,
            out uint refOutAuthBufferSize,
            ref bool fSave,
            CredentialFlag flags
        );

        public static void Prompt(string caption, string message)
        {
            var uiInfo = new CREDUI_INFO()
            {
                pszCaptionText = caption,
                pszMessageText = message
            };

            uiInfo.cbSize = Marshal.SizeOf(uiInfo);

            uint authPackage = 0;

            var save = false;

            CredUIPromptForWindowsCredentials(
                ref uiInfo,
                0,
                ref authPackage,
                IntPtr.Zero,
                0,
                out IntPtr outCredBuffer,
                out uint outCredSize,
                ref save,
                0
            );
        }
    }
}
