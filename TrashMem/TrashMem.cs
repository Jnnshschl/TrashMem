using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TrashMem
{
    public class TrashMem
    {
        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, IntPtr lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public Process Process { get; private set; }
        public int ProcessHandle { get; private set; }

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
        public TrashMem(Process process, int accessRights = 0x30)
        {
            Process = process;
            ProcessHandle = OpenProcess(accessRights, false, process.Id);
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
        public unsafe T ReadUnmanaged<T>(int address, int size = 0) where T : unmanaged
        {
            if (size == 0)
            {
                size = sizeof(T);
            }

            int numBytesRead = 0;
            byte[] readBuffer = new byte[size];

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, size, ref numBytesRead))
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
        public T ReadStruct<T>(int address)
        {
            int size = Marshal.SizeOf(typeof(T));
            int numBytesRead = 0;
            IntPtr readBuffer = Marshal.AllocHGlobal(size);

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, size, ref numBytesRead))
            {
                return (T)Marshal.PtrToStructure(readBuffer, typeof(T));
            }
            return default;
        }

        /// <summary>
        /// Read anything from memory. 
        /// 
        /// May be a bit slower than using the specialized 
        /// function like ReadInt(), just saying...
        /// </summary>
        /// <typeparam name="T">Type of thing to read</typeparam>
        /// <param name="address">address to read it from</param>
        /// <returns>the value or default(T) if it failed</returns>
        public string ReadString(int address, Encoding encoding, int lenght = 0)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[lenght];

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, lenght, ref numBytesRead))
            {
                return encoding.GetString(readBuffer);
            }
            return "";
        }
        #endregion

        #region Read => 1 Byte Char/Byte
        /// <summary>
        /// Read a char/byte from memory.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the short value or 0 if it failed</returns>
        public unsafe byte ReadChar(int address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[1];

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, 1, ref numBytesRead))
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
        public byte ReadCharSafe(int address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[1];

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, 1, ref numBytesRead))
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
        public unsafe short ReadInt16(int address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[2];

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, 2, ref numBytesRead))
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
        public short ReadInt16Safe(int address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[2];

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, 2, ref numBytesRead))
            {
                return BitConverter.ToInt16(readBuffer, 0);
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
        public unsafe int ReadInt32(int address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[4];

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, 4, ref numBytesRead))
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
        public int ReadInt32Safe(int address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[4];

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, 4, ref numBytesRead))
            {
                return BitConverter.ToInt32(readBuffer, 0);
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
        public unsafe long ReadInt64(int address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[8];

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, 8, ref numBytesRead))
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
        public long ReadInt64Safe(int address)
        {
            int numBytesRead = 0;
            byte[] readBuffer = new byte[8];

            if (ReadProcessMemory(ProcessHandle, address, readBuffer, 8, ref numBytesRead))
            {
                return BitConverter.ToInt64(readBuffer, 0);
            }
            return 0;
        }
        #endregion
    }
}
