using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TINY
{

    public class Parser
    {
        private Token[] _tokens;

        private int _pnt = 0;

        public Parser(Token[] tokens)
        {
            _tokens = tokens;
            Errors = new List<string>();
        }

        // Holds current token.
        private Token _token => _tokens[_pnt];

        // Set the current token and and advance the counter.
        private void GetToken() 
        {
            _pnt++;
            // Console.WriteLine($"Next token is {_token.Type}");
        }

        public bool Error { get; private set; }
        
        public List<string> Errors { get; private set; }

        public TreeNode CreateSyntaxTree()
        {
            var root = StatementSequence();
            
            // We do not count the token of the program end.
            if (_pnt != _tokens.Length - 1)
                SyntaxError("Code end before file.");
            return root;
        }

        private void SyntaxError(string error)
        {
            Error = true;
            var msg = error + $"\nat line {_token.Line}";
            Errors.Add(msg);
            Console.WriteLine(msg);
        }

        private void Match(TokenType expected)
        {
            if (_token.Type == expected)
                GetToken();
            else
            {
                SyntaxError($"Unexpected token -> {expected}");
            }
        }

        private TreeNode StatementSequence() 
        {
            var t = Statement();
            var p = t;
            while( 
                _token.Type != TokenType.PROGEND &&
                _token.Type != TokenType.END &&
                _token.Type != TokenType.ELSE &&
                _token.Type != TokenType.UNTIL )
            {
                TreeNode q;
                Match(TokenType.SEMI);
                q = Statement();
                if (q != null)
                {
                    if (t == null)
                        t = p = q;
                    else
                    {
                        p.Sibling = q;
                        p = q;
                    }
                }
            }
            return t;
        }
        private TreeNode Statement() 
        {
            TreeNode t = null;
            switch (_token.Type)
            {
                case TokenType.IF:
                    t = IfStmt();
                    break;
                case TokenType.REPEAT:
                    t = RepeatStmt();
                    break;
                case TokenType.ID:
                    t = AssignStmt();
                    break;
                case TokenType.READ:
                    t = ReadStmt();
                    break;
                case TokenType.WRITE:
                    t = WriteStmt();
                    break;
                case TokenType.PROGEND:
                    break;
                default:
                    SyntaxError($"Unexpected token -> {_token.Type}");
                    GetToken();
                    break;
            }
            return t;
        }
        private TreeNode IfStmt() 
        {
            var t = new TreeNode(_token);
            Match(TokenType.IF);
            t.Children[0] = Exp();
            Match(TokenType.THEN);
            t.Children[1] = StatementSequence();
            if (_token.Type == TokenType.ELSE)
                t.Children[2] = StatementSequence();
            Match(TokenType.END);
            return t;
        }

        private TreeNode RepeatStmt() 
        {
            var t = new TreeNode(_token);
            Match(TokenType.REPEAT);
            t.Children[0] = StatementSequence();
            Match(TokenType.UNTIL);
            t.Children[1] = Exp();
            return t;
        }

        private TreeNode AssignStmt() 
        {
            var t = new TreeNode(_token);
            Match(TokenType.ID);
            Match(TokenType.ASSIGN);
            t.Children[0] = Exp();
            return t;
        }

        private TreeNode ReadStmt() 
        {
            var t = new TreeNode(_token);
            Match(TokenType.READ);
            Match(TokenType.ID);
            return t;
        }

        private TreeNode WriteStmt() 
        {
            var t = new TreeNode(_token);
            Match(TokenType.WRITE);
            t.Children[0] = Exp();
            return t;
        }

        private TreeNode Exp() 
        {
            var t = SimpleExp();
            if (_token.Type == TokenType.LESS || _token.Type == TokenType.EQUAL)
            {
                var p = new TreeNode(_token);
                p.Children[0] = t;
                t = p;
                Match(_token.Type);
                t.Children[1] = SimpleExp();
            }
            return t;
        }
        private TreeNode SimpleExp() 
        {
            var t = Term();
            while (_token.Type == TokenType.PLUS || _token.Type == TokenType.MINUS)
            {
                var p = new TreeNode(_token);
                p.Children[0] = t;
                t = p;
                Match(_token.Type);
                t.Children[1] = Term();
            }
            return t;
        }
        private TreeNode Term() 
        {
            var t = Factor();
            while(_token.Type == TokenType.MULTIPLICATION || _token.Type == TokenType.DIVISION)
            {
                var p = new TreeNode(_token);
                p.Children[0] = t;
                t = p;
                Match(_token.Type);
                p.Children[1] = Factor();
            }
            return t;
        }
        private TreeNode Factor() 
        {
            TreeNode t = null;
            switch(_token.Type)
            {
                case TokenType.NUM:
                    t = new TreeNode(_token);
                    Match(TokenType.NUM);
                    break;
                case TokenType.ID:
                    t = new TreeNode(_token);
                    Match(TokenType.ID);
                    break;
                case TokenType.LEFTBRACKET:
                    Match(TokenType.LEFTBRACKET);
                    t = Exp();
                    Match(TokenType.RIGHTBRACKET);
                    break;
                default:
                    SyntaxError($"unexpected token -> {_token}");
                    GetToken();
                    break;
            }
            return t;
        }

        public void ShowTree(TreeNode node, int shift)
        {
            var s = "";
            for (int i = 0; i < shift; i++)
                s += " ";

            if (node is not null)
            {
                Console.WriteLine(s + node.Token.Lexeme);
            }
            
            for (var i = 0; i < Globals.MAXCHILDREN; i++)
            {
                if (node.Children[i] != null)
                    ShowTree(node.Children[i], shift);
            }

            if (node.Sibling is not null)
            {
                ShowTree(node.Sibling, shift + 1);
            }

        }
    }
}
