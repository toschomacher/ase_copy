using BOOSE;
using System;
using System.Drawing;

namespace BOOSEappTV
{
    /// <summary>
    /// Custom canvas implementation that extends <see cref="BOOSE.Canvas"/>
    /// and provides application-specific drawing functionality.
    /// </summary>
    /// <remarks>
    /// This class manages the drawing surface, graphics context, drawing state,
    /// and implements basic geometric and text rendering operations.
    /// It acts as the concrete canvas used by BOOSE drawing commands.
    /// </remarks>
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
        /// The internal bitmap used as the drawing surface.
        /// </summary>
        private Bitmap bitmap;//, cursorBitmap;

        /// <summary>
        /// The graphics context used to render onto the bitmap.
        /// </summary>
        private Graphics graphics;//, cursorGraphics;

        /// <summary>
        /// The background colour used when clearing the canvas.
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

        // track last drawn shapes
        private int _lastCircleRadius = -1;
        private string _lastRectParameters = "-1";
        private int _lastRectWidth = -1;
        private int _lastRectHeight = -1;

        /// <summary>
        /// Gets the currently active <see cref="AppCanvas"/> instance.
        /// </summary>
        /// <remarks>
        /// This static reference allows canvas commands to access
        /// the active drawing surface.
        /// </remarks>
        public static AppCanvas Current { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="AppCanvas"/> class.
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
        /// Gets or sets the pen colour.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Always thrown, as this property is not currently implemented.
        /// </exception>
        public object PenColour
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Draws a circle centred at the current cursor position.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="filled">
        /// <c>true</c> to draw a filled circle; <c>false</c> to draw an outline.
        /// </param>
        public void Circle(int radius, bool filled)
        {
            if (graphics == null) return;

            var rect = new Rectangle(
                xPos - radius,
                yPos - radius,
                radius * 2,
                radius * 2
            );

            if (filled)
                graphics.FillEllipse(brush, rect);
            else
                graphics.DrawEllipse(pen, rect);

            // record the last circle radius so other commands can inspect it
            _lastCircleRadius = radius;
        }

        /// <summary>
        /// Clears the canvas using the background colour and resets
        /// all stored shape-tracking values.
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
        /// Draws a line from the current cursor position to the specified coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the end point.</param>
        /// <param name="y">The Y-coordinate of the end point.</param>
        public void DrawTo(int x, int y)
        {
            if (graphics == null) return;

            graphics.DrawLine(pen, xPos, yPos, x, y);
            xPos = x;
            yPos = y;
        }

        /// <summary>
        /// Returns the bitmap used as the drawing surface.
        /// </summary>
        /// <returns>The underlying <see cref="Bitmap"/>.</returns>
        public object getBitmap()
        {
            return bitmap;
        }

        /// <summary>
        /// Moves the drawing cursor to the specified coordinates without drawing.
        /// </summary>
        /// <param name="x">The new X-coordinate.</param>
        /// <param name="y">The new Y-coordinate.</param>
        public void MoveTo(int x, int y)
        {
            xPos = x;
            yPos = y;
        }

        /// <summary>
        /// Draws a rectangle from the current cursor position.
        /// </summary>
        /// <param name="width">The rectangle width.</param>
        /// <param name="height">The rectangle height.</param>
        /// <param name="filled">
        /// <c>true</c> to draw a filled rectangle; <c>false</c> for an outline.
        /// </param>
        public void Rect(int width, int height, bool filled)
        {
            if (graphics == null) return;

            var rect = new Rectangle(xPos, yPos, width, height);

            if (filled)
                graphics.FillRectangle(brush, rect);
            else
                graphics.DrawRectangle(pen, rect);

            _lastRectWidth = width;
            _lastRectHeight = height;
            _lastRectParameters = $"{_lastRectWidth} {_lastRectHeight}";
        }

        /// <summary>
        /// Resets the drawing state and clears the canvas.
        /// </summary>
        public void Reset()
        {
            xPos = 100;
            yPos = 100;
            penSize = 3;

            penColour = Color.Black;

            brush?.Dispose();
            pen?.Dispose();

            brush = new SolidBrush(penColour);
            pen = new Pen(penColour, penSize);

            graphics.Clear(background_colour);
        }

        /// <summary>
        /// Reinitialises the canvas with new dimensions.
        /// </summary>
        /// <param name="xSize">The new canvas width.</param>
        /// <param name="ySize">The new canvas height.</param>
        public void Set(int xSize, int ySize)
        {
            if (bitmap != null)
            {
                graphics?.Dispose();
                bitmap.Dispose();
            }

            _xSize = xSize;
            _ySize = ySize;

            xPos = 0;
            yPos = 0;

            bitmap = new Bitmap(xSize, ySize);
            pen = new Pen(Color.Black, penSize);
            brush = new SolidBrush(Color.Black);
            graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);

            _lastCircleRadius = -1;
            _lastRectParameters = "-1";
            _lastRectWidth = -1;
            _lastRectHeight = -1;
        }

