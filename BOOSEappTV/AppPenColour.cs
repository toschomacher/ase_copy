using BOOSE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOOSEappTV
{
    public class AppPenColour : CommandThreeParameters
    {
        private int r, g, b;
        public AppPenColour() : base() { }
        public AppPenColour(Canvas c, int r, int g, int b) : base(c)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
        public override void Execute()
        {
            AppConsole.WriteLine("My AppPenColour method called");
            base.Execute();
            if (IsDouble)
                throw new CanvasException("RGB values must be integers.");
            r = Paramsint[0];
            g = Paramsint[1];
            b = Paramsint[2];
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
                throw new CanvasException("RGB values must be between 0 and 255.");
            canvas.SetColour(r, g, b);
        }
        public override void CheckParameters(string[] parameterList)
        {
            base.CheckParameters(parameterList);
        }
        public override string ToString()
        {
            return "PenColour " + r + " " + g + " " + b;
        }
        public override void Compile()
        {
            base.Compile();
        }
        public override void Set(StoredProgram Program, string Params)
        {
            base.Set(Program, Params);
        }
    }
}
