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

## Methods

Should be pretty self explaining unless otherwise specified...

```c#
// Ctor
TrashMem trashMem = new TrashMem(process);

// Possible Accessrights
// READ = 0x10 | WRITE = 0x20 | READWRITE = 0x30 | ALL = 0x1F0FFF
// for more see this page 
// => https://docs.microsoft.com/de-de/windows/desktop/ProcThread/process-security-and-access-rights
TrashMem trashMem = new TrashMem(process, ACCESS_RIGHTS);

// Generic Reading
public unsafe T ReadUnmanaged<T>(uint address);
public T ReadStruct<T>(uint address);
public string ReadString(uint address, Encoding encoding, int lenght);

// 1 byte char
public unsafe byte ReadChar(uint address);
public byte ReadCharSafe(uint address);

// X bytes chars
public byte[] ReadChars(uint address);

// 2 byte short
public unsafe short ReadInt16(uint address);
public short ReadInt16Safe(uint address);

public unsafe ushort ReadUInt16(uint address);
public ushort ReadUInt16Safe(uint address);

// 4 byte int
public unsafe int ReadInt32(uint address);
public int ReadInt32Safe(uint address);

public unsafe uint ReadUInt32(int address);
public uint ReadUInt32Safe(uint address);

// 8 byte long
public unsafe long ReadInt64(uint address);
public long ReadInt64Safe(uint address);

public unsafe ulong ReadUInt64(uint address);
public ulong ReadUInt64Safe(uint address);

// Generic Writing
public bool Write<T>(uint address, T value, int size = 0);
public bool WriteBytes(uint address, byte[] value);
public bool WriteString(uint address, string text, Encoding encoding);

// Memory Allocation/Freeing
public uint AllocateMemory(int size);
public bool FreeMemory(uint address);
```
