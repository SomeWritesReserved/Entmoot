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
			this.serverGroupBox = new System.Windows.Forms.GroupBox();
			this.clientGroupBox = new System.Windows.Forms.GroupBox();
			this.serverTimer = new System.Windows.Forms.Timer(this.components);
			this.clientTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// serverGroupBox
			// 
			this.serverGroupBox.BackColor = System.Drawing.Color.White;
			this.serverGroupBox.Location = new System.Drawing.Point(12, 12);
			this.serverGroupBox.Name = "serverGroupBox";
			this.serverGroupBox.Size = new System.Drawing.Size(300, 300);
			this.serverGroupBox.TabIndex = 0;
			this.serverGroupBox.TabStop = false;
			this.serverGroupBox.Text = "Server";
			this.serverGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gameGroupBox_Paint);
			// 
			// clientGroupBox
			// 
			this.clientGroupBox.BackColor = System.Drawing.Color.White;
			this.clientGroupBox.Location = new System.Drawing.Point(335, 12);
			this.clientGroupBox.Name = "clientGroupBox";
			this.clientGroupBox.Size = new System.Drawing.Size(300, 300);
			this.clientGroupBox.TabIndex = 1;
			this.clientGroupBox.TabStop = false;
			this.clientGroupBox.Text = "Client";
			this.clientGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gameGroupBox_Paint);
			// 
			// serverTimer
			// 
			this.serverTimer.Tick += new System.EventHandler(this.serverTimer_Tick);
			// 
			// clientTimer
			// 
			this.clientTimer.Tick += new System.EventHandler(this.clientTimer_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(647, 321);
			this.Controls.Add(this.clientGroupBox);
			this.Controls.Add(this.serverGroupBox);
			this.Name = "MainForm";
			this.Text = "Entmoot Test Game";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox serverGroupBox;
		private System.Windows.Forms.GroupBox clientGroupBox;
		private System.Windows.Forms.Timer serverTimer;
		private System.Windows.Forms.Timer clientTimer;
	}
}

