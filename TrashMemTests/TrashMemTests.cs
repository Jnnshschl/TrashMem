using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TrashMem.Tests
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
        private const int STATIC_ADDRESS_CHAR = 0x133A48C;
        private const int STATIC_ADDRESS_INT16 = 0x133A490;
        private const int STATIC_ADDRESS_INT32 = 0x133A494;
        private const int STATIC_ADDRESS_INT64 = 0x133A498;
        private const int STATIC_ADDRESS_STRING = 0xFCE478C;

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
    }
}