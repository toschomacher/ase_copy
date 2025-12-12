using BOOSE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOOSEappTV
{
    /// <summary>
    /// Extended canvas interface for custom BOOSE drawing commands.
    /// </summary>
    public interface IAppCanvas : ICanvas
    {
        /// <summary>
        /// Draws a star shape at the current cursor position.
        /// </summary>
        /// <param name="size">Size of the star.</param>
        /// <param name="filled">Whether the star is filled.</param>
        void Star(int size, bool filled);

        /// <summary>
        /// Draws a labeled shape at the current position.
        /// </summary>
        /// <param name="label">Text to display.</param>
        void DrawLabeledShape(string label);

        /// <summary>
        /// Gets the native bitmap for rendering.
        /// </summary>
        /// <returns>Bitmap object.</returns>
        //Bitmap GetBitmap();
    }
}