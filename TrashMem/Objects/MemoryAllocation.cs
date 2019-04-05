using System;
using TrashMemCore.Win32;

namespace TrashMemCore.Objects
{
    public class MemoryAllocation
    {
        public uint Address { get; private set; }
        public int Size { get; private set; }

        private uint ProcessHandle { get; set; }

        public MemoryAllocation(uint processHandle, int size)
        {
            ProcessHandle = processHandle;
            Size = size;
        }

        public bool Free()
        {
            return Kernel32.VirtualFreeEx(ProcessHandle, Address, 0, 0x8000);
        }

        public bool Allocate(ProtectionType protectionType = ProtectionType.PAGE_EXECUTE_READWRITE)
        {
            Address = Kernel32.VirtualAllocEx(
                ProcessHandle,
                0x0,
                Size,
                (uint)Memory.COMMIT,
                (uint)protectionType
            );

            return Address != 0x0;
        }

        public override string ToString()
        {
            return $"{Size} byte => 0x{Address.ToString("X")}";
        }
    }
}
