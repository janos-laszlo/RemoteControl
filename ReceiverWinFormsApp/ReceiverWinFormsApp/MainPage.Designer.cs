
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPage));
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            cancelShutdownButton = new System.Windows.Forms.Button();
            startStopButton = new System.Windows.Forms.Button();
            nextShutdownLabel = new System.Windows.Forms.Label();
            notifyIcon1 = new System.Windows.Forms.NotifyIcon(components);
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(cancelShutdownButton, 0, 1);
            tableLayoutPanel1.Controls.Add(startStopButton, 0, 0);
            tableLayoutPanel1.Controls.Add(nextShutdownLabel, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.Size = new System.Drawing.Size(306, 109);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // cancelShutdownButton
            // 
            cancelShutdownButton.Location = new System.Drawing.Point(3, 39);
            cancelShutdownButton.Name = "cancelShutdownButton";
            cancelShutdownButton.Size = new System.Drawing.Size(298, 30);
            cancelShutdownButton.TabIndex = 1;
            cancelShutdownButton.Text = "Cancel Shutdown";
            cancelShutdownButton.UseVisualStyleBackColor = true;
            cancelShutdownButton.Click += CancelShutdownButton_Click;
            // 
            // startStopButton
            // 
            startStopButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            startStopButton.Location = new System.Drawing.Point(3, 3);
            startStopButton.Name = "startStopButton";
            startStopButton.Size = new System.Drawing.Size(298, 30);
            startStopButton.TabIndex = 0;
            startStopButton.Text = "Start";
            startStopButton.UseVisualStyleBackColor = true;
            startStopButton.MouseClick += StartStopButton_MouseClick;
            // 
            // nextShutdownLabel
            // 
            nextShutdownLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            nextShutdownLabel.AutoSize = true;
            nextShutdownLabel.Location = new System.Drawing.Point(3, 83);
            nextShutdownLabel.Name = "nextShutdownLabel";
            nextShutdownLabel.Size = new System.Drawing.Size(139, 15);
            nextShutdownLabel.TabIndex = 2;
            nextShutdownLabel.Text = "Shutdown not scheduled";
            // 
            // notifyIcon1
            // 
            notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            notifyIcon1.BalloonTipText = "Receiver";
            notifyIcon1.BalloonTipTitle = "Receiver";
            notifyIcon1.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "Receiver";
            notifyIcon1.MouseClick += NotifyIcon1_MouseClick;
            // 
            // MainPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(306, 109);
            Controls.Add(tableLayoutPanel1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "MainPage";
            Text = "Remote Control Receiver";
            FormClosing += MainPage_FormClosing;
            Shown += MainPage_Shown;
            Resize += MainPage_Resize;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button startStopButton;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button cancelShutdownButton;
        private System.Windows.Forms.Label nextShutdownLabel;
    }
}

