﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Diagnostics.Runtime.Implementation
{
    internal sealed class SafeWin32Handle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeWin32Handle() : base(true)
        {
        }

        public SafeWin32Handle(IntPtr handle)
            : this(handle, true)
        {
        }

        public SafeWin32Handle(IntPtr handle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);
    }
}