using System.Collections.Generic;

namespace Compiler
{
    /// <summary>
    /// Represents a scope that contains variables and provides methods to search,
    /// add, and manage variables.
    /// </summary>
    public class Scope
    {
        /// <summary>
        /// The parent scope of the current scope. If this scope is the root (Program Expression), 
        /// parentScope will be null.
        /// </summary>
        public Scope parentScope;
        public Scope(Scope Parent = null!)
        {
            parentScope = Parent;
        }

        /// <summary>
        /// Indicates whether this scope should prevent duplicate variable declarations.
        /// If set to true, attempting to add a duplicate variable will cause an error.
        /// </summary>
        public bool WithoutReps = false;
        public List<Expression> Variables = new();

        /// <summary>
        /// Searches for a specific expression within this scope and its parent scopes.
        /// If found, the method returns the found expression and the scope where it was found.
        /// </summary>
        /// <param name="tofind">The expression to search for.</param>
        /// <param name="Finded">The found expression if it exists, or null if not found.</param>
        /// <param name="Where">The scope where the expression was found, or null if not found.</param>
        private void InternalFind(Expression tofind, out Expression Finded, out Scope Where)
        {
            bool b = false;
            Finded = null!;
            Where = null!;
            foreach (Expression indic in Variables)
            {
                if (tofind.Equals(indic))
                {
                    Where = this;
                    Finded = indic;
                    b = true;
                }
            }
            if (!b)
            {
                if (parentScope != null)
                {
                    parentScope.InternalFind(tofind, out Finded, out Where);
                }
                else
                {
                    Where = null!;
                    Finded = null!;
                }
            }
        }

        /// <summary>
        /// Finds an expression within this scope or its parent scopes and returns its type 
        /// and result if found.
        /// </summary>
        /// <param name="exp">The expression to search for.</param>
        /// <param name="type">The type of the found expression, or null if not found.</param>
        /// <param name="result">The result of the found expression, or null if not found.</param>
        /// <returns>True if the expression was found, otherwise false.</returns>
        public bool Find(Expression exp, out ValueType? type, out object result)
        {
            Expression Finded;
            Scope Where;
            InternalFind(exp, out Finded, out Where);
            if (Where != null)
            {
                type = Finded.CheckType;
                result = Finded.Result!;
                return true;
            }
            else
            {
                type = null;
                result = null!;
                return false;
            }
        }

        /// <summary>
        /// Adds a new expression to the current scope. If the expression already exists 
        /// and WithoutReps is false, it updates the existing expression.
        /// If WithoutReps is true, an error is reported if the expression already exists.
        /// </summary>
        /// <param name="exp">The expression to add or update.</param>
        public void AddVar(Expression exp)
        {
            Expression Finded;
            Scope Where;
            InternalFind(exp, out Finded, out Where);
            if (Where != null)
            {
                if (!WithoutReps)
                {
                    Finded.CheckType = exp.CheckType;
                    Finded.Result = exp.Result;
                }
                else
                    Errors.List.Add(new CompilingError("A no repeats statement was violated", new Position()));

            }
            else
            {
                Variables.Add(exp);
            }
        }
    }

    /// <summary>
    /// Enumerates the different value types that can be assigned to expressions or variables within the scope.
    /// </summary>
    public enum ValueType
    {
        Int,
        String,
        Bool,
        Unassigned,
        Null,
        Void,
        Player,
        Context,
        Predicate,
        CardCollection,
        Card,

        Checked,
    }
}

    
