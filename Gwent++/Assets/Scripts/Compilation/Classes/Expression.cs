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
    /// Represents an abstract base class for all expressions in the language.
    /// Provides a foundation for semantic checks and evaluation of expressions
    /// within a specific scope. This class must be inherited by concrete 
    /// expression types that define specific behaviors for semantic analysis 
    /// and evaluation.
    /// </summary>
    public abstract class Expression
    {
        public string? printed;
        public ValueType? CheckType;
        public Scope SemScope;
        public object? Result;
        public virtual void Print(int indentLevel = 0)
        {
            if (CheckType != null)
                Console.WriteLine(new string(' ', indentLevel * 4) + "Token " + printed + "--- Type" + CheckType);
            else
                Console.WriteLine(new string(' ', indentLevel * 4) + "Token " + printed);
        }
        public abstract ValueType? CheckSemantic(Scope scope);
        public abstract object Evaluate(Scope scope, object set, object instance = null);
    }
    
}
   