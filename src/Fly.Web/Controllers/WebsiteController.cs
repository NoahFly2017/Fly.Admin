using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
namespace Fly.Web.Controllers
{

    using ApplicationDbContext = Fly.Core.DataAccess.FlyDbContext;
    using Fly.Web.Models;
    using Fly.Core.Models;
    using System.Data.Entity;
    using Fly.Web.Infrastructure;
    using System.Text.RegularExpressions;
    using Fly.Web.Authorization;
    using Fly.Web.Infrastructure.HtmlSafer;
    using System.Net;
    using System.IO;

    public class WebsiteController : Controller
    {
        #region 构造函数
        public WebsiteController() { }
        public WebsiteController(ApplicationUserManager userManager, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext; _userManager = userManager;
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private ApplicationDbContext _dbContext;
        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            set
            {
                _dbContext = value;
            }
        }
        #endregion

        #region 获取平台ID和用户
        /// <summary>
        ///   根据当前用户ID获取平台ID
        /// </summary>
        /// <returns></returns>
        private Guid GetPlatformId()
        {
            Guid employeeId = GetEmployeeId();
            return DbContext.Employees.SingleOrDefault(p => p.Id == employeeId).PlatformId;
        }

        /// <summary>
        ///     获取当前用户ID
        /// </summary>
        /// <returns></returns>
        private Guid GetEmployeeId()
        {
            Guid employeeId = Guid.Parse(User.Identity.GetUserId());
            return employeeId;
        }

        /// <summary>
        ///     获取当前用户信息
        /// </summary>
        /// <returns></returns>
        private Employee GetEmployee()
        {
            Guid employeeId = Guid.Parse(User.Identity.GetUserId());
            return DbContext.Employees.SingleOrDefault(p => p.Id == employeeId);
        }
        #endregion

        #region 其它
        // ACTIONS
        public ActionResult PaymentType()
        {
            return View();
        }
        public ActionResult Announcement()
        {
            return View();
        }
        public ActionResult HelpCenter()
        {
            return View();
        }

        public ActionResult Article()
        {
            return View();
        }

        public ActionResult BBSSection()
        {
            return View();
        }

        public ActionResult BBSTopic()
        {
            return View();
        }

        public ActionResult IP() {
            return View();
        }
        #endregion

        #region Announcement

