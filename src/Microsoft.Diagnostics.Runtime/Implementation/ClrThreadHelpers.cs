﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Diagnostics.Runtime.DacInterface;
using Microsoft.Diagnostics.Runtime.Interfaces;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Diagnostics.Runtime.Implementation
{
    internal class ClrThreadHelpers : IClrThreadHelpers
    {
        private readonly ClrDataProcess _dac;
        private readonly SOSDac _sos;

        public IDataReader DataReader { get; }

        public ClrThreadHelpers(ClrDataProcess dac, SOSDac sos, IDataReader dataReader)
        {
            _dac = dac;
            _sos = sos;
            DataReader = dataReader;
        }

        public IEnumerable<ClrStackRoot> EnumerateStackRoots(ClrThread thread)
        {
            using SOSStackRefEnum? stackRefEnum = _sos.EnumerateStackRefs(thread.OSThreadId);
            if (stackRefEnum is null)
                yield break;

            ClrStackFrame[] stack = thread.EnumerateStackTrace().Take(2048).ToArray();

            ClrAppDomain? domain = thread.CurrentAppDomain;
            ClrHeap heap = thread.Runtime.Heap;
            StackRefData[] refs = new StackRefData[1024];

            const int GCInteriorFlag = 1;
            const int GCPinnedFlag = 2;
            int fetched = 0;
            while ((fetched = stackRefEnum.ReadStackReferences(refs)) != 0)
            {
                for (uint i = 0; i < fetched && i < refs.Length; ++i)
                {
                    if (refs[i].Object == 0)
                        continue;

                    bool interior = (refs[i].Flags & GCInteriorFlag) == GCInteriorFlag;
                    bool isPinned = (refs[i].Flags & GCPinnedFlag) == GCPinnedFlag;

                    ClrStackFrame? frame = stack.SingleOrDefault(f => f.StackPointer == refs[i].Source || f.StackPointer == refs[i].StackPointer && f.InstructionPointer == refs[i].Source);
                    frame ??= new ClrStackFrame(thread, null, refs[i].Source, refs[i].StackPointer, ClrStackFrameKind.Unknown, null, null);

                    if (interior)
                    {
                        // Check if the value lives on the heap.
                        ulong obj = refs[i].Object;
                        ClrSegment? segment = heap.GetSegmentByAddress(obj);

                        // If not, this may be a pointer to an object.
                        if (segment is null && DataReader.ReadPointer(obj, out obj))
                            segment = heap.GetSegmentByAddress(obj);

                        // Only yield return if we find a valid object on the heap
                        if (segment is not null)
                            yield return new ClrStackRoot(refs[i].Address, heap.GetObject(obj), isInterior: true, isPinned: isPinned, heap: heap, frame: frame);
                    }
                    else
                    {
                        // It's possible that heap.GetObjectType could return null and we construct a bad ClrObject, but this should
                        // only happen in the case of heap corruption and obj.IsValidObject will return null, so this is fine.
                        ClrObject obj = heap.GetObject(refs[i].Object);
                        yield return new ClrStackRoot(refs[i].Address, obj, isInterior: false, isPinned: isPinned, heap: heap, frame: frame);
                    }
                }
            }
        }

        public IEnumerable<ClrStackFrame> EnumerateStackTrace(ClrThread thread, bool includeContext)
        {
            using ClrStackWalk? stackwalk = _dac.CreateStackWalk(thread.OSThreadId, 0xf);
            if (stackwalk is null)
                yield break;

            int ipOffset;
            int spOffset;
            int contextSize;
            uint contextFlags = 0;
            if (DataReader.Architecture == Architecture.Arm)
            {
                ipOffset = 64;
                spOffset = 56;
                contextSize = 416;
            }
            else if (DataReader.Architecture == Architecture.Arm64)
            {
                ipOffset = 264;
                spOffset = 256;
                contextSize = 912;
            }
            else if (DataReader.Architecture == Architecture.X86)
            {
                ipOffset = 184;
                spOffset = 196;
                contextSize = 716;
                contextFlags = 0x1003f;
            }
            else // Architecture.X64
            {
                ipOffset = 248;
                spOffset = 152;
                contextSize = 1232;
                contextFlags = 0x10003f;
            }

            byte[] context = ArrayPool<byte>.Shared.Rent(contextSize);
            try
            {
                do
                {
                    if (!stackwalk.GetContext(contextFlags, contextSize, out _, context))
                        break;

                    ulong ip = context.AsSpan().AsPointer(ipOffset);
                    ulong sp = context.AsSpan().AsPointer(spOffset);

                    ulong frameVtbl = stackwalk.GetFrameVtable();
                    if (frameVtbl != 0)
                    {
                        sp = frameVtbl;
                        frameVtbl = DataReader.ReadPointer(sp);
                    }

                    byte[]? contextCopy = null;
                    if (includeContext)
                    {
                        contextCopy = context.AsSpan(0, contextSize).ToArray();
                    }

                    ClrStackFrame frame = GetStackFrame(thread, contextCopy, ip, sp, frameVtbl);
                    yield return frame;
                } while (stackwalk.Next().IsOK);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(context);
            }
        }

        private ClrStackFrame GetStackFrame(ClrThread thread, byte[]? context, ulong ip, ulong sp, ulong frameVtbl)
        {
            ClrRuntime runtime = thread.Runtime;

            // todo: pull Method from enclosing type, don't generate methods without a parent
            if (frameVtbl != 0)
            {

                ClrMethod? innerMethod = null;
                string frameName = _sos.GetFrameName(frameVtbl);

                ulong md = _sos.GetMethodDescPtrFromFrame(sp);
                if (md != 0)
                    innerMethod = runtime.GetMethodByHandle(md);

                return new ClrStackFrame(thread, context, ip, sp, ClrStackFrameKind.Runtime, innerMethod, frameName);
            }
            else
            {
                ClrMethod? method = runtime.GetMethodByInstructionPointer(ip);
                return new ClrStackFrame(thread, context, ip, sp, ClrStackFrameKind.ManagedMethod, method, null);
            }
        }
    }
}