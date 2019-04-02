#include <Windows.h>

extern "C" __declspec(dllexport) bool WINAPI DllMain(HINSTANCE hInstance, DWORD fwdReason, LPVOID lpvReserved)
{
	switch (fwdReason)
	{
		case DLL_PROCESS_ATTACH:
			MessageBox(NULL, L"Dll injected", L"TrashMem.HookDll", MB_OK);
			break;

	}

	return true;
}