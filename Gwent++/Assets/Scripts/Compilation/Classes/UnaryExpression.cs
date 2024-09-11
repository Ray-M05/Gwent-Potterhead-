using System.Reflection;
using ListExtensions;
using System.Collections.Generic;
using System;
using LogicalSide;
using Unity.VisualScripting;
using System.Reflection.Emit;

namespace Compiler
{
    /// <summary>
    /// Represents a unary expression in the abstract syntax tree (AST).
    /// A unary expression is an expression that consists of a single operand and an operator.
    /// This class handles various unary operations such as increment, decrement, logical negation,
    /// and card-specific operations like Add, Remove, Push, and others.
    /// </summary>
    public class UnaryExpression : Atom
    {
        public Expression Parameter { get; set; }
        public TokenType Operator { get; set; }

        public Dictionary<TokenType, ValueType> ValueTypers;
        public UnaryExpression(Expression operand, Token Operator) : base(Operator)
        {
            Parameter = operand;
            this.Operator = Operator.Type;
            printed = Operator.Type.ToString();
            ValueTypers = new()
        {
            { TokenType.SendBottom ,ValueType.Card },
            { TokenType.Remove ,ValueType.Card },
            { TokenType.Push ,ValueType.Card },
            { TokenType.Add ,ValueType.Card },
            { TokenType.HandOfPlayer ,ValueType.Player },
            { TokenType.DeckOfPlayer ,ValueType.Player },
            { TokenType.GraveYardOfPlayer ,ValueType.Player },
            { TokenType.FieldOfPlayer ,ValueType.Player },
            { TokenType.RDecrement ,ValueType.Int },
            { TokenType.LDecrement ,ValueType.Int },
            { TokenType.RIncrement ,ValueType.Int },
            { TokenType.LIncrement ,ValueType.Int },
            { TokenType.Not ,ValueType.Bool },
            { TokenType.Find, ValueType.Predicate}
        };
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            switch (Operator)
            {
                case TokenType.Add:
                case TokenType.Remove:
                case TokenType.Push:
                case TokenType.SendBottom:
                case TokenType.Shuffle:
                case TokenType.HandOfPlayer:
                case TokenType.DeckOfPlayer:
                case TokenType.GraveYardOfPlayer:
                case TokenType.FieldOfPlayer:
                case TokenType.Find:
                case TokenType.Pop:
                    {
                        System.Type type = instance.GetType();
                        string methodName = Operator.ToString();
                        if (methodName == "Add")
                            methodName = "AddCard";

                        else if (methodName == "Remove")
                            methodName = "RemoveCard";
                        MethodInfo methodInfo = type.GetMethod(methodName);
                        if (instance is List<UnityCard> list)
                        {
                            switch (methodName)
                            {
                                case "Push":
                                case "AddCard":
                                    {
                                        object card = Parameter.Evaluate(scope, null);
                                        list.AddCard((UnityCard)card);
                                        return null;
                                    }
                                case "RemoveCard":
                                    {
                                        object card = Parameter.Evaluate(scope, null);
                                        list.RemoveCard((UnityCard)card);
                                        return null;
                                    }
                                case "SendBottom":
                                    {
                                        object card = Parameter.Evaluate(scope, null);
                                        list.SendBottom((UnityCard)card);
                                        return null;
                                    }
                                case "Shuffle":
                                    {
                                        list.Shuffle();
                                        return null;
                                    }
                                case "Pop":
                                    {
                                        return list.Pop();
                                    }
                                case "Find":
                                    {
                                        return list.Find((Predicate)Parameter, scope);
                                    }

                            }

                        }

                        Result = methodInfo.Invoke(instance, new object[] { Parameter.Evaluate(scope, null) });
                        return Result!;
                    }

                case TokenType.RDecrement:
                    object Valor = (int)Parameter.Evaluate(scope, (int)Parameter.Evaluate(scope, null, instance) - 1, instance);
                    Result = (int)Valor + 1;
                    Processor.UpdateScope(Parameter, scope);
                    return (int)Result;

                case TokenType.LDecrement:
                    object Val = (int)Parameter.Evaluate(scope, (int)Parameter.Evaluate(scope, null, instance) - 1, instance);
                    Result = (int)Val;
                    Processor.UpdateScope(Parameter, scope);
                    return (int)Result;

                case TokenType.RIncrement:
                    object Valo = (int)Parameter.Evaluate(scope, (int)Parameter.Evaluate(scope, null, instance) + 1, instance);
                    Result = (int)Valo - 1;
                    Processor.UpdateScope(Parameter, scope);
                    return (int)Result;

                case TokenType.LIncrement:
                    object V = (int)Parameter.Evaluate(scope, (int)Parameter.Evaluate(scope, null, instance) + 1, instance);
                    Result = (int)V;
                    Processor.UpdateScope(Parameter, scope);
                    return (int)Result;

                case TokenType.Minus:
                    Result = (int)Parameter.Evaluate(scope, null) * -1;
                    return (int)Result - 1;

                case TokenType.Plus:
                    Result = Parameter.Evaluate(scope, null);
                    return Result;

                case TokenType.Not:
                    {
                        Result = Parameter.Evaluate(scope, null);
                        return !(bool)Result;
                    }
            }

            Errors.List.Add(new CompilingError("Invalid Unary Operator", new Position()));
            return null;
        }

        public override ValueType? CheckSemantic(Scope scope)
        {
            if (Operator == TokenType.RDecrement || Operator == TokenType.LDecrement || Operator == TokenType.RIncrement || Operator == TokenType.LIncrement)
            {
                if (!(Parameter is IdentifierExpression))
                {
                    if(!(Parameter is BinaryExpression param && param.Right is IdentifierExpression))
                    throw new Exception("Semantic Error, you can only use Decrement/Increment on Identifiers");
                }
            }

            if (Parameter != null && ValueTypers.ContainsKey(Operator))
            {
                ValueType type = ValueTypers[Operator];
                if (Parameter.CheckSemantic(scope) != type)
                    Errors.List.Add(new CompilingError($"Expected {type} Type as an {Operator} argument", new Position()));
                else
                    Parameter.CheckType = type;
            }
            CheckType = Tools.GetKeywordType(Operator);
            return CheckType;
        }
    }
}