using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TINY
{
    public class Scanner
    {
        private Dictionary<string, TokenType> _reserved;
        private Dictionary<string, TokenType> _symbols;
        private char[] _chars;
        private int _pos;
        private int _line;
        private int _lastLinePos;

        public Scanner(char[] chars)
        {
            _reserved = new Dictionary<string, TokenType>
            {
                {"if",  TokenType.IF},
                {"then",  TokenType.THEN},
                {"else",  TokenType.ELSE},
                {"end",  TokenType.END},
                {"repeat",  TokenType.REPEAT},
                {"until",  TokenType.UNTIL},
                {"read",  TokenType.READ},
                {"write",  TokenType.WRITE},

            };

            _symbols = new Dictionary<string, TokenType>
            {
                {"+",  TokenType.PLUS},
                {"-",  TokenType.MINUS},
                {"/",  TokenType.DIVISION},
                {"*",  TokenType.MULTIPLICATION},
                {"=",  TokenType.EQUAL},
                {"<",  TokenType.LESS},
                {"(",  TokenType.LEFTBRACKET},
                {")",  TokenType.RIGHTBRACKET},
                {";",  TokenType.SEMI},
                {":=",  TokenType.ASSIGN},
                {"{",  TokenType.OPENCOMMENT},
                {"}",  TokenType.CLOSECOMMENT},
            };

            _line = 0;
            _pos = 0;
            _lastLinePos = 0;
            _chars = chars;
        }

        public Token GetToken()
        {
            // Check for end of program
            if (_pos == _chars.Length)
                return CreateToken("");

            var current = _chars[_pos++];
            // Skip space
            if (current == ' ') { return GetToken(); }
            // Special symbol
            else if (IsSpecial(current)) { return System(current); }
            // Operator
            else if (_symbols.ContainsKey(Char.ToString(current)) || current == ':') { return Operator(current); }
            // User-defined
            else if (Char.IsLetter(current)) { return UserDefined(current); }
            // Number
            else if (Char.IsDigit(current)) { return Number(current); }
            else { return Error(current); }
        }

        private Token Error(char current, string msg = null)
        {

            return new Token()
            {
                Lexeme = $"{msg ?? "Unrecognized symbol"} {current.ToString()} at line {_line}, column {GetCol()}",
                Type = TokenType.ERROR,
                Column = GetCol(),
                Line = _line,
                Value = null
            };
        }

        private bool IsSpecial(char current) => current == '\t' || current == '\n' ? true : false;

        private Token System(char current)
        {
            if (current == '\t')
            {
                return GetToken();
            }
            else if (current == '\n')
            {
                _lastLinePos = _pos;
                _line++;
                return GetToken();
            }
            // Unsupported special symbol
            else
            {
                return Error(current);
            }
        }

        private Token UserDefined(char current)
        {
            var lexeme = Char.ToString(current);
            while (true)
            {
                current = _chars[_pos++];
                if (Char.IsLetter(current) || Char.IsDigit(current))
                    lexeme += Char.ToString(current);
                else
                {
                    _pos--;
                    return CreateToken(lexeme);
                }
            }
        }

        private Token Operator(char current)
        {
            // Throw an error on closing comment section without starting.
            if (current == '}')
                return Error(current, "Closing comment block without opening");

            // Skip comments.
            if (current == '{')
            {
                while (true)
                {
                    current = _chars[_pos++];
                    if (current == '\n')
                        _line++;
                    if (current == '}')
                        return GetToken();
                    if (_pos == _chars.Length)
                        return Error(current);
                }
            }

            if (current == ':' && _chars[_pos++] == '=')
            {
                return CreateToken(":=");
            }
            else
            {
                return CreateToken(Char.ToString(current));
            }
        }

        private Token Number(char current)
        {
            var lexeme = Char.ToString(current);
            while (true)
            {
                current = _chars[_pos++];
                if (Char.IsDigit(current))
                    lexeme += Char.ToString(current);
                else
                {
                    _pos--;
                    return CreateToken(lexeme, true);
                }
            }
        }

        private Token CreateToken(string lexeme, bool isNumeric = false)
        {
            TokenType type;

            if (lexeme == "") { type = TokenType.PROGEND; }
            else if (isNumeric) { type = TokenType.NUM; }
            else if (_reserved.ContainsKey(lexeme)) { type = _reserved[lexeme]; }
            else if (_symbols.ContainsKey(lexeme)) { type = _symbols[lexeme]; }
            else { type = TokenType.ID; }

            return new Token()
            {
                Lexeme = lexeme,
                Type = type,
                Column = GetCol(),
                Line = _line,
                Value = type != TokenType.NUM ? null : int.Parse(lexeme)
            };
        }

        private int GetCol()
        {
            return _pos - _lastLinePos - 1;
        }
    }
}

