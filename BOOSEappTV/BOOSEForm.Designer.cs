namespace BOOSEappTV
{
    partial class BOOSEForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            aboutBtn = new Button();
            commandsBox = new TextBox();
            outputBox = new PictureBox();
            compileBtn = new Button();
            clearBtn = new Button();
            canClearBtn = new Button();
            commandsLbl = new Label();
            consoleLbl = new Label();
            consoleBox = new RichTextBox();
            checkComReset = new CheckBox();
            clearConsole = new Button();
            ((System.ComponentModel.ISupportInitialize)outputBox).BeginInit();
            SuspendLayout();
            // 
            // aboutBtn
            // 
            aboutBtn.Location = new Point(344, 793);
            aboutBtn.Margin = new Padding(4, 5, 4, 5);
            aboutBtn.Name = "aboutBtn";
            aboutBtn.Size = new Size(133, 47);
            aboutBtn.TabIndex = 0;
            aboutBtn.Text = "About BOOSE";
            aboutBtn.UseVisualStyleBackColor = true;
            aboutBtn.Click += aboutBtn_Click;
            // 
            // commandsBox
            // 
            commandsBox.BackColor = Color.FromArgb(50, 48, 49);
            commandsBox.ForeColor = Color.LawnGreen;
            commandsBox.Location = new Point(10, 33);
            commandsBox.Multiline = true;
            commandsBox.Name = "commandsBox";
            commandsBox.Size = new Size(467, 395);
            commandsBox.TabIndex = 1;
            commandsBox.TextChanged += commandsBox_TextChanged;
            // 
            // outputBox
            // 
            outputBox.Location = new Point(501, 7);
            outputBox.Name = "outputBox";
            outputBox.Size = new Size(1069, 833);
            outputBox.TabIndex = 2;
            outputBox.TabStop = false;
            outputBox.MouseDown += outputBox_MouseDown;
            outputBox.MouseMove += outputBox_MouseMove;
            outputBox.MouseUp += outputBox_MouseUp;
            // 
            // compileBtn
            // 
            compileBtn.BackColor = Color.SeaShell;
            compileBtn.Enabled = false;
            compileBtn.Location = new Point(10, 470);
            compileBtn.Name = "compileBtn";
            compileBtn.Size = new Size(133, 47);
            compileBtn.TabIndex = 3;
            compileBtn.Text = "Compile";
            compileBtn.UseVisualStyleBackColor = false;
            compileBtn.Click += parserBtn_Click;
            // 
            // clearBtn
            // 
            clearBtn.Enabled = false;
            clearBtn.Location = new Point(161, 470);
            clearBtn.Name = "clearBtn";
            clearBtn.Size = new Size(164, 47);
            clearBtn.TabIndex = 4;
            clearBtn.Text = "Clear commands";
            clearBtn.UseVisualStyleBackColor = true;
            clearBtn.Click += clearBtn_Click;
            // 
            // canClearBtn
            // 
            canClearBtn.Location = new Point(344, 470);
            canClearBtn.Name = "canClearBtn";
            canClearBtn.Size = new Size(133, 47);
            canClearBtn.TabIndex = 6;
            canClearBtn.Text = "Clear canvas";
            canClearBtn.UseVisualStyleBackColor = true;
            canClearBtn.Click += canClearBtn_Click;
            // 
            // commandsLbl
            // 
            commandsLbl.AutoSize = true;
            commandsLbl.ForeColor = SystemColors.ButtonFace;
            commandsLbl.Location = new Point(10, 7);
            commandsLbl.Name = "commandsLbl";
            commandsLbl.Size = new Size(209, 25);
            commandsLbl.TabIndex = 8;
            commandsLbl.Text = "BOOSE commands input";
            // 
            // consoleLbl
            // 
            consoleLbl.AutoSize = true;
            consoleLbl.ForeColor = SystemColors.ButtonFace;
            consoleLbl.Location = new Point(10, 520);
            consoleLbl.Name = "consoleLbl";
            consoleLbl.Size = new Size(135, 25);
            consoleLbl.TabIndex = 9;
            consoleLbl.Text = "Console output";
            // 
            // consoleBox
            // 
            consoleBox.Location = new Point(10, 548);
            consoleBox.Name = "consoleBox";
            consoleBox.Size = new Size(484, 236);
            consoleBox.TabIndex = 10;
            consoleBox.Text = "";
            // 
            // checkComReset
            // 
            checkComReset.AutoSize = true;
            checkComReset.ForeColor = SystemColors.ButtonFace;
            checkComReset.Location = new Point(10, 435);
            checkComReset.Name = "checkComReset";
            checkComReset.Size = new Size(302, 29);
            checkComReset.TabIndex = 11;
            checkComReset.Text = "Reset commands before Compile";
            checkComReset.UseVisualStyleBackColor = true;
            checkComReset.CheckedChanged += checkComReset_CheckedChanged;
            // 
            // clearConsole
            // 
            clearConsole.Location = new Point(161, 793);
            clearConsole.Name = "clearConsole";
            clearConsole.Size = new Size(164, 47);
            clearConsole.TabIndex = 12;
            clearConsole.Text = "Clear console";
            clearConsole.UseVisualStyleBackColor = true;
            clearConsole.Click += clearConsole_Click;
            // 
            // BOOSEForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            ClientSize = new Size(1580, 847);
            Controls.Add(clearConsole);
            Controls.Add(consoleBox);
            Controls.Add(checkComReset);
            Controls.Add(outputBox);
            Controls.Add(consoleLbl);
            Controls.Add(commandsLbl);
            Controls.Add(canClearBtn);
            Controls.Add(clearBtn);
            Controls.Add(compileBtn);
            Controls.Add(commandsBox);
            Controls.Add(aboutBtn);
            DoubleBuffered = true;
            Margin = new Padding(3, 2, 3, 2);
            Name = "BOOSEForm";
            Text = "BOOSE compiler";
            Load += BOOSEForm_Load;
            Paint += BOOSEForm_Paint;
            ((System.ComponentModel.ISupportInitialize)outputBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Button aboutBtn;
        private TextBox commandsBox;
        private PictureBox outputBox;
        private Button compileBtn;
        private Button clearBtn;
        private Button canClearBtn;
        private Label commandsLbl;
        private Label consoleLbl;
        private RichTextBox consoleBox;
        private CheckBox checkComReset;
        private Button clearConsole;
    }
}
