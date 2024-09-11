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
    /// Represents a binary expression that consists of a left and right operand, and an operator.
    /// The class provides functionality for semantic checking and evaluation of various binary operations,
    /// including arithmetic, logical, comparison, and assignment operations.
    /// </summary>
    public class BinaryExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public TokenType Operator { get; set; }

        public BinaryExpression(Expression left, Expression right, TokenType Op)
        {
            Left = left;
            Right = right;
            Operator = Op;
            this.printed = Op.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj is BinaryExpression bin)
            {
                return bin.Left.Equals(Left) && bin.Right.Equals(Right) && bin.Operator == Operator;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (Left?.GetHashCode() ?? 0) ^ (Right?.GetHashCode() ?? 0) ^ Operator.GetHashCode();
        }

        public override ValueType? CheckSemantic(Scope scope)
        {
            if (Tools.GetOperatorType(Operator) != null)
            {
                var type = Tools.GetOperatorType(Operator);
                if (Left.CheckSemantic(scope) == type && Right.CheckSemantic(scope) == type)
                {
                    Left.CheckType = type;
                    Right.CheckType = type;
                    if (Tools.GetPrecedence.ContainsKey(Operator) && Tools.GetPrecedence[Operator] == 2)
                        return ValueType.Bool;
                    else
                        return type;
                }
                else
                    Errors.List.Add(new CompilingError($"Expected {Tools.GetOperatorType(Operator)} Type as an {Operator} argument", new Position()));
            }

            else if (Operator == TokenType.Equal)
            {
                Left.CheckType = Left.CheckSemantic(scope);
                Right.CheckType = Right.CheckSemantic(scope);
                if (Left.CheckType == Right.CheckType)
                {
                    return ValueType.Bool;
                }
                else
                    Errors.List.Add(new CompilingError("Expected the same type in both sides of the equal operator", new Position()));
            }

            else if (Operator == TokenType.Index)
            {
                if (Left.CheckType != ValueType.CardCollection)
                {
                    if (Left.CheckSemantic(scope) == ValueType.CardCollection) 
                    {
                        Left.CheckType = ValueType.CardCollection;
                    }
                    else
                        Errors.List.Add(new CompilingError("Expected a CardCollection as a left argument of the Index operator", new Position()));
                }
                if (Right.CheckSemantic(scope) == ValueType.Int)
                {
                    Right.CheckType = ValueType.Int;
                    return ValueType.Card;
                }
                else
                    Errors.List.Add(new CompilingError("Expected an Int as a right argument of the Index operator", new Position()));
            }

            else if (Operator == TokenType.Colon || Operator == TokenType.Assign)
            {
                if (Left is UnaryExpression un && (un.Operator == TokenType.RIncrement || un.Operator == TokenType.LIncrement
            || un.Operator == TokenType.RDecrement || un.Operator == TokenType.LDecrement))
                {
                    Errors.List.Add( new CompilingError($"Semantic Error at assignment, the left side can't be an increment or decrement", new Position()));
                }
                if (Left is BinaryExpression bin && bin.Right is UnaryExpression RightUn && (RightUn.Operator == TokenType.RIncrement || RightUn.Operator == TokenType.LIncrement
                    || RightUn.Operator == TokenType.RDecrement || RightUn.Operator == TokenType.LDecrement))
                {
                    Errors.List.Add(new CompilingError($"Semantic Error at assignment, the left side can't be an increment or decrement", new Position()));
                }



                Right.CheckType = Right.CheckSemantic(scope);
                ValueType? tempforOut;
                object v;
                if (scope == null || !scope.Find(Left, out tempforOut, out v) || !scope.WithoutReps)
                {
                    Left.CheckType = Left.CheckSemantic(scope);
                    if (Tools.VariableTypes.Contains(Left.CheckType))
                    {
                        if (Left.CheckType == Right.CheckType || Left.CheckType == ValueType.Unassigned)
                        {
                            Left.CheckType = Right.CheckType;
                            scope?.AddVar(Left);
                        }
                        else
                            Errors.List.Add(new CompilingError("Expected the same type in both sides of the equal operator", new Position()));
                    }
                    else
                        Errors.List.Add(new CompilingError("Expected a variable type in the left side of the equal operator", new Position()));
                }
                else
                    Errors.List.Add(new CompilingError("Variable already declared", new Position()));

                CheckType = Right.CheckType;
                return Right.CheckType;
            }

            else if (Operator == TokenType.Point)
            {
                Left.CheckType = Left.CheckSemantic(scope);
                if (Right is UnaryExpression unary && (unary.Operator == TokenType.RIncrement || unary.Operator == TokenType.LIncrement
                    || unary.Operator == TokenType.RDecrement || unary.Operator == TokenType.LDecrement))
                {
                    if (unary.Parameter is Atom T && Tools.GetPossibleMethods(Left.CheckType).Contains(T.Value.Type))
                    {
                        Right.CheckType = Right.CheckSemantic(scope);
                        return Tools.GetKeywordType(T.Value.Type);
                    }
                    else
                        Errors.List.Add(new CompilingError("Semantic Error, the operand of an increment is necesary to be a terminal", new Position()));
                }
                else if (Left.CheckType != ValueType.Null && Right is Atom right && Tools.GetPossibleMethods(Left.CheckType).Contains(right.Value.Type))
                {
                    Right.CheckType = right.CheckSemantic(scope);
                    return Tools.GetKeywordType(right.Value.Type);
                }
                
                else if (Left.CheckType != ValueType.Null && Right is BinaryExpression binary && binary.Operator == TokenType.Index)
                {
                    if (binary.Left is Atom left && Tools.GetPossibleMethods(Left.CheckType).Contains(left.Value.Type))
                    {
                        binary.Left.CheckType = Tools.GetKeywordType(left.Value.Type);
                        return binary.CheckSemantic(scope);
                    }
                    else
                        Errors.List.Add(new CompilingError("Expected a valid method", new Position()));
                }
            }

            else
                Errors.List.Add(new CompilingError("Unknown operator", new Position()));
            return null;
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            if (Operator != TokenType.Point && Operator != TokenType.Index
            && Operator != TokenType.Assign && Operator != TokenType.Colon
            && Operator != TokenType.PlusEqual && Operator != TokenType.MinusEqual)
            {
                Right.Result = Right.Evaluate(scope, set, instance);
                Left.Result = Left.Evaluate(scope, set, instance);

                switch (Operator)
                {
                    case TokenType.Plus:
                        this.Result = (int)Left.Result + (int)Right.Result;
                        break;

                    case TokenType.Minus:
                        this.Result = (int)Left.Result - (int)Right.Result;
                        break;

                    case TokenType.Multiply:
                        this.Result = (int)Left.Result * (int)Right.Result;
                        break;

                    case TokenType.Divide:
                        if ((int)Right.Result == 0)
                            Errors.List.Add(new CompilingError("Division by zero", new Position()));
                        else
                            this.Result = (int)Left.Result / (int)Right.Result;
                        break;

                    case TokenType.Pow:
                        this.Result = Convert.ToInt32(Math.Pow((int)Left.Result, (int)Right.Result));
                        break;

                    case TokenType.Less:
                        this.Result = (int)Left.Result < (int)Right.Result;
                        break;

                    case TokenType.More:
                        this.Result = (int)Left.Result > (int)Right.Result;
                        break;

                    case TokenType.And:
                        this.Result = (bool)Left.Result && (bool)Right.Result;
                        break;

                    case TokenType.Or:
                        this.Result = (bool)Left.Result || (bool)Right.Result;
                        break;

                    case TokenType.Concatenation:
                        this.Result = Left.Result.ToString() + Right.Result.ToString();
                        break;

                    case TokenType.SpaceConcatenation:
                        this.Result = Left.Result.ToString() + " " + Right.Result.ToString();
                        break;

                    case TokenType.Equal:
                        this.Result = Left.Result.Equals(Right.Result);
                        break;

                    case TokenType.LessEq:
                        this.Result = (int)Left.Result <= (int)Right.Result;
                        break;

                    case TokenType.MoreEq:
                        this.Result = (int)Left.Result >= (int)Right.Result;
                        break;
                }

                return this.Result!;
            }
            else if (Operator == TokenType.PlusEqual)
            {
                Right.Result = Right.Evaluate(scope, set, instance);
                object Result = Left.Evaluate(scope, set, instance);
                Left.Evaluate(scope, (int)Result! + (int)Right.Result);
                this.Result = Left.Result;
                return Left.Result;
            }
            else if (Operator == TokenType.MinusEqual)
            {
                Right.Result = Right.Evaluate(scope, set, instance);
                Result = Left.Evaluate(scope, set, instance);
                Left.Evaluate(scope, (int)Result! - (int)Right.Result);
                this.Result = Left.Result;
                return Left.Result;
            }
            else if (Operator == TokenType.Index)
            {
                Right.Result = Right.Evaluate(scope, set, instance);
                Left.Result = Left.Evaluate(scope, set, instance);
                if (Left.Result is List<UnityCard> list)
                {
                    if ((int)Right.Result < 0 || (int)Right.Result >= list.Count)
                        Errors.List.Add(new CompilingError("Index out of range", new Position()));

                    return list[(int)Right.Result];
                }
                else
                    Errors.List.Add(new CompilingError("Expected a CardCollection", new Position()));
            }
            else if (Operator == TokenType.Assign || Operator == TokenType.Colon)
            {
                Right.Result = Right.Evaluate(scope, null);
                Left.Evaluate(scope, Right.Result);
                Result = Left.Result;
                return Left.Result;
            }
            else if (Operator == TokenType.Point)
            {
                Left.Result = Left.Evaluate(scope, null!, instance);
                Right.Result = Right.Evaluate(scope, set, Left.Result);

                return Right.Result;
            }
            else
                Errors.List.Add(new CompilingError("Unknown operator", new Position()));
            return null;
        }

    }
}