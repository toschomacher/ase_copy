using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    /// <summary>
    /// Runtime array read:
    /// peek varName = arrayName index
    /// </summary>
    public class AppPeek : Command
    {
        private string targetVar;
        private string arrayName;
        private string indexExpr;

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            if (string.IsNullOrWhiteSpace(parameters))
                throw new ParserException("peek missing parameters");

            // Expected: x = nums 5
            var parts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4 || parts[1] != "=")
                throw new ParserException("Invalid peek syntax");

            targetVar = parts[0];
            arrayName = parts[2];
            indexExpr = parts[3];
        }

        public override void Compile()
        {
            // Runtime command → MUST be added to program
            Program.Add(this);
        }

        public override void Execute()
        {
            if (!Program.VariableExists(targetVar))
                throw new StoredProgramException(
                    $"Variable '{targetVar}' not declared"
                );

            if (!Program.VariableExists(arrayName))
                throw new StoredProgramException(
                    $"Array '{arrayName}' not declared"
                );

            if (!(Program.GetVariable(arrayName) is AppArray array))
                throw new StoredProgramException(
                    $"'{arrayName}' is not an array"
                );

            int index = EvaluateInt(indexExpr);

            AppConsole.WriteLine(
                $"[DEBUG] Peek '{arrayName}[{index}]' → '{targetVar}'"
            );

            var variable = Program.GetVariable(targetVar);

            if (array.ElementType == "int")
            {
                if (variable is not AppInt)
                    throw new StoredProgramException(
                        "Type mismatch, expected integer variable"
                    );

                int value = array.GetIntValue(index);
                Program.UpdateVariable(targetVar, value);

                AppConsole.WriteLine(
                    $"[DEBUG] Assigned '{targetVar}' = {value}"
                );
            }
            else if (array.ElementType == "real")
            {
                if (variable is not AppReal)
                    throw new StoredProgramException(
                        "Type mismatch, expected real variable"
                    );

                double value = array.GetRealValue(index);
                Program.UpdateVariable(targetVar, value);

                AppConsole.WriteLine(
                    $"[DEBUG] Assigned '{targetVar}' = {value}"
                );
            }
            else
            {
                throw new StoredProgramException("Unsupported array type");
            }
        }


        // =====================
        // Helpers
        // =====================

        private int EvaluateInt(string expr)
        {
            string replaced = ReplaceVariables(expr);
            var table = new DataTable();
            object result = table.Compute(replaced, "");
            return Convert.ToInt32(result);
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
