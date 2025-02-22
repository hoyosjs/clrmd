﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace Microsoft.Diagnostics.Runtime.DacInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct JitCodeHeapInfo
    {
        public readonly CodeHeapKind Kind;
        public readonly ClrDataAddress Address;
        public readonly ClrDataAddress CurrentAddress;
    }
}