        /// <summary>
        /// Sets the current drawing colour using RGB components.
        /// </summary>
        /// <param name="r">Red component (0–255).</param>
        /// <param name="g">Green component (0–255).</param>
        /// <param name="b">Blue component (0–255).</param>
        public void SetColour(int r, int g, int b)
        {
            penColour = Color.FromArgb(r, g, b);

            brush?.Dispose();
            pen?.Dispose();

            brush = new SolidBrush(penColour);
            pen = new Pen(penColour, penSize);
        }

        /// <summary>
        /// Prepares the geometry for drawing a triangle.
        /// </summary>
        /// <remarks>
        /// This method currently computes the triangle points only.
        /// Rendering logic is not yet implemented.
        /// </remarks>
        /// <param name="width">The base width of the triangle.</param>
        /// <param name="height">The height of the triangle.</param>
        public void Tri(int width, int height)
        {
            if (graphics == null) return;

            Point p1 = new Point(xPos + width / 2, yPos);
            Point p2 = new Point(xPos + width, yPos + height);
            Point p3 = new Point(xPos, yPos + height);

            Point[] pts = new[] { p1, p2, p3 };
        }

        /// <summary>
        /// Writes text to the canvas at the current cursor position.
        /// </summary>
        /// <param name="text">The text to render.</param>
        public void WriteText(string text)
        {
            if (brush == null)
                brush = new SolidBrush(penColour);

            if (graphics == null) return;

            using var font = new Font("Segoe UI", 10);
            graphics.DrawString(text, font, brush, new PointF(xPos, yPos));
        }

        /// <summary>
        /// Gets the radius of the last circle drawn.
        /// </summary>
        public int LastCircleRadius => _lastCircleRadius;

        /// <summary>
        /// Gets a value indicating whether a circle has been drawn.
        /// </summary>
        public bool HasLastCircle => _lastCircleRadius >= 0;

        /// <summary>
        /// Gets the dimensions of the last rectangle drawn.
        /// </summary>
        public string LastRectParameters => _lastRectParameters;

        /// <summary>
        /// Gets a value indicating whether a rectangle has been drawn.
        /// </summary>
        public bool HasLastRect => _lastRectWidth >= 0 && _lastRectHeight >= 0;

        /// <summary>
        /// Returns the current canvas instance.
        /// </summary>
        /// <returns>The active <see cref="AppCanvas"/>.</returns>
        public static AppCanvas GetCanvas() => Current;

        /// <summary>
        /// Draws a five-pointed star at the current cursor position.
        /// </summary>
        /// <param name="size">The outer radius of the star.</param>
        /// <param name="filled">
        /// <c>true</c> to draw a filled star; <c>false</c> to draw an outline.
        /// </param>
        /// <exception cref="CanvasException">
        /// Thrown when the graphics context is not initialised or the size is invalid.
        /// </exception>
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

            double angle = -Math.PI / 2;
            double step = Math.PI / 5;

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

        /// <summary>
        /// Draws a labelled shape at the current cursor position.
        /// </summary>
        /// <param name="label">The label to draw.</param>
        /// <exception cref="NotImplementedException">
        /// Thrown as this method is not yet implemented.
        /// </exception>
        public void DrawLabeledShape(string label)
        {
            throw new NotImplementedException();
        }
    }
}
