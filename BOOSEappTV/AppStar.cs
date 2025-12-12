using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Command to draw a star shape using IAppCanvas.
    /// </summary>
    public class AppStar : CommandTwoParameters
    {
        private int starSize;

        public AppStar() : base() { }

        public AppStar(Canvas c, int starSize) : base(c)
        {
            this.starSize = starSize;

        }
        public override void Execute()
        {
            AppConsole.WriteLine("My AppStar method called");
            int size = GetIntParameter(0);
            bool filled = GetBoolParameter(1);

            base.Execute();
            if (IsDouble)
                throw new CanvasException("Star size must be an integer.");
            starSize = Paramsint[0];


            if (starSize < 1)
                throw new CanvasException("Star size must be a positive integer.");

            if (canvas is IAppCanvas appCanvas)
            {
                appCanvas.Star(size, filled);
            }
        }


        private int GetIntParameter(int index)
        {
            if (Parameters == null)
                throw new InvalidOperationException("Parameters not set.");

            if (index < 0 || index >= Parameters.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Parameter index out of range.");

            if (!int.TryParse(Parameters[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                throw new FormatException($"Parameter at index {index} is not a valid integer: '{Parameters[index]}'.");

            return value;
        }

        private bool GetBoolParameter(int index)
        {
            if (Parameters == null)
                throw new InvalidOperationException("Parameters not set.");

            if (index < 0 || index >= Parameters.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Parameter index out of range.");

            // accept "true"/"false" (case-insensitive) and "1"/"0"
            var s = Parameters[index].Trim();
            if (bool.TryParse(s, out var b)) return b;
            if (s == "1") return true;
            if (s == "0") return false;

            throw new FormatException($"Parameter at index {index} is not a valid boolean: '{Parameters[index]}'");
        }
    }
}

