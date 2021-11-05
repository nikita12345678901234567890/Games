using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess.HackyStuff
{
    public partial class GameIDNotifierForm : Form
    {
        public GameIDNotifierForm()
        {
            InitializeComponent();
        }


        public static GameIDNotifierForm Instance { get; } = new GameIDNotifierForm();
        static GameIDNotifierForm() { }

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox gameIDTextBox;
        public void SetGameID(Guid gameID)
            => gameIDTextBox.Text = gameID.ToString();

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(gameIDTextBox.Text);
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
