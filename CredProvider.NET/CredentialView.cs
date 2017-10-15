using CredProvider.NET.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CredProvider.NET
{
    public class CredentialDescriptor
    {
        public _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR Descriptor { get; set; }

        public _CREDENTIAL_PROVIDER_FIELD_STATE State { get; set; }

        public object Value { get; set; }
    }

    public class CredentialView
    {
        private readonly List<CredentialDescriptor> fields
            = new List<CredentialDescriptor>();

        public bool Active { get; set; }

        public int DescriptorCount { get { return fields.Count; } }

        public virtual int CredentialCount { get { return 1; } }

        public virtual int DefaultCredential { get { return 0; } }

        public readonly static CredentialView NotActive = new CredentialView { Active = false };

        public CredentialView() { }

        public virtual void AddField(
            _CREDENTIAL_PROVIDER_FIELD_TYPE cpft,
            string pszLabel,
            _CREDENTIAL_PROVIDER_FIELD_STATE state,
            string defaultValue = null,
            Guid guidFieldType = default(Guid)
        )
        {
            if (!Active)
            {
                throw new NotSupportedException();
            }

            fields.Add(new CredentialDescriptor
            {
                State = state,
                Value = defaultValue,
                Descriptor = new _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR
                {
                    dwFieldID = (uint)fields.Count,
                    cpft = cpft,
                    pszLabel = pszLabel,
                    guidFieldType = guidFieldType
                }
            });
        }

        public virtual bool GetField(int dwIndex, [Out] IntPtr ppcpfd)
        {
            Logger.Write($"dwIndex: {dwIndex}; descriptors: {fields.Count}");

            if (dwIndex >= fields.Count)
            {
                return false;
            }

            var field = fields[dwIndex];

            var pcpfd = Marshal.AllocHGlobal(Marshal.SizeOf(field.Descriptor));

            Marshal.StructureToPtr(field.Descriptor, pcpfd, false);
            Marshal.StructureToPtr(pcpfd, ppcpfd, false);

            return true;
        }

        public string GetValue(int dwFieldId)
        {
            return (string)fields[dwFieldId].Value;
        }

        public void SetValue(int dwFieldId, string val)
        {
            fields[dwFieldId].Value = val;
        }

        public void GetFieldState(
            int dwFieldId,
            out _CREDENTIAL_PROVIDER_FIELD_STATE pcpfs,
            out _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE pcpfis
        )
        {
            var field = fields[dwFieldId];

            pcpfs = field.State;
            pcpfis = _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE;
        }

        private readonly Dictionary<int, ICredentialProviderCredential> credentials
            = new Dictionary<int, ICredentialProviderCredential>();

        public virtual ICredentialProviderCredential CreateCredential(int dwIndex)
        {
            if (credentials.TryGetValue(dwIndex, out ICredentialProviderCredential credential))
            {
                return credential;
            }

            credential = new CredentialProviderCredential(this);

            credentials[dwIndex] = credential;

            return credential;
        }
    }
}
