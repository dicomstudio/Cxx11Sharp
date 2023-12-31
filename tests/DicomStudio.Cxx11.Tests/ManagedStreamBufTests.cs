﻿using System.IO;
using System.Text;
using Xunit;

namespace DicomStudio.Cxx11.Tests
{
    public class ManagedStreamBufTests
    {
        [Fact]
        public void DefaultTest()
        {
            var bytes = Encoding.ASCII.GetBytes("BufferedStreamWrapperTests");
            var src = new MemoryStream();
            src.Write(bytes);
            src.Position = 0;
            var dst = new MemoryStream();
            var srcWrap = new ManagedStreamBuf(src);
            var dstWrap = new ManagedStreamBuf(dst);
            srcWrap.CopyTo(dstWrap);

            Assert.Equal(bytes, dst.ToArray());
        }
    }
}