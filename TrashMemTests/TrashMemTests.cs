using TrashMemCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TrashMemCore.Tests
{
    internal struct TestStruct
    {
        public int a;
        public int b;
        public long c;
    }

    [TestClass()]
    public class TrashMemTests
    {
        /// <summary>
        /// You need to use the ShittyMcUlow application from my GitHub
        /// to make thse tests work, maybe i will build some internal stuff
        /// but who knows...
        /// </summary>
        private const uint STATIC_ADDRESS_CHAR = 0x30A48C;
        private const uint STATIC_ADDRESS_INT16 = 0x30A490;
        private const uint STATIC_ADDRESS_INT32 = 0x30A494;
        private const uint STATIC_ADDRESS_INT64 = 0x30A498;
        private const uint STATIC_ADDRESS_STRING = 0x5B1A4785;

        private TrashMem TrashMem;

        [TestInitialize]
        public void Setup()
        {
            Process[] testProcesses = Process.GetProcessesByName("ShittyMcUlow");

            if (testProcesses.Length > 0)
            {
                TrashMem = new TrashMem(testProcesses.ToList().First());
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void TrashMemReadGenericTest()
        {
            Assert.AreEqual(0xF, TrashMem.ReadUnmanaged<byte>(STATIC_ADDRESS_CHAR));
            Assert.AreEqual(0xFF, TrashMem.ReadUnmanaged<short>(STATIC_ADDRESS_INT16));
            Assert.AreEqual(0xFFFF, TrashMem.ReadUnmanaged<int>(STATIC_ADDRESS_INT32));
            Assert.AreEqual(0xFFFFFFFFFFFFFF, TrashMem.ReadUnmanaged<long>(STATIC_ADDRESS_INT64));
        }

        [TestMethod()]
        public void ReadStruct()
        {
            TestStruct t = new TestStruct() { a = 0xFF, b = 0xFFFF, c = 0xFFFFFFFFFFFFFF };
            TestStruct r = TrashMem.ReadStruct<TestStruct>(STATIC_ADDRESS_INT16);
            Assert.AreEqual(t, r);
        }

        [TestMethod()]
        public void ReadString()
            => Assert.AreEqual("ShittyMcUlow", TrashMem.ReadString(STATIC_ADDRESS_STRING, Encoding.ASCII, 12));

        [TestMethod()]
        public void ReadCharTest()
            => Assert.AreEqual(0xF, TrashMem.ReadChar(STATIC_ADDRESS_CHAR));

        [TestMethod()]
        public void ReadCharSafeTest()
            => Assert.AreEqual(0xF, TrashMem.ReadCharSafe(STATIC_ADDRESS_CHAR));

        [TestMethod()]
        public void ReadInt16Test()
            => Assert.AreEqual(0xFF, TrashMem.ReadInt32(STATIC_ADDRESS_INT16));

        [TestMethod()]
        public void ReadInt16SafeTest()
            => Assert.AreEqual(0xFF, TrashMem.ReadInt32Safe(STATIC_ADDRESS_INT16));

        [TestMethod()]
        public void ReadInt32Test()
            => Assert.AreEqual(0xFFFF, TrashMem.ReadInt32(STATIC_ADDRESS_INT32));

        [TestMethod()]
        public void ReadInt32SafeTest()
            => Assert.AreEqual(0xFFFF, TrashMem.ReadInt32Safe(STATIC_ADDRESS_INT32));

        [TestMethod()]
        public void ReadInt64Test()
            => Assert.AreEqual(0xFFFFFFFFFFFFFF, TrashMem.ReadInt64(STATIC_ADDRESS_INT64));

        [TestMethod()]
        public void ReadInt64SafeTest()
            => Assert.AreEqual(0xFFFFFFFFFFFFFF, TrashMem.ReadInt64Safe(STATIC_ADDRESS_INT64));


        [TestMethod()]
        public void WriteCharTest()
        {
            Assert.IsTrue(TrashMem.Write<byte>(STATIC_ADDRESS_CHAR, 0xF));
            Assert.AreEqual(0xF, TrashMem.ReadChar(STATIC_ADDRESS_CHAR));
            Assert.IsTrue(TrashMem.Write<byte>(STATIC_ADDRESS_CHAR, 0xE));
            Assert.AreEqual(0xE, TrashMem.ReadChar(STATIC_ADDRESS_CHAR));
            Assert.IsTrue(TrashMem.Write<byte>(STATIC_ADDRESS_CHAR, 0xF));
            Assert.AreEqual(0xF, TrashMem.ReadChar(STATIC_ADDRESS_CHAR));
        }


        [TestMethod()]
        public void WriteInt16Test()
        {
            Assert.IsTrue(TrashMem.Write<short>(STATIC_ADDRESS_INT16, 0xFF));
            Assert.AreEqual(0xFF, TrashMem.ReadInt32(STATIC_ADDRESS_INT16));
            Assert.IsTrue(TrashMem.Write<short>(STATIC_ADDRESS_INT16, 0xFE));
            Assert.AreEqual(0xFE, TrashMem.ReadInt32(STATIC_ADDRESS_INT16));
            Assert.IsTrue(TrashMem.Write<short>(STATIC_ADDRESS_INT16, 0xFF));
            Assert.AreEqual(0xFF, TrashMem.ReadInt32(STATIC_ADDRESS_INT16));
        }


        [TestMethod()]
        public void WriteInt32Test()
        {
            Assert.IsTrue(TrashMem.Write<int>(STATIC_ADDRESS_INT32, 0xFFFF));
            Assert.AreEqual(0xFFFF, TrashMem.ReadInt32(STATIC_ADDRESS_INT32));
            Assert.IsTrue(TrashMem.Write<int>(STATIC_ADDRESS_INT32, 0xFFFE));
            Assert.AreEqual(0xFFFE, TrashMem.ReadInt32(STATIC_ADDRESS_INT32));
            Assert.IsTrue(TrashMem.Write<int>(STATIC_ADDRESS_INT32, 0xFFFF));
            Assert.AreEqual(0xFFFF, TrashMem.ReadInt32(STATIC_ADDRESS_INT32));
        }

        [TestMethod()]
        public void WriteInt64Test()
        {
            Assert.IsTrue(TrashMem.Write<long>(STATIC_ADDRESS_INT64, 0xFFFFFFFFFFFFFF));
            Assert.AreEqual(0xFFFFFFFFFFFFFF, TrashMem.ReadInt64(STATIC_ADDRESS_INT64));
            Assert.IsTrue(TrashMem.Write<long>(STATIC_ADDRESS_INT64, 0xFFFFFFFFFFFFFE));
            Assert.AreEqual(0xFFFFFFFFFFFFFE, TrashMem.ReadInt64(STATIC_ADDRESS_INT64));
            Assert.IsTrue(TrashMem.Write<long>(STATIC_ADDRESS_INT64, 0xFFFFFFFFFFFFFF));
            Assert.AreEqual(0xFFFFFFFFFFFFFF, TrashMem.ReadInt64(STATIC_ADDRESS_INT64));
        }

        [TestMethod()]
        public void WriteStruct()
        {
            TestStruct t = new TestStruct() { a = 0xFF, b = 0xFFFF, c = 0xFFFFFFFFFFFFFF };
            Assert.IsTrue(TrashMem.Write<TestStruct>(STATIC_ADDRESS_INT16, t));
            TestStruct r = TrashMem.ReadStruct<TestStruct>(STATIC_ADDRESS_INT16);
            Assert.AreEqual(t, r);

            t = new TestStruct() { a = 0xFE, b = 0xFFFE, c = 0xFFFFFFFFFFFFFE };
            Assert.IsTrue(TrashMem.Write<TestStruct>(STATIC_ADDRESS_INT16, t));
            r = TrashMem.ReadStruct<TestStruct>(STATIC_ADDRESS_INT16);
            Assert.AreEqual(t, r);

            t = new TestStruct() { a = 0xFF, b = 0xFFFF, c = 0xFFFFFFFFFFFFFF };
            Assert.IsTrue(TrashMem.Write<TestStruct>(STATIC_ADDRESS_INT16, t));
            r = TrashMem.ReadStruct<TestStruct>(STATIC_ADDRESS_INT16);
            Assert.AreEqual(t, r);
        }

        [TestMethod()]
        public void WriteStringTest()
        {
            Assert.IsTrue(TrashMem.WriteString(STATIC_ADDRESS_STRING, "ShittyMcUlow", Encoding.ASCII));
            Assert.AreEqual("ShittyMcUlow", TrashMem.ReadString(STATIC_ADDRESS_STRING, Encoding.ASCII, 12));
            Assert.IsTrue(TrashMem.WriteString(STATIC_ADDRESS_STRING, "ShittyMcUlox", Encoding.ASCII));
            Assert.AreEqual("ShittyMcUlox", TrashMem.ReadString(STATIC_ADDRESS_STRING, Encoding.ASCII, 12));
            Assert.IsTrue(TrashMem.WriteString(STATIC_ADDRESS_STRING, "ShittyMcUlow", Encoding.ASCII));
            Assert.AreEqual("ShittyMcUlow", TrashMem.ReadString(STATIC_ADDRESS_STRING, Encoding.ASCII, 12));
        }

        [TestMethod()]
        public void FasmTest()
        {
            TrashMem.FasmNet.Clear();
            TrashMem.FasmNet.AddLine("MOV EAX, 1");
            TrashMem.FasmNet.Assemble();
            TrashMem.FasmNet.Clear();
        }

        [TestMethod()]
        public void MemoryAllocFreeTest()
        {
            uint memaddr = TrashMem.AllocateMemory(32);
            Assert.IsTrue(TrashMem.FreeMemory(memaddr));
        }

        [TestMethod()]
        public void MemoryAllocFreeMultiCharsBytesTest()
        {
            uint memaddr = TrashMem.AllocateMemory(32);

            byte[] sampleBytes = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 };
            TrashMem.WriteBytes(memaddr, sampleBytes);
            byte[] bytesRead = TrashMem.ReadChars(memaddr, 5);

            Assert.AreEqual(sampleBytes[0], bytesRead[0]);
            Assert.AreEqual(sampleBytes[1], bytesRead[1]);
            Assert.AreEqual(sampleBytes[2], bytesRead[2]);
            Assert.AreEqual(sampleBytes[3], bytesRead[3]);
            Assert.AreEqual(sampleBytes[4], bytesRead[4]);

            Assert.IsTrue(TrashMem.FreeMemory(memaddr));
        }
    }
}