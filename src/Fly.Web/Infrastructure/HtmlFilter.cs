using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Fly.Web.Infrastructure
{
    public static class HtmlFilter
    {
        #region 过滤HTML中的脚本、事件
        public static string FilterHtml(string html)
        {
            Regex rexScriptBlock = new Regex(@"<\s*script\s*(.|\n)*\s*\/\s*script\s*>\s*", RegexOptions.IgnoreCase);
            html = rexScriptBlock.Replace(html, "");
            Regex rexTag = new Regex(@"<[^>]+", RegexOptions.IgnoreCase);
            html = rexTag.Replace(html, FilterEvent);
            html = rexTag.Replace(html, FilterHrefScript);
            return html;
        }

        private static string FilterEvent(Match m)
        {
            Regex rexEvent = new Regex(@"\s*on\w+\s*=\s*[^ ]+", RegexOptions.IgnoreCase);
            return rexEvent.Replace(m.Value, "");
        }

        private static string FilterHrefScript(Match m)
        {
            Regex rexEvent = new Regex(@"(' *(javascript|vbscript):([//S^'])*')|(/"" *(javascript|vbscript):[//S^/""]*/"")|([^=]*(javascript|vbscript):[^/> ]*)", RegexOptions.IgnoreCase);
            return rexEvent.Replace(m.Value, "\"\"");
        }
        #endregion
    }
}