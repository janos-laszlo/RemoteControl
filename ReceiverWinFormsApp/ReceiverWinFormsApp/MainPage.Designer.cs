
namespace ReceiverWinFormsApp
{
    partial class MainPage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cancelShutdownButton = new System.Windows.Forms.Button();
            this.startStopButton = new System.Windows.Forms.Button();
            this.nextShutdownLabel = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.cancelShutdownButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.startStopButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.nextShutdownLabel, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(306, 109);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cancelShutdownButton
            // 
            this.cancelShutdownButton.Location = new System.Drawing.Point(3, 39);
            this.cancelShutdownButton.Name = "cancelShutdownButton";
            this.cancelShutdownButton.Size = new System.Drawing.Size(298, 30);
            this.cancelShutdownButton.TabIndex = 1;
            this.cancelShutdownButton.Text = "Cancel Shutdown";
            this.cancelShutdownButton.UseVisualStyleBackColor = true;
            this.cancelShutdownButton.Click += new System.EventHandler(this.cancelShutdownButton_Click);
            // 
            // startStopButton
            // 
            this.startStopButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.startStopButton.Location = new System.Drawing.Point(3, 3);
            this.startStopButton.Name = "startStopButton";
            this.startStopButton.Size = new System.Drawing.Size(298, 30);
            this.startStopButton.TabIndex = 0;
            this.startStopButton.Text = "Start";
            this.startStopButton.UseVisualStyleBackColor = true;
            this.startStopButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Button1_MouseClick);
            // 
            // nextShutdownLabel
            // 
            this.nextShutdownLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nextShutdownLabel.AutoSize = true;
            this.nextShutdownLabel.Location = new System.Drawing.Point(3, 83);
            this.nextShutdownLabel.Name = "nextShutdownLabel";
            this.nextShutdownLabel.Size = new System.Drawing.Size(139, 15);
            this.nextShutdownLabel.TabIndex = 2;
            this.nextShutdownLabel.Text = "Shutdown not scheduled";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "Receiver";
            this.notifyIcon1.BalloonTipTitle = "Receiver";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Receiver";
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon1_MouseClick);
            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 109);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainPage";
            this.Text = "Remote Control Receiver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainPage_FormClosing);
            this.Shown += new System.EventHandler(this.MainPage_Shown);
            this.Resize += new System.EventHandler(this.MainPage_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button startStopButton;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button cancelShutdownButton;
        private System.Windows.Forms.Label nextShutdownLabel;
    }
}

