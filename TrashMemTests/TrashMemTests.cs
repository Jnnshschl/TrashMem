using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrashMem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TrashMem.Tests
{
    [TestClass()]
    public class TrashMemTests
    {
        /// <summary>
        /// You need to use the ShittyMcUlow application from my GitHub
        /// to make thse tests work, maybe i will build some internal stuff
        /// but who knows...
        /// </summary>
        const int STATIC_ADDRESS = 0xBEA14C;

        [TestMethod()]
        public void TrashMemReadTest()
        {
            Process[] testProcesses = Process.GetProcessesByName("ShittyMcUlow");

            if (testProcesses.Length > 0)
            {
                TrashMem TrashMem = new TrashMem(testProcesses.ToList().First());
                int value = TrashMem.Read<int>(STATIC_ADDRESS);
                Assert.AreEqual(1337, value);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void ReadIntTest()
        {
            Process[] testProcesses = Process.GetProcessesByName("ShittyMcUlow");

            if (testProcesses.Length > 0)
            {
                TrashMem TrashMem = new TrashMem(testProcesses.ToList().First());
                int value = TrashMem.ReadInt(STATIC_ADDRESS);
                Assert.AreEqual(1337, value);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}