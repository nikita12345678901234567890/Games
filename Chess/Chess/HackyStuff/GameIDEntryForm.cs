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
    public enum GameEntryFormShownStates
    {
        NotShown,
        WaitingForGameID,
        Ready
    }

    public partial class GameIDEntryForm : Form
    {
        private Guid gameID;
        public Guid GameID => gameID;

        public GameEntryFormShownStates State { get; private set; } = GameEntryFormShownStates.NotShown;

        public static GameIDEntryForm Instance { get; } = new GameIDEntryForm();
        static GameIDEntryForm() { }

        public GameIDEntryForm()
        {
            InitializeComponent();
            FormClosing += (s,e) => State = GameEntryFormShownStates.NotShown;
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            bool isReady = Guid.TryParse(gameIDTextBox.Text, out gameID);

            if (isReady)
            {
                State = GameEntryFormShownStates.Ready;
                Close();
            }
        }

        private void GameIDEntryForm_Load(object sender, EventArgs e)
        {
            State = GameEntryFormShownStates.WaitingForGameID;
        }        
    }
}
