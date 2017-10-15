using CredProvider.NET.Interop;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Security.Principal;
using static CredProvider.NET.Constants;

namespace CredProvider.NET
{
    public class CredentialProviderCredential : ICredentialProviderCredential2
    {
        private readonly CredentialView view;

        public CredentialProviderCredential(CredentialView view)
        {
            this.view = view;
        }

        public virtual int Advise(ICredentialProviderCredentialEvents pcpce)
        {
            Logger.Write();

            if (pcpce is ICredentialProviderCredentialEvents2 ev2)
            {
                Logger.Write("pcpce is ICredentialProviderCredentialEvents2");
            }

            return HRESULT.S_OK;
        }

        public virtual int UnAdvise()
        {
            Logger.Write();

            return HRESULT.E_NOTIMPL;
        }

        public virtual int SetSelected(out int pbAutoLogon)
        {
            Logger.Write();

            pbAutoLogon = 1;

            return HRESULT.S_OK;
        }

        public virtual int SetDeselected()
        {
            Logger.Write();

            return HRESULT.E_NOTIMPL;
        }

        public virtual int GetFieldState(
            uint dwFieldID,
            out _CREDENTIAL_PROVIDER_FIELD_STATE pcpfs,
            out _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE pcpfis
        )
        {
            Logger.Write($"dwFieldID: {dwFieldID}");

            view.GetFieldState((int)dwFieldID, out pcpfs, out pcpfis);

            return HRESULT.S_OK;
        }

        public virtual int GetStringValue(uint dwFieldID, out string ppsz)
        {
            Logger.Write($"dwFieldID: {dwFieldID}");

            ppsz = view.GetValue((int)dwFieldID);
            
            return HRESULT.S_OK;
        }

        private Bitmap tileIcon;

        public virtual int GetBitmapValue(uint dwFieldID, out IntPtr phbmp)
        {
            Logger.Write($"dwFieldID: {dwFieldID}");

            try
            {
                TryLoadUserIcon();
            }
            catch (Exception ex)
            {
                Logger.Write("Error: " + ex);
            }

            phbmp = tileIcon?.GetHbitmap() ?? IntPtr.Zero;

            return HRESULT.S_OK;
        }

        private void TryLoadUserIcon()
        {
            if (tileIcon == null)
            {
                using (var web = new WebClient())
                {
                    var bytes = web.DownloadData("https://syfuhs.blob.core.windows.net/images/2015/12/mad.jpg");

                    var image = Image.FromStream(new MemoryStream(bytes));

                    var imageStream = new MemoryStream();
                    image.Save(imageStream, ImageFormat.Bmp);
                    imageStream.Seek(0, SeekOrigin.Begin);

                    tileIcon = new Bitmap(imageStream);
                }
            }
        }

        public virtual int GetCheckboxValue(uint dwFieldID, out int pbChecked, out string ppszLabel)
        {
            Logger.Write($"dwFieldID: {dwFieldID}");

            pbChecked = 0;
            ppszLabel = "";

            return HRESULT.E_NOTIMPL;
        }

        public virtual int GetSubmitButtonValue(uint dwFieldID, out uint pdwAdjacentTo)
        {
            Logger.Write($"dwFieldID: {dwFieldID}");

            pdwAdjacentTo = 0;

            return HRESULT.E_NOTIMPL;
        }

        public virtual int GetComboBoxValueCount(uint dwFieldID, out uint pcItems, out uint pdwSelectedItem)
        {
            Logger.Write($"dwFieldID: {dwFieldID}");

            pcItems = 0;
            pdwSelectedItem = 0;

            return HRESULT.E_NOTIMPL;
        }

        public virtual int GetComboBoxValueAt(uint dwFieldID, uint dwItem, out string ppszItem)
        {
            Logger.Write($"dwFieldID: {dwFieldID}; dwItem: {dwItem}");

            ppszItem = "";

            return HRESULT.E_NOTIMPL;
        }

        public virtual int SetStringValue(uint dwFieldID, string psz)
        {
            Logger.Write($"dwFieldID: {dwFieldID}; psz: {psz}");

            return HRESULT.S_OK;
        }

        public virtual int SetCheckboxValue(uint dwFieldID, int bChecked)
        {
            Logger.Write($"dwFieldID: {dwFieldID}; bChecked: {bChecked}");

            return HRESULT.E_NOTIMPL;
        }

        public virtual int SetComboBoxSelectedValue(uint dwFieldID, uint dwSelectedItem)
        {
            Logger.Write($"dwFieldID: {dwFieldID}; dwSelectedItem: {dwSelectedItem}");

            return HRESULT.E_NOTIMPL;
        }

        public virtual int CommandLinkClicked(uint dwFieldID)
        {
            Logger.Write($"dwFieldID: {dwFieldID}");

            return HRESULT.E_NOTIMPL;
        }

        public virtual int GetSerialization(
            out _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE pcpgsr,
            out _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcs,
            out string ppszOptionalStatusText,
            out _CREDENTIAL_PROVIDER_STATUS_ICON pcpsiOptionalStatusIcon
        )
        {
            Logger.Write();

            pcpgsr = _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE.CPGSR_NO_CREDENTIAL_NOT_FINISHED;
            pcpcs = new _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION() { ulAuthenticationPackage = 1234 };
            ppszOptionalStatusText = "";
            pcpsiOptionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_NONE;

            return HRESULT.S_OK;
        }

        public virtual int ReportResult(
            int ntsStatus,
            int ntsSubstatus,
            out string ppszOptionalStatusText,
            out _CREDENTIAL_PROVIDER_STATUS_ICON pcpsiOptionalStatusIcon
        )
        {
            Logger.Write($"ntsStatus: {ntsStatus}; ntsSubstatus: {ntsSubstatus}");

            ppszOptionalStatusText = "";
            pcpsiOptionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_NONE;

            return HRESULT.S_OK;
        }

        public virtual int GetUserSid(out string sid)
        {
            var identity = WindowsIdentity.GetCurrent();

            if (!identity.IsAuthenticated || identity.IsSystem)
            {
                sid = null;
                return HRESULT.S_FALSE;
            }

            sid = identity.User.ToString();

            Logger.Write($"Identity: {identity.Name}");

            foreach (var claim in identity.Claims)
            {
                string type = claim.Type;

                if (claim.Type.LastIndexOf('/') >= 0)
                {
                    type = claim.Type.Substring(claim.Type.LastIndexOf('/') + 1);
                }

                Logger.Write($"{type}: {claim.Value}");
            }

            return HRESULT.S_OK;
        }
    }
}