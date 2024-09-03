using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;


namespace Compiler
{

    public class Parser
    {
        /// <summary>
        /// Determines whether the current token matches the expected token type.
        /// </summary>
        private bool LookAhead(TokenType token, TokenType token1)
        {
            return token == token1;
        }

        public int position;
        List<Token> tokens;
        public bool ExpectingAssigment;
        public bool ExpectingProp;

        private Dictionary<TokenType, Action<object>> Options;
        private Dictionary<string, Action<CardInstance, PropertyInfo>> CardParsing;
        private Dictionary<string, Action<EffectInstance, PropertyInfo>> EffectParsing;

        /// <summary>
        /// Initializes the tools and mappings used for parsing cards and effects.
        /// </summary
        private void InitializeTools()
        {
            Options = new Dictionary<TokenType, Action<object>>
        {
            { TokenType.Card, CardTreatment},
            { TokenType.Effect, EffectTreatment}
        };

            CardParsing = new Dictionary<string, Action<CardInstance, PropertyInfo>>
        {
            { "Name",  AssignTreatment},
            { "Type", AssignTreatment},
            { "Faction", AssignTreatment },
            { "Power", AssignTreatment },
            { "Range",RangeTreatment },
            { "OnActivation",  OnActivTreatment},
        };

            EffectParsing = new Dictionary<string, Action<EffectInstance, PropertyInfo>>
        {
            { "Name", AssignTreatment},
            { "Params",  ParamsTreatment},
            { "Action", ActionTreatment }
        };
        }

        /// <summary>
        /// Handles the parsing of card-related tokens and assigns parsed values to a <see cref="CardInstance"/>.
        /// </summary>
        private void CardTreatment(object program)
        {
            if (program is ProgramExpression p)
            {
                p.Instances.Add(ParseCard());
            }
        }

        /// <summary>
        /// Handles the parsing of effect-related tokens and assigns parsed values to an <see cref="EffectInstance"/>.
        /// </summary>
        private void EffectTreatment(object program)
        {
            if (program is ProgramExpression p)
            {
                p.Instances.Add(ParseEffect());
            }
        }

        /// <summary>
        /// Parses the tokens to create an <see cref="Expression"/> based on the current state of the parser.
        /// </summary>
        public Parser(List<Token> tokens)
        {
            position = 0;
            this.tokens = tokens;

            InitializeTools();
        }
        public Expression Parse()
        {
            Expression expression;
            expression = ParseGeneral();
            return expression;
        }

        /// <summary>
        /// Parses a general program expression from the list of tokens, processing cards and effects.
        /// </summary>
        private Expression ParseGeneral()
        {
            ProgramExpression general = new();
            Token token = tokens[position];
            int count = Errors.List.Count;
            while (position < tokens.Count && count == Errors.List.Count)
            {
                if (LookAhead(token.Type, TokenType.Effect) || LookAhead(token.Type, TokenType.Card))
                {
                    if (LookAhead(tokens[++position].Type, TokenType.LCurly))
                    {
                        position++;
                        Options[token.Type].Invoke(general);
                        if (position < tokens.Count)
                            token = tokens[position];
                    }
                    else
                        Errors.List.Add(new CompilingError("Expected Left Curly, invalid token", token.PositionError));
                }
                else
                    Errors.List.Add(new CompilingError("Only expects cards or effects", token.PositionError));
            }
            return general;
        }

        /// <summary>
        /// Parses a card definition from the token list and creates a <see cref="CardInstance"/>.
        /// Uses reflection to set properties of the <see cref="CardInstance"/> based on tokens.
        /// Each token is matched to a property of the card, and the corresponding parsing method
        /// is invoked to assign the property's value.
        /// </summary>
        /// <returns>A <see cref="CardInstance"/> object with properties set according to the parsed tokens.</returns>
        private CardInstance ParseCard()
        {
            CardInstance card = new();
            Token token = tokens[position];
            int count = Errors.List.Count;
            while (position < tokens.Count && count == Errors.List.Count)
            {
                if (LookAhead(token.Type, TokenType.Name) || LookAhead(token.Type, TokenType.Type) ||
                LookAhead(token.Type, TokenType.Range) || LookAhead(token.Type, TokenType.Power) ||
                LookAhead(token.Type, TokenType.Faction) || LookAhead(token.Type, TokenType.OnActivation))
                {
                    var instance = card.GetType();
                    var prop = instance.GetProperty(token.Meaning);
                    var pars = CardParsing[prop.Name];
                    pars.Invoke(card, prop);
                    token = tokens[position];
                }
                else if (LookAhead(token.Type, TokenType.RCurly))
                {
                    position++;
                    return card;
                }
                else
                    Errors.List.Add(new CompilingError("Invalid token, expecting properties of cards or Right Curly", token.PositionError));
            }
            return card;
        }


