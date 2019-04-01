using System;
using TrashMem.Win32;

namespace TrashMem.Objects
{
    public class MemoryAllocation
    {
        public IntPtr Address { get; private set; }
        public int Size { get; private set; }

        private IntPtr ProcessHandle { get; set; }

        public MemoryAllocation(IntPtr processHandle, int size)
        {
            ProcessHandle = processHandle;
            Size = size;
        }

        public bool Free()
        {
            return Kernel32.VirtualFreeEx(ProcessHandle, Address, 0, 0x8000);
        }

        public bool Allocate()
        {
            Address = Kernel32.VirtualAllocEx(
                ProcessHandle,
                new IntPtr(0x0),
                Size,
                (uint)Memory.COMMIT,
                (uint)ProtectionType.PAGE_READWRITE
            );

            return Address.ToInt32() != 0x0;
        }
    }
}
