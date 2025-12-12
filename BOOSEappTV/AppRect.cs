using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOOSE;

namespace BOOSEappTV
{
    internal class AppRect : CommandTwoParameters
    {
        private int width;
        private int height;
        public AppRect() : base() { }
        public AppRect(Canvas c, int width, int height) : base(c)
        {
            this.width = width;
            this.height = height;
        }
        public override void Execute()
        {
            AppConsole.WriteLine("My AppRect method called");
            base.Execute();
            if (IsDouble)
                throw new CanvasException("Width and height must be integers.");
            width = Paramsint[0];
            height = Paramsint[1];
            if (width < 1 || height < 1)
                throw new CanvasException("Width and height must be positive integers.");
            canvas.Rect(width, height, false); // Draw filled rectangle
        }
    }
}