        /// <summary>
        /// Parses an effect definition from the token list and creates a <see cref="EffectInstance"/>.
        /// Uses reflection to set properties of the <see cref="EffectInstance"/> based on tokens.
        /// Each token is matched to a property of the effect, and the corresponding parsing method
        /// is invoked to assign the property's value.
        /// </summary>
        /// <returns>A <see cref="EffectInstance"/> object with properties set according to the parsed tokens.</returns>
        private EffectInstance ParseEffect()
        {
            EffectInstance effect = new();
            Token token = tokens[position];
            int count = Errors.List.Count;
            while (position < tokens.Count && count == Errors.List.Count)
            {
                if (LookAhead(token.Type, TokenType.Name) ||
                LookAhead(token.Type, TokenType.Params) ||
                LookAhead(token.Type, TokenType.Action))
                {
                    var instance = effect.GetType();
                    var prop = instance.GetProperty(token.Meaning);
                    var pars = EffectParsing[prop.Name];
                    pars.Invoke(effect, prop);
                    token = tokens[position];
                }
                else if (LookAhead(token.Type, TokenType.RCurly))
                {
                    position++;
                    return effect;
                }
                else
                {
                    Errors.List.Add(new CompilingError("Invalid token, expecting properties of effects or Right Curly", token.PositionError));
                }
            }
            return effect;
        }


        private void AssignTreatment(CardInstance card, PropertyInfo p)
        {
            p.SetValue(card, ParsePropertyAssignment());
        }
        private void AssignTreatment(EffectInstance effect, PropertyInfo p)
        {
            p.SetValue(effect, ParsePropertyAssignment());
        }

        private void RangeTreatment(CardInstance card, PropertyInfo p)
        {
            p.SetValue(card, ParseRanges());
        }

        private void OnActivTreatment(CardInstance card, PropertyInfo p)
        {
            p.SetValue(card, ParseOnActivation());
        }

        private void ParamsTreatment(EffectInstance effect, PropertyInfo p)
        {
            p.SetValue(effect, ParseParams());
        }

        private void ActionTreatment(EffectInstance effect, PropertyInfo p)
        {
            p.SetValue(effect, ParseAction());
        }


        /// <summary>
        /// Parses an expression from the token list using a precedence-based parsing strategy.
        /// This method recursively parses primary expressions and constructs a binary expression tree based on operator precedence.
        /// It starts by parsing the primary expression. Then, it checks for operators with precedence higher than the current parent precedence.
        /// If an operator is found, it recursively parses the right-hand side of the expression, creating a binary expression node.
        /// The method continues parsing and constructing the expression tree until it encounters an operator with lower or equal precedence or reaches the end of the tokens.
        /// The method uses the <see cref="Tools.GetPrecedence"/> dictionary to retrieve operator precedence and ensures correct handling of nested expressions.
        /// </summary>
        /// <param name="parentprecedence">The precedence level of the parent expression to ensure correct operator associativity.</param>
        /// <returns>A binary expression tree representing the parsed expression, or a primary expression if no higher precedence operators are found.</returns>
        public Expression ParseExpression(int parentprecedence = 0)
        {
            var left = ParsePrimaryExpression();

            while (position < tokens.Count)
            {
                int precedence;
                if (Tools.GetPrecedence.ContainsKey(tokens[position].Type))
                    precedence = Tools.GetPrecedence[tokens[position].Type];
                else
                    precedence = 0;
                if (precedence == 0 || precedence <= parentprecedence)
                    break;

                var operatortoken = tokens[position++].Type;
                var right = ParseExpression(precedence);
                left = new BinaryExpression(left, right, operatortoken);
            }
            return left;
        }

