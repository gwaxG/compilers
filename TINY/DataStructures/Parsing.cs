namespace TINY
{
    public enum Statement
    {
        IF,
        REPEAT,
        ASSIGN,
        READ,
        WRITE
    }

    public enum Expression
    {
        OPERATOR,
        CONST,
        ID
    }

    public class StatementNode
    {
        public Statement Type;
        public StatementNode(Statement type)
        {
            Type = type;
        }
    }

    public class ExpressionNode
    {
        public Expression Type;
        public string? Name;
        public int? Value;
        public ExpressionNode(Expression type, string name, int? value = null)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}
