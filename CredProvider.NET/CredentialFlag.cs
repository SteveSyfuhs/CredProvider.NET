using System;

namespace CredProvider.NET
{
    [Flags]
    public enum CredentialFlag
    {
        CREDUIWIN_GENERIC = 0x1,
        CREDUIWIN_CHECKBOX = 0x2,
        CREDUIWIN_AUTHPACKAGE_ONLY = 0x10,
        CREDUIWIN_IN_CRED_ONLY = 0x20,
        CREDUIWIN_ENUMERATE_ADMINS = 0x100,
        CREDUIWIN_ENUMERATE_CURRENT_USER = 0x200,
        CREDUIWIN_SECURE_PROMPT = 0x1000,
        CREDUIWIN_PACK_32_WOW = 0x10000000,
    }

}
