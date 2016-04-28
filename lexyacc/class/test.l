%{
#include <string>
#define YYSTYPE std::string
#include "y.tab.h"
#define LEX_RETURN(arg) yylval=yytext;return arg
%}
d [0-9]
l [a-z]
u [A-Z]
a {l}|{u}
%%
[;{}]               { LEX_RETURN(yytext[0]); }
"class"             { LEX_RETURN(CLASS); }
(_|{a})(_|{a}|{d})* { LEX_RETURN(IDENTIFIER); }
[ \t\tn]
.
%%
int yywrap()
{
return 1;
}

