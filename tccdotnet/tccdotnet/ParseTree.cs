using System;
using System.Collections.Generic;
using System.Text;

namespace tccdotnet
{
    #region ParseTree
    public class ParseErrors : List<ParseError>
    {
    }

    public class ParseError
    {
        private string message;
        private int code;
        private int line;
        private int col;
        private int pos;
        private int length;

        public int Code { get { return code; } }
        public int Line { get { return line; } }
        public int Column { get { return col; } }
        public int Position { get { return pos; } }
        public int Length { get { return length; } }
        public string Message { get { return message; } }

        public ParseError(string message, int code, ParseNode node) : this(message, code,  0, node.Token.StartPos, node.Token.StartPos, node.Token.Length)
        {
        }

        public ParseError(string message, int code, int line, int col, int pos, int length)
        {
            this.message = message;
            this.code = code;
            this.line = line;
            this.col = col;
            this.pos = pos;
            this.length = length;
        }
    }

    // rootlevel of the node tree
    public partial class ParseTree : ParseNode
    {
        public ParseErrors Errors;

        public List<Token> Skipped;

        public ParseTree() : base(new Token(), "ParseTree")
        {
            Token.Type = TokenType.Start;
            Token.Text = "Root";
            Skipped = new List<Token>();
            Errors = new ParseErrors();
        }

        public string PrintTree()
        {
            StringBuilder sb = new StringBuilder();
            int indent = 0;
            PrintNode(sb, this, indent);
            return sb.ToString();
        }

        private void PrintNode(StringBuilder sb, ParseNode node, int indent)
        {
            
            string space = "".PadLeft(indent, ' ');

            sb.Append(space);
            sb.AppendLine(node.Text);

            foreach (ParseNode n in node.Nodes)
                PrintNode(sb, n, indent + 2);
        }
        
        /// <summary>
        /// this is the entry point for executing and evaluating the parse tree.
        /// </summary>
        /// <param name="paramlist">additional optional input parameters</param>
        /// <returns>the output of the evaluation function</returns>
        public object Eval(params object[] paramlist)
        {
            return Nodes[0].Eval(this, paramlist);
        }
    }

    public partial class ParseNode
    {
        protected string text;
        protected List<ParseNode> nodes;
        
        public List<ParseNode> Nodes { get {return nodes;} }
        
        public ParseNode Parent;
        public Token Token; // the token/rule
        public string Text { // text to display in parse tree 
            get { return text;} 
            set { text = value; }
        } 

        public virtual ParseNode CreateNode(Token token, string text)
        {
            ParseNode node = new ParseNode(token, text);
            node.Parent = this;
            return node;
        }

        protected ParseNode(Token token, string text)
        {
            this.Token = token;
            this.text = text;
            this.nodes = new List<ParseNode>();
        }

        protected object GetValue(ParseTree tree, TokenType type, int index)
        {
            return GetValue(tree, type, ref index);
        }

        protected object GetValue(ParseTree tree, TokenType type, ref int index)
        {
            object o = null;
            if (index < 0) return o;

            // left to right
            foreach (ParseNode node in nodes)
            {
                if (node.Token.Type == type)
                {
                    index--;
                    if (index < 0)
                    {
                        o = node.Eval(tree);
                        break;
                    }
                }
            }
            return o;
        }

        /// <summary>
        /// this implements the evaluation functionality, cannot be used directly
        /// </summary>
        /// <param name="tree">the parsetree itself</param>
        /// <param name="paramlist">optional input parameters</param>
        /// <returns>a partial result of the evaluation</returns>
        internal object Eval(ParseTree tree, params object[] paramlist)
        {
            object Value = null;

