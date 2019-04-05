using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TrashMemCore.Win32
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(uint handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateThread(uint handle, uint dwExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(uint hProcess, uint lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(uint hProcess, uint lpBaseAddress, uint lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(uint hProcess, uint lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(uint hProcess, uint lpBaseAddress, uint lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint VirtualAllocEx(uint hProcess, uint dwAddress, int dwSize, uint dwAllocationType, uint dwProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFreeEx(uint hProcess, uint dwAddress, int dwSize, uint dwFreeType);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern uint GetProcAddress(uint hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint CreateRemoteThread(uint hProcess, uint lpThreadAttributes, uint dwStackSize, uint lpStartAddress, uint lpParameter, uint dwCreationFlags, uint lpThreadId);
    }
}
