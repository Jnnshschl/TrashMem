#include "dllmain.hpp"

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

	// function pointer
	unsigned int p_output_int32_cdecl = 0x241340;
	// function pointer to function typedef
	cdecl_output_int32* func_output_int32_cdecl
		= (cdecl_output_int32*)p_output_int32_cdecl;

	unsigned int p_output_int32_stdcall = 0x241360;
	stdcall_output_int32* func_output_int32_stdcall
		= (stdcall_output_int32*)p_output_int32_stdcall;

	unsigned int p_output_int32_fastcall = 0x241380;
	fastcall_output_int32* func_output_int32_fastcall
		= (fastcall_output_int32*)p_output_int32_fastcall;

	while (g_running) {
		//# Simple function calls
		/*
		__asm
		{
			CALL p_output_trash_cdecl
			CALL p_output_int32_stdcall
			CALL p_output_int32_fastcall
		};
		*/

		//# Function calls with 2 integer args
		// CDECL, simple stuff, need to clean the stack
		// => output_int32_cdecl(4711, 1337)
		/*
		__asm
		{
			PUSH 1337
			PUSH 4711
			CALL p_output_int32_cdecl
			ADD ESP, 0x8
		};
		*/

		// STDCALL, simple stuff, 
		// no need to clean the stack
		// => output_int32_stdcall(4711, 1337)
		/*
		__asm
		{
			PUSH 1337
			PUSH 4711
			CALL p_output_int32_stdcall
		};
		*/

		// FASTCALL, this trash follows no standard, 
		// it may not work with some applications
		// => output_int32_fastcall(4711, 1337)
		/*
		__asm
		{
			MOV EAX, 4711
			MOV EBX, 1337
			CALL p_output_int32_fastcall
		};
		*/

		// THISCALL, will follow...

		//# Non asm calls
		func_output_int32_cdecl(4711, 1337);
		func_output_int32_stdcall(4711, 1337);
		func_output_int32_fastcall(4711, 1337);

		Sleep(1000);
	}
}