using BOOSE;
using BOOSEappTV;

public class AppAssign : Command
{
    private string varName;
    private string expression;

    public AppAssign() { }

    public AppAssign(string varName, string expression)
    {
        this.varName = varName;
        this.expression = expression;
    }

    public override void Set(StoredProgram program, string parameters)
    {
        Program = program;

        var parts = parameters.Split('=', StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
            throw new ParserException("Invalid assignment syntax");

        varName = parts[0];
        expression = parts[1];
    }

    public override void Execute()
    {
        int intValue;

        // ✅ FIRST: try literal
        if (int.TryParse(expression, out int literal))
        {
            intValue = literal;
        }
        // ✅ THEN: variable/expression
        else
        {
            if (!Program.VariableExists(expression))
                throw new StoredProgramException(
                    $"Undefined variable '{expression}'"
                );

            var v = Program.GetVariable(expression);
            intValue = Convert.ToInt32(v.Value);
        }

        Program.UpdateVariable(varName, intValue);

        AppConsole.WriteLine(
            $"[DEBUG] Assigned '{varName}' = {intValue}"
        );
    }

    public override void CheckParameters(string[] parameters)
    {
        // BOOSE requires this, even if empty
    }
}
