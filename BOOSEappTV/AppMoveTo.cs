using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOOSE;

namespace BOOSEappTV
{
    public class AppMoveTo : CommandTwoParameters
    {
        private int xPos, yPos;

        // **Getter and Setter Properties**
        public int XPos
        {
            get => xPos;
            set => xPos = value;
        }

        public int YPos
        {
            get => yPos;
            set => yPos = value;
        }

        public AppMoveTo() : base() { }
        public AppMoveTo(Canvas c, int xPos, int yPos) : base(c)
        {
            this.xPos = xPos;
            this.yPos = yPos;
        }
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
