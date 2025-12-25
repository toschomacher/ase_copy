using BOOSE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOOSEappTV
{
    /// <summary>
    /// Defines an extended canvas interface for custom BOOSE drawing commands.
    /// </summary>
    /// <remarks>
    /// This interface extends <see cref="ICanvas"/> with additional
    /// application-specific drawing operations that are not part of
    /// the standard BOOSE canvas API.
    /// </remarks>
    public interface IAppCanvas : ICanvas
    {
        /// <summary>
        /// Draws a star shape at the current cursor position.
        /// </summary>
        /// <param name="size">
        /// The size of the star, measured as the outer radius.
        /// </param>
        /// <param name="filled">
        /// <c>true</c> to draw a filled star; <c>false</c> to draw only the outline.
        /// </param>
        void Star(int size, bool filled);

        /// <summary>
        /// Draws a labelled shape at the current cursor position.
        /// </summary>
        /// <param name="label">
        /// The text label to display alongside or within the shape.
        /// </param>
        /// <remarks>
        /// This method is intended for future extensions and may not be
        /// implemented by all canvas types.
        /// </remarks>
        void DrawLabeledShape(string label);

        /// <summary>
        /// Gets the native bitmap used for rendering.
        /// </summary>
        /// <returns>
        /// The underlying bitmap object.
        /// </returns>
        /// <remarks>
        /// This member is currently unused and intentionally commented out.
        /// It is retained for potential future access to the rendering surface.
        /// </remarks>
        //Bitmap GetBitmap();
    }
}
