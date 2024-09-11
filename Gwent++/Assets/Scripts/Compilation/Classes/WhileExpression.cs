using System.Collections.Generic;
using System;
using LogicalSide;

namespace Compiler
{
    
    /// <summary>
    /// Represents a 'while' loop expression, continuously executing a set of instructions 
    /// as long as a specified condition is met. The class ensures semantic correctness by
    /// validating the condition and instructions, and then evaluates the loop.
    /// </summary>
    public class WhileExpression : Expression
    {
        public InstructionBlock? Instructions = new();
        public Expression? Condition;

        public override ValueType? CheckSemantic(Scope scope)
        {
            SemScope = new Scope(scope);
            if (Condition != null && Condition.CheckSemantic(scope) == ValueType.Bool)
            {
                Condition.CheckType = ValueType.Bool;
            }
            else
            {
                Errors.List.Add(new CompilingError("While must have a valid condition", new Position()));
            }

            if (Instructions != null && Instructions.CheckSemantic(SemScope) != ValueType.Checked)
            {
                Errors.List.Add(new CompilingError("While must have valid instructions", new Position()));
            }
            return ValueType.Checked;
        }

        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            Scope Evaluator = new Scope(scope);
            int count= 0;
            while (true)
            {
                if (count++ == 10000)
                    throw new Exception("Stack Overflow in while loop");
                if (!(bool)Condition.Evaluate(scope, null))
                {
                    break;
                }

                Instructions.Evaluate(Evaluator, null);
            }

            return null;
        }
        public override void Print(int indentLevel = 0)
        {
            printed = "While";
            Console.WriteLine(new string(' ', indentLevel * 4) + printed);
        }
    }

}