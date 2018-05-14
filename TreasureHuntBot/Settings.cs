using System;
using System.Windows.Forms;

namespace TreasureHuntBot
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();

            DialogResult = DialogResult.Cancel;

            tbKeywordBody.Text = Properties.Settings.Default.BodyKeyword;
            tbKeywordTopic.Text = Properties.Settings.Default.TitleKeyword;

            tbUser.Text = Properties.Settings.Default.RedditAccountName;
            tbPassword.Text = Properties.Settings.Default.RedditAccountPassword;

            tbResponse.Text = Properties.Settings.Default.Response;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.BodyKeyword = tbKeywordBody.Text.Trim();
            Properties.Settings.Default.TitleKeyword = tbKeywordTopic.Text.Trim();

            Properties.Settings.Default.RedditAccountName = tbUser.Text.Trim();
            Properties.Settings.Default.RedditAccountPassword = tbPassword.Text.Trim();

            Properties.Settings.Default.Response = tbResponse.Text.Trim();

            Properties.Settings.Default.Save();

            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}