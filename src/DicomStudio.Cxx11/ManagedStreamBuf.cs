using System.IO;

namespace DicomStudio.Cxx11
{
    /// <summary>
    /// Buffered stream wrapper.
    /// </summary>
    public sealed class ManagedStreamBuf /*: Stream*/
    {
        private readonly Stream _stream;
        private readonly ManagedStreamBufInternal _managedStreamBufInternal;

        /// <summary>
        /// Buffered stream wrapper.
        /// </summary>
        /// <param name="stream"></param>
        public ManagedStreamBuf(Stream stream)
        {
            _stream = stream;
            _managedStreamBufInternal = new ManagedStreamBufInternal(Read, Write);
        }

        private int Read(byte[] buffer, int count)
        {
            try
            {
                return _stream.Read(buffer, 0, count);
            }
            catch
            {
                // https://www.mono-project.com/docs/advanced/pinvoke/#runtime-exception-propagation
                // https://github.com/dotnet/runtime/issues/4756
                return -1;
            }
        }

        private int Write(byte[] buffer, int count)
        {
            try
            {
                _stream.Write(buffer, 0, count);
                return count;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Return the std::streambuf* handle.
        /// </summary>
        /// <param name="managedStreamBuf"></param>
        /// <returns></returns>
        public static implicit operator StreamBufHandle(ManagedStreamBuf managedStreamBuf)
        {
            return managedStreamBuf._managedStreamBufInternal;
        }

        public void CopyTo(ManagedStreamBuf destination)
        {
            _managedStreamBufInternal.CopyTo(destination._managedStreamBufInternal);
        }
    }
}