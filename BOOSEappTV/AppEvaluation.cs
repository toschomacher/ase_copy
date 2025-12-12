using BOOSE;
using BOOSEappTV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AppEvaluation : IEvaluation
{
    public string Expression { get; set; }
    public object Value { get; set; }
    public string VarName { get; set; }

    public void Set(StoredProgram program, string line) { }
    public void Compile() { }
    public void Execute() {
        AppConsole.WriteLine("My AppEvaluation method called");
    }
    public void CheckParameters(string[] parameters) { }
}

