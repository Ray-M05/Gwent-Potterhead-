using System.Reflection;
using ListExtensions;
using System.Collections.Generic;
using System;
using LogicalSide;
using Unity.VisualScripting;
using System.Reflection.Emit;

namespace Compiler
{
    public class StringExpression : Atom
    {
        public StringExpression(Token token) : base(token)
        {
            this.printed = "STRING"; 
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            return Value.Meaning.Substring(1, Value.Meaning.Length - 2);
        }
        public override ValueType? CheckSemantic(Scope scope)
        {
            CheckType = ValueType.String;
            return CheckType;
        }
    }
}