        public JsonResult Announcements(string keyword, int page = 1, int rows = 15)
        {
            Guid platformId = GetPlatformId();

            return Json(new { total = GetAnnouncementsCount(keyword, platformId), rows = GetAnnouncements(keyword, page, rows, platformId) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddAnnouncement(Announcement announcement)
        {
            announcement.EmployeeId = GetEmployeeId();
            announcement.CreateTime = DateTime.Now;
            announcement.PlatformId = GetPlatformId();
            announcement.Content = HtmlSaferAnalyser.ToSafeHtml(HttpUtility.UrlDecode(announcement.Content ?? ""), false);
            DbContext.Announcements.Add(announcement);
            try
            {
                DbContext.SaveChanges();
                return Json(new { resultCode = 1 });
            }
            catch (Exception)
            {
                return Json(new { resultCode = 0 });

            }
        }

        [HttpPost]
        public JsonResult EditAnnouncement(Announcement announcement)
        {
            Guid platformId = GetPlatformId();
            var targetAnnouncement = DbContext.Announcements.SingleOrDefault(p => p.PlatformId == platformId && p.Id == announcement.Id);
            if (targetAnnouncement != null)
            {
                targetAnnouncement.Content = HtmlSaferAnalyser.ToSafeHtml(HttpUtility.UrlDecode(announcement.Content ?? ""), false);
                targetAnnouncement.Title = announcement.Title;
                if (targetAnnouncement.IsPublished != announcement.IsPublished)
                {
                    targetAnnouncement.IsPublished = announcement.IsPublished;
                    if (announcement.IsPublished)
                    {
                        targetAnnouncement.PublishedTime = DateTime.Now;
                    }
                    else
                    {
                        targetAnnouncement.PublishedTime = null;
                    }
                }
                DbContext.SaveChanges();
                return Json(new { resultCode = 1 });
            }
            else
            {
                return Json(new { resultCode = 0, message = "没有找到相关公告" });
            }
        }

        [HttpPost]
        public JsonResult RemoveAnnouncement(Guid id)
        {
            Guid platformId = GetPlatformId();
            var targetAnnouncement = DbContext.Announcements.SingleOrDefault(p => p.PlatformId == platformId && p.Id == id);
            if (targetAnnouncement != null)
            {
                DbContext.Announcements.Remove(targetAnnouncement);
                DbContext.SaveChanges();
                return Json(new { resultCode = 1 });
            }
            else
            {
                return Json(new { resultCode = 0, message = "没有找到相关公告" });
            }
        }
        [HttpPost]
        public JsonResult ChangeAnnouncementStatus(Guid id)
        {
            Guid platformId = GetPlatformId();
            var targetAnnouncement = DbContext.Announcements.SingleOrDefault(p => p.PlatformId == platformId && p.Id == id);
            if (targetAnnouncement != null)
            {
                targetAnnouncement.IsPublished = !targetAnnouncement.IsPublished;
                if (targetAnnouncement.IsPublished)
                {
                    targetAnnouncement.PublishedTime = DateTime.Now;
                }
                else
                {
                    targetAnnouncement.PublishedTime = null;
                }
                DbContext.SaveChanges();
                return Json(new { resultCode = 1, status = targetAnnouncement.IsPublished, id = targetAnnouncement.Id, publishedTime = targetAnnouncement.PublishedTime });
            }
            else
            {
                return Json(new { resultCode = 0, message = "没有找到相关公告" });
            }
        }

        public JsonResult GetAnnouncement(Guid id)
        {
            Guid platformId = GetPlatformId();
            var an = DbContext.Announcements.Where(p => p.PlatformId == platformId && p.Id == id).Select(p => new
            {
                p.Content,
                p.CreateTime,
                EmployeeName = p.Employee.Name,
                p.Id,
                p.IsPublished,
                p.PublishedTime,
                p.Title
            }).FirstOrDefault();
            if (an != null)
            {

                return Json(an, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { resultCode = 0, JsonRequestBehavior.AllowGet });
            }

        }


        private int GetAnnouncementsCount(string keyword, Guid platformId)
        {
            return DbContext.Announcements.Count(p => p.PlatformId == platformId && (string.IsNullOrEmpty(keyword) ? true : (p.Title.Contains(keyword) || p.Employee.Name.Contains(keyword))));
        }

        private object GetAnnouncements(string keyword, int pageIndex, int pageSize, Guid platformId)
        {
            return DbContext.Announcements.Where(p => p.PlatformId == platformId && (string.IsNullOrEmpty(keyword) ? true : (p.Title.Contains(keyword) || p.Employee.Name.Contains(keyword)))).OrderByDescending(p => p.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(p => new
            {
                p.CreateTime,
                EmployeeName = p.Employee.Name,
                p.Id,
                p.IsPublished,
                p.PublishedTime,
                p.Title
            });
        }

        #endregion

        

        


        #region IPs


        [HttpPost]
        public JsonResult AddIP(IPAddressBlackWeb ip)
        {
            Guid platformId = GetPlatformId();
            ip.CreateTime = DateTime.Now;
            ip.PlatformId = platformId;
            Regex regexIPv4 = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$", RegexOptions.IgnoreCase);
            Regex regexIPv6 = new Regex(!string.IsNullOrEmpty(ip.IPv6) && ip.IPv6.Contains("::") ? @"^([\da-f]{1,4}(:|::)){1,6}[\da-f]{1,4}$" : @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$", RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(ip.IPv4) || !string.IsNullOrEmpty(ip.IPv6))
            {
                if (!string.IsNullOrEmpty(ip.IPv4) && !regexIPv4.IsMatch(ip.IPv4))
                {
                    return Json(new { isError = true, msg = "IPv4地址有误" });
                }
                if (!string.IsNullOrEmpty(ip.IPv6) && !regexIPv6.IsMatch(ip.IPv6))
                {
                    return Json(new { isError = true, msg = "IPv6地址有误" });
                }
                DbContext.IPAddressBlackWebs.Add(ip);
                DbContext.SaveChanges();
                return Json("操作成功");
            }
            else
            {
                return Json(new { isError = true, msg = "IPv4和IPv6地址不能全部为空" });
            }
        }
        [HttpPost]
        public JsonResult EditIP(IPAddressBlackWeb ip)
        {
            Guid platformId = GetPlatformId();
            var targetIP = DbContext.IPAddressBlackWebs.Where(p => p.PlatformId == platformId && p.Id == ip.Id).FirstOrDefault();
            if (targetIP != null)
            {
                Regex regexIPv4 = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$", RegexOptions.IgnoreCase);
                Regex regexIPv6 = new Regex((!string.IsNullOrEmpty(ip.IPv6) && ip.IPv6.Contains("::")) ? @"^([\da-f]{1,4}(:|::)){1,6}[\da-f]{1,4}$" : @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$", RegexOptions.IgnoreCase);
                if (!string.IsNullOrEmpty(ip.IPv4) || !string.IsNullOrEmpty(ip.IPv6))
                {
                    if (!string.IsNullOrEmpty(ip.IPv4) && !regexIPv4.IsMatch(ip.IPv4))
                    {
                        return Json(new { isError = true, msg = "IPv4地址有误" });
                    }
                    if (!string.IsNullOrEmpty(ip.IPv6) && !regexIPv6.IsMatch(ip.IPv6))
                    {
                        return Json(new { isError = true, msg = "IPv6地址有误" });
                    }
                    targetIP.IPv4 = ip.IPv4;
                    targetIP.IPv6 = ip.IPv6;
                    targetIP.Enable = ip.Enable;
                    DbContext.SaveChanges();
                    return Json("操作成功");
                }
                else
                {
                    return Json(new { isError = true, msg = "IPv4和IPv6地址不能全部为空" });
                }
            }
            else
            {
                return Json(new { isError = true, msg = "没有找到要修改的IP黑名单记录" });
            }
        }
        [HttpPost]
        public JsonResult RemoveIP(IPAddressBlackWeb ip)
        {
            Guid platformId = GetPlatformId();
            var targetIP = DbContext.IPAddressBlackWebs.Where(p => p.PlatformId == platformId && p.Id == ip.Id).FirstOrDefault();
            if (targetIP != null)
            {
                try
                {
                    DbContext.IPAddressBlackWebs.Remove(targetIP);
                    DbContext.SaveChanges();
                    return Json("操作成功");
                }
                catch (Exception)
                {
                    return Json(new { isError = true, msg = "删除失败" });
                }
            }
            else
            {
                return Json(new { isError = true, msg = "没有找到要删除的IP黑名单记录" });
            }
        }
        public JsonResult IPs(string keyword, int rows = 10, int page = 1)
        {
            Guid platformId = GetPlatformId();
            var ips = DbContext.IPAddressBlackWebs.Where(p => p.PlatformId == platformId && (string.IsNullOrEmpty(keyword) ? true : (p.IPv4.Contains(keyword)||p.IPv6.Contains(keyword)||p.Remark.Contains(keyword))));
            return Json(new
            {
                total = ips.Count(),
                rows = ips.OrderByDescending(s => s.CreateTime).Skip((page - 1) * rows).Take(rows).Select(s => new
                {
                    s.CreateTime,
                    s.Id,
                    s.IPv4,
                    s.IPv6,
                    s.Enable,
                    s.Remark
                })
            });
        }
        #endregion

        #region 首页
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取入库饼图数据
        /// </summary>5
        /// <returns></returns>
        public JsonResult GetStorageChartChart(DateTime? startDate, DateTime? endDate)
        {
            if (endDate == null)
            {
                endDate = DateTime.Now;
            }
            if (startDate == null)
            {
                startDate = DateTime.Parse("2015-01-01");
            }
            JsonResult jResult = new JsonResult();
            var strSite = GetJsonByUrl("http://192.168.0.204:8983/solr/article_shard1_replica2/select?q=createTime%3A[" + ((DateTime)startDate).ToString("yyyy-MM-dd") + "T00%3A00%3A00.000Z+TO+" + ((DateTime)endDate).AddDays(1).ToString("yyyy-MM-dd") + "T00%3A00%3A00.000Z]+AND+siteId%3A*&fl=numFound&wt=json&indent=true");
            var strNoSite = GetJsonByUrl("http://192.168.0.204:8983/solr/article_shard1_replica2/select?q=createTime%3A[" + ((DateTime)startDate).ToString("yyyy-MM-dd") + "T00%3A00%3A00.000Z+TO+" + ((DateTime)endDate).AddDays(1).ToString("yyyy-MM-dd") + "T00%3A00%3A00.000Z]+AND+NOT+siteId%3A*&fl=numFound&wt=json&indent=true");
            jResult.Data = new { strSite = strSite, strNoSite = strNoSite };
            return jResult;
        }
        /// <summary>
        /// 获取入库折线图数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStorageChartLine(DateTime? startDate, DateTime? endDate)
        {
            if (endDate == null)
            {
                endDate = DateTime.Now;
            }
            if (startDate == null)
            {
                startDate = ((DateTime)endDate).AddDays(-7);
            }
            JsonResult jResult = new JsonResult();
            var str = GetJsonByUrl("http://192.168.0.204:8983/bdsc-index/v2/group/date/count.serv?format=json&startDate=" + ((DateTime)startDate).ToString("yyyy-MM-dd") + "&endDate=" + ((DateTime)endDate).ToString("yyyy-MM-dd"));
            str = str.Replace(((DateTime)startDate).ToString("yyyy-"), "");
            str = str.Replace(((DateTime)endDate).ToString("yyyy-"), "");
            jResult.Data = str;
            return jResult;
        }
        /// <summary>
        /// GET获取url的页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetJsonByUrl(string url)
        {
            JsonResult jResult = new JsonResult();
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    return sr.ReadToEnd();
                }
            }
            catch { }
            return "";
        }
        #endregion

    }
}