using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a move-to command that updates the canvas cursor position
    /// without drawing.
    /// </summary>
    /// <remarks>
    /// This command evaluates its parameters at runtime and moves the current
    /// drawing position on the canvas. Only integer coordinates are permitted,
    /// and bounds checking is performed before the move is applied.
    /// </remarks>
    public class AppMoveTo : CommandTwoParameters
    {
        private int xPos, yPos;

        /// <summary>
        /// Gets or sets the X-coordinate of the destination position.
        /// </summary>
        public int XPos
        {
            get => xPos;
            set => xPos = value;
        }

        /// <summary>
        /// Gets or sets the Y-coordinate of the destination position.
        /// </summary>
        public int YPos
        {
            get => yPos;
            set => yPos = value;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="AppMoveTo"/> class.
        /// </summary>
        public AppMoveTo() : base() { }

        /// <summary>
        /// Initialises a new instance of the <see cref="AppMoveTo"/> class
        /// with an explicit canvas and coordinates.
        /// </summary>
        /// <param name="c">The canvas on which the cursor will be moved.</param>
        /// <param name="xPos">The destination X-coordinate.</param>
        /// <param name="yPos">The destination Y-coordinate.</param>
        public AppMoveTo(Canvas c, int xPos, int yPos) : base(c)
        {
            this.xPos = xPos;
            this.yPos = yPos;
        }

        /// <summary>
        /// Executes the move-to command by evaluating parameters
        /// and updating the canvas cursor position.
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

            xPos = Paramsint[0];
            yPos = Paramsint[1];

            if (xPos < 0 || xPos >= 748 || yPos < 0 || yPos >= 500)
                throw new CanvasException("Coordinates are out of canvas bounds.");

            canvas.MoveTo(xPos, yPos);
            AppConsole.WriteLine("My AppMoveTo method called");
        }
    }
}
