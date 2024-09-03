using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

namespace Compiler
{
    public static class Processor
    {
        public static Dictionary<string, List<IdentifierExpression>> ParamsRequiered = new Dictionary<string, List<IdentifierExpression>>();
        public static Dictionary<string, EffectInstance> Effects = new Dictionary<string, EffectInstance>();


        /// <summary>
        /// Finds the name from a list of identifier expressions. The name is extracted if the identifier is of type Name or EffectParam.
        /// </summary>
        /// <param name="expressions">List of identifier expressions to search through.</param>
        /// <returns>The name extracted from the expressions, or null if no suitable identifier is found.</returns>
        private static string? FindName(List<IdentifierExpression> expressions)
        {
            var expression = expressions.FirstOrDefault(expr => expr is IdentifierExpression id &&
                                                                 (id.Value.Type == TokenType.Name || 
                                                                  id.Value.Type == TokenType.EffectParam));
            if (expression != null)
            {
                expressions.Remove(expression);
                return (string)expression.Result!;
            }
            
            return null;
        }

        /// <summary>
        /// Finds and returns the effect instance based on the list of identifier expressions. Validates the effect name and checks parameter matching.
        /// </summary>
        /// <param name="expressions">List of identifier expressions used to find the effect.</param>
        /// <returns>The effect instance if found and parameters match; otherwise, null.</returns>
        public static EffectInstance FindEffect(List<IdentifierExpression> expressions)
        {
            string? name = FindName(expressions);
            ValidateEffectName(name);

            if (InternalFinder(ParamsRequiered[name], expressions))
            {
                return Effects[name];
            }
            else
            {
                Errors.List.Add(new CompilingError("Unexpected code Entrance", new Position()));
                return null;
            }
        }

        /// <summary>
        /// Sets parameters in expressions based on provided values. Updates the results of identifier expressions in the parameters.
        /// </summary>
        /// <param name="values">List of identifier expressions containing the new values.</param>
        /// <param name="parameters">List of expressions where parameters need to be set.</param>
        public static void SetParameters(List<IdentifierExpression> values, List<Expression> parameters)
        {
            foreach (var ex in parameters.OfType<BinaryExpression>())
            {
                var identifierExpression = ex.Left as IdentifierExpression;
                if (identifierExpression == null) continue;

                var match = values.FirstOrDefault(id => id.Value.Meaning == identifierExpression.Value.Meaning);
                if (match != null)
                {
                    identifierExpression.Result = match.Result;
                    ex.Result = match.Result;
                }
            }
        }

        /// <summary>
        /// Updates the given scope by adding the identifier expression if it is of a valid type.
        /// </summary>
        /// <param name="expression">The expression to be added to the scope.</param>
        /// <param name="scope">The scope to be updated.</param>
        public static void UpdateScope(Expression expression, Scope scope)
        {
            if (scope != null && expression is IdentifierExpression ide && 
                expression.CheckType != ValueType.Card && expression.CheckType != ValueType.Context && 
                expression.CheckType != ValueType.CardCollection)
            {
                scope.AddVar(ide);
            }
        }

        /// <summary>
        /// Validates the effect name to ensure it is not null and that it is declared in the effects dictionary.
        /// </summary>
        private static void ValidateEffectName(string? name)
        {
            if (name == null)
                Errors.List.Add(new CompilingError("Evaluate Error, There is no name given for the Effect of the Card", new Position()));
            if (!Effects.ContainsKey(name))
                Errors.List.Add(new CompilingError("Evaluate Error, The Effect of the Card is not declared", new Position()));
        }

        /// <summary>
        /// Internal method to check if the declared parameters match the asked parameters. Ensures the number of parameters and their types match.
        /// </summary>
        private static bool InternalFinder(List<IdentifierExpression> declared, List<IdentifierExpression> asked)
        {
            if (declared.Count != asked.Count)
                Errors.List.Add(new CompilingError("You must declare exactly params at the effect, you declared", new Position()));

            bool allMatch = asked.All(ask => declared.Any(dec => dec.Equals(ask) && dec.CheckType == ask.CheckType));

            if (!allMatch)
                Errors.List.Add(new CompilingError("The params you declared don't coincide with the effect", new Position()));

            return true;
        }
    }
}
