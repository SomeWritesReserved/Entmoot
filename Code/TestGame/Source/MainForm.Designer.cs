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
			this.clientPacketTimelineDisplay = new Entmoot.TestGame.PacketTimelineDisplay();
			this.clientGroupBox = new Entmoot.TestGame.DoubleBufferedGroupBox();
			this.serverGroupBox = new Entmoot.TestGame.DoubleBufferedGroupBox();
			((System.ComponentModel.ISupportInitialize)(this.serverStepNumberPad)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clientStepNumberPad)).BeginInit();
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
			this.serverStepButton.Location = new System.Drawing.Point(206, 318);
			this.serverStepButton.Name = "serverStepButton";
			this.serverStepButton.Size = new System.Drawing.Size(75, 23);
			this.serverStepButton.TabIndex = 4;
			this.serverStepButton.Text = "Step Forward";
			this.serverStepButton.UseVisualStyleBackColor = true;
			this.serverStepButton.Click += new System.EventHandler(this.serverStepButton_Click);
			// 
			// serverStepNumberPad
			// 
			this.serverStepNumberPad.Location = new System.Drawing.Point(142, 319);
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
			this.runPauseServerButton.Location = new System.Drawing.Point(35, 318);
			this.runPauseServerButton.Name = "runPauseServerButton";
			this.runPauseServerButton.Size = new System.Drawing.Size(101, 23);
			this.runPauseServerButton.TabIndex = 2;
			this.runPauseServerButton.Text = "Run/Pause";
			this.runPauseServerButton.UseVisualStyleBackColor = true;
			this.runPauseServerButton.Click += new System.EventHandler(this.runPauseServerButton_Click);
			// 
			// runPauseClientButton
			// 
			this.runPauseClientButton.Location = new System.Drawing.Point(480, 317);
			this.runPauseClientButton.Name = "runPauseClientButton";
			this.runPauseClientButton.Size = new System.Drawing.Size(101, 23);
			this.runPauseClientButton.TabIndex = 6;
			this.runPauseClientButton.Text = "Run/Pause";
			this.runPauseClientButton.UseVisualStyleBackColor = true;
			this.runPauseClientButton.Click += new System.EventHandler(this.runPauseClientButton_Click);
			// 
			// clientStepNumberPad
			// 
			this.clientStepNumberPad.Location = new System.Drawing.Point(587, 318);
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
			this.clientStepButton.Location = new System.Drawing.Point(651, 317);
			this.clientStepButton.Name = "clientStepButton";
			this.clientStepButton.Size = new System.Drawing.Size(75, 23);
			this.clientStepButton.TabIndex = 8;
			this.clientStepButton.Text = "Step Forward";
			this.clientStepButton.UseVisualStyleBackColor = true;
			this.clientStepButton.Click += new System.EventHandler(this.clientStepButton_Click);
			// 
			// runPauseBothButton
			// 
			this.runPauseBothButton.Location = new System.Drawing.Point(331, 315);
			this.runPauseBothButton.Name = "runPauseBothButton";
			this.runPauseBothButton.Size = new System.Drawing.Size(101, 23);
			this.runPauseBothButton.TabIndex = 5;
			this.runPauseBothButton.Text = "Run/Pause Both";
			this.runPauseBothButton.UseVisualStyleBackColor = true;
			this.runPauseBothButton.Click += new System.EventHandler(this.runPauseBothButton_Click);
			// 
			// clientPacketTimelineDisplay
			// 
			this.clientPacketTimelineDisplay.BackColor = System.Drawing.Color.WhiteSmoke;
			this.clientPacketTimelineDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clientPacketTimelineDisplay.ClientServerContext = Entmoot.TestGame.ClientServerContext.Client;
			this.clientPacketTimelineDisplay.Location = new System.Drawing.Point(12, 343);
			this.clientPacketTimelineDisplay.Name = "clientPacketTimelineDisplay";
			this.clientPacketTimelineDisplay.NetworkConnection = null;
			this.clientPacketTimelineDisplay.Size = new System.Drawing.Size(738, 74);
			this.clientPacketTimelineDisplay.TabIndex = 8;
			this.clientPacketTimelineDisplay.Text = "packetTimelineDisplay1";
			// 
			// clientGroupBox
			// 
			this.clientGroupBox.BackColor = System.Drawing.Color.White;
			this.clientGroupBox.Location = new System.Drawing.Point(384, 11);
			this.clientGroupBox.Name = "clientGroupBox";
			this.clientGroupBox.Size = new System.Drawing.Size(366, 300);
			this.clientGroupBox.TabIndex = 1;
			this.clientGroupBox.TabStop = false;
			this.clientGroupBox.Text = "Client";
			this.clientGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gameGroupBox_Paint);
			// 
			// serverGroupBox
			// 
			this.serverGroupBox.BackColor = System.Drawing.Color.White;
			this.serverGroupBox.Location = new System.Drawing.Point(12, 12);
			this.serverGroupBox.Name = "serverGroupBox";
			this.serverGroupBox.Size = new System.Drawing.Size(366, 300);
			this.serverGroupBox.TabIndex = 0;
			this.serverGroupBox.TabStop = false;
			this.serverGroupBox.Text = "Server";
			this.serverGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gameGroupBox_Paint);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(762, 426);
			this.Controls.Add(this.runPauseBothButton);
			this.Controls.Add(this.clientPacketTimelineDisplay);
			this.Controls.Add(this.runPauseClientButton);
			this.Controls.Add(this.clientStepNumberPad);
			this.Controls.Add(this.clientStepButton);
			this.Controls.Add(this.runPauseServerButton);
			this.Controls.Add(this.serverStepNumberPad);
			this.Controls.Add(this.serverStepButton);
			this.Controls.Add(this.clientGroupBox);
			this.Controls.Add(this.serverGroupBox);
			this.Name = "MainForm";
			this.Text = "Entmoot Test Game";
			((System.ComponentModel.ISupportInitialize)(this.serverStepNumberPad)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clientStepNumberPad)).EndInit();
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
	}
}

