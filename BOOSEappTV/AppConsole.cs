using System;
using System.Drawing;
using System.Windows.Forms;

namespace BOOSEappTV
{
    public static class AppConsole
    {
        private static RichTextBox targetBox;

        /// <summary>
        /// Initialize the RichTextBox console.
        /// </summary>
        public static void Initialize(RichTextBox box)
        {
            targetBox = box;
            targetBox.ReadOnly = true;
            targetBox.BackColor = ColorTranslator.FromHtml("#323031");
            targetBox.ForeColor = Color.LightGreen;
            targetBox.Font = new Font("Consolas", 8);
        }

        /// <summary>
        /// Write a message to the RichTextBox console.
        /// </summary>
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

        private static void AppendText(string timestamp, string message)
        {
            // Timestamp — smaller, gray
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
    }
}
