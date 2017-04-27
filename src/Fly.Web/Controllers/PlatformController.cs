using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fly.Core.DataAccess;
using Fly.Core.Models;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Fly.Web.Authorization;
using Microsoft.AspNet.Identity;
namespace Fly.Web.Controllers
{
    using ApplicationDbContext = Fly.Core.DataAccess.FlyDbContext;

    using Fly.Web.Models;
    using System.Text.RegularExpressions;
    [Permission]
    public class PlatformController : Controller
    {
        #region 构造函数
        public PlatformController() { }
        public PlatformController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
        private Platform GetPlatform()
        {
            Guid employeeId = GetEmployeeId();
            return DbContext.Employees.SingleOrDefault(p => p.Id == employeeId).Platform;
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

        // GET: Platform
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult IP()
        {
            return View();
        }

        public JsonResult GetPlatformById(Guid id)
        {
            if (id != Guid.Empty)
            {
                var platform = DbContext.Platforms.Where(p => p.Id == id).Select(p => new
                {
                    p.Address,
                    p.Contact,
                    p.CreateTime,
                    p.Domain,
                    p.Email,
                    p.ZipCode,
                    p.Id,
                    p.PlatformName,
                    p.PhoneNumber,
                    p.Remark
                }).FirstOrDefault();
                if (platform != null)
                {
                    return Json(new { row = platform }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPlatforms(string keyword)
        {
            int pageIndex = Convert.ToInt32(Request.Params["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request.Params["rows"] ?? "15");
            int count = GetPlatformCountByKeyword(keyword);
            return Json(new { total = count, rows = GetPlatformByPage(pageIndex, pageSize, keyword) }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult Edit(Platform platform)
        {


            Platform targetPlatform = DbContext.Platforms.Where(p => p.Id == platform.Id).FirstOrDefault();
            if (targetPlatform != null)
            {
                targetPlatform.Address = platform.Address;
                targetPlatform.Contact = platform.Contact;
                targetPlatform.Domain = platform.Domain;
                targetPlatform.Email = platform.Email;
                targetPlatform.PhoneNumber = platform.PhoneNumber;
                targetPlatform.PlatformName = platform.PlatformName;
                targetPlatform.ZipCode = platform.ZipCode;
                targetPlatform.Remark = platform.Remark;
                DbContext.SaveChanges();
            }

            return Json(new { resultCode = 1, message = "操作完成" });
        }

        [HttpPost]
        public JsonResult Add(Platform platform)
        {
            platform.CreateTime = DateTime.Now;

            DbContext.Platforms.Add(platform);
            DbContext.SaveChanges();
            return Json(new { resultCode = 1, message = "操作成功" });
        }

        [HttpPost]
        public JsonResult Remove(List<Guid> ids)
        {

            if (ids.Count > 0)
            {

                int count = 0;

                List<Platform> removeItems = (from p in DbContext.Platforms where (ids).Contains(p.Id) select p).ToList();
                count = removeItems.Count;
                DbContext.Platforms.RemoveRange(removeItems);
                try
                {
                    DbContext.SaveChanges();
                    return new JsonResult() { Data = new { resultCode = 1, message = "成功删除" + count + "项" } };
                }
                catch (Exception)
                {
                    return new JsonResult() { Data = new { resultCode = 0, message = "删除失败，有平台下存在数据，无法删除" } };

                }

            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
            }

        }


        [HttpPost]
        public JsonResult AddIP(IPAddressWhiteOA ip)
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
                DbContext.IPAddressWhiteOAs.Add(ip);
                DbContext.SaveChanges();
                RefreshWhiteIPsCache();
                return Json("操作成功");
            }
            else
            {
                return Json(new { isError = true, msg = "IPv4和IPv6地址不能全部为空" });
            }
        }
        [HttpPost]
        public JsonResult EditIP(IPAddressWhiteOA ip)
        {
            Guid platformId = GetPlatformId();
            var targetIP = DbContext.IPAddressWhiteOAs.Where(p => p.PlatformId == platformId && p.Id == ip.Id).FirstOrDefault();
            if (targetIP != null)
            {
                Regex regexIPv4 = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$", RegexOptions.IgnoreCase);
                Regex regexIPv6 = new Regex((!string.IsNullOrEmpty(ip.IPv6)&&ip.IPv6.Contains("::"))? @"^([\da-f]{1,4}(:|::)){1,6}[\da-f]{1,4}$" : @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$", RegexOptions.IgnoreCase);
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
                    RefreshWhiteIPsCache();
                    return Json("操作成功");
                }
                else
                {
                    return Json(new { isError = true, msg = "IPv4和IPv6地址不能全部为空" });
                }
            }
            else
            {
                return Json(new { isError = true, msg = "没有找到要修改的IP白名单记录" });
            }
        }
        [HttpPost]
        public JsonResult RemoveIP(IPAddressWhiteOA ip)
        {
            Guid platformId = GetPlatformId();
            var targetIP = DbContext.IPAddressWhiteOAs.Where(p => p.PlatformId == platformId && p.Id == ip.Id).FirstOrDefault();
            if (targetIP != null)
            {
                try
                {
                    DbContext.IPAddressWhiteOAs.Remove(targetIP);
                    DbContext.SaveChanges();
                    RefreshWhiteIPsCache();
                    return Json("操作成功");
                }
                catch (Exception)
                {
                    return Json(new { isError = true, msg = "删除失败" });
                }
            }
            else
            {
                return Json(new { isError = true, msg = "没有找到要删除的IP白名单记录" });
            }
        }
        public JsonResult IPs(string keyword, int rows = 10, int page = 1)
        {
            Guid platformId = GetPlatformId();
            var ips = DbContext.IPAddressWhiteOAs.Where(p => p.PlatformId == platformId && (string.IsNullOrEmpty(keyword) ? true : (p.IPv4.Contains(keyword) || p.IPv6.Contains(keyword) || p.Remark.Contains(keyword))));
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
   

        /// <summary>
        ///     手动刷新白名单缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateWhiteIPsStatus()
        {
            RefreshWhiteIPsCache();
            return Json(new { resultCode = 1 });
        }

   
        private void RefreshWhiteIPsCache() {
            var whiteIPs = new WhiteIPViewModel();
            var iplist =GetPlatform().IPAddressWhiteOAs.ToList();
            whiteIPs.IPv4 = iplist.Select(s => s.IPv4).Distinct().ToList();
            whiteIPs.IPv6 = iplist.Select(s => s.IPv6).Distinct().ToList();
            System.Web.HttpContext.Current.Cache.Insert("WhiteIPs", whiteIPs, null, DateTime.Now.AddMinutes(10), TimeSpan.Zero);
        }

        #region 分页获取


        private Int32 GetPlatformCountByKeyword(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                using (FlyDbContext ctx = new FlyDbContext())
                {
                    return ctx.Platforms.Count();
                }
            }
            else
            {
                using (FlyDbContext ctx = new FlyDbContext())
                {
                    return ctx.Platforms.Count(p => p.PlatformName.Contains(keyword) || p.PhoneNumber.Contains(keyword) || p.ZipCode.Contains(keyword) || p.Remark.Contains(keyword) || p.Contact.Contains(keyword));
                }
            }

        }

        private List<Platform> GetPlatformByPage(int pageIndex, int pageSize, string keyword)
        {
            using (FlyDbContext ctx = new FlyDbContext())
            {
                ctx.Configuration.ProxyCreationEnabled = false;
                if (string.IsNullOrEmpty(keyword))
                {
                    return ctx.Platforms.OrderBy(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    return ctx.Platforms.Where(p => p.PlatformName.Contains(keyword) || p.PhoneNumber.Contains(keyword) || p.ZipCode.Contains(keyword) || p.Remark.Contains(keyword) || p.Contact.Contains(keyword)).OrderBy(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
            }
        }



        #endregion

    }
}