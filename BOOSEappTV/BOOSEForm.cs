using System.Collections;
using System.Windows.Forms; // This using directive is for MessageBox
using BOOSE;
using System.Diagnostics;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents the main form of the BOOSEappTV application.
    /// </summary>
    /// <remarks>
    /// This form provides the user interface for entering BOOSE commands,
    /// compiling and executing them, and rendering graphical output on
    /// a canvas. It also manages application state, user interaction,
    /// and integration with the BOOSE interpreter components.
    /// </remarks>
    public partial class BOOSEForm : Form
    {
        /// <summary>
        /// The drawing canvas used to render graphical output.
        /// </summary>
        private AppCanvas myCanvas;

        /// <summary>
        /// Factory responsible for creating BOOSE command instances.
        /// </summary>
        private AppCommandFactory commandFactory;

        /// <summary>
        /// The stored program that holds parsed commands and variables.
        /// </summary>
        private AppStoredProgram storedProgram;

        /// <summary>
        /// The parser responsible for converting source text into executable commands.
        /// </summary>
        private IParser parser;

        /// <summary>
        /// Informational text describing BOOSE.
        /// </summary>
        private string infoBOOSE = AboutBOOSE.about();

        /// <summary>
        /// Indicates whether the mouse button is currently held down.
        /// </summary>
        private bool mouseDown = false;

        /// <summary>
        /// Controls whether commands and program state are reset before compilation.
        /// </summary>
        private bool resetCommands = false;

        /// <summary>
        /// Collection of predefined demonstration programs mapped by display name.
        /// </summary>
        private readonly Dictionary<string, string> demoPrograms = new();

        /// <summary>
        /// Initialises a new instance of the <see cref="BOOSEForm"/> class.
        /// </summary>
        /// <remarks>
        /// Sets up the form, initialises the canvas, console, command factory,
        /// stored program, and parser.
        /// </remarks>
        public BOOSEForm()
        {
            InitializeComponent();
            myCanvas = new AppCanvas(this.outputBox.ClientSize.Width, this.outputBox.ClientSize.Height);
            AppConsole.Initialize(consoleBox);
            storedProgram = new AppStoredProgram(myCanvas);
            commandFactory = new AppCommandFactory();
            parser = new AppParser(commandFactory, storedProgram);
        }

        /// <summary>
        /// Called when the form needs to be repainted.
        /// </summary>
        /// <param name="e">Paint event data including the graphics context.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            outputBox.Image = (Bitmap)myCanvas.getBitmap();
        }

        /// <summary>
        /// Called when the form is resized.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (outputBox == null)
                return;

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
        /// Handles the form load event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void BOOSEForm_Load(object sender, EventArgs e)
        {
            AppConsole.WriteLine(infoBOOSE);
            AppConsole.WriteLine("BOOSE application loaded");

            InitialiseDemoPrograms();

            programsBox.Items.Clear();
            foreach (var name in demoPrograms.Keys)
                programsBox.Items.Add(name);
            resetCommands = checkComReset.Checked;
        }

        /// <summary>
        /// Handles the mouse button press on the output canvas.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Mouse event data.</param>
        private void outputBox_MouseDown(object sender, MouseEventArgs e)
        {
            AppConsole.WriteLine("Mouse Down");
            mouseDown = true;
        }

        /// <summary>
        /// Handles the mouse button release on the output canvas.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Mouse event data.</param>
        private void outputBox_MouseUp(object sender, MouseEventArgs e)
        {
            AppConsole.WriteLine("Mouse Up");
            mouseDown = false;
        }

        /// <summary>
        /// Handles mouse movement over the output canvas.
        /// </summary>
        /// <remarks>
        /// While the mouse button is held down, a line is drawn
        /// following the cursor movement.
        /// </remarks>
        private void outputBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown) return;

            myCanvas.DrawTo(e.X, e.Y);
            outputBox.Image = (Bitmap)myCanvas.getBitmap();
        }

        /// <summary>
        /// Handles the form paint event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Paint event data.</param>
        private void BOOSEForm_Paint(object sender, PaintEventArgs e)
        {
            // Empty for now
        }

        /// <summary>
        /// Displays information about BOOSE when the About button is clicked.
        /// </summary>
        private void aboutBtn_Click(object sender, EventArgs e)
        {
            if (canvasOrPopUp.Checked)
            {
                myCanvas.WriteText(infoBOOSE);
            }
            else
            {
                MessageBox.Show(infoBOOSE);
            }

        }

        /// <summary>
        /// Enables or disables buttons depending on the content of the commands text box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void commandsBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(commandsBox.Text.Trim()))
            {
                compileBtn.Enabled = true;
                clearBtn.Enabled = true;
            }
            else
            {
                compileBtn.Enabled = false;
                clearBtn.Enabled = false;
            }
        }

        /// <summary>
        /// Parses and executes commands when the Compile button is clicked.
        /// </summary>
        /// <remarks>
        /// This acts as a compiler and verifier for BOOSE commands.
        /// If parsing succeeds, the program is executed; otherwise,
        /// errors are reported to the console.
        /// </remarks>
        private void parserBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (resetCommands)
                {
                    storedProgram.Clear();
                    storedProgram.ResetProgram();
                    myCanvas.Reset();
                }

                parser.ParseProgram(commandsBox.Text);
                storedProgram.Run();
            }
            catch (ParserException ex)
            {
                AppConsole.WriteLine(ex.ToString());
            }
            catch (StoredProgramException ex)
            {
                AppConsole.WriteLine(ex.ToString());
            }
            catch (NullReferenceException)
            {
                AppConsole.WriteLine("Null reference exception triggered");
            }
            catch (Exception ex)
            {
                AppConsole.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Clears all text from the commands input box.
        /// </summary>
        private void clearBtn_Click(object sender, EventArgs e)
        {
            commandsBox.Text = "";
        }

        /// <summary>
        /// Resets the canvas to its default state.
        /// </summary>
        private void canClearBtn_Click(object sender, EventArgs e)
        {
            myCanvas.Reset();
        }

        /// <summary>
        /// Toggles whether commands and the program counter are reset
        /// before each compilation.
        /// </summary>
        private void checkComReset_CheckedChanged(object sender, EventArgs e)
        {
            resetCommands = checkComReset.Checked;

            if (resetCommands)
                AppConsole.WriteLine("Command list and PC will reset");
            else
                AppConsole.WriteLine("Command list and PC will NOT reset");
        }

        /// <summary>
        /// Clears the application console output.
        /// </summary>
        private void clearConsole_Click(object sender, EventArgs e)
        {
            AppConsole.Clear();
        }

        /// <summary>
        /// Populates the internal collection of predefined demonstration programs.
        /// </summary>
        /// <remarks>
        /// These programs are displayed in the program selection drop-down
        /// and can be loaded directly into the commands input box.
        /// </remarks>
        private void InitialiseDemoPrograms()
        {
            demoPrograms.Add("INT PROGRAM 1",
        @"int num = 10
circle num
num = 20
circle num
num = num + 20
circle num
num = num + num
circle num
num = num + 10
circle num");

            demoPrograms.Add("INT PROGRAM 2",
        @"int num = 10
num = num + 5
num = num * 2
circle num
write num");

            demoPrograms.Add("REAL PROGRAM",
        @"pen 0,0,255
real length = 15.5
real width = 10.0
real pi = 3.14159
real radius = 27.7
real circ = 2 * pi * radius
real another
real more
moveto 100,100
write length * width
moveto 100,125
write circ
circle circ");

            demoPrograms.Add("BOOLEAN PROGRAM",
        @"boolean flag1 = true
boolean flag2 = true
boolean flag3 = flag1 && flag2
write flag3");

            demoPrograms.Add("ARRAY PROGRAM 1",
        @"int x
real y
real z
array int nums 10
array real prices 10
array real logs 10
poke nums 5 = 99
peek x = nums 5
circle x
pen 0,255,0
poke prices 5 = 99.99
peek y = prices 5
write ""£""+y
poke logs 5 = 100.01
peek z = logs 5
moveto 0,25
write z");

            demoPrograms.Add("ARRAY PROGRAM 2",
        @"moveto 0,0
int x = 0
real y
real z
array int nums 10
poke nums 5 = 99.99
peek x = nums 5
circle x
array real prices 10
poke prices 5 = 99.99
peek y = prices 5
pen 0,255,0
write ""£""+y
array real logs 10
poke logs 5 = 100.01
peek z = logs 5
moveto 0,25
write z");

            demoPrograms.Add("IF PROGRAM 1",
        @"int control = 50
if control < 10
    if control < 5
        pen 255,0,0
    else
        pen 0,0,255
    end if
    circle 20
    rect 20,20
else
    pen 0,255,0
    circle 100
    rect 100,100
end if");

            demoPrograms.Add("IF PROGRAM 2",
        @"int control = 50
if control < 10
    circle 20
else
    circle 90
end if");

            demoPrograms.Add("WHILE PROGRAM",
        @"moveto 100,100
int width = 9
int height = 150
pen 255,128,128
while height > 50
    circle height
    height = height - 15
    if height < 100
        pen 0,128,255
    end if
end while
pen 0,255,0
moveto 50,50
height = 50
while height > 10
    rect height, height
    height = height - 10
end while");

            demoPrograms.Add("FOR PROGRAM",
        @"pen 255,0,0
moveto 200,200
for count = 1 to 20 step 2
    circle count * 10
end for
pen 0,255,0
moveto 150,150
for count = 20 to 1 step -2
    circle count * 10
end for
pen 0,0,255
for count2 = 30 to 10 step -1
    circle count2 * 20
end for");

            demoPrograms.Add("METHOD PROGRAM",
        @"method int mulMethod int one, int two
  mulMethod = one * two
end method
method int divMethod int one, int two
  divMethod = one / two
end method
int global = 55
call mulMethod 10 5
moveto 100,100
write mulMethod
call divMethod 10 5
moveto 100,200
write divMethod");
        }

        /// <summary>
        /// Clears commands, canvas, and console output simultaneously.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void clearAll_Click(object sender, EventArgs e)
        {
            clearBtn_Click(sender, e);
            canClearBtn_Click(sender, e);
            clearConsole_Click(sender, e);
            myCanvas.Reset();
        }

        /// <summary>
        /// Loads the selected demo program into the commands input box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void programsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programsBox.SelectedItem == null)
                return;

            string key = programsBox.SelectedItem.ToString();

            if (demoPrograms.TryGetValue(key, out string programText))
            {
                commandsBox.Text = programText;
            }
        }

    }
}
