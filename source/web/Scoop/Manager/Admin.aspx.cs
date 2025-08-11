using System;

namespace Scoop
{
    public partial class AdminPage : ManagerPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PurgeButton.OnClientClick = "return confirm('Are you sure? This is massively destructive.')";

            PurgeButton.Click += ResetButton_Click;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            var helper = new DatabaseHelper();

            helper.Reset();

            Response.Redirect(Navigator.HomePath);
        }
    }
}