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
// READ = 0x10 | WRITE = 0x20 | READWRITE = 0x30
// for more see this page 
// => https://docs.microsoft.com/de-de/windows/desktop/ProcThread/process-security-and-access-rights
TrashMem trashMem = new TrashMem(process, ACCESS_RIGHTS);

// Generic Reading
public unsafe T ReadUnmanaged<T>(int address);
public T ReadStruct<T>(int address);
public string ReadString(int address, Encoding encoding, int lenght);

// 1 byte char
public unsafe byte ReadChar(int address);
public byte ReadCharSafe(int address);

// 2 byte short
public unsafe short ReadInt16(int address);
public short ReadInt16Safe(int address);

// 4 byte int
public unsafe int ReadInt32(int address);
public int ReadInt32Safe(int address);

// 8 byte long
public unsafe long ReadInt64(int address);
public long ReadInt64Safe(int address);
```