        /// <summary>
        /// Handles increment and index operations on expressions based on the current token.
        /// If an increment or decrement operator is found, it modifies the token type to represent the operation correctly.
        /// If an index operation is detected (square brackets), it parses the expression inside the brackets and constructs a binary expression with the index operation.
        /// If neither operation is found, it returns the original expression without modification.
        /// </summary>
        private Expression IncrementsorIndexer(bool increment, bool index, Expression returned)
        {
            if (increment && (tokens[position].Type == TokenType.Increment || tokens[position].Type == TokenType.Decrement))
            {//increments at right side
                if (tokens[position].Type == TokenType.Increment)
                    tokens[position].Type = TokenType.RIncrement;
                else
                    tokens[position].Type = TokenType.RDecrement;
                position++;
                return new UnaryExpression(returned, tokens[position - 1]);
            }
            else if (index && tokens[position].Type == TokenType.LBracket)
            {//Index
                Token token = tokens[position];
                if (tokens[++position].Type == TokenType.RBracket)
                {
                    Errors.List.Add(new CompilingError("Invalid Token, Expected an Expression to index", token.PositionError));
                }
                Expression Argument = ParseExpression();
                if (tokens[position].Type == TokenType.RBracket)
                {
                    position++;
                    return new BinaryExpression(returned, Argument, TokenType.Index);
                }
                else
                    Errors.List.Add(new CompilingError("Invalid Token, Expected a Right Bracket to index", token.PositionError));

            }
            return returned;
        }


        /// <summary>
        /// Parses a primary expression from the token list, handling various token types and constructs corresponding expression nodes.
        /// This method identifies and processes different types of tokens, such as literals, identifiers, unary operators, and method calls.
        /// </summary>
        public Expression ParsePrimaryExpression()
        {
            Expression returned = null;
            if (position >= tokens.Count)
                Errors.List.Add(new CompilingError("Unexpected end of input", tokens[position].PositionError));
            if (LookAhead(tokens[position].Type, TokenType.LParen))
            {
                position++;
                Expression expr = ParseExpression();
                if (!LookAhead(tokens[position].Type, TokenType.RParen))
                {
                    Errors.List.Add(new CompilingError("Missing closing parenthesis", tokens[position].PositionError));
                }
                position++;
                return IncrementsorIndexer(true, true, expr);
            }
            else if (tokens[position].Type == TokenType.Increment || tokens[position].Type == TokenType.Decrement)
            {
                if (tokens[position].Type == TokenType.Increment)
                    tokens[position].Type = TokenType.LIncrement;
                else
                    tokens[position].Type = TokenType.LDecrement;
                Token token = tokens[position];
                position++;
                Expression expr = ParsePrimaryExpression();
                return IncrementsorIndexer( true, true, new UnaryExpression(expr, token));
            }
            else if (LookAhead(tokens[position].Type, TokenType.False) || LookAhead(tokens[position].Type, TokenType.True))
            {
                position++;
                return new BooleanLiteral(tokens[position - 1]);
            }
            else if (LookAhead(tokens[position].Type, TokenType.Name) || LookAhead(tokens[position].Type, TokenType.Type) ||
                     LookAhead(tokens[position].Type, TokenType.Faction) || LookAhead(tokens[position].Type, TokenType.Power) ||
                     LookAhead(tokens[position].Type, TokenType.EffectParam) || LookAhead(tokens[position].Type, TokenType.Source) ||
                     LookAhead(tokens[position].Type, TokenType.Single) || LookAhead(tokens[position].Type, TokenType.Owner) ||
                     LookAhead(tokens[position].Type, TokenType.Deck) || LookAhead(tokens[position].Type, TokenType.GraveYard) ||
                     LookAhead(tokens[position].Type, TokenType.Field) || LookAhead(tokens[position].Type, TokenType.Board) ||
                     LookAhead(tokens[position].Type, TokenType.Hand) || LookAhead(tokens[position].Type, TokenType.TriggerPlayer)||
                     //FIXME: id here, there's nothing to fix just remember it
                     LookAhead(tokens[position].Type, TokenType.Id))
            {
                position++;
                return IncrementsorIndexer(true, true, new IdentifierExpression(tokens[position - 1]));
            }
            else if (LookAhead(tokens[position].Type, TokenType.String))
            {
                position++;
                return new StringExpression(tokens[position - 1]);
            }
            else if (LookAhead(tokens[position].Type, TokenType.Int))
            {
                position++;
                return new Number(tokens[position - 1]);
            }
            else if (LookAhead(tokens[position].Type, TokenType.Not) || LookAhead(tokens[position].Type, TokenType.Plus) || LookAhead(tokens[position].Type, TokenType.Minus))
            {
                Token unary = tokens[position];
                position++;
                Expression operand = ParsePrimaryExpression();
                return IncrementsorIndexer( true, true, new UnaryExpression(operand, unary));
            }
            else if (LookAhead(tokens[position].Type, TokenType.Shuffle) || LookAhead(tokens[position].Type, TokenType.Pop))
            {
                Token token = tokens[position];
                if (LookAhead(tokens[++position].Type, TokenType.LParen) && LookAhead(tokens[++position].Type, TokenType.RParen))
                {
                    position++;
                    return IncrementsorIndexer(true, true, new UnaryExpression(null, token));
                }
                else
                    Errors.List.Add(new CompilingError("Invalid Token, Expected a none parameters method sintax", token.PositionError));
            }
            else if (LookAhead(tokens[position].Type, TokenType.Push) || LookAhead(tokens[position].Type, TokenType.SendBottom)
                    || LookAhead(tokens[position].Type, TokenType.Remove) || LookAhead(tokens[position].Type, TokenType.HandOfPlayer)
                    || LookAhead(tokens[position].Type, TokenType.DeckOfPlayer) || LookAhead(tokens[position].Type, TokenType.Add)
                    || LookAhead(tokens[position].Type, TokenType.FieldOfPlayer) || LookAhead(tokens[position].Type, TokenType.GraveYardOfPlayer)
                    || LookAhead(tokens[position].Type, TokenType.Find))
            {
                Token token = tokens[position];
                if (LookAhead(tokens[++position].Type, TokenType.LParen))
                {
                    position++;
                    Expression argument;
                    if (!LookAhead(token.Type, TokenType.Find))
                        argument = ParseExpression();
                    else
                        argument = ParsePredicate(true);
                    if (LookAhead(tokens[position++].Type, TokenType.RParen))
                    {
                        return IncrementsorIndexer(true, true, new UnaryExpression(argument, token));
                    }
                }
            }
            Errors.List.Add(new CompilingError("Not recognizable primary token", tokens[position].PositionError));
            return null;
        }

