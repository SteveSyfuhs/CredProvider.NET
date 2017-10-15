namespace CredProvider.NET
{
    internal static class Constants
    {
        public static class HRESULT
        {
            // https://msdn.microsoft.com/en-us/library/windows/desktop/ff485842(v=vs.85).aspx

            public const int S_OK = 0x00000000;
            public const int S_FALSE = 0x00000001;

            public const int E_ACCESSDENIED = unchecked((int)0x80070005);
            public const int E_FAIL = unchecked((int)0x80004005);
            public const int E_INVALIDARG = unchecked((int)0x80070057);
            public const int E_OUTOFMEMORY = unchecked((int)0x8007000E);
            public const int E_POINTER = unchecked((int)0x80004003);
            public const int E_UNEXPECTED = unchecked((int)0x8000FFFF);

            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa378137(v=vs.85).aspx

            public const int E_ABORT = unchecked((int)0x80004004);
            public const int E_HANDLE = unchecked((int)0x80070006);
            public const int E_NOINTERFACE = unchecked((int)0x80004002);
            public const int E_NOTIMPL = unchecked((int)0x80004001);
        }
    }
}
