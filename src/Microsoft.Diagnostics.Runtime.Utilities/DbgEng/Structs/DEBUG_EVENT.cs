﻿namespace Microsoft.Diagnostics.Runtime.Utilities.DbgEng
{
    [Flags]
    public enum DEBUG_EVENT : uint
    {
        NONE = 0,
        BREAKPOINT = 1,
        EXCEPTION = 2,
        CREATE_THREAD = 4,
        EXIT_THREAD = 8,
        CREATE_PROCESS = 0x10,
        EXIT_PROCESS = 0x20,
        LOAD_MODULE = 0x40,
        UNLOAD_MODULE = 0x80,
        SYSTEM_ERROR = 0x100,
        SESSION_STATUS = 0x200,
        CHANGE_DEBUGGEE_STATE = 0x400,
        CHANGE_ENGINE_STATE = 0x800,
        CHANGE_SYMBOL_STATE = 0x1000
    }
}
