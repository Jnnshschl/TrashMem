﻿using Fasm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TrashMemCore.Objects;
using TrashMemCore.SizeManager;
using TrashMemCore.Win32;

namespace TrashMemCore
{
    public class TrashMem
    {
        public Process Process { get; private set; }
        public uint ProcessHandle { get; private set; }
        public uint Kernel32ModuleHandle { get; private set; }
        public uint LoadLibraryAAddress { get; private set; }

        public CachedSizeManager CachedSizeManager { get; private set; }

        public ManagedFasm Asm { get; private set; }

        public List<MemoryAllocation> MemoryAllocations { get; private set; }
        public List<RemoteThread> RemoteThreads { get; private set; }

        /// <summary>
        /// TrashMem instance, use it to do all the stuff you want.
        /// 
        /// Remember to give admin privileges.
        /// </summary>
        /// <param name="process">Process you want to edit</param>
        /// <param name="accessRights">
        /// READ = 0x10 | WRITE = 0x20 | READWRITE = 0x30
        /// => https://docs.microsoft.com/de-de/windows/desktop/ProcThread/process-security-and-access-rights
        /// </param>
        public TrashMem(Process process, ProcessAccess accessRights = ProcessAccess.PROCESS_ALL_ACCESS)
        {
            CachedSizeManager = new CachedSizeManager();            
            MemoryAllocations = new List<MemoryAllocation>();
            RemoteThreads = new List<RemoteThread>();

            Process = process;
            ProcessHandle = Kernel32.OpenProcess((uint)accessRights, false, process.Id);
            Kernel32ModuleHandle = Kernel32.GetModuleHandle("kernel32.dll");
            LoadLibraryAAddress = Kernel32.GetProcAddress(Kernel32ModuleHandle, "LoadLibraryA");

            Asm = new ManagedFasm(new IntPtr(ProcessHandle));
        }

        public void Detach()
        {
            foreach (MemoryAllocation memoryAllocation in MemoryAllocations)
            {
                memoryAllocation.Free();
            }
        }