            switch (Token.Type)
            {
                case TokenType.Start:
                    Value = EvalStart(tree, paramlist);
                    break;
                case TokenType.Header:
                    Value = EvalHeader(tree, paramlist);
                    break;
                case TokenType.GlobalDecl:
                    Value = EvalGlobalDecl(tree, paramlist);
                    break;
                case TokenType.Pointer:
                    Value = EvalPointer(tree, paramlist);
                    break;
                case TokenType.ArithmeticOperator:
                    Value = EvalArithmeticOperator(tree, paramlist);
                    break;
                case TokenType.Array:
                    Value = EvalArray(tree, paramlist);
                    break;
                case TokenType.ArrAssignment:
                    Value = EvalArrAssignment(tree, paramlist);
                    break;
                case TokenType.ArrContent:
                    Value = EvalArrContent(tree, paramlist);
                    break;
                case TokenType.VarArray:
                    Value = EvalVarArray(tree, paramlist);
                    break;
                case TokenType.LocalDecl:
                    Value = EvalLocalDecl(tree, paramlist);
                    break;
                case TokenType.Assignment:
                    Value = EvalAssignment(tree, paramlist);
                    break;
                case TokenType.DecAssignment:
                    Value = EvalDecAssignment(tree, paramlist);
                    break;
                case TokenType.Expr:
                    Value = EvalExpr(tree, paramlist);
                    break;
                case TokenType.Atom:
                    Value = EvalAtom(tree, paramlist);
                    break;
                case TokenType.Identifier:
                    Value = EvalIdentifier(tree, paramlist);
                    break;
                case TokenType.ParPass:
                    Value = EvalParPass(tree, paramlist);
                    break;
                case TokenType.Function:
                    Value = EvalFunction(tree, paramlist);
                    break;
                case TokenType.Parameters:
                    Value = EvalParameters(tree, paramlist);
                    break;
                case TokenType.ParArray:
                    Value = EvalParArray(tree, paramlist);
                    break;
                case TokenType.CodeBlock:
                    Value = EvalCodeBlock(tree, paramlist);
                    break;
                case TokenType.Break:
                    Value = EvalBreak(tree, paramlist);
                    break;
                case TokenType.Switch:
                    Value = EvalSwitch(tree, paramlist);
                    break;
                case TokenType.SwitchCase:
                    Value = EvalSwitchCase(tree, paramlist);
                    break;
                case TokenType.CaseComp:
                    Value = EvalCaseComp(tree, paramlist);
                    break;
                case TokenType.Statement:
                    Value = EvalStatement(tree, paramlist);
                    break;
                case TokenType.If:
                    Value = EvalIf(tree, paramlist);
                    break;
                case TokenType.Condition:
                    Value = EvalCondition(tree, paramlist);
                    break;
                case TokenType.CondLogExpr:
                    Value = EvalCondLogExpr(tree, paramlist);
                    break;
                case TokenType.CondExpr:
                    Value = EvalCondExpr(tree, paramlist);
                    break;
                case TokenType.Else:
                    Value = EvalElse(tree, paramlist);
                    break;
                case TokenType.IfForLoopBlock:
                    Value = EvalIfForLoopBlock(tree, paramlist);
                    break;
                case TokenType.For:
                    Value = EvalFor(tree, paramlist);
                    break;
                case TokenType.ForDeclaration:
                    Value = EvalForDeclaration(tree, paramlist);
                    break;
                case TokenType.ForAssignment:
                    Value = EvalForAssignment(tree, paramlist);
                    break;
                case TokenType.While:
                    Value = EvalWhile(tree, paramlist);
                    break;
                case TokenType.DoWhile:
                    Value = EvalDoWhile(tree, paramlist);
                    break;
                case TokenType.IncDec:
                    Value = EvalIncDec(tree, paramlist);
                    break;
                case TokenType.WhileLoopBlock:
                    Value = EvalWhileLoopBlock(tree, paramlist);
                    break;
                case TokenType.Printf:
                    Value = EvalPrintf(tree, paramlist);
                    break;
                case TokenType.Scanf:
                    Value = EvalScanf(tree, paramlist);
                    break;
                case TokenType.Return:
                    Value = EvalReturn(tree, paramlist);
                    break;
                case TokenType.BuiltInFunc:
                    Value = EvalBuiltInFunc(tree, paramlist);
                    break;
                case TokenType.Getch:
                    Value = EvalGetch(tree, paramlist);
                    break;
                case TokenType.Clrscr:
                    Value = EvalClrscr(tree, paramlist);
                    break;

                default:
                    Value = Token.Text;
                    break;
            }
            return Value;
        }

        protected virtual object EvalStart(ParseTree tree, params object[] paramlist)
        {
            return "Could not interpret input; no semantics implemented.";
        }

        protected virtual object EvalHeader(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalGlobalDecl(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalPointer(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalArithmeticOperator(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalArray(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalArrAssignment(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalArrContent(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalVarArray(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalLocalDecl(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalAssignment(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalDecAssignment(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalExpr(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalAtom(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalIdentifier(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalParPass(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalFunction(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalParameters(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalParArray(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalCodeBlock(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalBreak(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalSwitch(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalSwitchCase(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalCaseComp(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalStatement(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalIf(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalCondition(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalCondLogExpr(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalCondExpr(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalElse(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalIfForLoopBlock(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalFor(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalForDeclaration(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalForAssignment(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalWhile(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalDoWhile(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalIncDec(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalWhileLoopBlock(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalPrintf(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalScanf(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalReturn(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalBuiltInFunc(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalGetch(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }

        protected virtual object EvalClrscr(ParseTree tree, params object[] paramlist)
        {
            throw new NotImplementedException();
        }


    }
    
    #endregion ParseTree
}