        /// <summary>
        /// Parses property assignments in the code, handling various assignment operators and ensuring proper syntax.
        /// </summary>
        private Expression ParsePropertyAssignment()
        {
            Expression left;
            left = ParsePrimaryExpression();
            Token token = tokens[position];
            Expression right = null;
            Expression Binary = null;

            if (LookAhead(token.Type, TokenType.Assign) || LookAhead(token.Type, TokenType.Colon))
            {
                position++;
                right = ParseExpression();
                Binary = new BinaryExpression(left, right, token.Type);
            }
            else if (LookAhead(token.Type, TokenType.PlusEqual) || LookAhead(token.Type, TokenType.MinusEqual))
            {
                position++;
                right = ParseExpression();
                Binary = new BinaryExpression(left, right, token.Type);
            }


            if (LookAhead(tokens[position].Type, TokenType.Comma) || LookAhead(tokens[position].Type, TokenType.Semicolon) || LookAhead(tokens[position].Type, TokenType.RCurly))
            {
                if (!LookAhead(tokens[position].Type, TokenType.RCurly))
                    position++;
                if (Binary != null)
                    return Binary;
                else
                    Errors.List.Add(new CompilingError("Unexpected null reference", token.PositionError));
            }
            else
                Errors.List.Add(new CompilingError("Unexpected Comma or Semicolon", token.PositionError));
            return null;
        }

