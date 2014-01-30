namespace HLCTester
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.btSentGW = new System.Windows.Forms.Button();
            this.lbCommGW = new System.Windows.Forms.Label();
            this.tbxMsgLog = new System.Windows.Forms.TextBox();
            this.cbxTelegramList = new System.Windows.Forms.CheckedListBox();
            this.tbxMsgContent = new System.Windows.Forms.TextBox();
            this.lbMsgContent = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(703, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // btSentGW
            // 
            this.btSentGW.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btSentGW.Location = new System.Drawing.Point(606, 90);
            this.btSentGW.Name = "btSentGW";
            this.btSentGW.Size = new System.Drawing.Size(75, 36);
            this.btSentGW.TabIndex = 3;
            this.btSentGW.Text = ">> Sent2GW";
            this.btSentGW.UseVisualStyleBackColor = true;
            this.btSentGW.Click += new System.EventHandler(this.btSentGW_Click);
            // 
            // lbCommGW
            // 
            this.lbCommGW.AutoSize = true;
            this.lbCommGW.BackColor = System.Drawing.Color.Red;
            this.lbCommGW.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbCommGW.Location = new System.Drawing.Point(606, 60);
            this.lbCommGW.MinimumSize = new System.Drawing.Size(75, 27);
            this.lbCommGW.Name = "lbCommGW";
            this.lbCommGW.Size = new System.Drawing.Size(75, 27);
            this.lbCommGW.TabIndex = 4;
            this.lbCommGW.Text = "CommGW";
            this.lbCommGW.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbxMsgLog
            // 
            this.tbxMsgLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.tbxMsgLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxMsgLog.Location = new System.Drawing.Point(0, 217);
            this.tbxMsgLog.Multiline = true;
            this.tbxMsgLog.Name = "tbxMsgLog";
            this.tbxMsgLog.ReadOnly = true;
            this.tbxMsgLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxMsgLog.Size = new System.Drawing.Size(700, 217);
            this.tbxMsgLog.TabIndex = 6;
            // 
            // cbxTelegramList
            // 
            this.cbxTelegramList.CheckOnClick = true;
            this.cbxTelegramList.FormattingEnabled = true;
            this.cbxTelegramList.Location = new System.Drawing.Point(0, 63);
            this.cbxTelegramList.Name = "cbxTelegramList";
            this.cbxTelegramList.Size = new System.Drawing.Size(167, 124);
            this.cbxTelegramList.TabIndex = 6;
            this.cbxTelegramList.SelectedIndexChanged += new System.EventHandler(this.cbxTelegramList_SelectedIndexChanged);
            // 
            // tbxMsgContent
            // 
            this.tbxMsgContent.BackColor = System.Drawing.Color.White;
            this.tbxMsgContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxMsgContent.Location = new System.Drawing.Point(174, 63);
            this.tbxMsgContent.Multiline = true;
            this.tbxMsgContent.Name = "tbxMsgContent";
            this.tbxMsgContent.ReadOnly = true;
            this.tbxMsgContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxMsgContent.Size = new System.Drawing.Size(426, 126);
            this.tbxMsgContent.TabIndex = 6;
            // 
            // lbMsgContent
            // 
            this.lbMsgContent.AutoSize = true;
            this.lbMsgContent.BackColor = System.Drawing.SystemColors.Control;
            this.lbMsgContent.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbMsgContent.Location = new System.Drawing.Point(172, 35);
            this.lbMsgContent.MaximumSize = new System.Drawing.Size(50, 65);
            this.lbMsgContent.MinimumSize = new System.Drawing.Size(150, 22);
            this.lbMsgContent.Name = "lbMsgContent";
            this.lbMsgContent.Size = new System.Drawing.Size(150, 22);
            this.lbMsgContent.TabIndex = 6;
            this.lbMsgContent.Text = "Message Content";
            this.lbMsgContent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(0, 192);
            this.label1.MaximumSize = new System.Drawing.Size(50, 65);
            this.label1.MinimumSize = new System.Drawing.Size(150, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 22);
            this.label1.TabIndex = 6;
            this.label1.Text = "App Log";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 437);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbMsgContent);
            this.Controls.Add(this.tbxMsgContent);
            this.Controls.Add(this.cbxTelegramList);
            this.Controls.Add(this.tbxMsgLog);
            this.Controls.Add(this.lbCommGW);
            this.Controls.Add(this.btSentGW);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button btSentGW;
        private System.Windows.Forms.Label lbCommGW;
        private System.Windows.Forms.TextBox tbxMsgLog;
        private System.Windows.Forms.CheckedListBox cbxTelegramList;
        private System.Windows.Forms.TextBox tbxMsgContent;
        private System.Windows.Forms.Label lbMsgContent;
        private System.Windows.Forms.Label label1;
    }
}

