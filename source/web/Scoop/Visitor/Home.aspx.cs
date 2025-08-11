using System;

namespace Scoop
{
    public partial class HomePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DemoLink.NavigateUrl = Navigator.DemoUrl;
            MoreLink.NavigateUrl = Navigator.MoreUrl;

            var reader = new ReportReader();
            var statistics = reader.RetrievePlatformStatistics();

            var eventCountInThousands = (int)(statistics.EventCount / 1000m);

            EventCount.Text = eventCountInThousands.ToString();
            LearnerCount.Text = statistics.LearnerCount.ToString();
            PackageCount.Text = statistics.PackageCount.ToString();
        }
    }
}