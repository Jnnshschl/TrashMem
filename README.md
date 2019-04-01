# TrashMem 

A memoryediting libary...

## Usage

```c#
const int STATIC_ADDRESS = 0xBEA14C;	// static offset from my ShittyMcUlow.exe program

Process[] testProcesses = Process.GetProcessesByName("ShittyMcUlow"); 	// to find all ShittyMcUlow.exe processes
TrashMem TrashMem = new TrashMem(testProcesses.ToList().First());	// we will take the first

int value = TrashMem.ReadInt32(STATIC_ADDRESS);     // unsafe code
// or
int value = TrashMem.ReadUnmanaged<int>(STATIC_ADDRESS);    // generic
// or
int value = TrashMem.ReadInt32Safe(STATIC_ADDRESS); // No unsafe code
```

## Memory Modification

Should be pretty self explaining unless otherwise specified...

```c#
// Ctor
TrashMem trashMem = new TrashMem(process);

// Possible Accessrights
// see this page => https://docs.microsoft.com/de-de/windows/desktop/ProcThread/process-security-and-access-rights
TrashMem trashMem = new TrashMem(process, ACCESS_RIGHTS);

// Generic Reading
public unsafe T ReadUnmanaged<T>(IntPtr address);
public T ReadStruct<T>(IntPtr address);
public string ReadString(IntPtr address, Encoding encoding, int lenght);

// 1 byte char
public unsafe byte ReadChar(IntPtr address);
public byte ReadCharSafe(IntPtr address);

// X bytes chars
public byte[] ReadChars(IntPtr address);

// 2 byte short
public unsafe short ReadInt16(IntPtr address);
public short ReadInt16Safe(IntPtr address);

public unsafe ushort ReadUInt16(IntPtr address);
public ushort ReadUInt16Safe(IntPtr address);

// 4 byte int
public unsafe int ReadInt32(IntPtr address);
public int ReadInt32Safe(IntPtr address);

public unsafe uint ReadUInt32(int address);
public uint ReadUInt32Safe(IntPtr address);

// 8 byte long
public unsafe long ReadInt64(IntPtr address);
public long ReadInt64Safe(IntPtr address);

public unsafe ulong ReadUInt64(IntPtr address);
public ulong ReadUInt64Safe(IntPtr address);

// Generic Writing
public bool Write<T>(IntPtr address, T value, int size = 0);
public bool WriteBytes(IntPtr address, byte[] value);
public bool WriteString(IntPtr address, string text, Encoding encoding);
```

## Allocation/Freeing

```c#
// Memory Allocation/Freeing
MemoryAllocation memAlloc = TrashMem.AllocateMemory(4);

Trashmem.Write(memAlloc.Address, 1337);

TrashMem.FreeMemory(memAlloc);
```

## Assembly

```c#
// Memory Allocation/Freeing
MemoryAllocation memAlloc = TrashMem.AllocateMemory(32);

TrashMem.Asm.Clear();
byte[] asmBytes = TrashMem.Asm.Assemble("MOV EAX, 1");
Trashmem.WriteBytes(memAlloc.Address, asmBytes);

TrashMem.FreeMemory(memAlloc);
```

## Method Hooking

Coming soon!!!

## Debug Gui

This is a GUI to test the Allocation/Reading/Writing of Trashmem.

![alt text](https://raw.githubusercontent.com/Jnnshschl/TrashMem/master/images/trashmem_debug.png "Debug GUI")
