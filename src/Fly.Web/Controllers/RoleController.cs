using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fly.Core.DataAccess;
using Fly.Core.Models;
using Fly.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Fly.Web.Authorization;
namespace Fly.Web.Controllers
{
    using ApplicationDbContext = Fly.Core.DataAccess.FlyDbContext;
    [Permission]
    public class RoleController : Controller
    {
        #region 构造函数
        public RoleController() { }
        public RoleController(ApplicationDbContext dbContext)
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

        #region 开发过程中的临时方法
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
        ///     根据当前用户获取平台
        /// </summary>
        /// <returns></returns>
        private Platform GetPlatform()
        {
            return GetEmployee().Platform;
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

        public JsonResult GetComboData()
        {
            List<RoleComboItemViewModel> comboItemList = new List<RoleComboItemViewModel>();
            Guid platformGuid = GetPlatformId();
            var roles = DbContext.Roles.Where(r => r.PlatformId == platformGuid);
            foreach (var item in roles)
            {
                RoleComboItemViewModel comboItem = new RoleComboItemViewModel() { RoleId = item.Id, RoleName = item.Name };
                comboItemList.Add(comboItem);
            }
            return Json(comboItemList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetRolePermissionByRoleId(Guid id)
        {
            Guid platformId = GetPlatformId();
            if (id != Guid.Empty)
            {
                List<RolePermissionViewModel> viewList = new List<RolePermissionViewModel>();
                var rolePermissions = DbContext.RolePermissions.Where(p => p.RoleId == id && p.Role.PlatformId == platformId);
                foreach (var item in rolePermissions)
                {
                    viewList.Add(new RolePermissionViewModel() { PermissionLineId = item.PermissionLineId });
                }
                return Json(viewList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { resultCode = 0, message = "非法的请求" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetRoles(string keyword)
        {
            int pageIndex = Convert.ToInt32(Request.Params["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request.Params["rows"] ?? "15");
            int count = GetRolesCountByKeyword(keyword);
            return Json(new { total = count, rows = GetRolesByPage(pageIndex, pageSize, keyword) }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Add(List<Guid> permissionlineIds, Role role)
        {
            role.PlatformId = GetPlatformId();
            DbContext.Roles.Add(role);
            if ( permissionlineIds!=null&& permissionlineIds.Count > 0)
            {
                List<RolePermission> rolePsermissionList = new List<RolePermission>();
                foreach (Guid id in permissionlineIds)
                {
                    rolePsermissionList.Add(new RolePermission() { PermissionLineId = id });
                }
                DbContext.RolePermissions.AddRange(rolePsermissionList);

            }
            DbContext.SaveChanges();
            return Json(new { resultCode = 1, message = "操作成功" });

        }

        [HttpPost]
        public JsonResult Edit(Role role, List<Guid> permissionlineIds)
        {

            if (role.Id != Guid.Empty)
            {

                Guid platformId = GetPlatformId();
                Role targetRole = DbContext.Roles.Where(p => p.PlatformId == platformId && p.Id == role.Id).FirstOrDefault();
                if (targetRole != null)
                {
                    targetRole.Name = role.Name;
                    targetRole.CustomAttribute = role.CustomAttribute;
                    targetRole.Remark = role.Remark;
                  
                    var oldRolePermissions = DbContext.RolePermissions.Where(p => p.RoleId == targetRole.Id);
                    DbContext.RolePermissions.RemoveRange(oldRolePermissions);
                
                    if (permissionlineIds != null&&permissionlineIds.Count > 0)
                    {

                        List<RolePermission> rolePsermissionList = new List<RolePermission>();
                        foreach (Guid pid in permissionlineIds)
                        {
                            rolePsermissionList.Add(new RolePermission() { PermissionLineId = pid ,RoleId=role.Id});
                        }
                        DbContext.RolePermissions.AddRange(rolePsermissionList);
                    }
                    DbContext.SaveChanges();
                    if (User.IsInRole(targetRole.Name))
                        PermissionParticle.SetPermission();
                }
                return Json(new { resultCode = 1, message = "操作完成" });
            }
            else
            {
                return Json(new { resultCode = 0, message = "非法的请求" });
            }
        }

        [HttpPost]
        public JsonResult Remove(List<Guid> ids)
        {
            Guid platformId = GetPlatformId();
            if (ids.Count > 0)
            {
                int count = 0;
                try
                {
                    List<Role> removeItems = (from p in DbContext.Roles where (ids).Contains(p.Id) && p.PlatformId == platformId select p).ToList();
                    count = removeItems.Count;
                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            DbContext.Roles.Remove(removeItems[i]);
                        }

                    }
                    DbContext.SaveChanges();
                    return new JsonResult() { Data = new { resultCode = 1, message = "成功删除" + count + "项" } };
                }
                catch (Exception)
                {
                    return new JsonResult() { Data = new { resultCode = 0, message = "删除失败，的角色中存在正在被使用的角色" } };
                }
            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
            }
        }

        #region 分页获取

        private Int32 GetRolesCountByKeyword(string keyword)
        {
            Guid platformId = GetPlatformId();
            if (string.IsNullOrEmpty(keyword))
            {

                return DbContext.Roles.Count(p => p.PlatformId == platformId);

            }
            else
            {

                return DbContext.Roles.Count(p => p.PlatformId == platformId && (p.Remark.Contains(keyword) || p.CustomAttribute.Contains(keyword) || p.Name.Contains(keyword)));

            }

        }

        private List<Role> GetRolesByPage(int pageIndex, int pageSize, string keyword)
        {
            Guid platformId = GetPlatformId();
            using (FlyDbContext ctx = new ApplicationDbContext()) {
                ctx.Configuration.ProxyCreationEnabled = false;
                if (string.IsNullOrEmpty(keyword))
                {
                    return ctx.Roles.OrderBy(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    return ctx.Roles.Where(p => p.PlatformId == platformId && (p.Remark.Contains(keyword) || p.CustomAttribute.Contains(keyword) || p.Name.Contains(keyword))).OrderBy(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
            }
            
        }





        #endregion

        public JsonResult GetRoleByIds()
        {
            List<Guid> roleIds = new List<Guid>();
            Guid platformId = GetPlatformId();
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (!string.IsNullOrEmpty(Request.Params["ids"]))
            {
                string[] strIdArr = Request.Params["ids"].Split(',');
                foreach (var strId in strIdArr)
                {
                    Guid roleId;
                    if (Guid.TryParse(strId, out roleId))
                    {
                        roleIds.Add(roleId);
                    }
                    else
                    {
                        result.Data = "非法请求";
                        return result;
                    }
                }

                {
                    Guid platformGuid = GetPlatformId();
                    var role = DbContext.Roles.Where(r => roleIds.Contains(r.Id) && r.PlatformId == platformGuid).Select(p => new { p.CustomAttribute,p.Id,p.Name,p.Remark}).ToList();
                    result.Data = role;
                    return result;

                }
            }
            else
            {
                result.Data = "[]";
                return result;
            }
        }


    }
}