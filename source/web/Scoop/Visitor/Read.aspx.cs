using System;
using System.IO;

using Markdig;

namespace Scoop
{
    public partial class Read : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var article = Request.QueryString["article"];

            var physicalPath = Server.MapPath("~/Visitor/" + article + ".md");

            var md = File.ReadAllText(physicalPath);

            var html = Markdown.ToHtml(md);

            ArticleBody.Text = html;
        }
    }
}