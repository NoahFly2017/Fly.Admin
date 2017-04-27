using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fly.Core.DataAccess;
using Fly.Core.Models;
using Fly.Web.Authorization;
using Fly.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
namespace Fly.Web.Controllers
{
    using ApplicationDbContext = Fly.Core.DataAccess.FlyDbContext;

    using Fly.Web.Models;
    [Permission]
    public class OrganizationController : Controller
    {
        #region 构造函数
        public OrganizationController() { }
        public OrganizationController(ApplicationDbContext dbContext)
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
        public ActionResult Index()
        {
            return View();
        }
        [NonAction]
        /// <summary>
        ///   根据当前用户ID获取平台ID
        /// </summary>
        /// <returns></returns>
        private Guid GetPlatformId()
        {
            Guid employeeId = GetEmployeeId();
            return DbContext.Employees.SingleOrDefault(p => p.Id == employeeId).PlatformId;
        }
         [NonAction]
        /// <summary>
        ///     获取当前用户ID
        /// </summary>
        /// <returns></returns>
        private Guid GetEmployeeId()
        {
            Guid employeeId = Guid.Parse(User.Identity.GetUserId());
            return employeeId;
        }

        public JsonResult GetOrganizationList(string keyword)
        {
            Guid platformGuid = GetPlatformId();
            List<OrganizationViewModel> viewOrgList = new List<OrganizationViewModel>();
            //获取combo用的数据，区别在于1级节点的_parentId设为Empty
            if (Request.Params["type"] == "withroot")
            {
                List<Organization> orgList = DbContext.Organizations.Where(o => o.PlatformId == platformGuid).ToList();
                foreach (Organization org in orgList)
                {
                    viewOrgList.Add(new OrganizationViewModel() { _parentId = (org.ParentId == null ? Guid.Empty : org.ParentId), ParentId = org.ParentId, Id = org.Id, DisplayName = org.DisplayName });
                }
                return Json(new { rows = viewOrgList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    List<Organization> orgList = DbContext.Organizations.Where(o => o.PlatformId == platformGuid).ToList();
                    foreach (Organization org in orgList)
                    {
                        viewOrgList.Add(new OrganizationViewModel() { _parentId = org.ParentId, ParentId = org.ParentId, Id = org.Id, DisplayName = org.DisplayName });
                    }
                    return Json(new { rows = viewOrgList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<Organization> orgList = DbContext.Organizations.Where(o => o.PlatformId == platformGuid && o.DisplayName.Contains(keyword)).ToList();
                    int count = orgList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        LoopGetParent(orgList[i], orgList, platformGuid);
                    }
                    foreach (Organization org in orgList)
                    {
                        viewOrgList.Add(new OrganizationViewModel() { _parentId = org.ParentId, ParentId = org.ParentId, Id = org.Id, DisplayName = org.DisplayName });
                    }
                    return Json(new { rows = viewOrgList }, JsonRequestBehavior.AllowGet);
                }
            }


        }
         [NonAction]
        private void LoopGetParent(Organization org, List<Organization> orgList, Guid platformGuid)
        {
            if (org.ParentId != null && !orgList.Exists(delegate(Organization o) { return o.Id == org.ParentId; }))
            {
                Organization parentOrg = DbContext.Organizations.Where(o => o.PlatformId == platformGuid && o.Id == org.ParentId).FirstOrDefault();
                orgList.Add(parentOrg);
                LoopGetParent(parentOrg, orgList, platformGuid);
            }
        }


        [HttpPost]
        public JsonResult Edit(Organization org)
        {
            Guid platformGuid = GetPlatformId();
            if (org.Id != Guid.Empty) 
            {
                Organization targetOrg = DbContext.Organizations.Where(o => o.PlatformId == platformGuid && o.Id == org.Id).FirstOrDefault();
                targetOrg.DisplayName = org.DisplayName;

                if (targetOrg.Platform.Organizations.Count(p => targetOrg.ParentId == p.Id) > 0 && targetOrg.Id != org.ParentId)
                {
                    if (org.ParentId == Guid.Empty)
                    {
                        targetOrg.ParentId = null;
                    }
                    else {
                        targetOrg.ParentId = org.ParentId;
                    }
                }
                else {
                    targetOrg.ParentId = null;
                }
                DbContext.SaveChanges();
                return new JsonResult() { Data = new { resultCode = 1, message = "操作完成" } };
            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法请求" } };
            }
        }

        [HttpPost]
        public JsonResult Add(Organization org)
        {
            Guid platformGuid = GetPlatformId();
            org.PlatformId = platformGuid;
            if (org.ParentId == Guid.Empty)
            {
                org.ParentId = null;
            }
            DbContext.Organizations.Add(org);
            DbContext.SaveChanges();
            return new JsonResult() { Data = new { resultCode = 1, message = "操作完成" } };


        }

        [HttpPost]
        public JsonResult Remove(List<Guid> ids)
        {
            Guid platformGuid = GetPlatformId();
            if (ids.Count > 0)
            {
                int count = 0;
                using (FlyDbContext ctx = new FlyDbContext())
                {
                    List<Organization> removeItems = (from o in ctx.Organizations where (ids).Contains(o.Id) && o.PlatformId == platformGuid select o).ToList();
                    count = removeItems.Count;
                    try
                    {
                        ctx.Organizations.RemoveRange(removeItems);
                        ctx.SaveChanges();
                    }
                    catch (Exception)
                    {
                        return new JsonResult() { Data = new { resultCode = 0, message = "删除失败，某些项正在被使用" } };
                    }
                }
                return new JsonResult() { Data = new { resultCode = 1, message = "成功删除" + count + "项" } };
            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
            }

        }

        [HttpPost]
        public JsonResult Move(List<Guid> ids, Guid parentId)
        {
            Guid platformGuid = GetPlatformId();

            if (ids.Count > 0)
            {



                var items = from o in DbContext.Organizations where ((ids).Contains(o.Id) || o.Id == parentId) && o.PlatformId == platformGuid select o;
                bool ifInItem = false;

                foreach (var item in items)
                {
                    if (parentId == Guid.Empty)
                    {
                        item.ParentId = null;
                    }
                    else
                    {
                        if (item.Id == parentId) ifInItem = true; else item.ParentId = parentId;
                    }

                }
                if (ifInItem || parentId == Guid.Empty)
                {
                    DbContext.SaveChanges();
                }


                return new JsonResult() { Data = new { resultCode = 1, message = "操作成功" } };
            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
            }
        }
    }
}