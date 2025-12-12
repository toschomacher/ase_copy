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
            ((System.ComponentModel.ISupportInitialize)outputBox).BeginInit();
            SuspendLayout();
            // 
            // aboutBtn
            // 
            aboutBtn.Location = new Point(241, 478);
            aboutBtn.Name = "aboutBtn";
            aboutBtn.Size = new Size(93, 26);
            aboutBtn.TabIndex = 0;
            aboutBtn.Text = "About BOOSE";
            aboutBtn.UseVisualStyleBackColor = true;
            aboutBtn.Click += aboutBtn_Click;
            // 
            // commandsBox
            // 
            commandsBox.BackColor = Color.FromArgb(50, 48, 49);
            commandsBox.ForeColor = Color.LawnGreen;
            commandsBox.Location = new Point(7, 26);
            commandsBox.Margin = new Padding(2);
            commandsBox.Multiline = true;
            commandsBox.Name = "commandsBox";
            commandsBox.Size = new Size(328, 249);
            commandsBox.TabIndex = 1;
            commandsBox.TextChanged += commandsBox_TextChanged;
            // 
            // outputBox
            // 
            outputBox.Location = new Point(351, 4);
            outputBox.Margin = new Padding(2);
            outputBox.Name = "outputBox";
            outputBox.Size = new Size(748, 500);
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
            compileBtn.Location = new Point(7, 279);
            compileBtn.Margin = new Padding(2);
            compileBtn.Name = "compileBtn";
            compileBtn.Size = new Size(93, 28);
            compileBtn.TabIndex = 3;
            compileBtn.Text = "Compile";
            compileBtn.UseVisualStyleBackColor = false;
            compileBtn.Click += parserBtn_Click;
            // 
            // clearBtn
            // 
            clearBtn.Enabled = false;
            clearBtn.Location = new Point(113, 279);
            clearBtn.Margin = new Padding(2);
            clearBtn.Name = "clearBtn";
            clearBtn.Size = new Size(115, 28);
            clearBtn.TabIndex = 4;
            clearBtn.Text = "Clear commands";
            clearBtn.UseVisualStyleBackColor = true;
            clearBtn.Click += clearBtn_Click;
            // 
            // canClearBtn
            // 
            canClearBtn.Location = new Point(241, 279);
            canClearBtn.Margin = new Padding(2);
            canClearBtn.Name = "canClearBtn";
            canClearBtn.Size = new Size(93, 28);
            canClearBtn.TabIndex = 6;
            canClearBtn.Text = "Clear canvas";
            canClearBtn.UseVisualStyleBackColor = true;
            canClearBtn.Click += canClearBtn_Click;
            // 
            // commandsLbl
            // 
            commandsLbl.AutoSize = true;
            commandsLbl.ForeColor = SystemColors.ButtonFace;
            commandsLbl.Location = new Point(7, 10);
            commandsLbl.Margin = new Padding(2, 0, 2, 0);
            commandsLbl.Name = "commandsLbl";
            commandsLbl.Size = new Size(138, 15);
            commandsLbl.TabIndex = 8;
            commandsLbl.Text = "BOOSE commands input";
            // 
            // consoleLbl
            // 
            consoleLbl.AutoSize = true;
            consoleLbl.ForeColor = SystemColors.ButtonFace;
            consoleLbl.Location = new Point(7, 318);
            consoleLbl.Margin = new Padding(2, 0, 2, 0);
            consoleLbl.Name = "consoleLbl";
            consoleLbl.Size = new Size(89, 15);
            consoleLbl.TabIndex = 9;
            consoleLbl.Text = "Console output";
            // 
            // consoleBox
            // 
            consoleBox.Location = new Point(7, 335);
            consoleBox.Margin = new Padding(2);
            consoleBox.Name = "consoleBox";
            consoleBox.Size = new Size(340, 143);
            consoleBox.TabIndex = 10;
            consoleBox.Text = "";
            // 
            // BOOSEForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            ClientSize = new Size(1106, 508);
            Controls.Add(outputBox);
            Controls.Add(consoleBox);
            Controls.Add(consoleLbl);
            Controls.Add(commandsLbl);
            Controls.Add(canClearBtn);
            Controls.Add(clearBtn);
            Controls.Add(compileBtn);
            Controls.Add(commandsBox);
            Controls.Add(aboutBtn);
            DoubleBuffered = true;
            Margin = new Padding(2, 1, 2, 1);
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
    }
}
