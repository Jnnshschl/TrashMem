using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TrashMemCore.Objects;

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
        private readonly IntPtr STATIC_ADDRESS_CHAR = new IntPtr(0x3AA48C);
        private readonly IntPtr STATIC_ADDRESS_INT16 = new IntPtr(0x3AA490);
        private readonly IntPtr STATIC_ADDRESS_INT32 = new IntPtr(0x3AA494);
        private readonly IntPtr STATIC_ADDRESS_INT64 = new IntPtr(0x3AA498);
        private readonly IntPtr STATIC_ADDRESS_STRING = new IntPtr(0x5B9A4785);

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
            Assert.AreEqual((ushort)0xFF, TrashMem.ReadUnmanaged<ushort>(STATIC_ADDRESS_INT16));
            Assert.AreEqual((uint)0xFFFF, TrashMem.ReadUnmanaged<uint>(STATIC_ADDRESS_INT32));
            Assert.AreEqual((ulong)0xFFFFFFFFFFFFFF, TrashMem.ReadUnmanaged<ulong>(STATIC_ADDRESS_INT64));
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
            => Assert.AreEqual(0xFF, TrashMem.ReadInt16(STATIC_ADDRESS_INT16));

        [TestMethod()]
        public void ReadInt16SafeTest()
            => Assert.AreEqual(0xFF, TrashMem.ReadInt16Safe(STATIC_ADDRESS_INT16));

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
        public void ReadUInt16Test()
            => Assert.AreEqual((ushort)0xFF, TrashMem.ReadUInt16(STATIC_ADDRESS_INT16));

        [TestMethod()]
        public void ReadUInt16SafeTest()
            => Assert.AreEqual((ushort)0xFF, TrashMem.ReadUInt16Safe(STATIC_ADDRESS_INT16));

        [TestMethod()]
        public void ReadUInt32Test()
            => Assert.AreEqual((uint)0xFFFF, TrashMem.ReadUInt32(STATIC_ADDRESS_INT32));

        [TestMethod()]
        public void ReadUInt32SafeTest()
            => Assert.AreEqual((uint)0xFFFF, TrashMem.ReadUInt32Safe(STATIC_ADDRESS_INT32));

        [TestMethod()]
        public void ReadUInt64Test()
            => Assert.AreEqual((ulong)0xFFFFFFFFFFFFFF, TrashMem.ReadUInt64(STATIC_ADDRESS_INT64));

        [TestMethod()]
        public void ReadUInt64SafeTest()
            => Assert.AreEqual((ulong)0xFFFFFFFFFFFFFF, TrashMem.ReadUInt64Safe(STATIC_ADDRESS_INT64));


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
            TrashMem.Asm.Clear();
            TrashMem.Asm.AddLine("MOV EAX, 1");
            TrashMem.Asm.Assemble();
            TrashMem.Asm.Clear();
        }

        [TestMethod()]
        public void MemoryAllocFreeTest()
        {
            MemoryAllocation memAlloc = TrashMem.AllocateMemory(32);
            Assert.IsNotNull(memAlloc);
            Assert.IsTrue(memAlloc.Address.ToInt32() != 0x0);
            Assert.IsTrue(memAlloc.Free());
        }

        [TestMethod()]
        public void MemoryAllocFreeMultiCharsBytesTest()
        {
            MemoryAllocation memAlloc = TrashMem.AllocateMemory(32);

            byte[] sampleBytes = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 };
            TrashMem.WriteBytes(memAlloc.Address, sampleBytes);
            byte[] bytesRead = TrashMem.ReadChars(memAlloc.Address, 5);

            Assert.AreEqual(sampleBytes[0], bytesRead[0]);
            Assert.AreEqual(sampleBytes[1], bytesRead[1]);
            Assert.AreEqual(sampleBytes[2], bytesRead[2]);
            Assert.AreEqual(sampleBytes[3], bytesRead[3]);
            Assert.AreEqual(sampleBytes[4], bytesRead[4]);

            Assert.IsTrue(memAlloc.Free());
        }
    }
}