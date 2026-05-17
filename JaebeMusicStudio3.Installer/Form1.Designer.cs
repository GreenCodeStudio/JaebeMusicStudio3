namespace JaebeMusicStudio3.Installer
{
    partial class Form1
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
            Header = new Label();
            RunButton = new Button();
            Path = new TextBox();
            InstallButton = new Button();
            splitContainer1 = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // Header
            // 
            Header.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Header.AutoSize = true;
            Header.Font = new Font("Segoe UI", 18F);
            Header.Location = new Point(22, 9);
            Header.Name = "Header";
            Header.Size = new Size(459, 48);
            Header.TabIndex = 0;
            Header.Text = "Jaebe Music Studio Installer";
            Header.TextAlign = ContentAlignment.MiddleCenter;
            Header.Click += label1_Click;
            // 
            // RunButton
            // 
            RunButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RunButton.Location = new Point(3, 0);
            RunButton.Name = "RunButton";
            RunButton.Size = new Size(379, 362);
            RunButton.TabIndex = 2;
            RunButton.Text = "Run portable";
            RunButton.UseVisualStyleBackColor = true;
            RunButton.Click += button1_Click;
            // 
            // Path
            // 
            Path.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Path.Location = new Point(10, 16);
            Path.Name = "Path";
            Path.Size = new Size(383, 31);
            Path.TabIndex = 3;
            // 
            // InstallButton
            // 
            InstallButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            InstallButton.Location = new Point(3, 325);
            InstallButton.Name = "InstallButton";
            InstallButton.Size = new Size(390, 34);
            InstallButton.TabIndex = 4;
            InstallButton.Text = "Install";
            InstallButton.UseVisualStyleBackColor = true;
            InstallButton.Click += button2_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(12, 60);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(Path);
            splitContainer1.Panel1.Controls.Add(InstallButton);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(RunButton);
            splitContainer1.Size = new Size(785, 362);
            splitContainer1.SplitterDistance = 396;
            splitContainer1.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(809, 434);
            Controls.Add(splitContainer1);
            Controls.Add(Header);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Header;
        private Button RunButton;
        private TextBox Path;
        private Button InstallButton;
        private SplitContainer splitContainer1;
    }
}
