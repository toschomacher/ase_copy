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
            clearAll = new Button();
            canvasOrPopUp = new CheckBox();
            programsBox = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)outputBox).BeginInit();
            SuspendLayout();
            // 
            // aboutBtn
            // 
            aboutBtn.Location = new Point(344, 803);
            aboutBtn.Margin = new Padding(4, 5, 4, 5);
            aboutBtn.Name = "aboutBtn";
            aboutBtn.Size = new Size(133, 35);
            aboutBtn.TabIndex = 0;
            aboutBtn.Text = "About";
            aboutBtn.UseVisualStyleBackColor = true;
            aboutBtn.Click += aboutBtn_Click;
            // 
            // commandsBox
            // 
            commandsBox.BackColor = Color.FromArgb(50, 48, 49);
            commandsBox.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            compileBtn.Location = new Point(10, 434);
            compileBtn.Name = "compileBtn";
            compileBtn.Size = new Size(133, 36);
            compileBtn.TabIndex = 3;
            compileBtn.Text = "Compile";
            compileBtn.UseVisualStyleBackColor = false;
            compileBtn.Click += parserBtn_Click;
            // 
            // clearBtn
            // 
            clearBtn.Enabled = false;
            clearBtn.Location = new Point(161, 434);
            clearBtn.Name = "clearBtn";
            clearBtn.Size = new Size(164, 36);
            clearBtn.TabIndex = 4;
            clearBtn.Text = "Clear commands";
            clearBtn.UseVisualStyleBackColor = true;
            clearBtn.Click += clearBtn_Click;
            // 
            // canClearBtn
            // 
            canClearBtn.Location = new Point(344, 434);
            canClearBtn.Name = "canClearBtn";
            canClearBtn.Size = new Size(133, 36);
            canClearBtn.TabIndex = 6;
            canClearBtn.Text = "Clear canvas";
            canClearBtn.UseVisualStyleBackColor = true;
            canClearBtn.Click += canClearBtn_Click;
            // 
            // commandsLbl
            // 
            commandsLbl.AutoSize = true;
            commandsLbl.ForeColor = SystemColors.ButtonFace;
            commandsLbl.Location = new Point(5, 7);
            commandsLbl.Name = "commandsLbl";
            commandsLbl.Size = new Size(144, 25);
            commandsLbl.TabIndex = 8;
            commandsLbl.Text = "BOOSE program";
            // 
            // consoleLbl
            // 
            consoleLbl.AutoSize = true;
            consoleLbl.ForeColor = SystemColors.ButtonFace;
            consoleLbl.Location = new Point(6, 522);
            consoleLbl.Name = "consoleLbl";
            consoleLbl.Size = new Size(135, 25);
            consoleLbl.TabIndex = 9;
            consoleLbl.Text = "Console output";
            // 
            // consoleBox
            // 
            consoleBox.Location = new Point(10, 548);
            consoleBox.Name = "consoleBox";
            consoleBox.Size = new Size(484, 247);
            consoleBox.TabIndex = 10;
            consoleBox.Text = "";
            // 
            // checkComReset
            // 
            checkComReset.AutoSize = true;
            checkComReset.Checked = true;
            checkComReset.CheckState = CheckState.Checked;
            checkComReset.ForeColor = SystemColors.ButtonFace;
            checkComReset.Location = new Point(160, 6);
            checkComReset.Name = "checkComReset";
            checkComReset.Size = new Size(330, 29);
            checkComReset.TabIndex = 11;
            checkComReset.Text = "Reset commands before compilation";
            checkComReset.UseVisualStyleBackColor = true;
            checkComReset.CheckedChanged += checkComReset_CheckedChanged;
            // 
            // clearConsole
            // 
            clearConsole.Location = new Point(161, 803);
            clearConsole.Name = "clearConsole";
            clearConsole.Size = new Size(164, 35);
            clearConsole.TabIndex = 12;
            clearConsole.Text = "Clear console";
            clearConsole.UseVisualStyleBackColor = true;
            clearConsole.Click += clearConsole_Click;
            // 
            // clearAll
            // 
            clearAll.Location = new Point(10, 803);
            clearAll.Name = "clearAll";
            clearAll.Size = new Size(133, 35);
            clearAll.TabIndex = 13;
            clearAll.Text = "Clear all";
            clearAll.UseVisualStyleBackColor = true;
            clearAll.Click += clearAll_Click;
            // 
            // canvasOrPopUp
            // 
            canvasOrPopUp.AutoSize = true;
            canvasOrPopUp.BackColor = Color.White;
            canvasOrPopUp.Checked = true;
            canvasOrPopUp.CheckState = CheckState.Checked;
            canvasOrPopUp.Location = new Point(353, 811);
            canvasOrPopUp.Name = "canvasOrPopUp";
            canvasOrPopUp.Size = new Size(22, 21);
            canvasOrPopUp.TabIndex = 14;
            canvasOrPopUp.UseVisualStyleBackColor = false;
            // 
            // programsBox
            // 
            programsBox.FormattingEnabled = true;
            programsBox.Location = new Point(12, 476);
            programsBox.Name = "programsBox";
            programsBox.Size = new Size(465, 33);
            programsBox.TabIndex = 15;
            programsBox.Text = "Select a program";
            programsBox.SelectedIndexChanged += programsBox_SelectedIndexChanged;
            // 
            // BOOSEForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            ClientSize = new Size(1580, 847);
            Controls.Add(programsBox);
            Controls.Add(canvasOrPopUp);
            Controls.Add(clearAll);
            Controls.Add(clearConsole);
            Controls.Add(consoleBox);
            Controls.Add(outputBox);
            Controls.Add(commandsLbl);
            Controls.Add(canClearBtn);
            Controls.Add(clearBtn);
            Controls.Add(compileBtn);
            Controls.Add(commandsBox);
            Controls.Add(aboutBtn);
            Controls.Add(checkComReset);
            Controls.Add(consoleLbl);
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
        private Button clearAll;
        private CheckBox canvasOrPopUp;
        private ComboBox programsBox;
    }
}
