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
    /// Represents the simplest unit in an abstract syntax tree (AST),
    /// encapsulating a token that contains a literal value or identifier.
    /// </summary>
    public class Atom : Expression
    {
        public string? ValueForPrint;
        public Token Value { get; }
        public Atom(Token token)
        {
            this.ValueForPrint = token.Meaning;
            Value = token;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Atom atom)
            {
                return atom.Value.Meaning == Value.Meaning;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.Meaning?.GetHashCode() ?? 0;
        }

        public override ValueType? CheckSemantic(Scope scope)
        {
            throw new NotImplementedException();
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            throw new NotImplementedException();
        }
    }
}