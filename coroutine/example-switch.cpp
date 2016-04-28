#include <stdio.h>
#include <ucontext.h>
#include <unistd.h>

void func1(void* arg)
{
	puts("1");
	puts("11");
	puts("111");
	puts("1111");
}

void context_test()
{
	char stack[1024*1024];
	ucontext_t child, main;

	getcontext(&child);
	child.uc_stack.ss_sp = stack;
	child.uc_stack.ss_size = sizeof(stack);
	child.uc_stack.ss_flags = 0;
	child.uc_link = &main;

	makecontext(&child, (void (*)(void))func1, 0);

	swapcontext(&main, &child);
	puts("main");
}

int main()
{
	context_test();
	return 0;
}
