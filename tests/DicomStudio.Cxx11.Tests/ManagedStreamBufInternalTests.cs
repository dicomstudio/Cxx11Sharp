using System;
using System.IO;
using Xunit;

namespace DicomStudio.Cxx11.Tests
{
    public class ManagedStreamBufInternalTests
    {
        private static int ReadEmpty(byte[] buffer, int count)
        {
            return 0;
        }

        private static int WriteEmpty(byte[] buffer, int count)
        {
            return 0;
        }

        private static int ReadError(byte[] buffer, int count)
        {
            return -1;
        }

        private static int WriteError(byte[] buffer, int count)
        {
            return -1;
        }

        [Fact]
        public void Buffer0_ShouldFail()
        {
            // act & assert
            Assert.Throws<ArgumentException>(() => new ManagedStreamBufInternal(ReadEmpty, WriteEmpty, 0));
        }

        [Fact]
        public void ReadWriteError_ShouldFail()
        {
            //arrange
            var sut1 = new ManagedStreamBufInternal(ReadError, WriteError);
            var sut2 = new ManagedStreamBufInternal(ReadError, WriteError);

            // act & assert
            Assert.Throws<IOException>(() => sut1.CopyTo(sut2));
        }
    }
}