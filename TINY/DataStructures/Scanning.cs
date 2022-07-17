namespace TINY
{
    public enum TokenType
    {
        // Reserved
        IF,
        THEN,
        ELSE,
        END,
        REPEAT,
        UNTIL,
        READ,
        WRITE,

        // Symbols
        PLUS,
        MINUS,
        MULTIPLICATION,
        DIVISION,
        EQUAL,
        LESS,
        LEFTBRACKET,
        RIGHTBRACKET,
        COMMADOT,
        ASSIGN,

        // User-defined
        NUM,
        ID,

        // System
        PROGEND,
        ERROR
    }
    public class Token
    {
        public TokenType Type;
        public string? Lexeme;
        public int? Value;
        public int Line;
        public int Column;
    }

}