        /// <summary>
        /// Parses parameter assignments from effects, handling specific data types and assignment operators.
        /// </summary>
        private Expression ParseParamAssigment()
        {
            Expression left;
            left = ParsePrimaryExpression();
            Token token = tokens[position];
            Expression right = null;
            Expression Binary = null;
            if (LookAhead(token.Type, TokenType.Assign) || LookAhead(token.Type, TokenType.Colon))//Agregar formas como incremento etc...
            {
                position++;
                if (LookAhead(tokens[position].Type, TokenType.NumberType) || LookAhead(tokens[position].Type, TokenType.StringType) || LookAhead(tokens[position].Type, TokenType.Bool))
                {
                    right = new IdentifierExpression(tokens[position]);
                    position++;
                    Binary = new BinaryExpression(left, right, token.Type);
                    if (LookAhead(tokens[position].Type, TokenType.Comma) || LookAhead(tokens[position].Type, TokenType.Semicolon)
                    || LookAhead(tokens[position].Type, TokenType.RCurly))
                    {
                        if (!LookAhead(tokens[position].Type, TokenType.RCurly))
                            position++;
                        if (Binary != null)
                            return Binary;
                        else
                            Errors.List.Add(new CompilingError("Unexpected null reference", token.PositionError));
                    }
                }
            }
            Errors.List.Add(new CompilingError("Unexpected Comma or Semicolon", token.PositionError));
            return null;
        }

        /// <summary>
        /// Parses instruction assignments, handling assignment operators and ensuring proper syntax.
        /// </summary>
        private Expression ParseInstructionAssigment()
        {
            Expression left;
            left = ParseExpression();
            Token token = tokens[position];
            Expression right = null;
            Expression Binary = null;

            if (LookAhead(token.Type, TokenType.Assign) || LookAhead(token.Type, TokenType.Colon))
            {
                position++;
                right = ParseExpression();
                Binary = new BinaryExpression(left, right, token.Type);
            }
            else if (LookAhead(token.Type, TokenType.PlusEqual) || LookAhead(token.Type, TokenType.MinusEqual))
            {
                position++;
                right = ParseExpression();
                Binary = new BinaryExpression(left, right, token.Type);
            }


            if (LookAhead(tokens[position].Type, TokenType.Comma) || LookAhead(tokens[position].Type, TokenType.Semicolon) || LookAhead(tokens[position].Type, TokenType.RCurly))
            {
                if (!LookAhead(tokens[position].Type, TokenType.RCurly))
                    position++;
                if (Binary != null)
                    return Binary;
                else
                    return left;
            }
            else
            {
                Errors.List.Add(new CompilingError("Unexpected Comma or Semicolon", token.PositionError));
                return null;
            }
        }

        /// <summary>
        /// Parses a list of range expressions enclosed in brackets. It expects strings within the brackets and handles delimiters like commas and closing brackets.
        /// </summary>
        public List<Expression> ParseRanges()
        {
            if (LookAhead(tokens[++position].Type, TokenType.Colon) &&
               LookAhead(tokens[++position].Type, TokenType.LBracket))
            {
                List<Expression> ranges = new();
                position++;
                int count = Errors.List.Count;
                while (position < tokens.Count && count == Errors.List.Count)
                {
                    if (LookAhead(tokens[position].Type, TokenType.String))
                    {
                        ranges.Add(ParseExpression());
                        if (LookAhead(tokens[position].Type, TokenType.Comma) ||
                           LookAhead(tokens[position].Type, TokenType.RBracket))
                        {
                            position++;
                            if (LookAhead(tokens[position - 1].Type, TokenType.RBracket))
                            {
                                if (LookAhead(tokens[position].Type, TokenType.Comma) ||
                                  LookAhead(tokens[position].Type, TokenType.Semicolon)
                                || LookAhead(tokens[position].Type, TokenType.RCurly))
                                {
                                    if (!LookAhead(tokens[position].Type, TokenType.RCurly))
                                    {
                                        position++;
                                    }
                                    break;
                                }
                                else
                                    Errors.List.Add(new CompilingError("Invalid token, expecting RCurly, Semicolon or Comma", tokens[position].PositionError));
                            }
                        }
                        else
                            Errors.List.Add(new CompilingError("Invalid token, expecting Comma", tokens[position].PositionError));
                    }
                    else
                        Errors.List.Add(new CompilingError("Invalid token", tokens[position].PositionError));
                }
                return ranges;
            }
            else
            {
                Errors.List.Add(new CompilingError("Invalid token", tokens[position].PositionError));
                return null;
            }
        }

