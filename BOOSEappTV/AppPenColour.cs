using BOOSE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a pen colour command that sets the current drawing colour
    /// using RGB values.
    /// </summary>
    /// <remarks>
    /// This command updates the canvas pen and brush colour at runtime.
    /// All three parameters must be integers in the range 0–255.
    /// </remarks>
    public class AppPenColour : CommandThreeParameters
    {
        private int r, g, b;

        /// <summary>
        /// Initialises a new instance of the <see cref="AppPenColour"/> class.
        /// </summary>
        public AppPenColour() : base() { }

        /// <summary>
        /// Initialises a new instance of the <see cref="AppPenColour"/> class
        /// with an explicit canvas and RGB values.
        /// </summary>
        /// <param name="c">The canvas whose pen colour will be updated.</param>
        /// <param name="r">The red component (0–255).</param>
        /// <param name="g">The green component (0–255).</param>
        /// <param name="b">The blue component (0–255).</param>
        public AppPenColour(Canvas c, int r, int g, int b) : base(c)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        /// <summary>
        /// Executes the pen colour command at runtime.
        /// </summary>
        /// <remarks>
        /// The RGB parameters are evaluated, validated, and then applied
        /// to the canvas. Only integer values within the valid RGB range
        /// are permitted.
        /// </remarks>
        /// <exception cref="CanvasException">
        /// Thrown when non-integer or out-of-range RGB values are supplied.
        /// </exception>
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

        /// <summary>
        /// Performs parameter validation for the pen colour command.
        /// </summary>
        /// <param name="parameterList">The parameter list.</param>
        public override void CheckParameters(string[] parameterList)
        {
            base.CheckParameters(parameterList);
        }

        /// <summary>
        /// Returns a string representation of the pen colour command.
        /// </summary>
        /// <returns>
        /// A string containing the command name and RGB values.
        /// </returns>
        public override string ToString()
        {
            return "PenColour " + r + " " + g + " " + b;
        }

        /// <summary>
        /// Performs compile-time processing for the pen colour command.
        /// </summary>
        public override void Compile()
        {
            base.Compile();
        }

        /// <summary>
        /// Sets the parameters for the pen colour command.
        /// </summary>
        /// <param name="Program">The active <see cref="StoredProgram"/>.</param>
        /// <param name="Params">The parameter string.</param>
        public override void Set(StoredProgram Program, string Params)
        {
            base.Set(Program, Params);
        }
    }
}
