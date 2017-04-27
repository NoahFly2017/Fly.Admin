using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fly.Web.Models;

namespace Fly.Web.Infrastructure
{
    public class IPHelper
    {
        public static String GetClientIP()
        {
            var currentIp = HttpContext.Current.Request.UserHostAddress;
            string userHostAddress = HttpContext.Current.Request.UserHostAddress;
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return userHostAddress;
        }

        public static WhiteIPViewModel GetWhiteIPs()
        {
            //这是只有一个平台的做法
            var target = HttpContext.Current.Cache.Get("WhiteIPs");
            if (target == null)
            {
                var whiteIPs = new WhiteIPViewModel();
                using (Fly.Core.DataAccess.FlyDbContext ctx = new Core.DataAccess.FlyDbContext())
                {
                    var iplist = ctx.Platforms.FirstOrDefault().IPAddressWhiteOAs.Where(s=>s.Enable).ToList();
                    whiteIPs.IPv4 = iplist.Select(s => s.IPv4).Distinct().ToList();
                    whiteIPs.IPv6 = iplist.Select(s => s.IPv6).Distinct().ToList();
                }
                System.Web.HttpContext.Current.Cache.Insert("WhiteIPs", whiteIPs, null, DateTime.Now.AddMinutes(10), TimeSpan.Zero);
                return whiteIPs;
            }
            else
            {
                return target as WhiteIPViewModel;
            }

            #region 使用Session来存储
            //if (System.Web.HttpContext.Current.Session["WhiteIPs"] == null)
            //{
            //    var whiteIPs = new WhiteIPViewModel();
            //    using (Fly.Core.DataAccess.FlyDbContext ctx = new Core.DataAccess.FlyDbContext())
            //    {
            //        var iplist = ctx.Platforms.FirstOrDefault().IPAddressWhiteOAs.ToList();
            //        whiteIPs.IPv4 = iplist.Select(s => s.IPv4).Distinct().ToList();
            //        whiteIPs.IPv6 = iplist.Select(s => s.IPv6).Distinct().ToList();
            //    }
            //    System.Web.HttpContext.Current.Session["WhiteIPs"] = whiteIPs;
            //    return whiteIPs;
            //}
            //else
            //{
            //    return System.Web.HttpContext.Current.Session["WhiteIPs"] as WhiteIPViewModel;
            //}
            #endregion


        }

    }
}