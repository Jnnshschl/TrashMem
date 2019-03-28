using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TrashMem
{
    public class TrashMem
    {
        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

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

        /// <summary>
        /// Read anything from memory. 
        /// 
        /// May be a bit slower than using the specialized 
        /// function like ReadInt(), just saying...
        /// </summary>
        /// <typeparam name="T">Type of thing to read</typeparam>
        /// <param name="address">address to read it from</param>
        /// <returns>the value or default(T) if it failed</returns>
        public unsafe T Read<T>(int address) where T : unmanaged
        {
            int size = sizeof(T);
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
        /// Read an integer from memory.
        /// 
        /// This specialized function are a bit faster than Read<T>()
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>the interger value or 0 if it failed</returns>
        public unsafe int ReadInt(int address)
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
    }
}
