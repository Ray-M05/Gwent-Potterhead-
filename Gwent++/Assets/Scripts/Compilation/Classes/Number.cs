using System.Reflection;
using ListExtensions;
using System.Collections.Generic;
using System;
using LogicalSide;
using Unity.VisualScripting;
using System.Reflection.Emit;

namespace Compiler
{
    public class Number : Atom
    {
        public Number(Token token) : base(token)
        {
            this.printed = "Number";
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            return Convert.ToInt32(Value.Meaning);
        }
        public override ValueType? CheckSemantic(Scope scope)
        {
            CheckType = ValueType.Int;
            return CheckType;
        }
    }
}