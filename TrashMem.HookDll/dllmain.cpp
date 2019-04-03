#include "dllmain.hpp"

sig_atomic_t volatile g_running = 1;

void sig_handler(int signum)
{
	if (signum == SIGINT)
		g_running = 0;
}

extern "C" __declspec(dllexport) bool WINAPI DllMain(HINSTANCE hInstance, DWORD fwdReason, LPVOID lpvReserved)
{
	switch (fwdReason)
	{
		case DLL_PROCESS_ATTACH:
			signal(SIGINT, sig_handler);

			ExecuteOutputTrash();
			break;

	}

	return true;
}

void ExecuteOutputTrash()
{
	// functions from ShittyMcUlow.exe
	unsigned int func_output_trash_cdecl = 0x2412E0;
	unsigned int func_output_trash_stdcall = 0x241300;
	unsigned int func_output_trash_fastcall = 0x241320;

	unsigned int func_output_int32_cdecl = 0x241340;
	unsigned int func_output_int32_stdcall = 0x241360;
	unsigned int func_output_int32_fastcall = 0x241380;

	while (g_running) {
		//# Simple function calls
		/*
		__asm
		{
			CALL func_output_trash_cdecl
			CALL func_output_trash_stdcall
			CALL func_output_trash_fastcall
		};
		*/

		//# Function calls with 2 integer args
		// CDECL, simple stuff, need to clean the stack
		// => output_int32_cdecl(4711, 1337)
		__asm
		{
			PUSH 1337
			PUSH 4711
			CALL func_output_int32_cdecl
			ADD ESP, 0x8
		};

		// STDCALL, simple stuff, 
		// no need to clean the stack
		// => output_int32_stdcall(4711, 1337)
		__asm
		{
			PUSH 1337
			PUSH 4711
			CALL func_output_int32_stdcall
		};

		// FASTCALL, this trash follows no standard, 
		// it may not work with some applications
		// => output_int32_fastcall(4711, 1337)
		__asm
		{
			MOV EAX, 4711
			MOV EBX, 1337
			CALL func_output_int32_fastcall
		};

		// THISCALL, will follow...

		Sleep(1000);
	}
}