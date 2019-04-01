using System;

namespace TrashMem.Win32
{
    [Flags]
    public enum Memory : uint
    {
        COMMIT = 0x1000,
        RESERVE = 0x2000,
        MEM_DECOMMIT = 0x4000,
        MEM_RELEASE = 0x8000,
        RESET = 0x80000,
        UNDO = 0x1000000,
    }
}
