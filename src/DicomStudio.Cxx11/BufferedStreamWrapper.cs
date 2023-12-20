using System.IO;

namespace DicomStudio.Cxx11
{
    /// <summary>
    /// Buffered stream wrapper.
    /// </summary>
    public sealed class BufferedStreamWrapper /*: Stream*/
    {
        private readonly Stream _stream;
        private readonly ManagedStreamBuf _managedStreamBuf;

        /// <summary>
        /// Buffered stream wrapper.
        /// </summary>
        /// <param name="stream"></param>
        public BufferedStreamWrapper(Stream stream)
        {
            _stream = stream;
            _managedStreamBuf = new ManagedStreamBuf(Read, Write);
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

        public void CopyTo(BufferedStreamWrapper destination)
        {
            _managedStreamBuf.CopyTo(destination._managedStreamBuf
            );
        }
    }
}