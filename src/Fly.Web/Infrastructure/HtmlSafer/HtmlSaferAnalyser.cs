using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fly.Web.Infrastructure.HtmlSafer
{
    public class HtmlSaferAnalyser
    {
        const string htmlregex = @"\<(\w+)\s*(((\w+)\s*\=\s*\""([^\""]*)\""\s*)*|((\w+)\s*\=\s*\'([^\']*)\'\s*))*\>";
        const string endhtmlregex = @"\<\/(\w+)\>";
        const string htmlhidetags = @"\&\#\d+\;";

        public static string ClearHtmlTags(string txt)
        {

            txt = Regex.Replace(txt, htmlregex, "");
            txt = Regex.Replace(txt, endhtmlregex, "");
            txt = txt.Replace("<", "&lt").Replace(">", "&gt").Replace("<", "&lt");

            return txt;
        }

        /// <summary>
        /// 验证是否存在不允许的元素和属性
        /// </summary>
        public static string[] ValidHtml(string html,bool allowScript)
        {
            List<string> errors = new List<string>();
            MatchCollection mc = Regex.Matches(html, htmlregex);

            foreach(Match m in mc)
            {
                string tagname = m.Groups[1].Value;
                TagInfo tinfo=HtmlAnalyserConfiger.GetTag(tagname);
                bool haserror =false;
                if (tinfo==null)
                {
                    haserror = true;
                }

                if (!haserror)
                {
                    for(int i=0;i<m.Groups[4].Captures.Count;i++)
                    {
                        Capture c=m.Groups[4].Captures[i];
                        string attr_name = c.Value;
                        if (!tinfo.hasAttr(attr_name))
                        {
                            haserror = true; break;
                        }
                        if(!allowScript&&attr_name.ToLower()=="a")
                        {
                            string val = m.Groups[5].Captures[i].Value;
                            if(val.Trim().StartsWith("javascript:",StringComparison.OrdinalIgnoreCase))
                            {
                                haserror = true;
                                break;
                            }
                        }
                    }
                    if (!haserror)
                    {
                        for (int i = 0; i < m.Groups[7].Captures.Count; i++)
                        {
                            Capture c = m.Groups[7].Captures[i];
                            string attr_name = c.Value;
                            if (!tinfo.hasAttr(attr_name))
                            {
                                haserror = true; break;
                            }
                            if (!allowScript && attr_name.ToLower() == "a")
                            {
                                string val = m.Groups[5].Captures[i].Value;
                                if (val.Trim().StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                                {
                                    haserror = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if(haserror)
                {
                    errors.Add(m.Value);
                }
            }

            mc = Regex.Matches(html, endhtmlregex);
            foreach (Match m in mc)
            {
                string tagname = m.Groups[1].Value;
                if (!HtmlAnalyserConfiger.hastag(tagname))
                {
                    errors.Add(m.Value);
                }
            }
            return errors.ToArray();
        }

        /// <summary>
        /// 移除不允许的元素和属性
        /// </summary>
        public static string ToSafeHtml(string html,bool allowScript)
        {
            StringBuilder r = new StringBuilder();
            for(int i=0;i<html.Length;i++)
            {
                char c = html[i];
                if(c=='<')
                {
                    bool isequal = false;
                    //预取结束标签名
                    if (html.Length > i + 1&&html[i+1]=='/')
                    {
                        i++;
                        string tagname = "";
                        for (int j = i + 1; j < html.Length; j++)
                        {
                            char cc = html[j];
                            if (cc == '>')
                            {
                                tagname = tagname.Trim();
                                if (Regex.IsMatch(tagname, @"\w+"))
                                {
                                    //匹配结束标签
                                    isequal = true;
                                    if (HtmlAnalyserConfiger.hastag(tagname))
                                    {
                                        r.Append(c.ToString() + '/' + tagname + cc);
                                    }
                                    //更新匹配起始位置
                                    i = j ;
                                    break;
                                }
                            }
                            tagname += cc;
                        }
                    }
                    else
                    {
                        //预取元素
                        string tagname = "";
                        //act 取值
                        //0 匹配 tagname
                        //1 匹配 attribute name
                        //2 匹配 attribute value
                        //3 匹配结束
                        byte act = 0;
                        NameValueCollection attrs = new NameValueCollection();
                        string attr_name = "";
                        string attr_html = "";

                        for (int j = i + 1; j < html.Length; j++)
                        {
                            char cc = html[j];
                            if (cc == '>')
                            {
                                //匹配到结束字符
                                act = 3;
                                i = j;
                                isequal = true;
                                TagInfo tinfo=HtmlAnalyserConfiger.GetTag(tagname);
                                if (tinfo!=null)
                                {
                                    r.Append("<" + tagname);
                                    for (int a = 0; a < attrs.Count; a++)
                                    {
                                        attr_name=attrs.AllKeys[a];
                                        //匹配url中的js
                                        if(!allowScript&&tinfo.tagname.ToLower()=="a"&&attr_name.ToLower()=="href"&&attrs[a].Substring(1).StartsWith("javascript:",StringComparison.OrdinalIgnoreCase))
                                        {
                                            continue;
                                        }

                                        if(tinfo.hasAttr(attr_name))
                                        {
                                            r.Append(" " + attr_name + "=" + attrs[a]);
                                        }
                                    }
                                    r.Append(">");
                                }
                                break;
                            }

                            if (act==0 && cc == ' ' || cc == '\t' || cc == '\r' || cc == '\n')
                            {
                                tagname = tagname.Trim();
                                attr_name = "";
                                attr_html = "";
                                act = 1;
                            }
                            else if (act == 0)
                            {
                                tagname += cc;
                            }
                            else if (act == 1&&cc=='=')
                            {
                                act = 2;
                            }
                            else if (act == 1)
                            {
                                attr_name += cc;
                            }
                            else if (act == 2)
                            {
                                bool attr_pipei = false;
                                for(int k=j;k<html.Length;k++)
                                {
                                    char attrc=html[k];
                                    if((attrc==' '||attrc=='\t'||attrc=='\r'||attrc=='\n'))
                                    {
                                        continue;
                                    }
                                    else if ( attrc == '\''||attrc == '"' )
                                    {
                                        //预取 attribute value
                                        attr_html += attrc;
                                        for(int l=k+1;k<html.Length;l++)
                                        {
                                            char lc = html[l];
                                            attr_html += lc;
                                            if (lc == '\r' || lc == '\n')
                                                break;
                                            if (lc == attrc)
                                            {
                                                attr_pipei = true;
                                                attrs.Add(attr_name.Trim(), attr_html.Trim());
                                                attr_name = "";
                                                attr_html = "";
                                                j = l;
                                                break;
                                            }

                                        }
                                        break;
                                    }
                                    //没匹配到
                                }
                                if(!attr_pipei)
                                {
                                    break;
                                }
                                else
                                {
                                    attr_name = "";
                                    attr_html = "";
                                    act = 1;
                                }
                            }
                        }
                    }
                    if (!isequal)
                    {
                        r.Append("&lt;");
                    }
                }
                else
                {
                    r.Append(c);
                }
            }

            return r.ToString();
        }


    }
}
