﻿using Microsoft.Diagnostics.Runtime.Utilities;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Microsoft.Diagnostics.Runtime.DacInterface.SOSDac;

namespace Microsoft.Diagnostics.Runtime.DacInterface
{
    /// <summary>
    /// This is an undocumented, untested, and unsupported interface.  Do not use directly.
    /// </summary>
    public sealed unsafe class SOSDac13 : CallableCOMWrapper
    {
        internal static readonly Guid IID_ISOSDac13 = new("3176a8ed-597b-4f54-a71f-83695c6a8c5d");

        public SOSDac13(DacLibrary library, IntPtr ptr)
            : base(library?.OwningLibrary, IID_ISOSDac13, ptr)
        {
        }

        private ref readonly ISOSDac13VTable VTable => ref Unsafe.AsRef<ISOSDac13VTable>(_vtable);

        public HResult TraverseLoaderHeap(ulong heap, LoaderHeapKind kind, LoaderHeapTraverse callback)
        {
            HResult hr = VTable.TraverseLoaderHeap(Self, heap, kind, Marshal.GetFunctionPointerForDelegate(callback));
            GC.KeepAlive(callback);
            return hr;
        }

        public ClrDataAddress GetDomainLoaderAllocator(ClrDataAddress domainAddress)
        {
            if (domainAddress == 0)
                return 0;

            HResult hr = VTable.GetDomainLoaderAllocator(Self, domainAddress, out ClrDataAddress loaderAllocator);
            return hr ? loaderAllocator : 0;
        }

        public string[] GetLoaderAllocatorHeapNames()
        {
            HResult hr = VTable.GetLoaderAllocatorHeapNames(Self, 0, null, out int needed);
            if (hr && needed > 0)
            {
                nint[] pointers = new nint[needed];
                fixed (nint* ptr = pointers)
                {
                    if (hr = VTable.GetLoaderAllocatorHeapNames(Self, needed, ptr, out _))
                    {
                        string[] result = new string[needed];
                        for (int i = 0; i < needed; i++)
                            result[i] = Marshal.PtrToStringAnsi(pointers[i]) ?? "";

                        return result;
                    }
                }
            }

            return Array.Empty<string>();
        }

        public (ClrDataAddress Address, LoaderHeapKind Kind)[] GetLoaderAllocatorHeaps(ClrDataAddress loaderAllocator)
        {
            if (loaderAllocator != 0)
            {
                HResult hr = VTable.GetLoaderAllocatorHeaps(Self, loaderAllocator, 0, null, null, out int needed);

                if (hr && needed > 0)
                {
                    ClrDataAddress[] addresses = new ClrDataAddress[needed];
                    LoaderHeapKind[] kinds = new LoaderHeapKind[needed];

                    fixed (ClrDataAddress* ptrAddresses = addresses)
                    fixed (LoaderHeapKind* ptrKinds = kinds)
                    {
                        if (hr = VTable.GetLoaderAllocatorHeaps(Self, loaderAllocator, addresses.Length, ptrAddresses, ptrKinds, out _))
                        {
                            var result = new (ClrDataAddress, LoaderHeapKind)[needed];
                            for (int i = 0; i < needed; i++)
                                result[i] = (addresses[i], kinds[i]);

                            return result;
                        }
                    }
                }
            }

            return Array.Empty<(ClrDataAddress, LoaderHeapKind)>();
        }

        /// <summary>
        /// The type of the underlying loader heap.
        /// </summary>
        public enum LoaderHeapKind
        {
            /// <summary>
            /// A LoaderHeap in the CLR codebase.
            /// </summary>
            LoaderHeapKindNormal = 0,

            /// <summary>
            /// An ExplicitControlLoaderHeap in the CLR codebase.
            /// </summary>
            LoaderHeapKindExplicitControl = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly unsafe struct ISOSDac13VTable
        {
            public readonly delegate* unmanaged[Stdcall]<nint, ClrDataAddress, LoaderHeapKind, nint, int> TraverseLoaderHeap;
            public readonly delegate* unmanaged[Stdcall]<nint, ClrDataAddress, out ClrDataAddress, int> GetDomainLoaderAllocator;
            public readonly delegate* unmanaged[Stdcall]<nint, int, nint*, out int, int> GetLoaderAllocatorHeapNames;
            public readonly delegate* unmanaged[Stdcall]<nint, ClrDataAddress, int, ClrDataAddress*, LoaderHeapKind*, out int, int> GetLoaderAllocatorHeaps;
        }
    }
}
