using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fly.Core.DataAccess;
using Fly.Core.Models;
using Fly.Web.Models;
using Fly.Web.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Fly.Web.Controllers
{
    using ApplicationDbContext = Fly.Core.DataAccess.FlyDbContext;
    [Permission]
    public class PermissionController : Controller
    {
        #region 构造函数
        public PermissionController() { }
        public PermissionController(ApplicationDbContext dbContext)
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

        #endregion
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Group()
        {
            return View();
        }
  
        #region PermissionLine相关

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPermissionLines()
        {
            Guid platformGuid = GetPlatformId();
            string keyword = Request.Params["keyword"];
            int pageIndex = Convert.ToInt32(Request.Params["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request.Params["rows"] ?? "15");
            int count = 0;
            count = GetPermissionLineCountByKeyword(keyword, platformGuid);
            List<PermissionLineViewModel> viewList = GetPermissionLineyByPageWithKeyword(pageIndex, pageSize, keyword, platformGuid);
            JsonResult result = new JsonResult()
            {
                Data = new { total = count, rows = viewList }
            };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        /// <summary>
        /// 修改权限项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditPermissionLine()
        {
            Guid platformGuid = GetPlatformId();
            if (!string.IsNullOrEmpty(Request.Form["Id"]))
            {
                Guid id = Guid.Parse(Request.Form["Id"]);

                using (FlyDbContext ctx = new FlyDbContext())
                {
                    PermissionLine pl = ctx.PermissionLines.Where(o => o.Group.PlatformId == platformGuid && o.Id == id).FirstOrDefault();
                    UpdateModel(pl);
                    ctx.SaveChanges();
                    var PermissionLines = System.Web.HttpContext.Current.Session["PermissionLines"] as List<ViewPermissionLine>;
                    var i = (from t in PermissionLines where t.Id == id select t).Count();
                    if (i > 0)
                        PermissionParticle.SetPermission();
                    return new JsonResult() { Data = new { resultCode = 1, message = "操作完成" } };
                }
            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法请求" } };
            }
        }

        /// <summary>
        /// 添加权限项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddPermissionLine()
        {
            Guid platformGuid = GetPlatformId();
            PermissionLine pl = new PermissionLine();
            UpdateModel(pl);
            using (FlyDbContext ctx = new FlyDbContext())
            {
                ctx.PermissionLines.Add(pl);
                ctx.SaveChanges();
                return new JsonResult() { Data = new { resultCode = 1, message = "操作完成" } };
            }
        }

        /// <summary>
        /// 批量删除权限项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemovePermissionLines()
        {
            Guid platformGuid = GetPlatformId();
            string ids = Request.Form["ids"];
            if (!string.IsNullOrEmpty(ids))
            {
                string[] idArray = ids.Split(',');
                List<Guid> idlist = new List<Guid>();
                foreach (string id in idArray)
                {
                    Guid intId;
                    if (Guid.TryParse(id, out intId))
                    {
                        idlist.Add(intId);
                    }
                    else
                    {
                        return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
                    }
                }
                int count = 0;
                using (FlyDbContext ctx = new FlyDbContext())
                {
                    List<PermissionLine> removeItems = (from o in ctx.PermissionLines where (idlist).Contains(o.Id) && o.Group.PlatformId == platformGuid select o).ToList();
                    count = removeItems.Count;
                    ctx.PermissionLines.RemoveRange(removeItems);
                    ctx.SaveChanges();
                }
                return new JsonResult() { Data = new { resultCode = 1, message = "成功删除" + count + "项" } };
            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
            }

        }


        /// <summary>
        /// 批量移动权限项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MovePermissionLines()
        {
            Guid platformGuid = GetPlatformId();
            string ids = Request.Form["ids"];
            Guid parentid;
            if (!string.IsNullOrEmpty(ids) && Guid.TryParse(Request.Form["parentid"], out parentid))
            {
                string[] idArray = ids.Split(',');
                List<Guid> idlist = new List<Guid>();
                foreach (string id in idArray)
                {
                    Guid intId;
                    if (Guid.TryParse(id, out intId))
                    {
                        idlist.Add(intId);
                    }
                    else
                    {
                        return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
                    }
                }

                using (FlyDbContext ctx = new FlyDbContext())
                {

                    var parent = from p in ctx.PermissionGroups where p.Id == parentid && p.PlatformId == platformGuid select p;
                    if (parent != null && parent.Count() > 0)
                    {
                        var items = from o in ctx.PermissionLines where ((idlist).Contains(o.Id) || o.Id == parentid) && o.Group.PlatformId == platformGuid select o;
                        foreach (var item in items)
                        {
                            item.GroupId = parentid;
                        }
                        ctx.SaveChanges();
                    }
                }
                return new JsonResult() { Data = new { resultCode = 1, message = "操作成功" } };
            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
            }
        }




        /// <summary>
        /// 获取EasyUI树形数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPermissionLinesTree()
        {
            Guid platformGuid = GetPlatformId();
            using (FlyDbContext ctx = new FlyDbContext())
            {
                List<PermissionLine> plList = new List<PermissionLine>();
                //获取所有权限项
                plList = ctx.PermissionLines.Where(p => p.Group.PlatformId == platformGuid).OrderBy(s => s.Id).ToList();
                List<Guid> idList = new List<Guid>();
                //获取权限项用到的所有权限组
                foreach (var item in plList)
                {
                    if (!idList.Contains(item.GroupId))
                    {
                        idList.Add(item.GroupId);
                    }
                }
                List<PermissionGroup> pgList = ctx.PermissionGroups.Where(o => o.PlatformId == platformGuid && idList.Contains(o.Id)).ToList();
                int pgListCount = pgList.Count;
                for (int i = 0; i < pgListCount; i++)
                {
                    LoopGetParent(ctx, pgList[i], pgList, platformGuid);
                }
                //到这里所有权限项和相关所有权限组都取到了，下面递归构建树形试图对象
                List<PermissionLineTreeViewModel> viewTreeItemList = new List<PermissionLineTreeViewModel>();
                //一级节点
                List<PermissionGroup> rootPGList = pgList.FindAll(delegate(PermissionGroup pg) { return pg.ParentId == null; });
                foreach (var rootPG in rootPGList)
                {
                    PermissionLineTreeViewModel treeItem = new PermissionLineTreeViewModel();
                    treeItem.id = "-" + rootPG.Id.ToString();//负数以与权限项做区别
                    treeItem.text = rootPG.DisplayName;

                    List<PermissionGroup> childrenPGList = pgList.FindAll(delegate(PermissionGroup pg) { return pg.ParentId == rootPG.Id; });
                    foreach (var childrenPG in childrenPGList)
                    {
                        //递归子节点
                        LoopGetPermissionLinesChildrenTree(treeItem, childrenPG, pgList, plList);
                    }
                    //将该节点的权限项加入children属性中
                    List<PermissionLine> childrenPLList = plList.FindAll(delegate(PermissionLine pl) { return pl.Group.Id == rootPG.Id; });

                    foreach (var childrenPL in childrenPLList)
                    {
                        PermissionLineTreeViewModel childrenTreeItem = new PermissionLineTreeViewModel();
                        childrenTreeItem.id = childrenPL.Id.ToString();
                        childrenTreeItem.text = childrenPL.DisplayName;
                        treeItem.children.Add(childrenTreeItem);
                        plList.Remove(childrenPL);//移除使用过的项
                    }
                    viewTreeItemList.Add(treeItem);
                }
                JsonResult result = new JsonResult() { Data = new[] { new { id = 0, text = "所有权限", children = viewTreeItemList } } };
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        /// <summary>
        /// 循环获取子节点树形数据
        /// </summary>
        /// <param name="parentTreeItem"></param>
        /// <param name="currentPG"></param>
        /// <param name="pgList"></param>
        /// <param name="plList"></param>
        private void LoopGetPermissionLinesChildrenTree(PermissionLineTreeViewModel parentTreeItem, PermissionGroup currentPG, List<PermissionGroup> pgList, List<PermissionLine> plList)
        {
            PermissionLineTreeViewModel currentTreeItem = new PermissionLineTreeViewModel();
            currentTreeItem.id = "-" + currentPG.Id;//负数以与权限项做区别
            currentTreeItem.text = currentPG.DisplayName;
            List<PermissionGroup> childrenPGList = pgList.FindAll(delegate(PermissionGroup pg) { return pg.ParentId == currentPG.Id; });
            foreach (var childrenPG in childrenPGList)
            {
                //递归子节点
                LoopGetPermissionLinesChildrenTree(currentTreeItem, childrenPG, pgList, plList);
            }
            //将该节点的权限项加入children属性中
            List<PermissionLine> childrenPLList = plList.FindAll(delegate(PermissionLine pl) { return pl.Group.Id == currentPG.Id; });
            foreach (var childrenPL in childrenPLList)
            {
                PermissionLineTreeViewModel childrenTreeItem = new PermissionLineTreeViewModel();
                childrenTreeItem.id = childrenPL.Id.ToString();
                childrenTreeItem.text = childrenPL.DisplayName;
                currentTreeItem.children.Add(childrenTreeItem);
                plList.Remove(childrenPL);//移除使用过的项
            }
            parentTreeItem.children.Add(currentTreeItem);
            pgList.Remove(currentPG);//移除使用过的项
        }
        /// <summary>
        /// 关键字分页查找权限项目
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="platformGuid"></param>
        /// <returns></returns>
        private List<PermissionLineViewModel> GetPermissionLineyByPageWithKeyword(int pageIndex, int pageSize, string keyword, Guid platformGuid)
        {
            using (FlyDbContext ctx = new FlyDbContext())
            {
                List<PermissionLine> plList = new List<PermissionLine>();
                if (string.IsNullOrEmpty(keyword))
                {
                    plList = ctx.PermissionLines.Where(p => p.Group.PlatformId == platformGuid).OrderBy(s => s.GroupId).ThenBy(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    plList = ctx.PermissionLines.Where(p => p.Group.PlatformId == platformGuid && (p.DisplayName.Contains(keyword) || p.Tag.Contains(keyword) || p.Url.Contains(keyword))).OrderBy(s => s.GroupId).ThenBy(s=>s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                List<PermissionLineViewModel> viewList = new List<PermissionLineViewModel>();
                return GetViewModelList(platformGuid, plList, viewList);
            }
        }



        /// <summary>
        /// 获取符合条件的权限项总数
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="platformGuid"></param>
        /// <returns></returns>
        private Int32 GetPermissionLineCountByKeyword(string keyword, Guid platformGuid)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                using (FlyDbContext ctx = new FlyDbContext())
                {

                    return ctx.PermissionLines.Count(p => p.Group.PlatformId == platformGuid);

                }
            }
            else
            {
                using (FlyDbContext ctx = new FlyDbContext())
                {
                    return ctx.PermissionLines.Count(p => p.Group.PlatformId == platformGuid && (p.DisplayName.Contains(keyword) || p.Tag.Contains(keyword) || p.Url.Contains(keyword)));
                }
            }
        }


        /// <summary>
        /// 生成视图模型列表
        /// </summary>
        /// <param name="platformGuid"></param>
        /// <param name="permissionLineList"></param>
        /// <param name="viewList"></param>
        /// <returns></returns>
        private List<PermissionLineViewModel> GetViewModelList(Guid platformGuid, List<PermissionLine> permissionLineList, List<PermissionLineViewModel> viewList)
        {
            List<Guid> idList = new List<Guid>();
            foreach (var item in permissionLineList)
            {
                if (!idList.Contains(item.GroupId))
                {
                    idList.Add(item.GroupId);
                }
                PermissionLineViewModel viewitem = new PermissionLineViewModel();
                viewitem._parentId = "-" + item.GroupId;
                viewitem.DisplayName = item.DisplayName;
                viewitem.GroupId = item.GroupId.ToString();
                viewitem.Id = item.Id.ToString();
                viewitem.Tag = item.Tag;
                viewitem.Url = item.Url;
                viewList.Add(viewitem);
            }
            using (FlyDbContext ctx = new FlyDbContext())
            {
                List<PermissionGroup> pgList = ctx.PermissionGroups.Where(o => o.PlatformId == platformGuid && idList.Contains(o.Id)).ToList();
                int pgListCount = pgList.Count;
                for (int i = 0; i < pgListCount; i++)
                {
                    LoopGetParent(ctx, pgList[i], pgList, platformGuid);
                }
                foreach (PermissionGroup pg in pgList)
                {
                    PermissionLineViewModel viewItem = new PermissionLineViewModel();
                    viewItem._parentId = pg.ParentId == null ? null : "-" + pg.ParentId;
                    viewItem.DisplayName = pg.DisplayName;
                    viewItem.Id = "-" + pg.Id;
                    viewList.Add(viewItem);
                    viewItem.Url = pg.Url;
                    viewItem.Tag = pg.Tag;
                }
            }
            return viewList;
        }



        #endregion

        #region PermissionGroup相关

        public JsonResult GetPermissionGroups()
        {
            string keyword = Request.Params["keyword"];
            using (FlyDbContext ctx = new FlyDbContext())
            {
                Guid platformGuid = GetPlatformId();
                ctx.Configuration.ProxyCreationEnabled = false;
                List<PermissionGroupViewModel> viewPerGroupList = new List<PermissionGroupViewModel>();
                JsonResult result = new JsonResult();
                //获取combo用的数据，区别在于1级节点的_parentId设为Guid.Empty
                if (Request.Params["type"] == "withroot")
                {
                    List<PermissionGroup> perGroupList = ctx.PermissionGroups.Where(o => o.PlatformId == platformGuid).OrderBy(p => p.SN).ToList();

                    foreach (PermissionGroup perg in perGroupList)
                    {
                        viewPerGroupList.Add(new PermissionGroupViewModel() { _parentId = (perg.ParentId == null ? Guid.Empty : perg.ParentId), ParentId = perg.ParentId.ToString(), Tag = perg.Tag, Url = perg.Url, Id = perg.Id.ToString(), DisplayName = perg.DisplayName, Headshot = perg.Headshot, SN = perg.SN });

                    }
                    result = (new JsonResult() { Data = new { rows = viewPerGroupList } });
                }
                else
                {
                    if (string.IsNullOrEmpty(keyword))
                    {
                        List<PermissionGroup> perGroupList = ctx.PermissionGroups.Where(o => o.PlatformId == platformGuid).OrderBy(p => p.SN).ToList();
                        foreach (PermissionGroup perg in perGroupList)
                        {
                            viewPerGroupList.Add(new PermissionGroupViewModel() { _parentId = perg.ParentId.ToString(), ParentId = perg.ParentId.ToString(), Tag = perg.Tag, Url = perg.Url, Id = perg.Id.ToString(), DisplayName = perg.DisplayName, Headshot = perg.Headshot, SN = perg.SN });
                        }
                        result = (new JsonResult() { Data = new { rows = viewPerGroupList } });
                    }
                    else
                    {
                        List<PermissionGroup> perGroupList = ctx.PermissionGroups.Where(o => o.PlatformId == platformGuid && (o.DisplayName.Contains(keyword) || o.Url.Contains(keyword) || o.Tag.Contains(keyword))).ToList();
                        int count = perGroupList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            LoopGetParent(ctx, perGroupList[i], perGroupList, platformGuid);
                        }
                        foreach (PermissionGroup perg in perGroupList)
                        {
                            viewPerGroupList.Add(new PermissionGroupViewModel() { _parentId = perg.ParentId.ToString(), ParentId = perg.ParentId.ToString(), Tag = perg.Tag, Url = perg.Url, Id = perg.Id.ToString(), DisplayName = perg.DisplayName, Headshot = perg.Headshot, SN = perg.SN });
                        }
                        result = (new JsonResult() { Data = new { rows = viewPerGroupList } });
                    }
                }
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        private void LoopGetParent(FlyDbContext ctx, PermissionGroup perGroup, List<PermissionGroup> orgList, Guid platformGuid)
        {
            if (perGroup.ParentId != null && !orgList.Exists(delegate(PermissionGroup o) { return o.Id == perGroup.ParentId; }))
            {
                PermissionGroup parentPerGroup = ctx.PermissionGroups.Where(o => o.PlatformId == platformGuid && o.Id == perGroup.ParentId).FirstOrDefault();
                orgList.Add(parentPerGroup);
                LoopGetParent(ctx, parentPerGroup, orgList, platformGuid);
            }
        }



        [HttpPost]
        public JsonResult EditPermissionGroup(PermissionGroup pg)
        {
            Guid platformGuid = GetPlatformId();
            if (!string.IsNullOrEmpty(Request.Form["Id"]))
            {
                Guid id = Guid.Parse(Request.Form["Id"]);
                PermissionGroup targetpg = DbContext.PermissionGroups.Where(o => o.PlatformId == platformGuid && o.Id == id).FirstOrDefault();
                targetpg.DisplayName = pg.DisplayName;
                targetpg.Tag = pg.Tag;
                targetpg.Url = pg.Url;
                targetpg.SN = pg.SN;
                targetpg.Headshot = pg.Headshot;
                if (targetpg.Platform.PermissionGroups.Count(p => p.Id == pg.ParentId)>0 && targetpg.Id != pg.ParentId)
                {
                    if (pg.ParentId == Guid.Empty)
                    {
                        targetpg.ParentId = null;
                    }
                    else {
                        targetpg.ParentId = pg.ParentId;
                    }
                }
                else {
                    targetpg.ParentId = null;
                }
              
                DbContext.SaveChanges();
                var PermissionGroups = System.Web.HttpContext.Current.Session["PermissionGroups"] as List<ViewPermissionGroup>;
                var i = (from t in PermissionGroups where t.Id == targetpg.Id select t).Count();
                if (i > 0) PermissionParticle.SetPermission();
                return new JsonResult() { Data = new { resultCode = 1, message = "操作完成" } };

            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法请求" } };
            }
        }

        [HttpPost]
        public JsonResult AddPermissionGroup()
        {
            Guid platformGuid = GetPlatformId();
            PermissionGroup pg = new PermissionGroup();
            UpdateModel(pg);
            pg.PlatformId = platformGuid;
            using (FlyDbContext ctx = new FlyDbContext())
            {
                if (pg.ParentId == Guid.Empty)
                {
                    pg.ParentId = null;
                }
                ctx.PermissionGroups.Add(pg);
                ctx.SaveChanges();
                return new JsonResult() { Data = new { resultCode = 1, message = "操作完成" } };
            }
        }


        [HttpPost]
        public JsonResult RemovePermissionGroups()
        {
            Guid platformGuid = GetPlatformId();
            string ids = Request.Form["ids"];
            if (!string.IsNullOrEmpty(ids))
            {
                string[] idArray = ids.Split(',');
                List<Guid> idlist = new List<Guid>();
                foreach (string id in idArray)
                {
                    Guid intId;
                    if (Guid.TryParse(id, out intId))
                    {
                        idlist.Add(intId);
                    }
                    else
                    {
                        return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
                    }
                }
                int count = 0;
                using (FlyDbContext ctx = new FlyDbContext())
                {
                    List<PermissionGroup> removeItems = (from o in ctx.PermissionGroups where (idlist).Contains(o.Id) && o.PlatformId == platformGuid select o).ToList();
                    count = removeItems.Count;
                    ctx.PermissionGroups.RemoveRange(removeItems);
                    ctx.SaveChanges();
                }
                return new JsonResult() { Data = new { resultCode = 1, message = "成功删除" + count + "项" } };
            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
            }

        }

        [HttpPost]
        public JsonResult MovePermissionGroups()
        {
            Guid platformGuid = GetPlatformId();
            string ids = Request.Form["ids"];
            Guid parentid;
            if (!string.IsNullOrEmpty(ids) && Guid.TryParse(Request.Form["parentid"], out parentid))
            {
                string[] idArray = ids.Split(',');
                List<Guid> idlist = new List<Guid>();
                foreach (string id in idArray)
                {
                    Guid intId;
                    if (Guid.TryParse(id, out intId))
                    {
                        idlist.Add(intId);
                    }
                    else
                    {
                        return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
                    }
                }

                using (FlyDbContext ctx = new FlyDbContext())
                {
                    var items = from o in ctx.PermissionGroups where ((idlist).Contains(o.Id) || o.Id == parentid) && o.PlatformId == platformGuid select o;
                    bool ifInItem = false;

                    foreach (var item in items)
                    {
                        if (parentid == null)
                        {
                            item.ParentId = null;
                        }
                        else
                        {
                            if (item.Id == parentid) ifInItem = true; else item.ParentId = parentid;
                        }

                    }
                    if (ifInItem || parentid == null)
                    {
                        ctx.SaveChanges();
                    }

                }
                return new JsonResult() { Data = new { resultCode = 1, message = "操作成功" } };
            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
            }
        }
        #endregion

        //  [PermissionAttribute]
        public JsonResult UnPermissionMessage()
        {
            return Json(new { resultCode = -1, message = "非法的请求" }, JsonRequestBehavior.AllowGet);
        }
    }
}