        /// <summary>
        /// Parses an OnActivation block, It collects effects enclosed in curly braces until a closing bracket is encountered.
        /// </summary>
        private OnActivation ParseOnActivation()
        {
            OnActivation activation = new();
            position++;
            if (LookAhead(tokens[position++].Type, TokenType.Colon) &&
                LookAhead(tokens[position++].Type, TokenType.LBracket))
            {
                int count = Errors.List.Count;
                while (position < tokens.Count && count == Errors.List.Count)
                {
                    if (LookAhead(tokens[position].Type, TokenType.LCurly))
                    {
                        activation.Effects.Add(ParseEffectParam());
                    }
                    else if (LookAhead(tokens[position].Type, TokenType.RBracket))
                    {
                        position++;
                        break;
                    }
                    else
                        Errors.List.Add(new CompilingError("Invalid token in OnActivation method", tokens[position].PositionError));
                }
            }
            return activation;
        }

        /// <summary>
        /// Parses a list of parameter assignments, It collects assignments identified by Id tokens and handles delimiters.
        /// </summary>
        private List<Expression> ParseParams()
        {
            List<Expression> parameters = new();
            Token token = tokens[++position];
            if (LookAhead(tokens[position++].Type, TokenType.Colon) &&
               LookAhead(tokens[position++].Type, TokenType.LCurly))
            {
                int count = Errors.List.Count;
                while (position < tokens.Count && count == Errors.List.Count)
                {
                    token = tokens[position];
                    if (LookAhead(token.Type, TokenType.Id))
                    {
                        parameters.Add(ParseParamAssigment());
                        token = tokens[position];
                    }

                    else if (LookAhead(tokens[position++].Type, TokenType.RCurly))
                    {
                        if (LookAhead(tokens[position++].Type, TokenType.Semicolon) ||
                           LookAhead(tokens[position - 1].Type, TokenType.Comma))
                            break;
                    }
                    else
                        Errors.List.Add(new CompilingError("Invalid params definition", token.PositionError));
                }
            }
            else
                Errors.List.Add(new CompilingError("Invalid token", token.PositionError));
            return parameters;
        }

        /// <summary>
        /// Parses a list of effect parameters.
        /// </summary>
        private List<Expression> ParseEffParams()
        {
            List<Expression> parameters = new();
            Token token = tokens[++position];
            if (LookAhead(tokens[position++].Type, TokenType.Colon) &&
               LookAhead(tokens[position++].Type, TokenType.LCurly))
                {
                int count = Errors.List.Count;
                while (position < tokens.Count && count == Errors.List.Count)
                {
                    token = tokens[position];
                    if (LookAhead(token.Type, TokenType.Id))
                    {
                        parameters.Add(ParsePropertyAssignment());
                        token = tokens[position];
                    }
                    else if (LookAhead(token.Type, TokenType.Name))
                    {
                        parameters.Add(ParsePropertyAssignment());
                    }
                    else if (LookAhead(tokens[position].Type, TokenType.RCurly))
                    {
                        if (LookAhead(tokens[++position].Type, TokenType.Semicolon) ||
                           LookAhead(tokens[position].Type, TokenType.Comma))
                        {
                            position++;
                            break;
                        }
                    }
                    else
                        Errors.List.Add(new CompilingError("Invalid effect parameter definition", token.PositionError));
                }
            }
            else
                Errors.List.Add(new CompilingError("Invalid token", token.PositionError));
            return parameters;
        }

        /// <summary>
        /// Parses an Action declaration, which includes targets, context, and a block of instructions. It expects specific tokens to define the action's structure.
        /// </summary>
        private Action ParseAction()
        {
            Action Action = new();
            position++;
            if (LookAhead(tokens[position++].Type, TokenType.Colon))
            {
                if (LookAhead(tokens[position++].Type, TokenType.LParen) &&
                   LookAhead(tokens[position++].Type, TokenType.Id))
                    Action.Targets = new IdentifierExpression(tokens[position - 1]);

                if (LookAhead(tokens[position++].Type, TokenType.Comma) &&
                   LookAhead(tokens[position++].Type, TokenType.Id))
                    Action.Context = new IdentifierExpression(tokens[position - 1]);

                if (LookAhead(tokens[position++].Type, TokenType.RParen) &&
                   LookAhead(tokens[position++].Type, TokenType.Arrow))
                    if (LookAhead(tokens[position].Type, TokenType.LCurly))
                    {
                        position++;
                        Action.Instructions = ParseInstructionBlock();
                    }
                    else
                        Action.Instructions = ParseInstructionBlock(true);
            }
            else
                Errors.List.Add(new CompilingError("Invalid Action declaration", tokens[position].PositionError));
            return Action;
        }

