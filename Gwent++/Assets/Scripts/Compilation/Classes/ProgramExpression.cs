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
    /// Represents a program consisting of a list of instances that can only be cards or effects.
    /// This class is responsible for evaluating each instance and ensuring semantic correctness.
    /// </summary>
    public class ProgramExpression : Expression
    {
        public List<Expression> Instances;
        public ProgramExpression()
        {
            Instances = new();
        }


        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            List<Card> cards = new();
            object values = null;
            foreach (Expression exp in Instances)
            {
                values = exp.Evaluate(scope, null, instance);
                if (exp is CardInstance card)
                {
                    cards.Add((Card)values);
                }
            }
            return cards;
        }
        public override ValueType? CheckSemantic(Scope scope)
        {
            //if(Instances.Count != 0)
            foreach (var instance in Instances)
            {
                if (instance.CheckSemantic(scope) != ValueType.Checked)
                {
                    Errors.List.Add(new CompilingError("Semantic Error at the Program", new Position()));
                }
            }
            return ValueType.Void;
        }
    }
}