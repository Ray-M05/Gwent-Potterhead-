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
    /// Represents an identifier expression in the abstract syntax tree (AST).
    /// An identifier expression refers to a variable, property, or method by its name.
    /// </summary>
    public class IdentifierExpression : Atom
    {
        public IdentifierExpression(Token token) : base(token)
        {
            this.printed = "ID";
        }

        public override ValueType? CheckSemantic(Scope scope)
        {
            if (Tools.GetKeywordType(Value.Type) != null)
            {
                CheckType = Tools.GetKeywordType(Value.Type);
                return CheckType;
            }
            else
            {
                ValueType? type;
                object v;
                if (scope != null && scope.Find(this, out type, out v))
                {
                    CheckType = type;
                    return type;
                }
                else
                {
                    CheckType = ValueType.Unassigned;
                    return ValueType.Unassigned;
                }
            }
        }

        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            if (Tools.GetPossibleMethods(ValueType.Context).Contains(Value.Type))
            {
                if (instance is IDeckContext context)
                {
                    var cont = context.GetType();
                    return cont.GetProperty(Value.Meaning).GetValue(context);
                }
                else
                    Errors.List.Add(new CompilingError("Expected a context", new Position()));
            }
            if (Tools.GetPossibleMethods(ValueType.Card).Contains(Value.Type) && instance is Card card)
            {
                if (set != null)
                {
                    string propertyName = Value.Meaning;
                    PropertyInfo property = card.GetType().GetProperty(propertyName);
                    property.SetValue(card, set);
                    return set;
                }
                else
                {
                    return card.GetType().GetProperty(Value.Meaning).GetValue(card);
                }
            }
            else if (set != null)
            {
                Result = set;
                if (scope != null)
                    scope.AddVar(this);
                return Result;
            }
            else
            {
                object value = null;
                if (Value != null)
                {
                    ValueType? v;
                    if (scope != null)
                        scope.Find(this, out v, out value);
                }
                return value;
            }
        }
    }
}