        /// <summary>
        /// Parses an EffectParam object enclosed in curly braces. It handles various types of parameters such as effects, selectors, and post-actions, and checks for proper syntax.
        /// </summary>
        private EffectParam ParseEffectParam()
        {
            EffectParam effect = new();
            if (LookAhead(tokens[position++].Type, TokenType.LCurly))
            {
                int count = Errors.List.Count;
                while (position < tokens.Count && count == Errors.List.Count)
                {
                    switch (tokens[position].Type)
                    {
                        case TokenType.EffectParam:
                            if (LookAhead(tokens[position + 2].Type, TokenType.LCurly))
                                effect.Effect = ParseEffParams();
                            else
                                effect.Effect.Add(ParsePropertyAssignment());
                            break;
                        case TokenType.Selector:
                            effect.Selector = ParseSelector();
                            break;
                        case TokenType.PostAction:
                            position++;
                            if (LookAhead(tokens[position++].Type, TokenType.Colon))
                                effect.PostAction = ParseEffectParam();
                            else
                                Errors.List.Add(new CompilingError("Expected Colon in effect parameter definition", tokens[position].PositionError));
                            break;
                        case TokenType.RCurly:
                            if (LookAhead(tokens[++position].Type, TokenType.Comma) ||
                               LookAhead(tokens[position].Type, TokenType.Semicolon) ||
                               LookAhead(tokens[position].Type, TokenType.RBracket))
                            {
                                if (!LookAhead(tokens[position].Type, TokenType.RBracket))
                                    position++;
                            }
                            return effect;
                        default:
                            {
                                Errors.List.Add(new CompilingError("Expected card property", tokens[position].PositionError));
                                return null;
                            }
                    }
                }
            }
            return effect;
        }

        /// <summary>
        /// Parses a block of instructions enclosed in curly braces. It handles various types of instructions and continues until a closing curly brace is found. It can parse multiple instructions or just a single instruction based on the parameter.
        /// </summary>
        /// <param name="single">Whether to parse a single instruction block or multiple instructions.</param>
        private InstructionBlock ParseInstructionBlock(bool single = false)
        {
            InstructionBlock block = new();
            int count = Errors.List.Count;
            do
            {
                if (LookAhead(tokens[position].Type, TokenType.Id) || LookAhead(tokens[position].Type, TokenType.Increment) || LookAhead(tokens[position].Type, TokenType.Decrement))
                {
                    block.Instructions.Add(ParseInstructionAssigment());
                }
                else if (LookAhead(tokens[position].Type, TokenType.For))
                {
                    block.Instructions.Add(ParseFor());
                }
                else if (LookAhead(tokens[position].Type, TokenType.While))
                {
                    block.Instructions.Add(ParseWhile());
                }
                else if (LookAhead(tokens[position].Type, TokenType.RCurly))
                {
                    position++;
                    break;
                }
                else
                    Errors.List.Add(new CompilingError("Invalid instruction definition", tokens[position].PositionError));
            }
            while (count == Errors.List.Count && !single);
            return block;
        }

        /// <summary>
        /// Parses a Selector object enclosed in curly braces. It processes tokens for source, single, and predicate properties, handling errors for invalid tokens.
        /// </summary>
        private Selector ParseSelector()
        {
            Selector selector = new();
            position++;
            if (LookAhead(tokens[position++].Type, TokenType.Colon) && LookAhead(tokens[position++].Type, TokenType.LCurly))
                {
                int count = Errors.List.Count;
                while (position < tokens.Count && count == Errors.List.Count)
                {
                    switch (tokens[position].Type)
                    {
                        case TokenType.Source:
                            selector.Source = ParsePropertyAssignment();
                            break;
                        case TokenType.Single:
                            selector.Single = ParsePropertyAssignment();
                            break;
                        case TokenType.Predicate:
                            position++;
                            if (LookAhead(tokens[position++].Type, TokenType.Colon))
                                selector.Predicate = ParsePredicate();
                            else
                                Errors.List.Add(new CompilingError($"Invalid token {tokens[position-1]}, Expected Colon", tokens[position-1].PositionError));
                            break;
                        case TokenType.RCurly:
                            if (LookAhead(tokens[++position].Type, TokenType.Comma) || LookAhead(tokens[position].Type, TokenType.Semicolon) || LookAhead(tokens[position].Type, TokenType.RCurly))
                            {
                                if (!LookAhead(tokens[position].Type, TokenType.RCurly))
                                    position++;
                                return selector;
                            }
                            else
                            {
                                Errors.List.Add(new CompilingError("Invalid token", tokens[position].PositionError));
                                break;
                            }

                        default:
                            {
                                Errors.List.Add(new CompilingError("Invalid token", tokens[position].PositionError));
                                return null;
                            }
                    }
                }
            }
            return selector;
        }

