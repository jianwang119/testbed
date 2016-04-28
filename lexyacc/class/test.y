%{
#include <iostream>
#define YYSTYPE std::string
extern int yylex();
void yyerror(const char* msg);
%}
%token CLASS IDENTIFIER
%%
program:
       | program class
       | program error ';'
       ;
class: CLASS IDENTIFIER '{' program '}' ';' { 
	std::cout << "find class:" << $2 << std::endl; 
	std::cout << "program: " << $3 << std::endl; }
%%
void yyerror(const char* msg)
{
	std::cerr << "find error" << std::endl;
}
int main()
{
	yyparse();
	return 0;
}
