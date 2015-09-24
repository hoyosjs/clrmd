﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Collections.Generic;

namespace Microsoft.Diagnostics.Runtime.Utilities
{
    using Pdb;
    /// <summary>
    /// An object that can map offsets in an IL stream to source locations and block scopes.
    /// </summary>
    internal sealed class PdbReader :
      IDisposable
    {
        private IEnumerable<PdbSource> _sources;
        private Dictionary<uint, PdbFunction> _pdbFunctionMap = new Dictionary<uint, PdbFunction>();
        private List<StreamReader> _sourceFilesOpenedByReader = new List<StreamReader>();
        private int _ver;
        private int _sig;
        private int _age;
        private Guid _guid;

        /// <summary>
        /// Allocates an object that can map some kinds of ILocation objects to IPrimarySourceLocation objects. 
        /// For example, a PDB reader that maps offsets in an IL stream to source locations.
        /// </summary>
        public PdbReader(Stream pdbStream)
        {
#if NO
      this.host = host;
#endif
            foreach (PdbFunction pdbFunction in PdbFile.LoadFunctions(pdbStream, true, out _ver, out _sig, out _age, out _guid, out _sources))
                _pdbFunctionMap[pdbFunction.token] = pdbFunction;
        }

        public IEnumerable<PdbSource> Sources { get { return _sources; } }

        public IEnumerable<PdbFunction> Functions { get { return _pdbFunctionMap.Values; } }

        public int Version { get { return _ver; } }
        public Guid Signature { get { return _guid; } }
        public int Age { get { return _age; } }

        /// <summary>
        /// Closes all of the source files that have been opened to provide the contents source locations corresponding to IL offsets.
        /// </summary>
        public void Dispose()
        {
            this.Close();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes all of the source files that have been opened to provide the contents source locations corresponding to IL offsets.
        /// </summary>
        ~PdbReader()
        {
            this.Close();
        }

        private void Close()
        {
            foreach (var source in _sourceFilesOpenedByReader)
                source.Dispose();
        }

        public PdbFunction GetPdbFunctionFor(uint methodToken)
        {
            PdbFunction result = null;
            _pdbFunctionMap.TryGetValue(methodToken, out result);
            return result;
        }
    }
}