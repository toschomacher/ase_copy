using BOOSE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a drawing command that draws a line from the current
    /// cursor position to a specified coordinate.
    /// </summary>
    /// <remarks>
    /// This command evaluates its parameters at runtime and draws a line
    /// using integer coordinates only. It follows BOOSE canvas command
    /// semantics and performs bounds checking before drawing.
    /// </remarks>
    public class AppDrawTo : CommandTwoParameters
    {
        private int x, y;

        /// <summary>
        /// Gets or sets the X-coordinate of the destination point.
        /// </summary>
        public int X { get => x; set => x = value; }

        /// <summary>
        /// Gets or sets the Y-coordinate of the destination point.
        /// </summary>
        public int Y { get => y; set => y = value; }

        /// <summary>
        /// Initialises a new instance of the <see cref="AppDrawTo"/> class.
        /// </summary>
        public AppDrawTo() : base() { }

        /// <summary>
        /// Initialises a new instance of the <see cref="AppDrawTo"/> class
        /// with an explicit canvas and coordinates.
        /// </summary>
        /// <param name="c">The canvas on which to draw.</param>
        /// <param name="x">The destination X-coordinate.</param>
        /// <param name="y">The destination Y-coordinate.</param>
        public AppDrawTo(Canvas c, int x, int y) : base(c)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Executes the draw-to command by evaluating parameters
        /// and drawing a line on the canvas.
        /// </summary>
        /// <exception cref="CanvasException">
        /// Thrown when non-integer coordinates are supplied or
        /// when the coordinates lie outside the canvas bounds.
        /// </exception>
        public override void Execute()
        {
            base.Execute();

            if (IsDouble)
                throw new CanvasException("Coordinates must be integers.");

            x = Paramsint[0];
            y = Paramsint[1];

            if (x < 0 || x >= 748 || y < 0 || y >= 500)
                throw new CanvasException("Coordinates are out of canvas bounds.");

            canvas.DrawTo(x, y);
            AppConsole.WriteLine("My AppDrawTo method called");
        }
    }
}
