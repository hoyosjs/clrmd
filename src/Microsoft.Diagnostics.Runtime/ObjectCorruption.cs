﻿using System;

namespace Microsoft.Diagnostics.Runtime
{
    public class ObjectCorruption
    {
        public ClrObject Object { get; }
        public int Offset { get; }
        public ObjectCorruptionKind Kind { get; }

        /// <summary>
        /// The object's SyncBlock index, only filled for SyncBlock related failures.
        /// </summary>
        public int SyncBlockIndex { get; } = -1;

        /// <summary>
        /// The system's expected SyncBlock index for this object.  Only filled for
        /// SyncBlock related failures.
        /// </summary>
        public int ClrSyncBlockIndex { get; } = -1;

        public ObjectCorruption(ClrObject obj, int offset, ObjectCorruptionKind kind)
        {
            Object = obj;
            Offset = offset;
            Kind = kind;
        }

        public ObjectCorruption(ClrObject obj, int offset, ObjectCorruptionKind kind, int syncBlockIndex, int clrSyncBlockIndex)
        {
            Object = obj;
            Offset = offset;
            Kind = kind;
            SyncBlockIndex = syncBlockIndex;
            ClrSyncBlockIndex = clrSyncBlockIndex;
        }

        public override string ToString()
        {
            if (Kind == ObjectCorruptionKind.None)
                return "";

            string offset = "";
            if (Offset > 0)
                offset = $"+{Offset:x}";
            else if (Offset < 0)
                offset = $"-{Math.Abs(Offset):x}";

            string type = Object.Type?.Name != null ? $" {Object.Type.Name}" : "";

            return $"[{Kind}] {Object.Address:x}{offset}{type}";
        }
    }

    public enum ObjectCorruptionKind
    {
        None = 0,

        // Not object failures
        ObjectNotOnTheHeap,
        ObjectNotPointerAligned,

        // Object failures
        ObjectTooLarge = 10,
        InvalidMethodTable,
        InvalidThinlock,
        SyncBlockMismatch,
        SyncBlockZero,

        // Object reference failures
        ObjectReferenceNotPointerAligned = 100,
        InvalidObjectReference,
        FreeObjectReference,

        // Memory Read Errors
        CouldNotReadMethodTable = 200,
        CouldNotReadCardTable,
        CouldNotReadObject,
        CouldNotReadGCDesc,
    }
}
