using System;
using System.Drawing;
using System.Windows.Forms;

namespace BOOSEappTV
{
    /// <summary>
    /// Provides a simple console output facility backed by a <see cref="RichTextBox"/>.
    /// </summary>
    /// <remarks>
    /// This class is used to display diagnostic and debug output within the
    /// BOOSEappTV user interface. It supports timestamped output and ensures
    /// thread-safe updates to the underlying control.
    /// </remarks>
    public static class AppConsole
    {
        /// <summary>
        /// The target <see cref="RichTextBox"/> used as the console output surface.
        /// </summary>
        private static RichTextBox targetBox;

        /// <summary>
        /// Initialises the RichTextBox console.
        /// </summary>
        /// <param name="box">
        /// The <see cref="RichTextBox"/> control to use for console output.
        /// </param>
        /// <remarks>
        /// This method configures the visual appearance of the console and
        /// sets it to read-only mode.
        /// </remarks>
        public static void Initialize(RichTextBox box)
        {
            targetBox = box;
            targetBox.ReadOnly = true;
            targetBox.BackColor = ColorTranslator.FromHtml("#323031");
            targetBox.ForeColor = Color.LightGreen;
            targetBox.Font = new Font("Consolas", 8);
        }

        /// <summary>
        /// Writes a line of text to the console.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="includeTimestamp">
        /// <c>true</c> to prefix the message with a timestamp; otherwise <c>false</c>.
        /// </param>
        /// <remarks>
        /// This method is thread-safe and will marshal the call onto the
        /// UI thread if required.
        /// </remarks>
        public static void WriteLine(string message, bool includeTimestamp = true)
        {
            if (targetBox == null) return;

            string timestamp = includeTimestamp ? $"[{DateTime.Now:HH:mm:ss}] " : "";

            if (targetBox.InvokeRequired)
            {
                targetBox.Invoke(new Action(() => AppendText(timestamp, message)));
            }
            else
            {
                AppendText(timestamp, message);
            }
        }

        /// <summary>
        /// Appends formatted text to the RichTextBox.
        /// </summary>
        /// <param name="timestamp">The timestamp prefix.</param>
        /// <param name="message">The message text.</param>
        /// <remarks>
        /// The timestamp is rendered in a smaller grey font, while the
        /// message is rendered in the standard console font and colour.
        /// </remarks>
        private static void AppendText(string timestamp, string message)
        {
            // Timestamp — smaller, grey
            int start = targetBox.TextLength;
            targetBox.SelectionStart = start;
            targetBox.SelectionFont = new Font("Consolas", 6, FontStyle.Regular);
            targetBox.SelectionColor = Color.Gray;
            targetBox.AppendText(timestamp);

            // Message — normal size, green
            targetBox.SelectionFont = new Font("Consolas", 8, FontStyle.Regular);
            targetBox.SelectionColor = Color.LightGreen;
            targetBox.AppendText(message + Environment.NewLine);

            targetBox.ScrollToCaret();
        }

        /// <summary>
        /// Clears all output from the console.
        /// </summary>
        /// <remarks>
        /// This method is thread-safe and will invoke the clear operation
        /// on the UI thread if required.
        /// </remarks>
        public static void Clear()
        {
            if (targetBox == null) return;

            if (targetBox.InvokeRequired)
            {
                targetBox.Invoke(new Action(() => targetBox.Clear()));
            }
            else
            {
                targetBox.Clear();
            }
        }
    }
}
