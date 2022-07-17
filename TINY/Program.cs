using TINY;

var program = 
    "\tx = 5;\n" +
    "x = y + 5;\n" +
    "(sdf = sdf \n" +
    "dsf qsdf )\n" +
    "y =     \t 2 + 2;\n";

var scanner = new Scanner(program.ToArray<char>());

Token? tok = null;
while (tok is null || !(tok.Type == TokenType.PROGEND || tok.Type == TokenType.ERROR))
{
    tok = scanner.GetToken();
    if (tok.Value is not null)
        Console.WriteLine($"lex:{tok.Lexeme} line:{tok.Line} col:{tok.Column} type:{tok.Type} value:{tok.Value}");
    else
        Console.WriteLine($"lex:{tok.Lexeme} line:{tok.Line} col:{tok.Column} type:{tok.Type}");
}

