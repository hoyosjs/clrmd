﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace Microsoft.Diagnostics.Runtime
{
    /// <summary>
    /// X86-specific windows thread context.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct X86Context
    {
        public const uint Context = 0x00010000;
        public const uint ContextControl = Context | 0x1;
        public const uint ContextInteger = Context | 0x2;
        public const uint ContextSegments = Context | 0x4;
        public const uint ContextFloatingPoint = Context | 0x8;
        public const uint ContextDebugRegisters = Context | 0x10;

        public static int Size => 0x2cc;

        // Control flags

        [FieldOffset(0x0)]
        public uint ContextFlags;

        #region Debug registers

        [Register(RegisterType.Debug)]
        [FieldOffset(0x4)]
        public uint Dr0;

        [Register(RegisterType.Debug)]
        [FieldOffset(0x8)]
        public uint Dr1;

        [Register(RegisterType.Debug)]
        [FieldOffset(0xc)]
        public uint Dr2;

        [Register(RegisterType.Debug)]
        [FieldOffset(0x10)]
        public uint Dr3;

        [Register(RegisterType.Debug)]
        [FieldOffset(0x14)]
        public uint Dr6;

        [Register(RegisterType.Debug)]
        [FieldOffset(0x18)]
        public uint Dr7;

        #endregion

        #region Floating point registers

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x1c)]
        public uint ControlWord;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x20)]
        public uint StatusWord;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x24)]
        public uint TagWord;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x28)]
        public uint ErrorOffset;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x2c)]
        public uint ErrorSelector;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x30)]
        public uint DataOffset;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x34)]
        public uint DataSelector;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x38)]
        public Float80 ST0;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x42)]
        public Float80 ST1;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x4c)]
        public Float80 ST2;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x56)]
        public Float80 ST3;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x60)]
        public Float80 ST4;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x6a)]
        public Float80 ST5;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x74)]
        public Float80 ST6;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x7e)]
        public Float80 ST7;

        [Register(RegisterType.FloatingPoint)]
        [FieldOffset(0x88)]
        public uint Cr0NpxState;

        #endregion

        #region Segment Registers

        [Register(RegisterType.Segments)]
        [FieldOffset(0x8c)]
        public uint Gs;

        [Register(RegisterType.Segments)]
        [FieldOffset(0x90)]
        public uint Fs;

        [Register(RegisterType.Segments)]
        [FieldOffset(0x94)]
        public uint Es;

        [Register(RegisterType.Segments)]
        [FieldOffset(0x98)]
        public uint Ds;

        #endregion

        #region Integer registers

        [Register(RegisterType.General)]
        [FieldOffset(0x9c)]
        public uint Edi;

        [Register(RegisterType.General)]
        [FieldOffset(0xa0)]
        public uint Esi;

        [Register(RegisterType.General)]
        [FieldOffset(0xa4)]
        public uint Ebx;

        [Register(RegisterType.General)]
        [FieldOffset(0xa8)]
        public uint Edx;

        [Register(RegisterType.General)]
        [FieldOffset(0xac)]
        public uint Ecx;

        [Register(RegisterType.General)]
        [FieldOffset(0xb0)]
        public uint Eax;

        #endregion

        #region Control registers

        [Register(RegisterType.Control | RegisterType.FramePointer)]
        [FieldOffset(0xb4)]
        public uint Ebp;

        [Register(RegisterType.Control | RegisterType.ProgramCounter)]
        [FieldOffset(0xb8)]
        public uint Eip;

        [Register(RegisterType.Segments)]
        [FieldOffset(0xbc)]
        public uint Cs;

        [Register(RegisterType.General)]
        [FieldOffset(0xc0)]
        public uint EFlags;

        [Register(RegisterType.Control | RegisterType.StackPointer)]
        [FieldOffset(0xc4)]
        public uint Esp;

        [Register(RegisterType.Segments)]
        [FieldOffset(0xc8)]
        public uint Ss;

        #endregion

        [FieldOffset(0xcc)]
        public unsafe fixed byte ExtendedRegisters[512];
    }
}
