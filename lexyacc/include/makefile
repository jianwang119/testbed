LEX=lex
YACC=yacc
CC=g++

test:lex.yy.o y.tab.o main.o
	$(CC) lex.yy.o y.tab.o main.o -o test

lex.yy.o:lex.yy.c y.tab.h main.hpp
	$(CC) -c lex.yy.c

y.tab.o:y.tab.c main.hpp
	$(CC) -c y.tab.c

main.o:main.hpp main.cpp
	$(CC) -c main.cpp

y.tab.c y.tab.h:frame.y
	$(YACC) -d frame.y

lex.yy.c:frame.l
	$(LEX) frame.l

clean:
	rm -f test *.o *.c *.h