        #region Read => Generic
        /// <summary>
        /// Read anything unmanaged from memory. 
        /// 
        /// May be a bit slower than using the specialized 
        /// function like ReadInt(), just saying...
        /// </summary>
        /// <typeparam name="T">Type of thing to read</typeparam>
        /// <param name="address">address to read it from</param>
        /// <returns>the value or default(T) if it failed</returns>
        public unsafe T ReadUnmanaged<T>(uint address, int size = 0) where T : unmanaged
        {
            if (size == 0)
            {
                size = sizeof(T);
            }

            int numBytesRead = 0;
            byte[] readBuffer = new byte[size];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, size, ref numBytesRead))
            {
                fixed (byte* ptr = readBuffer)
                {
                    return *(T*)ptr;
                }
            }
            return default;
        }

        /// <summary>
        /// Read any struct from memory. 
        /// </summary>
        /// <typeparam name="T">Type of thing to read</typeparam>
        /// <param name="address">address to read it from</param>
        /// <returns>the struct or default if it failed</returns>
        public T ReadStruct<T>(uint address)
        {
            int size = Marshal.SizeOf(typeof(T));
            int numBytesRead = 0;
            uint readBuffer = (uint)Marshal.AllocHGlobal(size).ToInt32();

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, size, ref numBytesRead))
            {
                return (T)Marshal.PtrToStructure(new IntPtr(readBuffer), typeof(T));
            }
            return default;
        }

        /// <summary>
        /// Read a string from memory
        /// </summary>
        /// <param name="address">address to read it from</param>
        /// <param name="encoding">Encoding to use</param>
        /// <param name="lenght">lenght of the string to read</param>
        /// <returns>the string read from memory</returns>
        public string ReadString(uint address, Encoding encoding, int lenght = 0)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[lenght];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, lenght, ref numBytesRead))
            {
                List<byte> newBytes = new List<byte>();

                foreach (byte b in readBuffer)
                {
                    if (b == 0b0)
                    {
                        break;
                    }

                    newBytes.Add(b);
                }

                return encoding.GetString(newBytes.ToArray()).Trim();
            }
            return "";
        }
        #endregion

        #region Read => X Bytes Chars/Bytes
        /// <summary>
        /// Read a char/byte from memory.
        /// 
        /// Uses the BitConverter instead
        /// of the unsafe code.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the short value or 0 if it failed</returns>
        public byte[] ReadChars(uint address, int size)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[size];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, size, ref numBytesRead))
            {
                return readBuffer;
            }
            return new byte[] { 0x0 };
        }
        #endregion

        #region Read => 1 Byte Char/Byte
        /// <summary>
        /// Read a char/byte from memory.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the short value or 0 if it failed</returns>
        public unsafe byte ReadChar(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[1];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 1, ref numBytesRead))
            {
                fixed (byte* ptr = readBuffer)
                {
                    return *ptr;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read a char/byte from memory.
        /// 
        /// Uses the BitConverter instead
        /// of the unsafe code.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the short value or 0 if it failed</returns>
        public byte ReadCharSafe(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[1];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 1, ref numBytesRead))
            {
                return readBuffer[0];
            }
            return 0;
        }
        #endregion

        #region Read => 2 Byte Short
        /// <summary>
        /// Read a short from memory.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the short value or 0 if it failed</returns>
        public unsafe short ReadInt16(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[2];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 2, ref numBytesRead))
            {
                fixed (byte* ptr = readBuffer)
                {
                    return *(short*)ptr;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read a short from memory.
        /// 
        /// Uses the BitConverter instead
        /// of the unsafe code.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the short value or 0 if it failed</returns>
        public short ReadInt16Safe(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[2];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 2, ref numBytesRead))
            {
                return BitConverter.ToInt16(readBuffer, 0);
            }
            return 0;
        }

        /// <summary>
        /// Read a short from memory.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the short value or 0 if it failed</returns>
        public unsafe ushort ReadUInt16(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[2];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 2, ref numBytesRead))
            {
                fixed (byte* ptr = readBuffer)
                {
                    return *(ushort*)ptr;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read a short from memory.
        /// 
        /// Uses the BitConverter instead
        /// of the unsafe code.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the short value or 0 if it failed</returns>
        public ushort ReadUInt16Safe(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[2];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 2, ref numBytesRead))
            {
                return BitConverter.ToUInt16(readBuffer, 0);
            }
            return 0;
        }
        #endregion

        #region Read => 4 Byte Integer
        /// <summary>
        /// Read a int from memory.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the int value or 0 if it failed</returns>
        public unsafe int ReadInt32(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[4];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 4, ref numBytesRead))
            {
                fixed (byte* ptr = readBuffer)
                {
                    return *(int*)ptr;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read a int from memory.
        /// 
        /// Uses the BitConverter instead
        /// of the unsafe code.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the int value or 0 if it failed</returns>
        public int ReadInt32Safe(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[4];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 4, ref numBytesRead))
            {
                return BitConverter.ToInt32(readBuffer, 0);
            }
            return 0;
        }

        /// <summary>
        /// Read a uint from memory.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the int value or 0 if it failed</returns>
        public unsafe uint ReadUInt32(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[4];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 4, ref numBytesRead))
            {
                fixed (byte* ptr = readBuffer)
                {
                    return *(uint*)ptr;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read a int from memory.
        /// 
        /// Uses the BitConverter instead
        /// of the unsafe code.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the int value or 0 if it failed</returns>
        public uint ReadUInt32Safe(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[4];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 4, ref numBytesRead))
            {
                return BitConverter.ToUInt32(readBuffer, 0);
            }
            return 0;
        }
        #endregion

        #region Read => 8 Byte Long
        /// <summary>
        /// Read a long from memory.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the long value or 0 if it failed</returns>
        public unsafe long ReadInt64(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[8];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 8, ref numBytesRead))
            {
                fixed (byte* ptr = readBuffer)
                {
                    return *(long*)ptr;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read a long from memory.
        /// 
        /// Uses the BitConverter instead
        /// of the unsafe code.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the long value or 0 if it failed</returns>
        public long ReadInt64Safe(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[8];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 8, ref numBytesRead))
            {
                return BitConverter.ToInt64(readBuffer, 0);
            }
            return 0;
        }

        /// <summary>
        /// Read a ulong from memory.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the long value or 0 if it failed</returns>
        public unsafe ulong ReadUInt64(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[8];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 8, ref numBytesRead))
            {
                fixed (byte* ptr = readBuffer)
                {
                    return *(ulong*)ptr;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read a long from memory.
        /// 
        /// Uses the BitConverter instead
        /// of the unsafe code.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the long value or 0 if it failed</returns>
        public ulong ReadUInt64Safe(uint address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[8];

            if (Kernel32.ReadProcessMemory(ProcessHandle, address, readBuffer, 8, ref numBytesRead))
            {
                return BitConverter.ToUInt64(readBuffer, 0);
            }
            return 0;
        }
        #endregion

        #region Write => Generic
        /// <summary>
        /// Write anything to memory.
        /// </summary>
        /// <typeparam name="T">Type of thing to read</typeparam>
        /// <param name="address">address to read it from</param>
        /// <param name="value">thing ti write</param>
        /// <param name="size">optional size</param>
        /// <returns>true if successful, false if it failed</returns>
        public bool Write<T>(uint address, T value, int size = 0)
        {
            if (size == 0)
            {
                size = CachedSizeManager.SizeOf(typeof(T));
            }

            uint writeBuffer = (uint)Marshal.AllocHGlobal(size).ToInt32();
            Marshal.StructureToPtr(value, new IntPtr(writeBuffer), false);

            int numBytesWritten = 0;
            bool result = Kernel32.WriteProcessMemory(ProcessHandle, address, writeBuffer, size, ref numBytesWritten);

            Marshal.DestroyStructure(new IntPtr(writeBuffer), typeof(T));
            Marshal.FreeHGlobal(new IntPtr(writeBuffer));

            return result;
        }


        /// <summary>
        /// Write anything to memory.
        /// </summary>
        /// <typeparam name="T">Type of thing to read</typeparam>
        /// <param name="address">address to read it from</param>
        /// <param name="value">thing ti write</param>
        /// <param name="size">optional size</param>
        /// <returns>true if successful, false if it failed</returns>
        public bool WriteBytes(uint address, byte[] value)
        {
            int numBytesWritten = 0;
            bool result = Kernel32.WriteProcessMemory(ProcessHandle, address, value, value.Length, ref numBytesWritten);
            return result;
        }

        /// <summary>
        /// Write a string to memory
        /// </summary>
        /// <param name="address">address to write it to</param>
        /// <param name="text">the actual text to write</param>
        /// <param name="encoding">Encoding to use</param>
        /// <returns>true if successful, false if it failed</returns>
        public bool WriteString(uint address, string text, Encoding encoding)
        {
            byte[] stringBytes = encoding.GetBytes(text);
            int size = stringBytes.Length;

            int numBytesWritten = 0;
            bool result = Kernel32.WriteProcessMemory(ProcessHandle, address, stringBytes, size, ref numBytesWritten);

            return result;
        }
        #endregion

        #region Memory
        /// <summary>
        /// Allocate Memory in the attached process
        /// </summary>
        /// <param name="size">size of the desired allocation</param>
        /// <returns>
        /// MemoryAllocation if memory got allocated,
        /// null if there were errors
        /// </returns>
        public MemoryAllocation AllocateMemory(int size)
        {
            MemoryAllocation memoryAllocation = new MemoryAllocation(ProcessHandle, size);
            if (memoryAllocation.Allocate())
            {
                MemoryAllocations.Add(memoryAllocation);
                return memoryAllocation;
            }
            return null;
        }


        /// <summary>
        /// Free Memory in the attached process
        /// </summary>
        /// <param name="memoryAllocation">MemoryAllocation to Free</param>
        /// <returns>
        /// true if successfully freed memory
        /// </returns>
        public bool FreeMemory(MemoryAllocation memoryAllocation)
        {
            if (memoryAllocation.Free())
            {
                MemoryAllocations.Remove(memoryAllocation);
                return true;
            }
            return false;
        }
        #endregion

        #region Injection/Threads
        public bool InjectDll(string dllPath)
        {
            string dllName = Path.GetFileName(dllPath);
            int dllNameSize = Encoding.ASCII.GetBytes(dllName).Length;

            MemoryAllocation memAlloc = AllocateMemory(dllNameSize);
            WriteString(memAlloc.Address, dllName, Encoding.ASCII);

            RemoteThreads.Add
            (
                new RemoteThread
                (
                    Kernel32.CreateRemoteThread
                    (
                        ProcessHandle,
                        0,
                        0,
                        LoadLibraryAAddress,
                        memAlloc.Address,
                        0,
                        0
                    )
                )
            );

            return true;
        }

        public bool TerminateThread(RemoteThread thread, uint exitCode = 0x0)
        {
            RemoteThreads.Remove(thread);
            return Kernel32.TerminateThread(thread.Handle, exitCode);
        }

        public bool CloseHandle(uint handle)
        {
            return Kernel32.CloseHandle(handle);
        }
        #endregion
    }
}