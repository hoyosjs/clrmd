﻿namespace Microsoft.Diagnostics.Runtime.Utilities.DbgEng
{
    [Flags]
    public enum DEBUG_ATTACH : uint
    {
        KERNEL_CONNECTION = 0,
        LOCAL_KERNEL = 1,
        EXDI_DRIVER = 2,

        DEFAULT = 0,
        NONINVASIVE = 1,
        EXISTING = 2,
        NONINVASIVE_NO_SUSPEND = 4,
        INVASIVE_NO_INITIAL_BREAK = 8,
        INVASIVE_RESUME_PROCESS = 0x10,
        NONINVASIVE_ALLOW_PARTIAL = 0x20
    }
}
