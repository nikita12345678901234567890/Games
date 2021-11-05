
namespace Chess.HackyStuff
{
    partial class GameIDEntryForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.EnterButton = new System.Windows.Forms.Button();
            this.gameIDTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please enter a GameID";
            // 
            // EnterButton
            // 
            this.EnterButton.BackColor = System.Drawing.Color.Fuchsia;
            this.EnterButton.Location = new System.Drawing.Point(202, 56);
            this.EnterButton.Name = "EnterButton";
            this.EnterButton.Size = new System.Drawing.Size(70, 34);
            this.EnterButton.TabIndex = 2;
            this.EnterButton.Text = "Enter";
            this.EnterButton.UseVisualStyleBackColor = false;
            this.EnterButton.Click += new System.EventHandler(this.EnterButton_Click);
            // 
            // gameIDTextBox
            // 
            this.gameIDTextBox.Location = new System.Drawing.Point(12, 27);
            this.gameIDTextBox.Name = "gameIDTextBox";
            this.gameIDTextBox.Size = new System.Drawing.Size(260, 23);
            this.gameIDTextBox.TabIndex = 3;
            // 
            // GameIDEntryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LawnGreen;
            this.ClientSize = new System.Drawing.Size(284, 104);
            this.Controls.Add(this.gameIDTextBox);
            this.Controls.Add(this.EnterButton);
            this.Controls.Add(this.label1);
            this.Name = "GameIDEntryForm";
            this.Text = "GameIDEntryForm";
            this.Load += new System.EventHandler(this.GameIDEntryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button EnterButton;
        private System.Windows.Forms.TextBox gameIDTextBox;
    }
}