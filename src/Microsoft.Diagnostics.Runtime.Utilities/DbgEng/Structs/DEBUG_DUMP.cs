﻿namespace Microsoft.Diagnostics.Runtime.Utilities.DbgEng
{
    public enum DEBUG_DUMP : uint
    {
        SMALL = 1024,
        DEFAULT = 1025,
        FULL = 1026,
        IMAGE_FILE = 1027,
        TRACE_LOG = 1028,
        WINDOWS_CD = 1029,
        KERNEL_DUMP = 1025,
        KERNEL_SMALL_DUMP = 1024,
        KERNEL_FULL_DUMP = 1026
    }
}
