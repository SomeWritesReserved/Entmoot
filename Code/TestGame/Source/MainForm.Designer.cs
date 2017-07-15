namespace Entmoot.TestGame
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.serverTimer = new System.Windows.Forms.Timer(this.components);
			this.clientTimer = new System.Windows.Forms.Timer(this.components);
			this.serverStepButton = new System.Windows.Forms.Button();
			this.serverStepNumberPad = new System.Windows.Forms.NumericUpDown();
			this.runPauseServerButton = new System.Windows.Forms.Button();
			this.runPauseClientButton = new System.Windows.Forms.Button();
			this.clientStepNumberPad = new System.Windows.Forms.NumericUpDown();
			this.clientStepButton = new System.Windows.Forms.Button();
			this.runPauseBothButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.serverGroupBox = new Entmoot.TestGame.DoubleBufferedGroupBox();
			this.clientGroupBox = new Entmoot.TestGame.DoubleBufferedGroupBox();
			this.clientPacketTimelineDisplay = new Entmoot.TestGame.PacketTimelineDisplay();
			((System.ComponentModel.ISupportInitialize)(this.serverStepNumberPad)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clientStepNumberPad)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// serverTimer
			// 
			this.serverTimer.Interval = 25;
			this.serverTimer.Tick += new System.EventHandler(this.serverTimer_Tick);
			// 
			// clientTimer
			// 
			this.clientTimer.Interval = 25;
			this.clientTimer.Tick += new System.EventHandler(this.clientTimer_Tick);
			// 
			// serverStepButton
			// 
			this.serverStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.serverStepButton.Location = new System.Drawing.Point(206, 316);
			this.serverStepButton.Name = "serverStepButton";
			this.serverStepButton.Size = new System.Drawing.Size(75, 23);
			this.serverStepButton.TabIndex = 4;
			this.serverStepButton.Text = "Step Forward";
			this.serverStepButton.UseVisualStyleBackColor = true;
			this.serverStepButton.Click += new System.EventHandler(this.serverStepButton_Click);
			// 
			// serverStepNumberPad
			// 
			this.serverStepNumberPad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.serverStepNumberPad.Location = new System.Drawing.Point(142, 317);
			this.serverStepNumberPad.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.serverStepNumberPad.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.serverStepNumberPad.Name = "serverStepNumberPad";
			this.serverStepNumberPad.Size = new System.Drawing.Size(58, 20);
			this.serverStepNumberPad.TabIndex = 3;
			this.serverStepNumberPad.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
			// 
			// runPauseServerButton
			// 
			this.runPauseServerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.runPauseServerButton.Location = new System.Drawing.Point(35, 316);
			this.runPauseServerButton.Name = "runPauseServerButton";
			this.runPauseServerButton.Size = new System.Drawing.Size(101, 23);
			this.runPauseServerButton.TabIndex = 2;
			this.runPauseServerButton.Text = "Run/Pause";
			this.runPauseServerButton.UseVisualStyleBackColor = true;
			this.runPauseServerButton.Click += new System.EventHandler(this.runPauseServerButton_Click);
			// 
			// runPauseClientButton
			// 
			this.runPauseClientButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.runPauseClientButton.Location = new System.Drawing.Point(480, 316);
			this.runPauseClientButton.Name = "runPauseClientButton";
			this.runPauseClientButton.Size = new System.Drawing.Size(101, 23);
			this.runPauseClientButton.TabIndex = 6;
			this.runPauseClientButton.Text = "Run/Pause";
			this.runPauseClientButton.UseVisualStyleBackColor = true;
			this.runPauseClientButton.Click += new System.EventHandler(this.runPauseClientButton_Click);
			// 
			// clientStepNumberPad
			// 
			this.clientStepNumberPad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.clientStepNumberPad.Location = new System.Drawing.Point(587, 317);
			this.clientStepNumberPad.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.clientStepNumberPad.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.clientStepNumberPad.Name = "clientStepNumberPad";
			this.clientStepNumberPad.Size = new System.Drawing.Size(58, 20);
			this.clientStepNumberPad.TabIndex = 7;
			this.clientStepNumberPad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// clientStepButton
			// 
			this.clientStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.clientStepButton.Location = new System.Drawing.Point(651, 316);
			this.clientStepButton.Name = "clientStepButton";
			this.clientStepButton.Size = new System.Drawing.Size(75, 23);
			this.clientStepButton.TabIndex = 8;
			this.clientStepButton.Text = "Step Forward";
			this.clientStepButton.UseVisualStyleBackColor = true;
			this.clientStepButton.Click += new System.EventHandler(this.clientStepButton_Click);
			// 
			// runPauseBothButton
			// 
			this.runPauseBothButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.runPauseBothButton.Location = new System.Drawing.Point(331, 316);
			this.runPauseBothButton.Name = "runPauseBothButton";
			this.runPauseBothButton.Size = new System.Drawing.Size(101, 23);
			this.runPauseBothButton.TabIndex = 5;
			this.runPauseBothButton.Text = "Run/Pause Both";
			this.runPauseBothButton.UseVisualStyleBackColor = true;
			this.runPauseBothButton.Click += new System.EventHandler(this.runPauseBothButton_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.serverGroupBox, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.clientGroupBox, 1, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(738, 297);
			this.tableLayoutPanel1.TabIndex = 9;
			// 
			// serverGroupBox
			// 
			this.serverGroupBox.BackColor = System.Drawing.Color.White;
			this.serverGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.serverGroupBox.Location = new System.Drawing.Point(3, 3);
			this.serverGroupBox.Name = "serverGroupBox";
			this.serverGroupBox.Size = new System.Drawing.Size(363, 291);
			this.serverGroupBox.TabIndex = 0;
			this.serverGroupBox.TabStop = false;
			this.serverGroupBox.Text = "Server";
			this.serverGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gameGroupBox_Paint);
			// 
			// clientGroupBox
			// 
			this.clientGroupBox.BackColor = System.Drawing.Color.White;
			this.clientGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.clientGroupBox.Location = new System.Drawing.Point(372, 3);
			this.clientGroupBox.Name = "clientGroupBox";
			this.clientGroupBox.Size = new System.Drawing.Size(363, 291);
			this.clientGroupBox.TabIndex = 1;
			this.clientGroupBox.TabStop = false;
			this.clientGroupBox.Text = "Client";
			this.clientGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gameGroupBox_Paint);
			// 
			// clientPacketTimelineDisplay
			// 
			this.clientPacketTimelineDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clientPacketTimelineDisplay.BackColor = System.Drawing.Color.White;
			this.clientPacketTimelineDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clientPacketTimelineDisplay.ClientServerContext = Entmoot.TestGame.ClientServerContext.Client;
			this.clientPacketTimelineDisplay.Location = new System.Drawing.Point(12, 343);
			this.clientPacketTimelineDisplay.Name = "clientPacketTimelineDisplay";
			this.clientPacketTimelineDisplay.NetworkConnection = null;
			this.clientPacketTimelineDisplay.Size = new System.Drawing.Size(738, 94);
			this.clientPacketTimelineDisplay.TabIndex = 8;
			this.clientPacketTimelineDisplay.Text = "packetTimelineDisplay1";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(762, 446);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.runPauseBothButton);
			this.Controls.Add(this.clientPacketTimelineDisplay);
			this.Controls.Add(this.runPauseClientButton);
			this.Controls.Add(this.clientStepNumberPad);
			this.Controls.Add(this.clientStepButton);
			this.Controls.Add(this.runPauseServerButton);
			this.Controls.Add(this.serverStepNumberPad);
			this.Controls.Add(this.serverStepButton);
			this.Name = "MainForm";
			this.Text = "Entmoot Test Game";
			((System.ComponentModel.ISupportInitialize)(this.serverStepNumberPad)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clientStepNumberPad)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Timer serverTimer;
		private System.Windows.Forms.Timer clientTimer;
		private DoubleBufferedGroupBox serverGroupBox;
		private DoubleBufferedGroupBox clientGroupBox;
		private System.Windows.Forms.Button serverStepButton;
		private System.Windows.Forms.NumericUpDown serverStepNumberPad;
		private System.Windows.Forms.Button runPauseServerButton;
		private System.Windows.Forms.Button runPauseClientButton;
		private System.Windows.Forms.NumericUpDown clientStepNumberPad;
		private System.Windows.Forms.Button clientStepButton;
		private PacketTimelineDisplay clientPacketTimelineDisplay;
		private System.Windows.Forms.Button runPauseBothButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}

