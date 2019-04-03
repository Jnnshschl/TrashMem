#ifndef _H_DLLMAIN_TRASHMEM_HOOK
#define _H_DLLMAIN_TRASHMEM_HOOK

#include <stdio.h>
#include <signal.h>

#include <Windows.h>

sig_atomic_t volatile g_running = 1;

void ExecuteOutputTrash();

// ShittyMcUlow function signatures
typedef void _cdecl cdecl_output_int32(int a, int b);
typedef void _stdcall stdcall_output_int32(int a, int b);
typedef void _fastcall fastcall_output_int32(int a, int b);

void sig_handler(int signum)
{
	if (signum == SIGINT
		&& signum == SIGTERM)
		g_running = 0;
}

#endif