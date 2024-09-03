using System.Collections.Generic;
using System;

namespace Compiler
{
    /// <summary>
    /// A class that epresents a compilation error with a message and its location in the source code.
    /// </summary>
    public class CompilingError
    {
        public string Message { get; private set; }

        public Position Location { get; private set; }

        /// <param name="message">The error message.</param>
        /// <param name="posError">The position where the error occurred.</param>
        public CompilingError(string message, Position posError)
        {
            this.Message = message;
            Location = posError;
        }

        public override string ToString()
        {
            return $"{Message} at Row:{Location.Row}, Column:{Location.Column}";
        }
    }

    /// <summary>
    /// A static class that manages a list of compilation errors.
    /// </summary>
    public static class Errors
    {
        public static List<CompilingError> List = new List<CompilingError>();
        public static void PrintAll()
        {
            foreach (CompilingError item in List)
            {
                Console.WriteLine(item);
            }
        }
    }
}