options {
    language  = "CSharp";
    namespace = "CooS.Interpret.CLI.Syntax";
}

class L extends Lexer;

// one-or-more letters followed by a newline
NAME:   ( 'a'..'z'|'A'..'Z' )+ NEWLINE
    ;
HASTHIS
	:	"HASTHIS"
	;
EXPLICITTHIS
	:	"EXPLICIT"
	;
NEWLINE
    :   '\r' '\n'   // DOS
    |   '\n'        // UNIX
    ;

class P extends Parser;

methodDefSig
	:	(HASTHIS (EXPLICITTHIS));
startRule
    :   n:NAME
        { System.out.println("Hi there, " + n.getText()); }
    ;
