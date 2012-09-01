using System;
using System.Collections.Generic;

namespace tccdotnet
{
    #region Parser

    public partial class Parser 
    {
        private Scanner scanner;
        private ParseTree tree;
        
        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

        public ParseTree Parse(string input)
        {
            tree = new ParseTree();
            return Parse(input, tree);
        }

        public ParseTree Parse(string input, ParseTree tree)
        {
            scanner.Init(input);

            this.tree = tree;
            ParseStart(tree);
            tree.Skipped = scanner.Skipped;

            return tree;
        }

        private void ParseStart(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Start), "Start");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.SHARP);
            while (tok.Type == TokenType.SHARP)
            {
                ParseHeader(node);
            tok = scanner.LookAhead(TokenType.SHARP);
            }

            
            tok = scanner.LookAhead(TokenType.DATATYPE, TokenType.VOID, TokenType.ASTERISK, TokenType.IDENTIFIER);
            while (tok.Type == TokenType.DATATYPE
                || tok.Type == TokenType.VOID
                || tok.Type == TokenType.ASTERISK
                || tok.Type == TokenType.IDENTIFIER)
            {
                ParseGlobalDecl(node);
            tok = scanner.LookAhead(TokenType.DATATYPE, TokenType.VOID, TokenType.ASTERISK, TokenType.IDENTIFIER);
            }

            
            tok = scanner.Scan(TokenType.EOF);
            if (tok.Type != TokenType.EOF)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EOF.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseHeader(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Header), "Header");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.SHARP);
            if (tok.Type != TokenType.SHARP)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SHARP.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.INCLUDE);
            if (tok.Type != TokenType.INCLUDE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.INCLUDE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.DIRECTIVE);
            if (tok.Type != TokenType.DIRECTIVE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DIRECTIVE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseGlobalDecl(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.GlobalDecl), "GlobalDecl");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.DATATYPE, TokenType.VOID);
            if (tok.Type == TokenType.DATATYPE
                || tok.Type == TokenType.VOID)
            {
                tok = scanner.LookAhead(TokenType.DATATYPE, TokenType.VOID);
                switch (tok.Type)
                {
                    case TokenType.DATATYPE:
                        tok = scanner.Scan(TokenType.DATATYPE);
                        if (tok.Type != TokenType.DATATYPE)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DATATYPE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    case TokenType.VOID:
                        tok = scanner.Scan(TokenType.VOID);
                        if (tok.Type != TokenType.VOID)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.VOID.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        break;
                }
            }

            
            tok = scanner.LookAhead(TokenType.ASTERISK);
            if (tok.Type == TokenType.ASTERISK)
            {
                ParsePointer(node);
            }

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            if (tok.Type != TokenType.IDENTIFIER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.LPAREN, TokenType.LBRACKET, TokenType.EQUALS, TokenType.COMMA, TokenType.SEMICOL);
            switch (tok.Type)
            {
                case TokenType.LPAREN:
                    tok = scanner.LookAhead(TokenType.LPAREN);
                    if (tok.Type == TokenType.LPAREN)
                    {
                        ParseFunction(node);
                    }
                    break;
                case TokenType.LBRACKET:
                case TokenType.EQUALS:
                case TokenType.COMMA:
                case TokenType.SEMICOL:

                    
                    tok = scanner.LookAhead(TokenType.LBRACKET, TokenType.EQUALS);
                    if (tok.Type == TokenType.LBRACKET
                        || tok.Type == TokenType.EQUALS)
                    {
                        tok = scanner.LookAhead(TokenType.LBRACKET, TokenType.EQUALS);
                        switch (tok.Type)
                        {
                            case TokenType.LBRACKET:
                                ParseArray(node);
                                break;
                            case TokenType.EQUALS:
                                ParseDecAssignment(node);
                                break;
                            default:
                                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                                break;
                        }
                    }

                    
                    tok = scanner.LookAhead(TokenType.COMMA);
                    while (tok.Type == TokenType.COMMA)
                    {

                        
                        tok = scanner.Scan(TokenType.COMMA);
                        if (tok.Type != TokenType.COMMA)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);

                        
                        tok = scanner.LookAhead(TokenType.ASTERISK);
                        if (tok.Type == TokenType.ASTERISK)
                        {
                            ParsePointer(node);
                        }

                        
                        tok = scanner.Scan(TokenType.IDENTIFIER);
                        if (tok.Type != TokenType.IDENTIFIER)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);

                        
                        tok = scanner.LookAhead(TokenType.LBRACKET);
                        if (tok.Type == TokenType.LBRACKET)
                        {
                            ParseArray(node);
                        }

                        
                        tok = scanner.LookAhead(TokenType.EQUALS);
                        if (tok.Type == TokenType.EQUALS)
                        {
                            ParseDecAssignment(node);
                        }
                    tok = scanner.LookAhead(TokenType.COMMA);
                    }

                    
                    tok = scanner.Scan(TokenType.SEMICOL);
                    if (tok.Type != TokenType.SEMICOL)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParsePointer(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Pointer), "Pointer");
            parent.Nodes.Add(node);

            do {
                tok = scanner.Scan(TokenType.ASTERISK);
                if (tok.Type != TokenType.ASTERISK)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.ASTERISK.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                tok = scanner.LookAhead(TokenType.ASTERISK);
            } while (tok.Type == TokenType.ASTERISK);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseArithmeticOperator(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ArithmeticOperator), "ArithmeticOperator");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.PLUS, TokenType.MINUS, TokenType.ASTERISK, TokenType.FSLASH, TokenType.PERCENT);
            switch (tok.Type)
            {
                case TokenType.PLUS:
                    tok = scanner.Scan(TokenType.PLUS);
                    if (tok.Type != TokenType.PLUS)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PLUS.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.MINUS:
                    tok = scanner.Scan(TokenType.MINUS);
                    if (tok.Type != TokenType.MINUS)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.MINUS.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.ASTERISK:
                    tok = scanner.Scan(TokenType.ASTERISK);
                    if (tok.Type != TokenType.ASTERISK)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.ASTERISK.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.FSLASH:
                    tok = scanner.Scan(TokenType.FSLASH);
                    if (tok.Type != TokenType.FSLASH)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.FSLASH.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.PERCENT:
                    tok = scanner.Scan(TokenType.PERCENT);
                    if (tok.Type != TokenType.PERCENT)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PERCENT.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseArray(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Array), "Array");
            parent.Nodes.Add(node);


            
            do {

                
                tok = scanner.Scan(TokenType.LBRACKET);
                if (tok.Type != TokenType.LBRACKET)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LBRACKET.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.Scan(TokenType.NUMBER);
                if (tok.Type != TokenType.NUMBER)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.Scan(TokenType.RBRACKET);
                if (tok.Type != TokenType.RBRACKET)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RBRACKET.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                tok = scanner.LookAhead(TokenType.LBRACKET);
            } while (tok.Type == TokenType.LBRACKET);

            
            tok = scanner.LookAhead(TokenType.EQUALS);
            if (tok.Type == TokenType.EQUALS)
            {
                ParseArrAssignment(node);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseArrAssignment(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ArrAssignment), "ArrAssignment");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.EQUALS);
            if (tok.Type != TokenType.EQUALS)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EQUALS.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LBRACE);
            if (tok.Type != TokenType.LBRACE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LBRACE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseArrContent(node);

            
            tok = scanner.Scan(TokenType.RBRACE);
            if (tok.Type != TokenType.RBRACE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RBRACE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseArrContent(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ArrContent), "ArrContent");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.STRING, TokenType.CHARVALUE, TokenType.NUMBER, TokenType.FLOATVALUE);
            switch (tok.Type)
            {
                case TokenType.STRING:
                    tok = scanner.Scan(TokenType.STRING);
                    if (tok.Type != TokenType.STRING)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRING.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.CHARVALUE:
                    tok = scanner.Scan(TokenType.CHARVALUE);
                    if (tok.Type != TokenType.CHARVALUE)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CHARVALUE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.NUMBER:
                    tok = scanner.Scan(TokenType.NUMBER);
                    if (tok.Type != TokenType.NUMBER)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.FLOATVALUE:
                    tok = scanner.Scan(TokenType.FLOATVALUE);
                    if (tok.Type != TokenType.FLOATVALUE)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.FLOATVALUE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                if (tok.Type != TokenType.COMMA)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.LookAhead(TokenType.STRING, TokenType.CHARVALUE, TokenType.NUMBER, TokenType.FLOATVALUE);
                switch (tok.Type)
                {
                    case TokenType.STRING:
                        tok = scanner.Scan(TokenType.STRING);
                        if (tok.Type != TokenType.STRING)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRING.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    case TokenType.CHARVALUE:
                        tok = scanner.Scan(TokenType.CHARVALUE);
                        if (tok.Type != TokenType.CHARVALUE)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CHARVALUE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    case TokenType.NUMBER:
                        tok = scanner.Scan(TokenType.NUMBER);
                        if (tok.Type != TokenType.NUMBER)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    case TokenType.FLOATVALUE:
                        tok = scanner.Scan(TokenType.FLOATVALUE);
                        if (tok.Type != TokenType.FLOATVALUE)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.FLOATVALUE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        break;
                }
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseVarArray(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.VarArray), "VarArray");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.LBRACKET);
            while (tok.Type == TokenType.LBRACKET)
            {

                
                tok = scanner.Scan(TokenType.LBRACKET);
                if (tok.Type != TokenType.LBRACKET)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LBRACKET.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.LookAhead(TokenType.NUMBER, TokenType.IDENTIFIER);
                switch (tok.Type)
                {
                    case TokenType.NUMBER:
                        tok = scanner.Scan(TokenType.NUMBER);
                        if (tok.Type != TokenType.NUMBER)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    case TokenType.IDENTIFIER:
                        tok = scanner.Scan(TokenType.IDENTIFIER);
                        if (tok.Type != TokenType.IDENTIFIER)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        break;
                }

                
                tok = scanner.Scan(TokenType.RBRACKET);
                if (tok.Type != TokenType.RBRACKET)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RBRACKET.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
            tok = scanner.LookAhead(TokenType.LBRACKET);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseLocalDecl(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.LocalDecl), "LocalDecl");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.DATATYPE);
            if (tok.Type != TokenType.DATATYPE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DATATYPE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.ASTERISK);
            if (tok.Type == TokenType.ASTERISK)
            {
                ParsePointer(node);
            }

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            if (tok.Type != TokenType.IDENTIFIER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.LBRACKET);
            if (tok.Type == TokenType.LBRACKET)
            {
                ParseArray(node);
            }

            
            tok = scanner.LookAhead(TokenType.EQUALS);
            if (tok.Type == TokenType.EQUALS)
            {
                ParseDecAssignment(node);
            }

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                if (tok.Type != TokenType.COMMA)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.LookAhead(TokenType.ASTERISK);
                if (tok.Type == TokenType.ASTERISK)
                {
                    ParsePointer(node);
                }

                
                tok = scanner.Scan(TokenType.IDENTIFIER);
                if (tok.Type != TokenType.IDENTIFIER)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.LookAhead(TokenType.LBRACKET);
                if (tok.Type == TokenType.LBRACKET)
                {
                    ParseArray(node);
                }

                
                tok = scanner.LookAhead(TokenType.EQUALS);
                if (tok.Type == TokenType.EQUALS)
                {
                    ParseDecAssignment(node);
                }
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseAssignment(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Assignment), "Assignment");
            parent.Nodes.Add(node);


            
            ParseIdentifier(node);

            
            tok = scanner.Scan(TokenType.EQUALS);
            if (tok.Type != TokenType.EQUALS)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EQUALS.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseExpr(node);

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseDecAssignment(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.DecAssignment), "DecAssignment");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.EQUALS);
            if (tok.Type != TokenType.EQUALS)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EQUALS.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.NUMBER, TokenType.FLOATVALUE, TokenType.ASTERISK, TokenType.REFOPER, TokenType.IDENTIFIER, TokenType.LPAREN, TokenType.CHARVALUE);
            switch (tok.Type)
            {
                case TokenType.NUMBER:
                case TokenType.FLOATVALUE:
                case TokenType.ASTERISK:
                case TokenType.REFOPER:
                case TokenType.IDENTIFIER:
                case TokenType.LPAREN:
                    ParseExpr(node);
                    break;
                case TokenType.CHARVALUE:
                    tok = scanner.Scan(TokenType.CHARVALUE);
                    if (tok.Type != TokenType.CHARVALUE)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CHARVALUE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseExpr(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Expr), "Expr");
            parent.Nodes.Add(node);


            
            ParseAtom(node);

            
            tok = scanner.LookAhead(TokenType.PLUS, TokenType.MINUS, TokenType.ASTERISK, TokenType.FSLASH, TokenType.PERCENT);
            while (tok.Type == TokenType.PLUS
                || tok.Type == TokenType.MINUS
                || tok.Type == TokenType.ASTERISK
                || tok.Type == TokenType.FSLASH
                || tok.Type == TokenType.PERCENT)
            {

                
                ParseArithmeticOperator(node);

                
                ParseAtom(node);
            tok = scanner.LookAhead(TokenType.PLUS, TokenType.MINUS, TokenType.ASTERISK, TokenType.FSLASH, TokenType.PERCENT);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseAtom(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Atom), "Atom");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.NUMBER, TokenType.FLOATVALUE, TokenType.ASTERISK, TokenType.REFOPER, TokenType.IDENTIFIER, TokenType.LPAREN);
            switch (tok.Type)
            {
                case TokenType.NUMBER:
                    tok = scanner.Scan(TokenType.NUMBER);
                    if (tok.Type != TokenType.NUMBER)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.FLOATVALUE:
                    tok = scanner.Scan(TokenType.FLOATVALUE);
                    if (tok.Type != TokenType.FLOATVALUE)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.FLOATVALUE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.ASTERISK:
                case TokenType.REFOPER:
                case TokenType.IDENTIFIER:

                    
                    tok = scanner.LookAhead(TokenType.ASTERISK, TokenType.REFOPER);
                    if (tok.Type == TokenType.ASTERISK
                        || tok.Type == TokenType.REFOPER)
                    {
                        tok = scanner.LookAhead(TokenType.ASTERISK, TokenType.REFOPER);
                        switch (tok.Type)
                        {
                            case TokenType.ASTERISK:
                                ParsePointer(node);
                                break;
                            case TokenType.REFOPER:
                                tok = scanner.Scan(TokenType.REFOPER);
                                if (tok.Type != TokenType.REFOPER)
                                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.REFOPER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                                n = node.CreateNode(tok, tok.ToString() );
                                node.Token.UpdateRange(tok);
                                node.Nodes.Add(n);
                                break;
                            default:
                                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                                break;
                        }
                    }

                    
                    tok = scanner.Scan(TokenType.IDENTIFIER);
                    if (tok.Type != TokenType.IDENTIFIER)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);

                    
                    tok = scanner.LookAhead(TokenType.LBRACKET);
                    if (tok.Type == TokenType.LBRACKET)
                    {
                        ParseVarArray(node);
                    }
                    break;
                case TokenType.LPAREN:

                    
                    tok = scanner.Scan(TokenType.LPAREN);
                    if (tok.Type != TokenType.LPAREN)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);

                    
                    ParseExpr(node);

                    
                    tok = scanner.Scan(TokenType.RPAREN);
                    if (tok.Type != TokenType.RPAREN)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseIdentifier(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Identifier), "Identifier");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.INCREDECRE);
            if (tok.Type == TokenType.INCREDECRE)
            {
                tok = scanner.Scan(TokenType.INCREDECRE);
                if (tok.Type != TokenType.INCREDECRE)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.INCREDECRE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
            }

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            if (tok.Type != TokenType.IDENTIFIER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.INCREDECRE);
            if (tok.Type == TokenType.INCREDECRE)
            {
                tok = scanner.Scan(TokenType.INCREDECRE);
                if (tok.Type != TokenType.INCREDECRE)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.INCREDECRE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
            }

            
            tok = scanner.LookAhead(TokenType.LPAREN);
            if (tok.Type == TokenType.LPAREN)
            {

                
                tok = scanner.Scan(TokenType.LPAREN);
                if (tok.Type != TokenType.LPAREN)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                ParseParPass(node);

                
                tok = scanner.Scan(TokenType.RPAREN);
                if (tok.Type != TokenType.RPAREN)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseParPass(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ParPass), "ParPass");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.REFOPER);
            if (tok.Type == TokenType.REFOPER)
            {
                tok = scanner.Scan(TokenType.REFOPER);
                if (tok.Type != TokenType.REFOPER)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.REFOPER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
            }

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            if (tok.Type != TokenType.IDENTIFIER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                if (tok.Type != TokenType.COMMA)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.LookAhead(TokenType.REFOPER);
                if (tok.Type == TokenType.REFOPER)
                {
                    tok = scanner.Scan(TokenType.REFOPER);
                    if (tok.Type != TokenType.REFOPER)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.REFOPER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                }

                
                tok = scanner.Scan(TokenType.IDENTIFIER);
                if (tok.Type != TokenType.IDENTIFIER)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseFunction(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Function), "Function");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.LPAREN);
            if (tok.Type != TokenType.LPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.DATATYPE, TokenType.VOID);
            if (tok.Type == TokenType.DATATYPE
                || tok.Type == TokenType.VOID)
            {
                tok = scanner.LookAhead(TokenType.DATATYPE, TokenType.VOID);
                switch (tok.Type)
                {
                    case TokenType.DATATYPE:
                        ParseParameters(node);
                        break;
                    case TokenType.VOID:
                        tok = scanner.Scan(TokenType.VOID);
                        if (tok.Type != TokenType.VOID)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.VOID.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        break;
                }
            }

            
            tok = scanner.Scan(TokenType.RPAREN);
            if (tok.Type != TokenType.RPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.SEMICOL, TokenType.BOOL, TokenType.LBRACE);
            switch (tok.Type)
            {
                case TokenType.SEMICOL:
                    tok = scanner.Scan(TokenType.SEMICOL);
                    if (tok.Type != TokenType.SEMICOL)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.BOOL:
                case TokenType.LBRACE:
                    ParseCodeBlock(node);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseParameters(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Parameters), "Parameters");
            parent.Nodes.Add(node);


            

            
            tok = scanner.Scan(TokenType.DATATYPE);
            if (tok.Type != TokenType.DATATYPE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DATATYPE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.ASTERISK);
            if (tok.Type == TokenType.ASTERISK)
            {
                ParsePointer(node);
            }

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            if (tok.Type != TokenType.IDENTIFIER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.RBRACKET);
            if (tok.Type == TokenType.RBRACKET)
            {
                ParseParArray(node);
            }

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                if (tok.Type != TokenType.COMMA)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.LookAhead(TokenType.DATATYPE, TokenType.VARIABLEPARAMS);
                switch (tok.Type)
                {
                    case TokenType.DATATYPE:

                        
                        tok = scanner.Scan(TokenType.DATATYPE);
                        if (tok.Type != TokenType.DATATYPE)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DATATYPE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);

                        
                        tok = scanner.LookAhead(TokenType.ASTERISK);
                        if (tok.Type == TokenType.ASTERISK)
                        {
                            ParsePointer(node);
                        }

                        
                        tok = scanner.Scan(TokenType.IDENTIFIER);
                        if (tok.Type != TokenType.IDENTIFIER)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);

                        
                        tok = scanner.LookAhead(TokenType.RBRACKET);
                        if (tok.Type == TokenType.RBRACKET)
                        {
                            ParseParArray(node);
                        }
                        break;
                    case TokenType.VARIABLEPARAMS:
                        tok = scanner.Scan(TokenType.VARIABLEPARAMS);
                        if (tok.Type != TokenType.VARIABLEPARAMS)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.VARIABLEPARAMS.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        break;
                }
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseParArray(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ParArray), "ParArray");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.RBRACKET);
            while (tok.Type == TokenType.RBRACKET)
            {

                
                tok = scanner.Scan(TokenType.RBRACKET);
                if (tok.Type != TokenType.RBRACKET)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RBRACKET.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.Scan(TokenType.NUMBER);
                if (tok.Type != TokenType.NUMBER)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.Scan(TokenType.RBRACKET);
                if (tok.Type != TokenType.RBRACKET)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RBRACKET.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
            tok = scanner.LookAhead(TokenType.RBRACKET);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseCodeBlock(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.CodeBlock), "CodeBlock");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.BOOL, TokenType.LBRACE);
            switch (tok.Type)
            {
                case TokenType.BOOL:
                    tok = scanner.Scan(TokenType.BOOL);
                    if (tok.Type != TokenType.BOOL)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BOOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.LBRACE:

                    
                    tok = scanner.Scan(TokenType.LBRACE);
                    if (tok.Type != TokenType.LBRACE)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LBRACE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);

                    
                    tok = scanner.LookAhead(TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
                    while (tok.Type == TokenType.RETURN
                        || tok.Type == TokenType.SWITCH
                        || tok.Type == TokenType.IF
                        || tok.Type == TokenType.FOR
                        || tok.Type == TokenType.WHILE
                        || tok.Type == TokenType.DO
                        || tok.Type == TokenType.DATATYPE
                        || tok.Type == TokenType.INCREDECRE
                        || tok.Type == TokenType.IDENTIFIER
                        || tok.Type == TokenType.BREAK
                        || tok.Type == TokenType.GETCH
                        || tok.Type == TokenType.CLRSCR
                        || tok.Type == TokenType.SCANF
                        || tok.Type == TokenType.PRINTF)
                    {
                        ParseStatement(node);
                    tok = scanner.LookAhead(TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
                    }

                    
                    tok = scanner.Scan(TokenType.RBRACE);
                    if (tok.Type != TokenType.RBRACE)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RBRACE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseBreak(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Break), "Break");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.BREAK);
            if (tok.Type != TokenType.BREAK)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BREAK.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseSwitch(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Switch), "Switch");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.SWITCH);
            if (tok.Type != TokenType.SWITCH)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SWITCH.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LPAREN);
            if (tok.Type != TokenType.LPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            if (tok.Type != TokenType.IDENTIFIER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.LBRACKET);
            if (tok.Type == TokenType.LBRACKET)
            {
                ParseVarArray(node);
            }

            
            tok = scanner.Scan(TokenType.RPAREN);
            if (tok.Type != TokenType.RPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LBRACE);
            if (tok.Type != TokenType.LBRACE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LBRACE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseSwitchCase(node);

            
            tok = scanner.Scan(TokenType.RBRACE);
            if (tok.Type != TokenType.RBRACE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RBRACE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseSwitchCase(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.SwitchCase), "SwitchCase");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.CASE, TokenType.DEFAULT, TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
            while (tok.Type == TokenType.CASE
                || tok.Type == TokenType.DEFAULT
                || tok.Type == TokenType.RETURN
                || tok.Type == TokenType.SWITCH
                || tok.Type == TokenType.IF
                || tok.Type == TokenType.FOR
                || tok.Type == TokenType.WHILE
                || tok.Type == TokenType.DO
                || tok.Type == TokenType.DATATYPE
                || tok.Type == TokenType.INCREDECRE
                || tok.Type == TokenType.IDENTIFIER
                || tok.Type == TokenType.BREAK
                || tok.Type == TokenType.GETCH
                || tok.Type == TokenType.CLRSCR
                || tok.Type == TokenType.SCANF
                || tok.Type == TokenType.PRINTF)
            {

                
                tok = scanner.LookAhead(TokenType.CASE);
                while (tok.Type == TokenType.CASE)
                {

                    
                    tok = scanner.Scan(TokenType.CASE);
                    if (tok.Type != TokenType.CASE)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CASE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);

                    
                    ParseCaseComp(node);

                    
                    tok = scanner.Scan(TokenType.COLON);
                    if (tok.Type != TokenType.COLON)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COLON.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                tok = scanner.LookAhead(TokenType.CASE);
                }

                
                tok = scanner.LookAhead(TokenType.DEFAULT);
                if (tok.Type == TokenType.DEFAULT)
                {

                    
                    tok = scanner.Scan(TokenType.DEFAULT);
                    if (tok.Type != TokenType.DEFAULT)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DEFAULT.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);

                    
                    tok = scanner.Scan(TokenType.COLON);
                    if (tok.Type != TokenType.COLON)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COLON.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                }

                
                tok = scanner.LookAhead(TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
                if (tok.Type == TokenType.RETURN
                    || tok.Type == TokenType.SWITCH
                    || tok.Type == TokenType.IF
                    || tok.Type == TokenType.FOR
                    || tok.Type == TokenType.WHILE
                    || tok.Type == TokenType.DO
                    || tok.Type == TokenType.DATATYPE
                    || tok.Type == TokenType.INCREDECRE
                    || tok.Type == TokenType.IDENTIFIER
                    || tok.Type == TokenType.BREAK
                    || tok.Type == TokenType.GETCH
                    || tok.Type == TokenType.CLRSCR
                    || tok.Type == TokenType.SCANF
                    || tok.Type == TokenType.PRINTF)
                {
                    ParseStatement(node);
                }
            tok = scanner.LookAhead(TokenType.CASE, TokenType.DEFAULT, TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
            }

            
            tok = scanner.LookAhead(TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
            while (tok.Type == TokenType.RETURN
                || tok.Type == TokenType.SWITCH
                || tok.Type == TokenType.IF
                || tok.Type == TokenType.FOR
                || tok.Type == TokenType.WHILE
                || tok.Type == TokenType.DO
                || tok.Type == TokenType.DATATYPE
                || tok.Type == TokenType.INCREDECRE
                || tok.Type == TokenType.IDENTIFIER
                || tok.Type == TokenType.BREAK
                || tok.Type == TokenType.GETCH
                || tok.Type == TokenType.CLRSCR
                || tok.Type == TokenType.SCANF
                || tok.Type == TokenType.PRINTF)
            {
                ParseStatement(node);
            tok = scanner.LookAhead(TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseCaseComp(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.CaseComp), "CaseComp");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.NUMBER, TokenType.CHARVALUE, TokenType.STRING);
            switch (tok.Type)
            {
                case TokenType.NUMBER:
                    tok = scanner.Scan(TokenType.NUMBER);
                    if (tok.Type != TokenType.NUMBER)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.CHARVALUE:
                    tok = scanner.Scan(TokenType.CHARVALUE);
                    if (tok.Type != TokenType.CHARVALUE)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CHARVALUE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.STRING:
                    tok = scanner.Scan(TokenType.STRING);
                    if (tok.Type != TokenType.STRING)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRING.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseStatement(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Statement), "Statement");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
            switch (tok.Type)
            {
                case TokenType.RETURN:
                    ParseReturn(node);
                    break;
                case TokenType.SWITCH:
                    ParseSwitch(node);
                    break;
                case TokenType.IF:
                    ParseIf(node);
                    break;
                case TokenType.FOR:
                    ParseFor(node);
                    break;
                case TokenType.WHILE:
                    ParseWhile(node);
                    break;
                case TokenType.DO:
                    ParseDoWhile(node);
                    break;
                case TokenType.DATATYPE:
                    ParseLocalDecl(node);
                    break;
                case TokenType.INCREDECRE:
                case TokenType.IDENTIFIER:
                    ParseAssignment(node);
                    break;
                case TokenType.BREAK:
                    ParseBreak(node);
                    break;
                case TokenType.GETCH:
                case TokenType.CLRSCR:
                case TokenType.SCANF:
                case TokenType.PRINTF:
                    ParseBuiltInFunc(node);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseIf(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.If), "If");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.IF);
            if (tok.Type != TokenType.IF)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IF.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LPAREN);
            if (tok.Type != TokenType.LPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseCondition(node);

            
            tok = scanner.Scan(TokenType.RPAREN);
            if (tok.Type != TokenType.RPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseIfForLoopBlock(node);

            
            tok = scanner.LookAhead(TokenType.ELSE);
            if (tok.Type == TokenType.ELSE)
            {
                ParseElse(node);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseCondition(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Condition), "Condition");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.BOOL, TokenType.IDENTIFIER, TokenType.NUMBER, TokenType.STRING, TokenType.NOT, TokenType.LPAREN);
            switch (tok.Type)
            {
                case TokenType.BOOL:
                    tok = scanner.Scan(TokenType.BOOL);
                    if (tok.Type != TokenType.BOOL)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BOOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.IDENTIFIER:
                case TokenType.NUMBER:
                case TokenType.STRING:
                case TokenType.NOT:
                case TokenType.LPAREN:

                    
                    ParseCondLogExpr(node);

                    
                    tok = scanner.LookAhead(TokenType.LOGOP);
                    while (tok.Type == TokenType.LOGOP)
                    {

                        
                        tok = scanner.Scan(TokenType.LOGOP);
                        if (tok.Type != TokenType.LOGOP)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LOGOP.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);

                        
                        ParseCondLogExpr(node);
                    tok = scanner.LookAhead(TokenType.LOGOP);
                    }
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseCondLogExpr(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.CondLogExpr), "CondLogExpr");
            parent.Nodes.Add(node);


            
            ParseCondExpr(node);

            
            tok = scanner.LookAhead(TokenType.RELOP);
            while (tok.Type == TokenType.RELOP)
            {

                
                tok = scanner.Scan(TokenType.RELOP);
                if (tok.Type != TokenType.RELOP)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RELOP.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                ParseCondExpr(node);
            tok = scanner.LookAhead(TokenType.RELOP);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseCondExpr(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.CondExpr), "CondExpr");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.IDENTIFIER, TokenType.NUMBER, TokenType.STRING, TokenType.NOT, TokenType.LPAREN);
            switch (tok.Type)
            {
                case TokenType.IDENTIFIER:
                case TokenType.NUMBER:
                case TokenType.STRING:
                    tok = scanner.LookAhead(TokenType.IDENTIFIER, TokenType.NUMBER, TokenType.STRING);
                    switch (tok.Type)
                    {
                        case TokenType.IDENTIFIER:

                            
                            tok = scanner.Scan(TokenType.IDENTIFIER);
                            if (tok.Type != TokenType.IDENTIFIER)
                                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                            n = node.CreateNode(tok, tok.ToString() );
                            node.Token.UpdateRange(tok);
                            node.Nodes.Add(n);

                            
                            tok = scanner.LookAhead(TokenType.LBRACKET);
                            if (tok.Type == TokenType.LBRACKET)
                            {
                                ParseVarArray(node);
                            }
                            break;
                        case TokenType.NUMBER:
                            tok = scanner.Scan(TokenType.NUMBER);
                            if (tok.Type != TokenType.NUMBER)
                                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                            n = node.CreateNode(tok, tok.ToString() );
                            node.Token.UpdateRange(tok);
                            node.Nodes.Add(n);
                            break;
                        case TokenType.STRING:
                            tok = scanner.Scan(TokenType.STRING);
                            if (tok.Type != TokenType.STRING)
                                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRING.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                            n = node.CreateNode(tok, tok.ToString() );
                            node.Token.UpdateRange(tok);
                            node.Nodes.Add(n);
                            break;
                        default:
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                            break;
                    }
                    break;
                case TokenType.NOT:
                case TokenType.LPAREN:

                    
                    tok = scanner.LookAhead(TokenType.NOT);
                    if (tok.Type == TokenType.NOT)
                    {
                        tok = scanner.Scan(TokenType.NOT);
                        if (tok.Type != TokenType.NOT)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NOT.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                    }

                    
                    tok = scanner.Scan(TokenType.LPAREN);
                    if (tok.Type != TokenType.LPAREN)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);

                    
                    ParseCondition(node);

                    
                    tok = scanner.Scan(TokenType.RPAREN);
                    if (tok.Type != TokenType.RPAREN)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseElse(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Else), "Else");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.ELSE);
            if (tok.Type != TokenType.ELSE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.ELSE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseIfForLoopBlock(node);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseIfForLoopBlock(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.IfForLoopBlock), "IfForLoopBlock");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.LBRACE, TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
            switch (tok.Type)
            {
                case TokenType.LBRACE:

                    
                    tok = scanner.Scan(TokenType.LBRACE);
                    if (tok.Type != TokenType.LBRACE)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LBRACE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);

                    
                    tok = scanner.LookAhead(TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
                    while (tok.Type == TokenType.RETURN
                        || tok.Type == TokenType.SWITCH
                        || tok.Type == TokenType.IF
                        || tok.Type == TokenType.FOR
                        || tok.Type == TokenType.WHILE
                        || tok.Type == TokenType.DO
                        || tok.Type == TokenType.DATATYPE
                        || tok.Type == TokenType.INCREDECRE
                        || tok.Type == TokenType.IDENTIFIER
                        || tok.Type == TokenType.BREAK
                        || tok.Type == TokenType.GETCH
                        || tok.Type == TokenType.CLRSCR
                        || tok.Type == TokenType.SCANF
                        || tok.Type == TokenType.PRINTF)
                    {
                        ParseStatement(node);
                    tok = scanner.LookAhead(TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
                    }

                    
                    tok = scanner.Scan(TokenType.RBRACE);
                    if (tok.Type != TokenType.RBRACE)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RBRACE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    break;
                case TokenType.RETURN:
                case TokenType.SWITCH:
                case TokenType.IF:
                case TokenType.FOR:
                case TokenType.WHILE:
                case TokenType.DO:
                case TokenType.DATATYPE:
                case TokenType.INCREDECRE:
                case TokenType.IDENTIFIER:
                case TokenType.BREAK:
                case TokenType.GETCH:
                case TokenType.CLRSCR:
                case TokenType.SCANF:
                case TokenType.PRINTF:
                    ParseStatement(node);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseFor(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.For), "For");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.FOR);
            if (tok.Type != TokenType.FOR)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.FOR.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LPAREN);
            if (tok.Type != TokenType.LPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.DATATYPE, TokenType.IDENTIFIER);
            if (tok.Type == TokenType.DATATYPE
                || tok.Type == TokenType.IDENTIFIER)
            {
                tok = scanner.LookAhead(TokenType.DATATYPE, TokenType.IDENTIFIER);
                switch (tok.Type)
                {
                    case TokenType.DATATYPE:
                        ParseForDeclaration(node);
                        break;
                    case TokenType.IDENTIFIER:

                        
                        ParseForAssignment(node);

                        
                        tok = scanner.LookAhead(TokenType.COMMA);
                        while (tok.Type == TokenType.COMMA)
                        {

                            
                            tok = scanner.Scan(TokenType.COMMA);
                            if (tok.Type != TokenType.COMMA)
                                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                            n = node.CreateNode(tok, tok.ToString() );
                            node.Token.UpdateRange(tok);
                            node.Nodes.Add(n);

                            
                            ParseForAssignment(node);
                        tok = scanner.LookAhead(TokenType.COMMA);
                        }
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        break;
                }
            }

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.BOOL, TokenType.IDENTIFIER, TokenType.NUMBER, TokenType.STRING, TokenType.NOT, TokenType.LPAREN);
            if (tok.Type == TokenType.BOOL
                || tok.Type == TokenType.IDENTIFIER
                || tok.Type == TokenType.NUMBER
                || tok.Type == TokenType.STRING
                || tok.Type == TokenType.NOT
                || tok.Type == TokenType.LPAREN)
            {

                
                ParseCondition(node);

                
                tok = scanner.LookAhead(TokenType.COMMA);
                while (tok.Type == TokenType.COMMA)
                {

                    
                    tok = scanner.Scan(TokenType.COMMA);
                    if (tok.Type != TokenType.COMMA)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);

                    
                    ParseCondition(node);
                tok = scanner.LookAhead(TokenType.COMMA);
                }
            }

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.INCREDECRE, TokenType.IDENTIFIER);
            if (tok.Type == TokenType.INCREDECRE
                || tok.Type == TokenType.IDENTIFIER)
            {

                
                ParseIncDec(node);

                
                tok = scanner.LookAhead(TokenType.COMMA);
                while (tok.Type == TokenType.COMMA)
                {

                    
                    tok = scanner.Scan(TokenType.COMMA);
                    if (tok.Type != TokenType.COMMA)
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);

                    
                    ParseIncDec(node);
                tok = scanner.LookAhead(TokenType.COMMA);
                }
            }

            
            tok = scanner.Scan(TokenType.RPAREN);
            if (tok.Type != TokenType.RPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseIfForLoopBlock(node);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseForDeclaration(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ForDeclaration), "ForDeclaration");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.DATATYPE);
            if (tok.Type != TokenType.DATATYPE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DATATYPE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            if (tok.Type != TokenType.IDENTIFIER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.EQUALS);
            if (tok.Type == TokenType.EQUALS)
            {
                ParseDecAssignment(node);
            }

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                if (tok.Type != TokenType.COMMA)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.Scan(TokenType.IDENTIFIER);
                if (tok.Type != TokenType.IDENTIFIER)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.LookAhead(TokenType.EQUALS);
                if (tok.Type == TokenType.EQUALS)
                {
                    ParseDecAssignment(node);
                }
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseForAssignment(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ForAssignment), "ForAssignment");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            if (tok.Type != TokenType.IDENTIFIER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.EQUALS);
            if (tok.Type != TokenType.EQUALS)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EQUALS.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseExpr(node);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseWhile(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.While), "While");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.WHILE);
            if (tok.Type != TokenType.WHILE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.WHILE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LPAREN);
            if (tok.Type != TokenType.LPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            

            
            ParseCondition(node);

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                if (tok.Type != TokenType.COMMA)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                ParseCondition(node);
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            
            tok = scanner.Scan(TokenType.RPAREN);
            if (tok.Type != TokenType.RPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseWhileLoopBlock(node);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseDoWhile(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.DoWhile), "DoWhile");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.DO);
            if (tok.Type != TokenType.DO)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DO.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseWhileLoopBlock(node);

            
            tok = scanner.Scan(TokenType.WHILE);
            if (tok.Type != TokenType.WHILE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.WHILE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LPAREN);
            if (tok.Type != TokenType.LPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            

            
            ParseCondition(node);

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                if (tok.Type != TokenType.COMMA)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                ParseCondition(node);
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            
            tok = scanner.Scan(TokenType.RPAREN);
            if (tok.Type != TokenType.RPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseIncDec(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.IncDec), "IncDec");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.INCREDECRE);
            if (tok.Type == TokenType.INCREDECRE)
            {
                tok = scanner.Scan(TokenType.INCREDECRE);
                if (tok.Type != TokenType.INCREDECRE)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.INCREDECRE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
            }

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            if (tok.Type != TokenType.IDENTIFIER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.INCREDECRE);
            if (tok.Type == TokenType.INCREDECRE)
            {
                tok = scanner.Scan(TokenType.INCREDECRE);
                if (tok.Type != TokenType.INCREDECRE)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.INCREDECRE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseWhileLoopBlock(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.WhileLoopBlock), "WhileLoopBlock");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.LBRACE);
            if (tok.Type != TokenType.LBRACE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LBRACE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
            while (tok.Type == TokenType.RETURN
                || tok.Type == TokenType.SWITCH
                || tok.Type == TokenType.IF
                || tok.Type == TokenType.FOR
                || tok.Type == TokenType.WHILE
                || tok.Type == TokenType.DO
                || tok.Type == TokenType.DATATYPE
                || tok.Type == TokenType.INCREDECRE
                || tok.Type == TokenType.IDENTIFIER
                || tok.Type == TokenType.BREAK
                || tok.Type == TokenType.GETCH
                || tok.Type == TokenType.CLRSCR
                || tok.Type == TokenType.SCANF
                || tok.Type == TokenType.PRINTF)
            {
                ParseStatement(node);
            tok = scanner.LookAhead(TokenType.RETURN, TokenType.SWITCH, TokenType.IF, TokenType.FOR, TokenType.WHILE, TokenType.DO, TokenType.DATATYPE, TokenType.INCREDECRE, TokenType.IDENTIFIER, TokenType.BREAK, TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
            }

            
            tok = scanner.Scan(TokenType.RBRACE);
            if (tok.Type != TokenType.RBRACE)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RBRACE.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParsePrintf(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Printf), "Printf");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.PRINTF);
            if (tok.Type != TokenType.PRINTF)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PRINTF.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LPAREN);
            if (tok.Type != TokenType.LPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.STRING);
            if (tok.Type != TokenType.STRING)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRING.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                if (tok.Type != TokenType.COMMA)
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);

                
                tok = scanner.LookAhead(TokenType.IDENTIFIER, TokenType.NUMBER);
                switch (tok.Type)
                {
                    case TokenType.IDENTIFIER:

                        
                        tok = scanner.Scan(TokenType.IDENTIFIER);
                        if (tok.Type != TokenType.IDENTIFIER)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);

                        
                        tok = scanner.LookAhead(TokenType.LBRACKET);
                        if (tok.Type == TokenType.LBRACKET)
                        {
                            ParseVarArray(node);
                        }
                        break;
                    case TokenType.NUMBER:
                        tok = scanner.Scan(TokenType.NUMBER);
                        if (tok.Type != TokenType.NUMBER)
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                        break;
                }
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            
            tok = scanner.Scan(TokenType.RPAREN);
            if (tok.Type != TokenType.RPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseScanf(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Scanf), "Scanf");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.SCANF);
            if (tok.Type != TokenType.SCANF)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SCANF.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LPAREN);
            if (tok.Type != TokenType.LPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.TYPESPEC);
            if (tok.Type != TokenType.TYPESPEC)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.TYPESPEC.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.COMMA);
            if (tok.Type != TokenType.COMMA)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.REFOPER);
            if (tok.Type != TokenType.REFOPER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.REFOPER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            if (tok.Type != TokenType.IDENTIFIER)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.RPAREN);
            if (tok.Type != TokenType.RPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseReturn(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Return), "Return");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.RETURN);
            if (tok.Type != TokenType.RETURN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RETURN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            ParseExpr(node);

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseBuiltInFunc(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.BuiltInFunc), "BuiltInFunc");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.GETCH, TokenType.CLRSCR, TokenType.SCANF, TokenType.PRINTF);
            switch (tok.Type)
            {
                case TokenType.GETCH:
                    ParseGetch(node);
                    break;
                case TokenType.CLRSCR:
                    ParseClrscr(node);
                    break;
                case TokenType.SCANF:
                    ParseScanf(node);
                    break;
                case TokenType.PRINTF:
                    ParsePrintf(node);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseGetch(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Getch), "Getch");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.GETCH);
            if (tok.Type != TokenType.GETCH)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.GETCH.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LPAREN);
            if (tok.Type != TokenType.LPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.RPAREN);
            if (tok.Type != TokenType.RPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseClrscr(ParseNode parent)
        {
            Token tok;
           ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Clrscr), "Clrscr");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.CLRSCR);
            if (tok.Type != TokenType.CLRSCR)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CLRSCR.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.LPAREN);
            if (tok.Type != TokenType.LPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.RPAREN);
            if (tok.Type != TokenType.RPAREN)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.RPAREN.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            
            tok = scanner.Scan(TokenType.SEMICOL);
            if (tok.Type != TokenType.SEMICOL)
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOL.ToString(), 0x1001, tok.LinePos, tok.ColumnPos, tok.StartPos, tok.Length));
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);

            parent.Token.UpdateRange(node.Token);
        }


    }

    #endregion Parser
}