        /// <summary>
        /// Parses a Predicate object, which includes a unit identifier and a condition expression. It optionally checks for additional tokens based on the context.
        /// </summary>
        public Predicate ParsePredicate(bool frommethod = false)
        {
            
                Predicate predicate = new();
                if (LookAhead(tokens[position].Type, TokenType.LParen) && LookAhead(tokens[++position].Type, TokenType.Id))
                    predicate.Unit = new IdentifierExpression(tokens[position]);
                if (LookAhead(tokens[++position].Type, TokenType.RParen) && LookAhead(tokens[++position].Type, TokenType.Arrow))
                {
                    position++;
                    predicate.Condition = ParseExpression();
                    if (!frommethod)
                        if (LookAhead(tokens[position].Type, TokenType.Comma) || LookAhead(tokens[position].Type, TokenType.RCurly))
                        {
                            if (LookAhead(tokens[position].Type, TokenType.Comma))
                                position++;
                        }
                        else
                            Errors.List.Add(new CompilingError("Expected Comma", tokens[position].PositionError));
                    return predicate;
                }
                else
                    Errors.List.Add(new CompilingError("Invalid token", tokens[position].PositionError));
            return null;
        }

        /// <summary>
        /// Parses a ForExpression object that includes a variable identifier, a collection expression, and a block of instructions. It handles syntax for For expressions and errors for invalid tokens.
        /// </summary>
        private ForExpression ParseFor()
        {
            ForExpression ForExp = new();
            position++;
            if (LookAhead(tokens[position++].Type, TokenType.Id))
            {
                ForExp.Variable = new IdentifierExpression(tokens[position - 1]);
                if (LookAhead(tokens[position++].Type, TokenType.In))
                {
                    ForExp.Collection = ParseExpression();
                    if (LookAhead(tokens[position++].Type, TokenType.LCurly))
                    {
                        ForExp.Instructions = ParseInstructionBlock();
                        if (LookAhead(tokens[position].Type, TokenType.Comma) || LookAhead(tokens[position].Type, TokenType.Semicolon))
                        {
                            position++;
                        }
                    }
                    else
                    {
                        position--;
                        ForExp.Instructions = ParseInstructionBlock(true);
                    }
                }
                else
                {
                    Errors.List.Add(new CompilingError("Invalid token", tokens[position].PositionError));
                }
            }
            else
                Errors.List.Add(new CompilingError("Invalid for declaration statement", tokens[position].PositionError));
            return ForExp;
        }

        /// <summary>
        /// Parses a WhileExpression object, which includes a condition expression and a block of instructions. It handles syntax for While expressions and errors for invalid tokens.
        /// </summary
        private WhileExpression ParseWhile()
        {
            WhileExpression WhileExp = new();
            position++;
            if (LookAhead(tokens[position++].Type, TokenType.LParen))
            {
                WhileExp.Condition = ParseExpression();
                if (LookAhead(tokens[position++].Type, TokenType.RParen))
                {
                    if (LookAhead(tokens[position++].Type, TokenType.LCurly))
                    {
                        WhileExp.Instructions = ParseInstructionBlock();
                    }
                    else
                    {
                        position--;
                        WhileExp.Instructions = ParseInstructionBlock(true);
                    }

                }
            }
            else
                Errors.List.Add(new CompilingError("Invalid while declaration statement", tokens[position].PositionError));
            return WhileExp;
        }
    }
}