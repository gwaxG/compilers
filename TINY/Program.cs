using TINY;

var program = 
    "\tx := 5;\n" +
    "x := y + 5;\n" +
    "{sdf = sdf \n" +
    "dsf qsdf }\n" +
    "y :=     \t 2 + 2;";

var scanner = new Scanner(program.ToArray<char>());

var tokens = new List<Token>();
Token? tok = null;
while (tok is null || !(tok.Type == TokenType.PROGEND || tok.Type == TokenType.ERROR))
{
    tok = scanner.GetToken();
    tokens.Add(tok);
    if (tok.Value is not null)
        Console.WriteLine($"lex:{tok.Lexeme} line:{tok.Line} col:{tok.Column} type:{tok.Type} value:{tok.Value}");
    else
        Console.WriteLine($"lex:{tok.Lexeme} line:{tok.Line} col:{tok.Column} type:{tok.Type}");
    if (tok.Type == TokenType.ERROR)
    {
        Console.WriteLine("Error in scanning");
        Environment.Exit(-1);
    }
}

var parser = new Parser(tokens.ToArray());
var root = parser.CreateSyntaxTree();

if (parser.Error)
{
    foreach (var err in parser.Errors)
    {
        Console.WriteLine(err);
    } 
    Environment.Exit(-1);
}

parser.ShowTree(root, 0);


