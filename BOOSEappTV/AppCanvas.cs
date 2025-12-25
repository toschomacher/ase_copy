using BOOSE;
using System;
using System.Drawing;

namespace BOOSEappTV
{
    /// <summary>
    /// Custom canvas implementation that extends BOOSE.Canvas and adds application-specific features.
    /// This class manages the drawing state, drawing surface (Bitmap), and basic geometric drawing operations.
    /// </summary>
    public class AppCanvas : BOOSE.Canvas, ICanvas, IAppCanvas
    {
        /// <summary>
        /// The default width of the canvas.
        /// </summary>
        private const int XSIZE = 748;

        /// <summary>
        /// The default height of the canvas.
        /// </summary>
        private const int YSIZE = 500;

        /// <summary>
        /// The internal drawing surface (Bitmap) where all graphics operations are performed.
        /// </summary>
        private Bitmap bitmap;//, cursorBitmap;

        /// <summary>
        /// The Graphics object used to perform drawing operations onto the <see cref="bitmap"/>.
        /// </summary>
        private Graphics graphics;//, cursorGraphics;

        /// <summary>
        /// The default background colour used when clearing the canvas.
        /// </summary>
        protected Color background_colour = Color.MediumAquamarine;

        // drawing state
        private Color penColour = Color.Black;
        private Brush brush;
        private Pen pen;
        private int penSize = 3;
        private int xPos = 100;
        private int yPos = 100;

        // store current canvas dimensions and expose them
        private int _xSize;
        private int _ySize;

        // track last circle radius (use -1 to indicate "none")
        private int _lastCircleRadius = -1;
        private string _lastRectParameters = "-1";
        private int _lastRectWidth = -1;
        private int _lastRectHeight = -1;

