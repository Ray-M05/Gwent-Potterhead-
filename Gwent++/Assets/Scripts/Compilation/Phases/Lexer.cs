using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Compiler
{

    /// <summary>
    /// Represents a token in the source code, including its type, value, and position where it was found.
    /// </summary>
    public class Token
    {
        public TokenType Type;
        public string Meaning;
        public Position PositionError { get; set; }

        /// <param name="type">The type of the token.</param>
        /// <param name="value">The string value of the token.</param>
        /// <param name="positionError">The position in the source code where the token was found.</param>
        public Token(TokenType type, string value, (int, int) positionError)
        {
            Type = type;
            Meaning = value;
            PositionError = new Position { Row = positionError.Item1, Column = positionError.Item2 };
        }
    }

    public struct Position
    {
        public int Row;
        public int Column;
    }


    /// <summary>
    /// The Lexer class is responsible for tokenizing the input string into a list of tokens based on defined patterns and keywords.
    /// </summary>
    public class Lexer
    {
        private string input;
        private List<Token> tokens;

        /// <summary>
        /// A dictionary mapping token types to their corresponding regular expression patterns.
        /// </summary>
        public Dictionary<TokenType, string> Keywords = new Dictionary<TokenType, string> 
        {

            //Lines
            { TokenType.LineChange, @"\r"},
            {TokenType.Whitespace, @"\s+"},

            //Keywords
            { TokenType.Effect, @"\beffect\b"},
            { TokenType.Card, @"\bcard\b"},
            { TokenType.EffectParam, @"\bEffect\b"},
            { TokenType.Name, @"\bName\b" },
            { TokenType.Params, @"\bParams\b" },
            { TokenType.Action, @"\bAction\b" },
            { TokenType.Type, @"\bType\b" },
            { TokenType.Faction, @"\bFaction\b" },
            { TokenType.Power, @"\bPower\b" },
            { TokenType.Range, @"\bRange\b" },
            { TokenType.OnActivation, @"\bOnActivation\b" },
            { TokenType.Selector, @"\bSelector\b" },
            { TokenType.PostAction, @"\bPostAction\b" },
            { TokenType.Source, @"\bSource\b" },
            { TokenType.Single, @"\bSingle\b" },
            { TokenType.Predicate, @"\bPredicate\b" },
            { TokenType.In, @"\bin\b" },
            { TokenType.HandOfPlayer, @"\bHandOfPlayer\b" },
            { TokenType.Hand, @"\bHand\b" },
            { TokenType.DeckOfPlayer, @"\bDeckOfPlayer\b" },
            { TokenType.Deck, @"\bDeck\b" },
            { TokenType.Board, @"\bBoard\b" },

            { TokenType.TriggerPlayer, @"\bTriggerPlayer\b" },
            { TokenType.GraveYardOfPlayer, @"\bGraveYardOfPlayer\b"},
            { TokenType.GraveYard, @"\bGraveYard\b" },
            
            
            { TokenType.FieldOfPlayer, @"\bFieldOfPlayer\b" },
            { TokenType.Find, @"\bFind\b" },
            { TokenType.Push, @"\bPush\b" },
            { TokenType.Field, @"\bField\b" },
            { TokenType.SendBottom, @"\bSendBottom\b" },
            { TokenType.Pop, @"\bPop\b" },
            { TokenType.Add, @"\bAdd\b" },
            { TokenType.Remove, @"\bRemove\b" },
            { TokenType.Shuffle, @"\bShuffle\b" },
            { TokenType.Owner, @"\bOwner\b" },
            

            { TokenType.NumberType, @"\bNumber\b" },
            { TokenType.StringType, @"\bString\b" },
            {TokenType.Bool, @"\bBool\b"},

            //Booleans
            { TokenType.True, @"\btrue\b" },
            { TokenType.False, @"\bfalse\b" },
            {TokenType.For, @"\bfor\b"},
            {TokenType.While, @"\bwhile\b"},
            {TokenType.If, @"\bif\b"},
            {TokenType.ElIf, @"\belif\b"},
            {TokenType.Else, @"\belse\b"},
            {TokenType.Not, @"!" },
            {TokenType.And, @"\&\&"},
            {TokenType.Or, @"\|\|"},


            //Operators
            {TokenType.Pow, @"\^"},
            { TokenType.PlusEqual, @"\+\=" },
            { TokenType.MinusEqual, @"\-\=" },
            {TokenType.Increment, @"\+\+"},
            {TokenType.Decrement, @"\-\-"},
            {TokenType.Plus, @"\+"},
            {TokenType.Minus, @"\-"},
            {TokenType.Multiply, @"\*"},
            {TokenType.Divide, @"\/"},
            {TokenType.Equal, "=="},
            {TokenType.Arrow, "=>"},
            {TokenType.LessEq, "<="},
            {TokenType.MoreEq, ">="},
            {TokenType.Less, "<"},
            {TokenType.More, ">"},
            {TokenType.RIncrement, @"\+\+"},
            {TokenType.LIncrement, @"\+\+"},
            {TokenType.RDecrement, @"\-\-"},
            {TokenType.LDecrement, @"\-\-"},

            //Symbols
            {TokenType.SpaceConcatenation, @"@@"},
            {TokenType.Concatenation, @"@"},
            {TokenType.Assign, "="},
            {TokenType.Colon, @":" },
            {TokenType.Comma, @"," },
            {TokenType.Semicolon, @";" },
            {TokenType.LParen, @"\("},
            {TokenType.RParen, @"\)"},
            {TokenType.LBracket, @"\["},
            {TokenType.Index, @"\["},
            {TokenType.RBracket, @"\]"},
            {TokenType.LCurly, @"\{"},
            {TokenType.RCurly, @"\}"},

            //Identifiers
            {TokenType.Point, @"\." },
            {TokenType.Int, @"\b\d+\b"},
            {TokenType.String, "\".*?\""},
            {TokenType.Id, @"\b[A-Za-z_][A-Za-z_0-9]*\b"},

            //Comments
            {TokenType.Comment, @"\/\/[^\n]*\n"},
            {TokenType.Comments, @"/\*.*?\*/" },
        };

        public Lexer(string input)
        {
            this.input = input;
            this.tokens = new List<Token>();
        }

    /// <summary>
    /// Breaks down the input string into a sequence of tokens based on predefined patterns.
    /// Matches each segment with corresponding patterns, ignoring whitespace and line changes.
    /// Updates row and column counters as needed and returns a list of recognized tokens,
    /// preserving their types, values, and positions within the input.
    /// </summary>
        public List<Token> Tokenize()
        {
            int row = 0;
            int column = 0;
            while (input.Length != 0)
            {
                bool isfound = false;
                foreach (TokenType type in Keywords.Keys)
                {
                    string pattern = Keywords[type];
                    Match match = Regex.Match(input, "^" + pattern);
                    if (match.Success)
                    {
                        if (type != TokenType.Whitespace && type != TokenType.LineChange)
                        {
                            Token token = new Token(type, match.Value, (row, column));
                            tokens.Add(token);
                        }
                        if (type == TokenType.LineChange)
                        {
                            row++;
                            column = 0;
                        }
                        input = input.Substring(match.Value.Length);
                        column += match.Value.Length;
                        isfound = true;
                        break;
                    }
                }
                if (!isfound)
                {
                    break;
                }
            }
            return tokens;
        }

    }


    /// <summary>
    /// Enum representing the different types of tokens that can be identified in the source code.
    /// </summary>
    public enum TokenType
    {

        //Lines
        Whitespace,
        LineChange,

        //Operators
        For,
        While,
        True,
        False,
        If,
        ElIf,
        Else,
        Pow,
        Increment,
        Decrement,
        Plus,
        Minus,
        PlusEqual,
        MinusEqual,
        Multiply,
        Divide,
        And,
        Or,
        Less,
        More,
        Equal,
        LessEq,
        MoreEq,
        RIncrement,
        LIncrement,
        RDecrement,
        LDecrement,
        SpaceConcatenation,
        Concatenation,
        Assign,
        LParen,
        RParen,
        LBracket,
        RBracket,
        LCurly,
        RCurly,
        Int,
        String,
        Bool,
        Id,
        Colon,
        Comma,
        Semicolon,
        Not,
        Point,

        //Keywords
        Effect,
        EffectParam,
        Card,
        Name,
        Params,
        Action,
        Type,
        Faction,
        Power,
        Range,
        OnActivation,
        Selector,
        PostAction,
        Source,
        Single,
        Predicate,
        In,
        Hand,
        Owner,
        Deck,
        Board,
        TriggerPlayer,
        Find,
        Push,
        Field,
        DeckOfPlayer,
        GraveYard,
        GraveYardOfPlayer,
        HandOfPlayer,
        FieldOfPlayer,
        SendBottom,
        Index,
        Pop,
        Add,
        Remove,
        Shuffle,
        NumberType,
        StringType,
        Arrow,

        //Comments
        Comment,
        Comments,
    }
}
