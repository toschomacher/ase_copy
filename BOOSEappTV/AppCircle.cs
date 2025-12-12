using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOOSE;

namespace BOOSEappTV
{
    public class AppCircle : CommandOneParameter
    {
        private int radius;

        public AppCircle() : base() { }

        public AppCircle(Canvas c, int radius) : base(c)
        {
            this.radius = radius;

        }


        public override void Execute()
        {
            AppConsole.WriteLine("My AppCircle method called");
            base.Execute();
            if (IsDouble)
                throw new CanvasException("Radius must be an integer.");
            radius = Paramsint[0];

            if (radius < 1)
                throw new CanvasException("Radius must be a positive integer.");
            canvas.Circle(radius, false); // Draw filled circle
        }

        public override void CheckParameters(string[] parameterList)
        {
            base.CheckParameters(parameterList);
        }

        public override string ToString()
        {
            return "Circle " + radius;
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
