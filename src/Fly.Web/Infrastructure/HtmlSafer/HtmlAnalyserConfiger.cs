using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Fly.Web.Infrastructure.HtmlSafer
{
    public static class HtmlAnalyserConfiger
    {
        public static List<TagInfo> tags = new List<TagInfo>();

        public static bool hastag(string tagname)
        {
            tagname=tagname.ToLower();
            foreach (TagInfo t in tags)
            {
                if (t.tagname == tagname)
                {
                    return true;
                }
            }
            return false;
        }

        public static TagInfo GetTag(string tagname)
        {
            tagname = tagname.ToLower();
            foreach (TagInfo t in tags)
            {
                if (t.tagname == tagname)
                {
                    return t;
                }
            }
            return null;

        }

        static HtmlAnalyserConfiger()
        {
            string xmlfpath = AppDomain.CurrentDomain.BaseDirectory + "\\HtmlAnalyserConfig.xml";
            if (!File.Exists(xmlfpath))
                throw new Exception("在以下路径没有找到HTML过滤配置文件："+xmlfpath);
            XElement xml= XElement.Load(xmlfpath);
            XElement AllowTags = xml.Element("AllowTags");
            foreach(XElement el in AllowTags.Elements())
            {
                TagInfo tinfo=new TagInfo();
                tinfo.tagname = el.Name.LocalName;
                if(el.Attributes("attrs").Count()>0){
                    tinfo.attrs=el.Attribute("attrs").Value.Split('|');
                }else tinfo.attrs=new string[0];
                tags.Add(tinfo);
            }
        }
        




    }
}