        /// <summary>
        /// Gets the currently active instance of <see cref="AppCanvas"/>.
        /// </summary>
        public static AppCanvas Current { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="AppCanvas"/> class.
        /// </summary>
        /// <param name="width">The initial width of the canvas in pixels.</param>
        /// <param name="height">The initial height of the canvas in pixels.</param>
        public AppCanvas(int width, int height)
        {
            bitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(bitmap);
            pen = new Pen(Color.Black, penSize);
            Current = this;
        }

        /// <summary>
        /// Gets or sets the current X-coordinate of the drawing cursor.
        /// </summary>
        public int Xpos { get => xPos; set => xPos = value; }

        /// <summary>
        /// Gets or sets the current Y-coordinate of the drawing cursor.
        /// </summary>
        public int Ypos { get => yPos; set => yPos = value; }

        /// <summary>
        /// Gets the current width of the canvas in pixels.
        /// </summary>
        public int XSize => _xSize;

        /// <summary>
        /// Gets the current height of the canvas in pixels.
        /// </summary>
        public int YSize => _ySize;

        /// <summary>
        /// Gets or sets the pen colour. (Currently not implemented.)
        /// </summary>
        /// <exception cref="NotImplementedException">Always thrown as this property is not yet implemented.</exception>
        public object PenColour { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Draws a circle centered at the current cursor position (<see cref="Xpos"/>, <see cref="Ypos"/>).
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="filled">If set to <c>true</c>, the circle is filled; otherwise, it is outlined.</param>
        public void Circle(int radius, bool filled)
        {
            if (graphics == null) return;
            var rect = new System.Drawing.Rectangle(xPos - radius, yPos - radius, radius * 2, radius * 2);
            if (filled)
                graphics.FillEllipse(brush, rect);
            else
                graphics.DrawEllipse(pen, rect);
            // These methods draw a perfect circle (or an ellipse) that fits inside the rectangle.
            // So the rectangle isn’t drawn — it’s just a frame of reference for the circle.

            // record the last circle radius so other commands can inspect it
            _lastCircleRadius = radius;
        }

        /// <summary>
        /// Clears the canvas using the default background colour and resets the last drawn shape parameters.
        /// </summary>
        public void Clear()
        {
            if (graphics == null) return;
            graphics.Clear(background_colour);
            _lastCircleRadius = -1;
            _lastRectParameters = "-1";
            _lastRectWidth = -1;
            _lastRectHeight = -1;
        }

        /// <summary>
        /// Draws a line from the current cursor position to the specified coordinates (x, y)
        /// and then updates the cursor position to (x, y).
        /// </summary>
        /// <param name="x">The X-coordinate of the line's end point.</param>
        /// <param name="y">The Y-coordinate of the line's end point.</param>
        public void DrawTo(int x, int y)
        {
            if (graphics == null) return;
            graphics.DrawLine(pen, xPos, yPos, x, y);
            xPos = x;
            yPos = y;
        }

        /// <summary>
        /// Gets the underlying <see cref="Bitmap"/> object used as the drawing surface.
        /// </summary>
        /// <returns>The <see cref="Bitmap"/> instance.</returns>
        public object getBitmap()
        {
            return bitmap;
        }

        /// <summary>
        /// Moves the drawing cursor to the specified coordinates without drawing.
        /// </summary>
        /// <param name="x">The new X-coordinate for the cursor.</param>
        /// <param name="y">The new Y-coordinate for the cursor.</param>
        public void MoveTo(int x, int y)
        {
            xPos = x;
            yPos = y;
        }

        /// <summary>
        /// Draws a rectangle starting from the current cursor position (<see cref="Xpos"/>, <see cref="Ypos"/>).
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="filled">If set to <c>true</c>, the rectangle is filled; otherwise, it is outlined.</param>
        public void Rect(int width, int height, bool filled)
        {
            if (graphics == null) return;
            var rect = new System.Drawing.Rectangle(xPos, yPos, width, height);
            if (filled)
                graphics.FillRectangle(brush, rect);
            else
                graphics.DrawRectangle(pen, rect);
            _lastRectWidth = width;
            _lastRectHeight = height;
            _lastRectParameters = _lastRectWidth.ToString() + " " + _lastRectHeight.ToString();
        }

        /// <summary>
        /// Resets the drawing state (cursor position and pen size) and clears the canvas.
        /// </summary>
        public void Reset()
        {
            xPos = 100;
            yPos = 100;
            penSize = 3;
            pen = new Pen(Color.Black, penSize);
            graphics.Clear(background_colour);
        }

        /// <summary>
        /// Reinitializes the canvas with new dimensions, disposes of old resources,
        /// and resets the drawing state.
        /// </summary>
        /// <param name="xSize">The new width of the canvas.</param>
        /// <param name="ySize">The new height of the canvas.</param>
        public void Set(int xSize, int ySize)
        {
            if (bitmap != null)
            {
                graphics?.Dispose();
                bitmap.Dispose();
            }
            // record size
            _xSize = xSize;
            _ySize = ySize;

            xPos = 0;
            yPos = 0;
            bitmap = new Bitmap(xSize, ySize);
            //cursorBitmap = new Bitmap(xSize, ySize);
            pen = new Pen(Color.Black, penSize);
            brush = new SolidBrush(Color.Black);
            graphics = Graphics.FromImage(bitmap);
            //cursorGraphics = Graphics.FromImage(cursorBitmap);
            graphics.Clear(Color.White);
            //Reset();
            _lastCircleRadius = -1;
            _lastRectParameters = "-1";
            _lastRectWidth = -1;
            _lastRectHeight = -1;
        }

        /// <summary>
        /// Sets the drawing colour for the pen and brush using RGB values.
        /// </summary>
        /// <param name="r">The red component (0-255).</param>
        /// <param name="g">The green component (0-255).</param>
        /// <param name="b">The blue component (0-255).</param>
        public void SetColour(int r, int g, int b)
        {
            penColour = Color.FromArgb(r, g, b);
            brush?.Dispose();
            pen?.Dispose();
            brush = new SolidBrush(penColour);
            pen = new Pen(penColour, penSize);
        }

        /// <summary>
        /// Prepares the points for drawing a triangle (currently incomplete/not fully implemented).
        /// </summary>
        /// <param name="width">The width of the triangle's base.</param>
        /// <param name="height">The height of the triangle.</param>
        public void Tri(int width, int height)
        {
            if (graphics == null) return;
            Point p1 = new Point(xPos + width / 2, yPos);
            Point p2 = new Point(xPos + width, yPos + height);
            Point p3 = new Point(xPos, yPos + height);
            Point[] pts = new[] { p1, p2, p3 };
            // TODO: Add actual drawing logic (e.g., DrawPolygon or FillPolygon)
        }

        /// <summary>
        /// Writes the specified text string onto the canvas at the current cursor position.
        /// </summary>
        /// <param name="text">The string to be drawn.</param>
        public void WriteText(string text)
        {
            if (brush == null)
            {
                brush = new SolidBrush(penColour);
            }

            if (graphics == null) return;
            using var font = new Font("Segoe UI", 10);
            graphics.DrawString(text, font, brush, new PointF(xPos, yPos));
        }

        /// <summary>
        /// Returns the radius of the last circle that was drawn, or -1 if none has been drawn
        /// since the last <see cref="Clear"/> or <see cref="Set"/> operation.
        /// </summary>
        public int LastCircleRadius => _lastCircleRadius;

        /// <summary>
        /// Gets a value indicating whether a circle has been drawn recently.
        /// </summary>
        public bool HasLastCircle => _lastCircleRadius >= 0;

        /// <summary>
        /// Returns the dimensions (Width Height) of the last rectangle drawn, or "-1" if none.
        /// </summary>
        public string LastRectParameters => _lastRectParameters;

        /// <summary>
        /// Gets a value indicating whether a rectangle has been drawn recently.
        /// </summary>
        public bool HasLastRect => _lastRectWidth >= 0 && _lastRectHeight >= 0;

        /// <summary>
        /// Returns the current <see cref="AppCanvas"/> instance.
        /// </summary>
        /// <returns>The static singleton instance of <see cref="AppCanvas"/>.</returns>
        public static AppCanvas GetCanvas() => Current;

        public void Star(int size, bool filled)
        {
            if (graphics == null)
                throw new CanvasException("Graphics context not initialised.");

            if (size < 1)
                throw new CanvasException("Star size must be positive.");

            // 5-point star geometry
            PointF[] points = new PointF[10];

            float cx = xPos;
            float cy = yPos;

            float outerRadius = size;
            float innerRadius = size * 0.4f;

            double angle = -Math.PI / 2;       // start at top
            double step = Math.PI / 5;         // 36°

            for (int i = 0; i < 10; i++)
            {
                float r = (i % 2 == 0) ? outerRadius : innerRadius;

                points[i] = new PointF(
                    cx + (float)(Math.Cos(angle) * r),
                    cy + (float)(Math.Sin(angle) * r)
                );

                angle += step;
            }

            if (filled)
            {
                using var fillBrush = new SolidBrush(penColour);
                graphics.FillPolygon(fillBrush, points);
            }
            else
            {
                graphics.DrawPolygon(pen, points);
            }
        }

        public void DrawLabeledShape(string label)
        {
            throw new NotImplementedException();
        }
    }
}