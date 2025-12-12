using BOOSE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOOSEappTV
{
    public class AppDrawTo : CommandTwoParameters
    {
        private int x, y;

        // **Getter and Setter Properties**
        public int X { get => x; set => x = value; }

        public int Y { get => y; set => y = value; }

        public AppDrawTo() : base() { }
        public AppDrawTo(Canvas c, int x, int y) : base(c)
        {
            this.x = x;
            this.y = y;
        }
        
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
