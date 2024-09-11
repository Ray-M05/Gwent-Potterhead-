using System.Collections.Generic;
using System;
using LogicalSide;

namespace Compiler
{
    public class EffectInstance : Expression
    {
        public Expression? Name { get; set; }
        public List<Expression>? Params { get; set; }
        public Action? Action { get; set; }

        public override ValueType? CheckSemantic(Scope scope)
        {
            SemScope = new Scope(scope);
            if (Name == null || Name.CheckSemantic(scope)!= ValueType.String)
            {
                Errors.List.Add(new CompilingError("Effect must have a name", new Position()));
            }

            SemScope.WithoutReps = true;
            if (Params != null)
                foreach (var param in Params)
                {
                    param.CheckSemantic(SemScope);
                }
            SemScope.WithoutReps = false;


            if (Action != null && Action.CheckSemantic(SemScope) != ValueType.Checked)
                Errors.List.Add(new CompilingError("Effect must have an action", new Position()));

            return ValueType.Checked;
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            string name = (string)Name!.Evaluate(scope, set);
            if (Processor.ParamsRequiered.ContainsKey(name))
                Errors.List.Add(new CompilingError("You declared at least two effects with the same name", new Position()));

            Processor.ParamsRequiered.Add(name, new List<IdentifierExpression>());
            Processor.Effects.Add(name, this);
            if (Params != null && Params.Count > 0)
                foreach (Expression exp in Params)
                {
                    if (exp is BinaryExpression bin)
                    {
                        if (bin.Left is IdentifierExpression id)
                        {
                            Processor.ParamsRequiered[name].Add(id);
                        }
                        else
                        {
                            Errors.List.Add(new CompilingError("Effect parameters must be identifiers", new Position()));
                        }
                    }
                    else
                    {
                        Errors.List.Add(new CompilingError("Effect parameters must be identifiers", new Position()));
                    }
                }
            return true;
        }

        public void Execute(IDeckContext context, List<UnityCard> targets, List<IdentifierExpression> Param)
        {
            Scope Evaluator = new Scope();
            Action.Context.Result = context;
            Action.Targets.Result = targets;
            if (Params != null && Params.Count > 0)
            {
                Processor.SetParameters(Param, Params);
                IdentifierExpression id;

                foreach (Expression exp in Params)
                {
                    if (exp is BinaryExpression bin)
                    {
                        id = (IdentifierExpression)bin.Left;
                        Evaluator.AddVar(id);
                    }
                }
            }
            Action.Evaluate(Evaluator, null);
        }
    }

    /// <summary>
    /// Represents a block of instructions, containing a list of expressions.
    /// The class handles semantic checks and evaluates ensuring all instructions are valid and executed in sequence.
    /// </summary>
    public class InstructionBlock : Expression
    {
        public List<Expression>? Instructions = new();
        public override ValueType? CheckSemantic(Scope scope)
        {
            SemScope = new Scope(scope);
            foreach (var instruction in Instructions)
            {
                ValueType? type = instruction.CheckSemantic(SemScope);
            }
            return ValueType.Checked;
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            Scope Evaluator = new Scope(scope);
            foreach (Expression item in Instructions)
            {
                item.Evaluate(Evaluator, null);
            }
            return true;
        }
        public override void Print(int indentLevel = 0)
        {
            printed = "Instruction Block";
            Console.WriteLine(new string(' ', indentLevel * 4) + printed);
        }
    }

    /// <summary>
    /// Represents an action that consists of targets, context, and a set of instructions.
    /// This class ensures that the action is semantically correct, properly defines its targets 
    /// and context, and executes the provided instructions in the given scope.
    /// </summary>
    public class Action : Expression
    {
        public IdentifierExpression? Targets;
        public IdentifierExpression? Context;
        public InstructionBlock? Instructions;
        public override ValueType? CheckSemantic(Scope scope)
        {
            SemScope = new Scope(scope);
            if (Targets != null)
            {
                SemScope.AddVar(Targets);
                Targets.CheckType = ValueType.CardCollection;
            }
            else
            {
                Errors.List.Add(new CompilingError("Action must have a valid target", new Position()));
            }

            if (Context != null)
            {
                SemScope.AddVar(Context);
                Context.CheckType = ValueType.Context;
            }
            else
            {
                Errors.List.Add(new CompilingError("Action must have a valid context", new Position()));
            }

            if (Instructions != null && Instructions.CheckSemantic(SemScope) == ValueType.Checked)
            {
                Instructions.CheckType = ValueType.Checked;
            }
            else
            {
                Errors.List.Add(new CompilingError("Action must have valid instructions", new Position()));
            }
            return ValueType.Checked;
        }
        public override object Evaluate(Scope scope, object set, object instance = null)
        {
            Scope Evaluator = new Scope(scope);
            if (Targets.Value != null)
            {
                Evaluator.AddVar(Targets);
            }
            else Errors.List.Add(new CompilingError("Evaluate Error, Targets is not set correctly", new Position()));
            if (Context.Value != null)
            {
                Evaluator.AddVar(Context);
            }
            else Errors.List.Add(new CompilingError("Evaluate Error, Context is not set correctly", new Position()));
            Instructions.Evaluate(Evaluator, null);
            return true;
        }
        public override void Print(int indentLevel = 0)
        {
            printed = "Action";
            Console.WriteLine(new string(' ', indentLevel * 4) + printed);
        }
    }

    /// <summary>
    /// Interface that defines the structure for an effect in the game. It includes the effect instance,
    /// parameters, and a selector for determining the targets. The interface provides an `Execute` method.
    /// </summary>
    public interface IEffect
    {
        EffectInstance effect { get; set; }
        List<IdentifierExpression> Params { get; set; }
        Selector Selector { get; set; }

        void Execute(IDeckContext context)
        {
            List<UnityCard> targets;
            if (Selector != null)
                targets = Selector.Execute(context);
            else targets = new List<UnityCard>();

            effect.Execute(context, targets, Params);
        }
    }

    /// <summary>
    /// Represents a specific implementation of the IEffect interface. This class defines an effect
    /// with a given instance, parameters, and a selector to determine the targets
    /// </summary
    public class MyEffect : IEffect
    {
        public MyEffect(EffectInstance eff, Selector Sel, List<IdentifierExpression> Par)
        {
            effect = eff;
            Selector = Sel;
            Params = Par;
        }
        public List<IdentifierExpression> Params { get; set; }

        public EffectInstance effect { get; set; }

        public Selector Selector { get; set; }
        public override string ToString()
        {
            string s = "Efecto: " + (string)effect.Name.Result + "\n";
            foreach (IdentifierExpression identifier in Params)
            {
                s += identifier.Value.Meaning + "\n";
            }
            return s;
        }
    }
}
