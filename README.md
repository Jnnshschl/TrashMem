# TrashMem 

A memoryediting libary...

## Usage

```c#
const int STATIC_ADDRESS = 0xBEA14C;	// static offset from my ShittyMcUlow.exe program that will always be 1337

Process[] testProcesses = Process.GetProcessesByName("ShittyMcUlow"); 	// to find all ShittyMcUlow.exe processes
TrashMem TrashMem = new TrashMem(testProcesses.ToList().First());		// we will take the first

int value = TrashMem.Read<int>(STATIC_ADDRESS);
or (the generic method is slower than the direct method, keep that in mind...)
int value = TrashMem.ReadInt(STATIC_ADDRESS);

Assert.AreEqual(1337, value);	// will return true if you use ShittyMcUlow.exe
```

## Methods

Should be pretty self explaining unless otherwise specified...

```c#
public unsafe T Read<T>(int address)

public unsafe int ReadInt(int address)
```