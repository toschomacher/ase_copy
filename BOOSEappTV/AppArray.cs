using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Array variable declaration (BOOSE-correct).
    /// Runtime access is handled by POKE / PEEK commands.
    /// </summary>
    public class AppArray : Evaluation, ICommand
    {
        public string ElementType { get; private set; }   // "int" or "real"
        public int Size { get; private set; }

        private int[] intData;
        private double[] realData;

        public AppArray()
        {
            // arrays are not numeric expressions
            IsDouble = false;
        }

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

        public override void Execute()
        {
            // Arrays do nothing at runtime
        }

        // Accessors for POKE / PEEK
        public void SetValue(int index, int value)
        {
            ValidateIndex(index);
            intData[index] = value;
        }

        public void SetValue(int index, double value)
        {
            ValidateIndex(index);
            realData[index] = value;
        }

        public int GetIntValue(int index)
        {
            ValidateIndex(index);
            return intData[index];
        }

        public double GetRealValue(int index)
        {
            ValidateIndex(index);
            return realData[index];
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Size)
                throw new StoredProgramException(
                    $"Array index {index} out of bounds for '{VarName}'"
                );
        }
    }
}
