﻿using System;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace DicomStudio.Cxx11
{
    /// <summary>
    /// SafeHandle for a C++ streambuf.
    /// </summary>
    public sealed class StreamBufHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public StreamBufHandle() : base(ownsHandle: true)
        {
        }

        /// <summary>
        /// release the handle
        /// </summary>
        /// <returns>true</returns>
        protected override bool ReleaseHandle()
        {
            NativeLibrary.delete_managed_streambuf(handle);
            return true;
        }
    }

    /// <summary>
    /// Managed streambuf for C++.
    /// </summary>
    internal sealed class ManagedStreamBufInternal : IDisposable
    {
        // System.IO.Stream expects an input byte[] to read into,
        // since we do not want to go the `unsafe` road, create one here:
        // this also define the buffering (aka `setvbuf`) implicitly:
        // https://stackoverflow.com/questions/1862982/c-sharp-filestream-optimal-buffer-size-for-writing-large-files
        // System.IO.Stream uses this as default:
        // private const int DefaultCopyBufferSize = 81920;

        private readonly Func<byte[], int, int> _readFunction;
        private readonly Func<byte[], int, int> _writeFunction;
        private readonly NativeLibrary.ReadDelegate _readDelegate; // keep alive
        private readonly NativeLibrary.WriteDelegate _writeDelegate; // keep alive
        private readonly byte[] _buffer;
        private readonly StreamBufHandle _handle;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="readFunction">function to fill buffer</param>
        /// <param name="writeFunction">function to flush buffer</param>
        /// <param name="buffering">size of buffering</param>
        public ManagedStreamBufInternal(Func<byte[], int, int> readFunction, Func<byte[], int, int> writeFunction,
            int buffering = 4096)
        {
            _readFunction = readFunction;
            _writeFunction = writeFunction;
            _buffer = new byte[buffering];
            _readDelegate = Read;
            _writeDelegate = Write;
            _handle = NativeLibrary.create_managed_streambuf(_readDelegate, _writeDelegate, _buffer);

            if (_handle.IsInvalid)
                throw new ArgumentException("Invalid parameters");
        }

        private int Read()
        {
            return _readFunction(_buffer, _buffer.Length);
        }

        private int Write(int count)
        {
            return _writeFunction(_buffer, count);
        }

        public void CopyTo(StreamBufHandle destination)
        {
            var ret = NativeLibrary.copy_to_managed_streambuf(_handle, destination);
            if (ret < 0)
            {
                throw new IOException("CopyTo failure");
            }
        }

        /// <summary>
        /// Implicit conversion to StreamBufHandle.
        /// </summary>
        /// <param name="managedStreamBufInternal"></param>
        /// <returns></returns>
        public static implicit operator StreamBufHandle(ManagedStreamBufInternal managedStreamBufInternal)
        {
            return managedStreamBufInternal._handle;
        }

        /// <summary>
        /// Dispose implementation
        /// </summary>
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}