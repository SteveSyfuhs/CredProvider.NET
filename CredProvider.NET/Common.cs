using CredProvider.NET.Interop2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CredProvider.NET
{
    static class Common
    {
        //Determine authentication package required
        public static int RetrieveNegotiateAuthPackage(out uint authPackage)
        {
            Logger.Write();

            var status = PInvoke.LsaConnectUntrusted(out var lsaHandle);

            //Use Negotiate to allow LSA to decide whether to use local or Kerberos authentication package.
            //Yubikey sub auth module link: https://github.com/Yubico/yubico-windows-auth
            using (var name = new PInvoke.LsaStringWrapper("Negotiate"))
            {
                status = PInvoke.LsaLookupAuthenticationPackage(lsaHandle, ref name._string, out authPackage);
            }

            PInvoke.LsaDeregisterLogonProcess(lsaHandle);

            Logger.Write($"Using authentication package id: {authPackage}");

            return (int)status;
        }

        public static string GetNameFromSid(string value)
        {
            Logger.Write();

            var sid = new SecurityIdentifier(value);
            var ntAccount = (NTAccount)sid.Translate(typeof(NTAccount));

            return ntAccount.ToString();
        }
    }
}
