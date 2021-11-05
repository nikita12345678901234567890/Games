
namespace Chess.HackyStuff
{
    partial class GameIDNotifierForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.gameIDTextBox = new System.Windows.Forms.TextBox();
            this.copyButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            // 
            // gameIDTextBox
            // 
            this.gameIDTextBox.Location = new System.Drawing.Point(12, 27);
            this.gameIDTextBox.Name = "gameIDTextBox";
            this.gameIDTextBox.Size = new System.Drawing.Size(260, 23);
            this.gameIDTextBox.TabIndex = 0;
            // 
            // copyButton
            // 
            this.copyButton.BackColor = System.Drawing.Color.DarkOrange;
            this.copyButton.Location = new System.Drawing.Point(185, 56);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(87, 40);
            this.copyButton.TabIndex = 1;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = false;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "This is your GameID:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // GameIDNotifierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ClientSize = new System.Drawing.Size(284, 106);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gameIDTextBox);
            this.Controls.Add(this.copyButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GameIDNotifierForm";
            this.Text = "Game ID:";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Label label1;
    }
}