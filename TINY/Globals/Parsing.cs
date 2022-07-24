namespace TINY
{
    public enum NodeKind
    {
        StmtK,
        ExpK
    }

    public enum StmtKind
    {
        IfK,
        RepeatK,
        AssignK,
        ReadK,
        WriteK
    }

    public enum ExpKind
    {
        OpK,
        ConstK,
        IdK
    }

    public enum ExpType
    {
        Void, 
        Integer, 
        Boolean
    }

    public static class Globals
    {
        public const int MAXCHILDREN = 3;
    }

    public class TreeNode
    {
        // Parser defined fields.
        public TreeNode[] Children = new TreeNode[Globals.MAXCHILDREN];
        public TreeNode? Sibling;
        // TreeNode defined fields.
        public Token Token;
        public int LineNumber { get => Token.Line; }
        public NodeKind NodeK;
        public StmtKind? StmtK;
        public ExpKind? ExpK;
        public ExpType? ExpT;

        public TreeNode(Token tok)
        {
            Token = tok;
            if (
                tok.Type == TokenType.REPEAT ||
                tok.Type == TokenType.UNTIL ||
                tok.Type == TokenType.IF ||
                tok.Type == TokenType.THEN ||
                tok.Type == TokenType.ELSE ||
                tok.Type == TokenType.END ||
                tok.Type == TokenType.READ ||
                tok.Type == TokenType.WRITE)
            {
                NodeK = NodeKind.StmtK;
            }
            else
            {
                NodeK = NodeKind.ExpK;
            }

            switch (NodeK)
            {
                case NodeKind.StmtK:
                    ExpK = null;
                    ExpT = null;
                    break;
                case NodeKind.ExpK:
                    if (tok.Type == TokenType.NUM)
                    {
                        ExpK = ExpKind.ConstK;
                        ExpT = ExpType.Integer;
                    }                        
                    else if (tok.Type == TokenType.ID)
                    {
                        ExpK = ExpKind.IdK;
                        ExpT = ExpType.Void;
                    }
                    else
                    {
                        ExpK = ExpKind.OpK;
                        ExpT = ExpType.Void;
                    }
                    break;
            }

        }
    }
}
