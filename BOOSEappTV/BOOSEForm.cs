using System.Collections;
using System.Windows.Forms; // This using directive is for MessageBox
using BOOSE;
using System.Diagnostics;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents the main form of the BOOSEappTV application.
    /// Provides drawing functionality and handles user interactions.
    /// </summary>
    public partial class BOOSEForm : Form
    {
        private AppCanvas myCanvas;
        CommandFactory commandFactory;
        StoredProgram storedProgram;
        IParser parser;
        string infoBOOSE = AboutBOOSE.about();
        bool mouseDown = false;

        /// <summary>
        /// Initialises a new instance of the BOOSEForm class.
        /// Sets up the form and initialises refferences and variables.
        /// </summary>
        public BOOSEForm()
        {
            InitializeComponent();
            myCanvas = new AppCanvas(this.outputBox.ClientSize.Width, this.outputBox.ClientSize.Height);
            AppConsole.Initialize(consoleBox);
            commandFactory = new AppCommandFactory();
            storedProgram = new StoredProgram(myCanvas);
            parser = new Parser(commandFactory, storedProgram);
        }

        /// <summary>
        /// This method is called when the form (application's main window) needs to be repainted.
        /// </summary>
        /// <param name="e">Paint event data including graphics context.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            outputBox.Image = (Bitmap)myCanvas.getBitmap();
            //AppConsole.WriteLine("OnPaint called");
        }

        /// <summary>
        /// This method is called when the form (application's main window) is resized.
        /// </summary>
        /// <param name="e">Paint event data including graphics context.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (outputBox == null)
            {
                return;
            }
            if (myCanvas == null)
            {
                myCanvas = new AppCanvas(outputBox.ClientSize.Width, outputBox.ClientSize.Height);
            }
            else
            {
                myCanvas.Set(outputBox.ClientSize.Width, outputBox.ClientSize.Height);
            }
            outputBox.Image = (Bitmap)myCanvas.getBitmap();
        }

        /// <summary>
        /// Handles the Load event.
        /// This method is called when the form is first loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void BOOSEForm_Load(object sender, EventArgs e)
        {
            AppConsole.WriteLine(infoBOOSE);
            AppConsole.WriteLine("BOOSE application loaded");
        }

        /// <summary>
        /// Handles the MouseDown event.
        /// Activates drawing mode when the mouse button is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Mouse event data.</param>
        private void outputBox_MouseDown(object sender, MouseEventArgs e)
        {
            AppConsole.WriteLine("Mouse Down");
            mouseDown = true;
        }

        /// <summary>
        /// Handles the MouseUp event.
        /// Deactivates drawing mode when the mouse button is released.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Mouse event data.</param>
        private void outputBox_MouseUp(object sender, MouseEventArgs e)
        {
            AppConsole.WriteLine("Mouse Up");
            mouseDown = false;
        }

        /// <summary>
        /// Handles the MouseMove event for the PictureBox.
        /// Draws a red line on the bitmap while the mouse is moved with the button held down.
        /// </summary>
        private void outputBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown) return;

            myCanvas.DrawTo(e.X, e.Y);
            outputBox.Image = (Bitmap)myCanvas.getBitmap();
        }

        /// <summary>
        /// Handles the Paint event.
        /// Draws a red ellipse and renders the bitmap onto the form.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Paint event data including graphics context.</param>
        private void BOOSEForm_Paint(object sender, PaintEventArgs e)
        {
            // Empry for now
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            // Show information about BOOSE when the form loads
            MessageBox.Show(infoBOOSE);
        }

        /// <summary>
        /// Enables/disables buttons depending on whether there is text in commandsBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Paint event data including graphics context.</param>
        private void commandsBox_TextChanged(object sender, EventArgs e)
        {
            // Check if the text in commandsBox is NOT null or empty (i.e., it contains text).
            // The Trim() method removes leading/trailing whitespace, ensuring a button press
            // isn't triggered just by spaces.
            if (!string.IsNullOrEmpty(commandsBox.Text.Trim()))
            {
                // If there is text (excluding whitespace), enable button2.
                compileBtn.Enabled = true;
                clearBtn.Enabled = true;
            }
            else
            {
                // If the text box is empty or only contains whitespace, disable button2.
                compileBtn.Enabled = false;
                clearBtn.Enabled = false;
            }
        }

        /// <summary>
        /// Text from commandsBox is parsed and executed when the "Compile" button is clicked.
        /// This acts as a compiler and verifier for the BOOSE commands.
        /// If all commands are valid, they are executed on the canvas, otherwise an error message is shown.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Paint event data including graphics context.</param>
        private void parserBtn_Click(object sender, EventArgs e)
        {
            try
            {
                parser.ParseProgram(commandsBox.Text);
                storedProgram.Run();
            }
            catch (BOOSE.StoredProgramException ex)
            {
                AppConsole.WriteLine(ex.ToString());
                //MessageBox.Show(ex.Message + "\n\n" + ex.ToString(), "Runtime error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.NullReferenceException)
            {
                AppConsole.WriteLine("Null refference exception triggered");
            }
        }

        /// <summary>
        /// This method is called when the "Clear commands" button is pressed and it will clear the commandsBox text.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Paint event data including graphics context.</param>
        private void clearBtn_Click(object sender, EventArgs e)
        {
            commandsBox.Text = "";
        }

        /// <summary>
        /// Resets the canvas to a default colour when the "Clear Canvas" button is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Paint event data including graphics context.</param>
        private void canClearBtn_Click(object sender, EventArgs e)
        {
            myCanvas.Reset();
        }
    }
}
