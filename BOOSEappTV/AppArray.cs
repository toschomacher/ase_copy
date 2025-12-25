using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents an array variable declaration within the BOOSE interpreter.
    /// </summary>
    /// <remarks>
    /// This class provides a BOOSE-correct implementation of array variables.
    /// Arrays are declared at compile time and do not perform any action at runtime.
    /// Element access and mutation are handled exclusively via the POKE and PEEK commands.
    /// </remarks>
    public class AppArray : Evaluation, ICommand
    {
        /// <summary>
        /// Gets the element type of the array.
        /// </summary>
        /// <remarks>
        /// Valid values are <c>"int"</c> or <c>"real"</c>.
        /// </remarks>
        public string ElementType { get; private set; }   // "int" or "real"

        /// <summary>
        /// Gets the size of the array.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Internal storage for integer array elements.
        /// Used when <see cref="ElementType"/> is <c>"int"</c>.
        /// </summary>
        private int[] intData;

        /// <summary>
        /// Internal storage for real (double) array elements.
        /// Used when <see cref="ElementType"/> is <c>"real"</c>.
        /// </summary>
        private double[] realData;

        /// <summary>
        /// Initialises a new instance of the <see cref="AppArray"/> class.
        /// </summary>
        /// <remarks>
        /// Arrays are not numeric expressions and therefore do not participate
        /// in expression evaluation.
        /// </remarks>
        public AppArray()
        {
            // arrays are not numeric expressions
            IsDouble = false;
        }

        /// <summary>
        /// Parses and stores the parameters required to declare the array.
        /// </summary>
        /// <param name="program">
        /// The current <see cref="StoredProgram"/> instance.
        /// </param>
        /// <param name="parameters">
        /// The parameter string defining the array type, name, and size.
        /// </param>
        /// <exception cref="ParserException">
        /// Thrown when the declaration syntax is invalid or incomplete.
        /// </exception>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            if (string.IsNullOrWhiteSpace(parameters))
                throw new ParserException("Array declaration missing parameters");

            var parts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
                throw new ParserException("Invalid array declaration syntax");

            ElementType = parts[0].ToLower();
            VarName = parts[1];

            if (!int.TryParse(parts[2], out int size) || size <= 0)
                throw new ParserException("Array size must be a positive integer");

            Size = size;

            if (ElementType != "int" && ElementType != "real")
                throw new ParserException("Array type must be int or real");
        }

        /// <summary>
        /// Declares the array within the stored program and allocates its storage.
        /// </summary>
        /// <remarks>
        /// Allocation occurs at compile time in accordance with BOOSE semantics.
        /// </remarks>
        /// <exception cref="ParserException">
        /// Thrown if the array name is missing or already declared.
        /// </exception>
        public override void Compile()
        {
            if (string.IsNullOrWhiteSpace(VarName))
                throw new ParserException("Array name missing");

            if (Program.VariableExists(VarName))
                throw new ParserException($"Variable '{VarName}' already declared");

            // allocate storage
            if (ElementType == "int")
                intData = new int[Size];
            else
                realData = new double[Size];

            Program.AddVariable(this);

            AppConsole.WriteLine(
                $"[DEBUG] Array '{VarName}' declared ({ElementType}[{Size}])"
            );
        }

        /// <summary>
        /// Executes the array command at runtime.
        /// </summary>
        /// <remarks>
        /// Arrays perform no action during execution.
        /// All interactions occur through POKE and PEEK commands.
        /// </remarks>
        public override void Execute()
        {
            // Arrays do nothing at runtime
        }

        /// <summary>
        /// Sets an integer value at the specified array index.
        /// </summary>
        /// <param name="index">The zero-based index to update.</param>
        /// <param name="value">The integer value to assign.</param>
        public void SetValue(int index, int value)
        {
            ValidateIndex(index);
            intData[index] = value;
        }

        /// <summary>
        /// Sets a real (double) value at the specified array index.
        /// </summary>
        /// <param name="index">The zero-based index to update.</param>
        /// <param name="value">The real value to assign.</param>
        public void SetValue(int index, double value)
        {
            ValidateIndex(index);
            realData[index] = value;
        }

        /// <summary>
        /// Retrieves an integer value from the specified array index.
        /// </summary>
        /// <param name="index">The zero-based index to access.</param>
        /// <returns>The integer value stored at the index.</returns>
        public int GetIntValue(int index)
        {
            ValidateIndex(index);
            return intData[index];
        }

        /// <summary>
        /// Retrieves a real (double) value from the specified array index.
        /// </summary>
        /// <param name="index">The zero-based index to access.</param>
        /// <returns>The real value stored at the index.</returns>
        public double GetRealValue(int index)
        {
            ValidateIndex(index);
            return realData[index];
        }

        /// <summary>
        /// Validates that the specified index is within the bounds of the array.
        /// </summary>
        /// <param name="index">The index to validate.</param>
        /// <exception cref="StoredProgramException">
        /// Thrown when the index is outside the valid range.
        /// </exception>
        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Size)
                throw new StoredProgramException(
                    $"Array index {index} out of bounds for '{VarName}'"
                );
        }
    }
}
