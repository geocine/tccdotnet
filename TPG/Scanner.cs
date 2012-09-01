using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace tccdotnet
{
    #region Scanner

    public partial class Scanner
    {
        public string Input;
        public int LastPos = 0;
        public int StartPos = 0;
        public int EndPos = 0;
        public int CurrentLine;
        public int CurrentColumn;
        public int CurrentPosition;
        public List<Token> Skipped; // tokens that were skipped
        public Dictionary<TokenType, Regex> Patterns;

        private Token LookAheadToken;
        private List<TokenType> Tokens;
        private List<TokenType> SkipList; // tokens to be skipped

        public Scanner()
        {
            Regex regex;
            Patterns = new Dictionary<TokenType, Regex>();
            Tokens = new List<TokenType>();
            LookAheadToken = null;

            SkipList = new List<TokenType>();
            SkipList.Add(TokenType.WHITESPACE);
            SkipList.Add(TokenType.EOL);
            SkipList.Add(TokenType.COMMENTLINE);
            SkipList.Add(TokenType.COMMENTBLOCK);

            regex = new Regex(@"<.+?>", RegexOptions.Compiled);
            Patterns.Add(TokenType.DIRECTIVE, regex);
            Tokens.Add(TokenType.DIRECTIVE);

            regex = new Regex(@"float|double|u?int|char|u?longlong|u?long|u?short", RegexOptions.Compiled);
            Patterns.Add(TokenType.DATATYPE, regex);
            Tokens.Add(TokenType.DATATYPE);

            regex = new Regex(@"include", RegexOptions.Compiled);
            Patterns.Add(TokenType.INCLUDE, regex);
            Tokens.Add(TokenType.INCLUDE);

            regex = new Regex(@"void", RegexOptions.Compiled);
            Patterns.Add(TokenType.VOID, regex);
            Tokens.Add(TokenType.VOID);

            regex = new Regex(@"switch", RegexOptions.Compiled);
            Patterns.Add(TokenType.SWITCH, regex);
            Tokens.Add(TokenType.SWITCH);

            regex = new Regex(@"case", RegexOptions.Compiled);
            Patterns.Add(TokenType.CASE, regex);
            Tokens.Add(TokenType.CASE);

            regex = new Regex(@"if", RegexOptions.Compiled);
            Patterns.Add(TokenType.IF, regex);
            Tokens.Add(TokenType.IF);

            regex = new Regex(@"else", RegexOptions.Compiled);
            Patterns.Add(TokenType.ELSE, regex);
            Tokens.Add(TokenType.ELSE);

            regex = new Regex(@"for", RegexOptions.Compiled);
            Patterns.Add(TokenType.FOR, regex);
            Tokens.Add(TokenType.FOR);

            regex = new Regex(@"while", RegexOptions.Compiled);
            Patterns.Add(TokenType.WHILE, regex);
            Tokens.Add(TokenType.WHILE);

            regex = new Regex(@"default", RegexOptions.Compiled);
            Patterns.Add(TokenType.DEFAULT, regex);
            Tokens.Add(TokenType.DEFAULT);

            regex = new Regex(@"return", RegexOptions.Compiled);
            Patterns.Add(TokenType.RETURN, regex);
            Tokens.Add(TokenType.RETURN);

            regex = new Regex(@"printf", RegexOptions.Compiled);
            Patterns.Add(TokenType.PRINTF, regex);
            Tokens.Add(TokenType.PRINTF);

            regex = new Regex(@"scanf", RegexOptions.Compiled);
            Patterns.Add(TokenType.SCANF, regex);
            Tokens.Add(TokenType.SCANF);

            regex = new Regex(@"do", RegexOptions.Compiled);
            Patterns.Add(TokenType.DO, regex);
            Tokens.Add(TokenType.DO);

            regex = new Regex(@"getch", RegexOptions.Compiled);
            Patterns.Add(TokenType.GETCH, regex);
            Tokens.Add(TokenType.GETCH);

            regex = new Regex(@"clrscr", RegexOptions.Compiled);
            Patterns.Add(TokenType.CLRSCR, regex);
            Tokens.Add(TokenType.CLRSCR);

            regex = new Regex(@"break", RegexOptions.Compiled);
            Patterns.Add(TokenType.BREAK, regex);
            Tokens.Add(TokenType.BREAK);

            regex = new Regex(@"@?\""%[dfsc]\""", RegexOptions.Compiled);
            Patterns.Add(TokenType.TYPESPEC, regex);
            Tokens.Add(TokenType.TYPESPEC);

            regex = new Regex(@"&", RegexOptions.Compiled);
            Patterns.Add(TokenType.REFOPER, regex);
            Tokens.Add(TokenType.REFOPER);

            regex = new Regex(@"[a-zA-Z_][a-zA-Z0-9_]*", RegexOptions.Compiled);
            Patterns.Add(TokenType.IDENTIFIER, regex);
            Tokens.Add(TokenType.IDENTIFIER);

            regex = new Regex(@"\'(([a-zA-Z_][a-zA-Z_]|[0-9_][0-9_])|([a-zA-Z_][0-9_])|([0-9_][a-zA-Z_]))\'|\'([a-zA-Z_]|[0-9_])\'", RegexOptions.Compiled);
            Patterns.Add(TokenType.CHARVALUE, regex);
            Tokens.Add(TokenType.CHARVALUE);

            regex = new Regex(@"[0-9]+\.[0-9]+", RegexOptions.Compiled);
            Patterns.Add(TokenType.FLOATVALUE, regex);
            Tokens.Add(TokenType.FLOATVALUE);

            regex = new Regex(@"\+", RegexOptions.Compiled);
            Patterns.Add(TokenType.PLUS, regex);
            Tokens.Add(TokenType.PLUS);

            regex = new Regex(@"-", RegexOptions.Compiled);
            Patterns.Add(TokenType.MINUS, regex);
            Tokens.Add(TokenType.MINUS);

            regex = new Regex(@"\*", RegexOptions.Compiled);
            Patterns.Add(TokenType.ASTERISK, regex);
            Tokens.Add(TokenType.ASTERISK);

            regex = new Regex(@"/", RegexOptions.Compiled);
            Patterns.Add(TokenType.FSLASH, regex);
            Tokens.Add(TokenType.FSLASH);

            regex = new Regex(@"%", RegexOptions.Compiled);
            Patterns.Add(TokenType.PERCENT, regex);
            Tokens.Add(TokenType.PERCENT);

            regex = new Regex(@"[0-9]+", RegexOptions.Compiled);
            Patterns.Add(TokenType.NUMBER, regex);
            Tokens.Add(TokenType.NUMBER);

            regex = new Regex(@"\'", RegexOptions.Compiled);
            Patterns.Add(TokenType.QUOT, regex);
            Tokens.Add(TokenType.QUOT);

            regex = new Regex(@"\""", RegexOptions.Compiled);
            Patterns.Add(TokenType.DQUOT, regex);
            Tokens.Add(TokenType.DQUOT);

            regex = new Regex(@",", RegexOptions.Compiled);
            Patterns.Add(TokenType.COMMA, regex);
            Tokens.Add(TokenType.COMMA);

            regex = new Regex(@"=", RegexOptions.Compiled);
            Patterns.Add(TokenType.EQUALS, regex);
            Tokens.Add(TokenType.EQUALS);

            regex = new Regex(@";", RegexOptions.Compiled);
            Patterns.Add(TokenType.SEMICOL, regex);
            Tokens.Add(TokenType.SEMICOL);

            regex = new Regex(@":", RegexOptions.Compiled);
            Patterns.Add(TokenType.COLON, regex);
            Tokens.Add(TokenType.COLON);

            regex = new Regex(@"#", RegexOptions.Compiled);
            Patterns.Add(TokenType.SHARP, regex);
            Tokens.Add(TokenType.SHARP);

            regex = new Regex(@"^$", RegexOptions.Compiled);
            Patterns.Add(TokenType.EOF, regex);
            Tokens.Add(TokenType.EOF);

            regex = new Regex(@"\(", RegexOptions.Compiled);
            Patterns.Add(TokenType.LPAREN, regex);
            Tokens.Add(TokenType.LPAREN);

            regex = new Regex(@"\)", RegexOptions.Compiled);
            Patterns.Add(TokenType.RPAREN, regex);
            Tokens.Add(TokenType.RPAREN);

            regex = new Regex(@"\{", RegexOptions.Compiled);
            Patterns.Add(TokenType.LBRACE, regex);
            Tokens.Add(TokenType.LBRACE);

            regex = new Regex(@"\}", RegexOptions.Compiled);
            Patterns.Add(TokenType.RBRACE, regex);
            Tokens.Add(TokenType.RBRACE);

            regex = new Regex(@"\[", RegexOptions.Compiled);
            Patterns.Add(TokenType.LBRACKET, regex);
            Tokens.Add(TokenType.LBRACKET);

            regex = new Regex(@"\]", RegexOptions.Compiled);
            Patterns.Add(TokenType.RBRACKET, regex);
            Tokens.Add(TokenType.RBRACKET);

            regex = new Regex(@">=|<=|==|!=|>|<", RegexOptions.Compiled);
            Patterns.Add(TokenType.RELOP, regex);
            Tokens.Add(TokenType.RELOP);

            regex = new Regex(@"&&|\|\|", RegexOptions.Compiled);
            Patterns.Add(TokenType.LOGOP, regex);
            Tokens.Add(TokenType.LOGOP);

            regex = new Regex(@"\+\+|\-\-", RegexOptions.Compiled);
            Patterns.Add(TokenType.INCREDECRE, regex);
            Tokens.Add(TokenType.INCREDECRE);

            regex = new Regex(@"true|false", RegexOptions.Compiled);
            Patterns.Add(TokenType.BOOL, regex);
            Tokens.Add(TokenType.BOOL);

            regex = new Regex(@"!", RegexOptions.Compiled);
            Patterns.Add(TokenType.NOT, regex);
            Tokens.Add(TokenType.NOT);

            regex = new Regex(@"", RegexOptions.Compiled);
            Patterns.Add(TokenType.NULL, regex);
            Tokens.Add(TokenType.NULL);

            regex = new Regex(@"@?\""(\""\""|[^\""])*\""", RegexOptions.Compiled);
            Patterns.Add(TokenType.STRING, regex);
            Tokens.Add(TokenType.STRING);

            regex = new Regex(@"\.\.\.", RegexOptions.Compiled);
            Patterns.Add(TokenType.VARIABLEPARAMS, regex);
            Tokens.Add(TokenType.VARIABLEPARAMS);

            regex = new Regex(@"\s+", RegexOptions.Compiled);
            Patterns.Add(TokenType.WHITESPACE, regex);
            Tokens.Add(TokenType.WHITESPACE);

            regex = new Regex(@"[\n\r]", RegexOptions.Compiled);
            Patterns.Add(TokenType.EOL, regex);
            Tokens.Add(TokenType.EOL);

            regex = new Regex(@"//[^\n]*\n?", RegexOptions.Compiled);
            Patterns.Add(TokenType.COMMENTLINE, regex);
            Tokens.Add(TokenType.COMMENTLINE);

            regex = new Regex(@"/\*[^*]*\*+(?:[^/*][^*]*\*+)*/", RegexOptions.Compiled);
            Patterns.Add(TokenType.COMMENTBLOCK, regex);
            Tokens.Add(TokenType.COMMENTBLOCK);


        }

        public void Init(string input)
        {
            this.Input = input;
            StartPos = 0;
            EndPos = 0;
            CurrentLine = 0;
            CurrentColumn = 0;
            CurrentPosition = 0;
            Skipped = new List<Token>();
            LookAheadToken = null;
        }

        public Token GetToken(TokenType type)
        {
            Token t = new Token(this.StartPos, this.EndPos,this.CurrentLine, this.CurrentColumn);
            t.Type = type;
            return t;
        }

         /// <summary>
        /// executes a lookahead of the next token
        /// and will advance the scan on the input string
        /// </summary>
        /// <returns></returns>
        public Token Scan(params TokenType[] expectedtokens)
        {
            Token tok = LookAhead(expectedtokens); // temporarely retrieve the lookahead
            LookAheadToken = null; // reset lookahead token, so scanning will continue
            StartPos = tok.EndPos;
            EndPos = tok.EndPos; // set the tokenizer to the new scan position
            return tok;
        }

        /// <summary>
        /// returns token with longest best match
        /// </summary>
        /// <returns></returns>
        public Token LookAhead(params TokenType[] expectedtokens)
        {
            int i;
            int startpos = StartPos;
            Token tok = null;
            List<TokenType> scantokens;


            // this prevents double scanning and matching
            // increased performance
            if (LookAheadToken != null 
                && LookAheadToken.Type != TokenType._UNDETERMINED_ 
                && LookAheadToken.Type != TokenType._NONE_) return LookAheadToken;

            // if no scantokens specified, then scan for all of them (= backward compatible)
            if (expectedtokens.Length == 0)
                scantokens = Tokens;
            else
            {
                scantokens = new List<TokenType>(expectedtokens);
                scantokens.AddRange(SkipList);
            }

            do
            {

                int len = -1;
                TokenType index = (TokenType)int.MaxValue;
                string input = Input.Substring(startpos);
                string count = Input.Substring(0, EndPos);
                CurrentLine = count.Split('\n').Length;
                int a = count.LastIndexOf(Environment.NewLine);
                CurrentColumn = EndPos - a;

                tok = new Token(startpos, EndPos, CurrentLine, CurrentColumn);

                for (i = 0; i < scantokens.Count; i++)
                {
                    Regex r = Patterns[scantokens[i]];
                    Match m = r.Match(input);
                    if (m.Success && m.Index == 0 && ((m.Length > len) || (scantokens[i] < index && m.Length == len )))
                    {
                        len = m.Length;
                        index = scantokens[i];  
                    }
                }

                if (index >= 0 && len >= 0)
                {
                    tok.EndPos = startpos + len;
                    tok.Text = Input.Substring(tok.StartPos, len);
                    tok.Type = index;
                }
                else
                {
                    if (tok.StartPos < tok.EndPos - 1)
                        tok.Text = Input.Substring(tok.StartPos, 1);
                }

                if (SkipList.Contains(tok.Type))
                {
                    startpos = tok.EndPos;
                    Skipped.Add(tok);
                }
            }
            while (SkipList.Contains(tok.Type));

            LookAheadToken = tok;
            return tok;
        }
    }

    #endregion

    #region Token

    public enum TokenType
    {

            //Non terminal tokens:
            _NONE_  = 0,
            _UNDETERMINED_= 1,

            //Non terminal tokens:
            Start   = 2,
            Header  = 3,
            GlobalDecl= 4,
            Pointer = 5,
            ArithmeticOperator= 6,
            Array   = 7,
            ArrAssignment= 8,
            ArrContent= 9,
            VarArray= 10,
            LocalDecl= 11,
            Assignment= 12,
            DecAssignment= 13,
            Expr    = 14,
            Atom    = 15,
            Identifier= 16,
            ParPass = 17,
            Function= 18,
            Parameters= 19,
            ParArray= 20,
            CodeBlock= 21,
            Break   = 22,
            Switch  = 23,
            SwitchCase= 24,
            CaseComp= 25,
            Statement= 26,
            If      = 27,
            Condition= 28,
            CondLogExpr= 29,
            CondExpr= 30,
            Else    = 31,
            IfForLoopBlock= 32,
            For     = 33,
            ForDeclaration= 34,
            ForAssignment= 35,
            While   = 36,
            DoWhile = 37,
            IncDec  = 38,
            WhileLoopBlock= 39,
            Printf  = 40,
            Scanf   = 41,
            Return  = 42,
            BuiltInFunc= 43,
            Getch   = 44,
            Clrscr  = 45,

            //Terminal tokens:
            DIRECTIVE= 46,
            DATATYPE= 47,
            INCLUDE = 48,
            VOID    = 49,
            SWITCH  = 50,
            CASE    = 51,
            IF      = 52,
            ELSE    = 53,
            FOR     = 54,
            WHILE   = 55,
            DEFAULT = 56,
            RETURN  = 57,
            PRINTF  = 58,
            SCANF   = 59,
            DO      = 60,
            GETCH   = 61,
            CLRSCR  = 62,
            BREAK   = 63,
            TYPESPEC= 64,
            REFOPER = 65,
            IDENTIFIER= 66,
            CHARVALUE= 67,
            FLOATVALUE= 68,
            PLUS    = 69,
            MINUS   = 70,
            ASTERISK= 71,
            FSLASH  = 72,
            PERCENT = 73,
            NUMBER  = 74,
            QUOT    = 75,
            DQUOT   = 76,
            COMMA   = 77,
            EQUALS  = 78,
            SEMICOL = 79,
            COLON   = 80,
            SHARP   = 81,
            EOF     = 82,
            LPAREN  = 83,
            RPAREN  = 84,
            LBRACE  = 85,
            RBRACE  = 86,
            LBRACKET= 87,
            RBRACKET= 88,
            RELOP   = 89,
            LOGOP   = 90,
            INCREDECRE= 91,
            BOOL    = 92,
            NOT     = 93,
            NULL    = 94,
            STRING  = 95,
            VARIABLEPARAMS= 96,
            WHITESPACE= 97,
            EOL     = 98,
            COMMENTLINE= 99,
            COMMENTBLOCK= 100
    }

    public class Token
    {
        private int startpos;
        private int endpos;
        private int linepos;
        private int columnpos;
        private string text;
        private object value;

        public int LinePos
        {
            get { return linepos; }
            set { linepos = value; }
        }

        public int ColumnPos
        {
            get { return columnpos; }
            set { columnpos = value; }
        }

        public int StartPos { 
            get { return startpos;} 
            set { startpos = value; }
        }

        public int Length { 
            get { return endpos - startpos;} 
        }

        public int EndPos { 
            get { return endpos;} 
            set { endpos = value; }
        }

        public string Text { 
            get { return text;} 
            set { text = value; }
        }

        public object Value { 
            get { return value;} 
            set { this.value = value; }
        }

        public TokenType Type;

        public Token()
            : this(0, 0, 0, 0)
        {
        }

        public Token(int start, int end, int line, int column)
        {
            Type = TokenType._UNDETERMINED_;
            startpos = start;
            endpos = end;
            linepos = line;
            columnpos = column;
            Text = ""; // must initialize with empty string, may cause null reference exceptions otherwise
            Value = null;
        }

        public void UpdateRange(Token token)
        {
            if (token.StartPos < startpos) startpos = token.StartPos;
            if (token.EndPos > endpos) endpos = token.EndPos;
        }

        public override string ToString()
        {
            if (Text != null)
                return Type.ToString() + " '" + Text + "'";
            else
                return Type.ToString();
        }
    }

    #endregion
}
