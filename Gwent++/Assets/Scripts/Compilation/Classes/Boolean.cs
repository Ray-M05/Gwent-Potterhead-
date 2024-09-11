using System.Reflection;
using ListExtensions;
using System.Collections.Generic;
using System;
using LogicalSide;
using Unity.VisualScripting;
using System.Reflection.Emit;

namespace Compiler
{
    public class BooleanLiteral : Atom
    {
        public BooleanLiteral(Token token) : base(token)
        {
            this.printed = "Boolean";
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            return Convert.ToBoolean(Value.Meaning);
        }
        public override ValueType? CheckSemantic(Scope scope)
        {
            CheckType = ValueType.Bool;
            return CheckType;
        }
    }
}