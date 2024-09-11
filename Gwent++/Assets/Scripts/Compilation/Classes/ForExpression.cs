using System.Collections.Generic;
using System;
using LogicalSide;

namespace Compiler
{
    /// <summary>
    /// Represents a 'for' loop expression, iterating over a collection with a specified variable.
    /// This class ensures semantic correctness by validating the variable, collection, and instructions.
    /// It evaluates the loop iterating through each item in the collection.
    /// </summary>
    public class ForExpression : Expression
    {
        public InstructionBlock? Instructions = new();
        public IdentifierExpression? Variable;
        public Expression? Collection;

        public override ValueType? CheckSemantic(Scope scope)
        {
            SemScope = new Scope(scope);
            if (Variable != null)
            {
                ValueType? type;
                object v;
                if (!scope.Find(Variable, out type, out v))
                {
                    Variable.CheckType = ValueType.Card;
                    SemScope.AddVar(Variable);
                }
                else
                {
                    Errors.List.Add(new CompilingError("Variable already declarated, isnt available in a for loop", new Position()));
                }
            }
            else
            {
                Errors.List.Add(new CompilingError("For must have a variable", new Position()));
            }

            if (Collection != null && Collection.CheckSemantic(scope) == ValueType.CardCollection)
            {
                Collection.CheckType = ValueType.CardCollection;
            }
            else
            {
                Errors.List.Add(new CompilingError("For must have a collection", new Position()));
            }

            if (Instructions != null && Instructions.CheckSemantic(SemScope) != ValueType.Checked)
            {
                Errors.List.Add(new CompilingError("For must have valid instructions", new Position()));
            }
            return ValueType.Checked;
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            Scope Evaluator = new Scope(scope);

            Collection!.Result = Collection.Evaluate(scope, null);

            List<UnityCard> list = (List<UnityCard>)Collection.Result;
            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (count++ == 10000)
                    throw new Exception("Stack Overflow in for loop");
                Variable!.Result = list[i];
                Evaluator.AddVar(Variable);
                Instructions!.Evaluate(Evaluator, null);
            }

            return null;
        }
        public override void Print(int indentLevel = 0)
        {
            printed = "For";
            Console.WriteLine(new string(' ', indentLevel * 4) + printed);
        }
    }
}