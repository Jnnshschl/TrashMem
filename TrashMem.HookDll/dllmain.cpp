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
	unsigned int func_output_trash_cdecl = 0xD71280;
	unsigned int func_output_trash_stdcall = 0xD712A0;
	unsigned int func_output_trash_fastcall = 0xD712C0;

	while (g_running) {
		__asm
		{
			CALL func_output_trash_cdecl
			CALL func_output_trash_stdcall
			CALL func_output_trash_fastcall
		};

		Sleep(1000);
	}
}