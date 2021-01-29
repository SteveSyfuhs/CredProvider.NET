using CredProvider.NET.Interop2;
using System;
using System.Runtime.InteropServices;

namespace CredProvider.NET
{
    [ComVisible(true)]
    [Guid("00006d50-0000-0000-b090-00006b0b0000")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("CredProvider.NET")]
    public class CredentialProvider : CredentialProviderBase
    {
        public static CredentialView NotActive;        

        public CredentialProvider()
        {

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
            var userNameState = (cpus == _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CREDUI) ?
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

            view.AddField(
                cpft: _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_PASSWORD_TEXT,
                pszLabel: "Confirm password",
                state: confirmPasswordState,
                guidFieldType: Guid.Parse(CredentialView.CPFG_LOGON_PASSWORD_GUID)
            );

            view.AddField(
                cpft: _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_LARGE_TEXT,
                pszLabel: "Click Me!",
                defaultValue: "Click Me!",
                state: _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_DESELECTED_TILE
            );

            return view;
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
    }
}
