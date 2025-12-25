using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    /// <summary>
    /// Runtime array assignment:
    /// poke arrayName index = value
    /// </summary>
    public class AppPoke : Command
    {
        private string arrayName;
        private string indexExpr;
        private string valueExpr;

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            if (string.IsNullOrWhiteSpace(parameters))
                throw new ParserException("poke missing parameters");

            var parts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4 || parts[2] != "=")
                throw new ParserException("Invalid poke syntax");

            arrayName = parts[0];
            indexExpr = parts[1];
            valueExpr = string.Join(' ', parts, 3, parts.Length - 3);
        }

        public override void Compile()
        {
            // Runtime command - must be added to program
            Program.Add(this);
        }

        public override void Execute()
        {
            if (!Program.VariableExists(arrayName))
                throw new StoredProgramException($"Array '{arrayName}' not declared");

            if (!(Program.GetVariable(arrayName) is AppArray array))
                throw new StoredProgramException($"'{arrayName}' is not an array");

            // Evaluate index
            int index;
            try
            {
                index = EvaluateInt(indexExpr);
                AppConsole.WriteLine($"[DEBUG] Poke '{arrayName}[{index}]' = {valueExpr}");
            }
            catch
            {
                throw new StoredProgramException("Array index must be an integer");
            }

            // Evaluate value
            if (array.ElementType == "int")
            {
                int value;
                try
                {
                    value = EvaluateInt(valueExpr);
                }
                catch
                {
                    throw new StoredProgramException(
                        "Type mismatch, expected integer value"
                    );
                }

                array.SetValue(index, value);
                AppConsole.WriteLine($"[DEBUG] Array '{arrayName}[{index}]' now = {value}");
            }
            else if (array.ElementType == "real")
            {
                double value;
                try
                {
                    value = EvaluateDouble(valueExpr);
                }
                catch
                {
                    throw new StoredProgramException(
                        "Type mismatch, expected real value"
                    );
                }

                array.SetValue(index, value);
                AppConsole.WriteLine($"[DEBUG] Array '{arrayName}[{index}]' now = {value}");
            }
            else
            {
                throw new StoredProgramException(
                    "Unsupported array element type"
                );
            }
        }

        // Helpers
        private int EvaluateInt(string expr)
        {
            string replaced = ReplaceVariables(expr);
            var table = new DataTable();
            object result = table.Compute(replaced, "");
            return Convert.ToInt32(result);
        }

        private double EvaluateDouble(string expr)
        {
            string replaced = ReplaceVariables(expr);
            var table = new DataTable();
            object result = table.Compute(replaced, "");
            return Convert.ToDouble(result);
        }

        private string ReplaceVariables(string exp)
        {
            var tokens = exp.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];

                if (double.TryParse(t, out _))
                    continue;

                if (t is "+" or "-" or "*" or "/" or "(" or ")")
                    continue;

                if (Program.VariableExists(t))
                {
                    tokens[i] = Program.GetVarValue(t);
                    continue;
                }

                throw new StoredProgramException("Invalid variable or expressions.");
            }

            return string.Join(" ", tokens);
        }

        public override void CheckParameters(string[] parameter)
        {
            // handled by Set()
        }
    }
}
