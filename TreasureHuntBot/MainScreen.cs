using RedditSharp;
using RedditSharp.Things;
using System;
using System.Threading;
using System.Windows.Forms;

namespace TreasureHuntBot
{
    public partial class MainScreen : Form
    {
        private Reddit reddit;
        private int messageCount = 0;

        public MainScreen()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Settings settingsWindow = new Settings();
            DialogResult result = settingsWindow.ShowDialog();
        }

        private bool redditLogin()
        {
            try
            {
                reddit = new Reddit(Properties.Settings.Default.RedditAccountName, Properties.Settings.Default.RedditAccountPassword);
                lRedditName.Text = "Logged in as " + reddit.User.Name;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while trying to login. Is your user/pass correct?", "Error", MessageBoxButtons.OK);
                return false;
            }
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                if (redditLogin())
                {
                    backgroundWorker1.RunWorkerAsync();
                    labelBotStatus.Text = "The Bot is now running!!!";
                }
            }
            else
            {
                var result = MessageBox.Show("The worker is already running!\nAsk the bot to stop reading messages? May take up to 2 minutes.",
                                                "Oops!", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                    backgroundWorker1.CancelAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (reddit != null)
            {
                while (!backgroundWorker1.CancellationPending)
                {
                    foreach (PrivateMessage message in reddit.User.UnreadMessages)
                    {
                        if (message.Unread && message.Subject.ToLower().Contains(Properties.Settings.Default.TitleKeyword.ToLower())
                            && message.Body.ToLower().Contains(Properties.Settings.Default.BodyKeyword.ToLower()))
                        {
                            message.Reply(Properties.Settings.Default.Response);
                            message.SetAsRead();
                            backgroundWorker1.ReportProgress(0, messageCount + 1);
                            saveToLog(message.Author, message.SentUTC);
                        }
                    }
                    Thread.Sleep(1000 * 60 * 2);//Sleep for 2 minutes
                }
            }
        }

        private void saveToLog(string username, DateTimeOffset timeSent)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("UsageLog.txt", true))
            {
                file.WriteLine(username + " at " + timeSent.ToString(@"yyyy/M/d hh:mm:ss tt"));
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            messageCount = (int)e.UserState;
            labelCount.Text = messageCount.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            labelBotStatus.Text = "The Bot is not running";
            lRedditName.Text = "Not Logged In";
        }
    }
}