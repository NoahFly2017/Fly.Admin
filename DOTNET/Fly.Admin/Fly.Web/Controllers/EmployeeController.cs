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
    using System.Threading.Tasks;
    using Fly.Web.Models;
    using System.Security.Claims;
    using Fly.Web.Infrastructure;
    [Permission]
    public class EmployeeController : Controller
    {
        #region 构造函数

        public EmployeeController() { }
        public EmployeeController(ApplicationUserManager userManager, ApplicationDbContext context, ApplicationSignInManager signInMamager)
        {
            _userManager = userManager;
            _dbContext = context;
            _signInManager = signInMamager;
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

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        #endregion

        [AllowAnonymous]
        // GET: Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ContentResult GetIP()
        {
            return new ContentResult() { Content = IPHelper.GetClientIP() };
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
            //var userId = User.Identity.GetUserId();

            switch (result)
            {
                case SignInStatus.Success:
                    //PermissionParticle.GetPermissionGroup(DbContext, Guid.Parse(User.Identity.GetUserId()));
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "无效的登录尝试。");
                    return View(model);
            }
        }
        [AllowAnonymous]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            Session.RemoveAll();
            HttpCookie cookie = new HttpCookie("name");
            cookie.Expires = DateTime.MinValue;
            Response.Cookies.Add(cookie);
            return RedirectToAction("Login");
        }

        [Authorize]
        public JsonResult GetEmployeeName()
        {
            Guid employeeId = GetEmployeeId();
            return Json(new { name = DbContext.Employees.SingleOrDefault(p => p.Id == employeeId).Name }, JsonRequestBehavior.AllowGet);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "WebSite");
        }

        [Authorize]
        public async Task<JsonResult> ChangeMinePassword(string oldpassword, string newpassword)
        {
            if (!string.IsNullOrEmpty(oldpassword) && !string.IsNullOrEmpty(newpassword) && oldpassword.Length > 5 && newpassword.Length > 5)
            {
                var employeeId = GetEmployeeId();

                Employee targetEmp = await UserManager.FindByIdAsync(employeeId);
                if (targetEmp.PlatformId == GetPlatformId())
                {
                    var result = await UserManager.ChangePasswordAsync(targetEmp.Id, oldpassword, newpassword);
                    if (result.Succeeded)
                    {
                        return Json(new { resultCode = 1, message = "修改成功" });
                    }
                    else
                    {

                        return Json(new { resultCode = 0, message = "原密码有误" });
                    }
                }
                else
                {
                    return Json(new { resultCode = 0, message = "用户不存在" });
                }
            }
            else
            {
                return Json(new { resultCode = 0, message = "输入的密码长度必须大于6位" });
            }
        }

        public ActionResult Index()
        {
            return View();
        }


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
        /// <summary>
        /// 获得员工树形数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEmployees(string keyword)
        {
            Guid platformGuid = GetPlatformId();
            int pageIndex = Convert.ToInt32(Request.Params["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request.Params["rows"] ?? "15");
            int count = 0;
            List<EmployeeOrOrganizationView> employeeViewList = new List<EmployeeOrOrganizationView>();
            switch (keyword)
            {
                case "女":
                    employeeViewList = GetEmployeeByPageWithSex(pageIndex, pageSize, Sex.Female, platformGuid);
                    count = GetEmployeeCountBySex(Sex.Female, platformGuid);
                    break;
                case "男":
                    employeeViewList = GetEmployeeByPageWithSex(pageIndex, pageSize, Sex.Male, platformGuid);
                    count = GetEmployeeCountBySex(Sex.Male, platformGuid);
                    break;
                case "保密":
                    employeeViewList = GetEmployeeByPageWithSex(pageIndex, pageSize, Sex.Unknown, platformGuid);
                    count = GetEmployeeCountBySex(Sex.Unknown, platformGuid);
                    break;
                case "禁用":
                    employeeViewList = GetEmployeeByPageWithStatus(pageIndex, pageSize, EmployeeStatus.None, platformGuid);
                    count = GetEmployeeCountByStatus(EmployeeStatus.None, platformGuid);
                    break;
                case "在职":
                    employeeViewList = GetEmployeeByPageWithStatus(pageIndex, pageSize, EmployeeStatus.Normal, platformGuid);
                    count = GetEmployeeCountByStatus(EmployeeStatus.Normal, platformGuid);
                    break;
                case "离职":
                    employeeViewList = GetEmployeeByPageWithStatus(pageIndex, pageSize, EmployeeStatus.Leave, platformGuid);
                    count = GetEmployeeCountByStatus(EmployeeStatus.Leave, platformGuid);
                    break;
                default:
                    employeeViewList = GetEmployeeByPageWithKeyword(pageIndex, pageSize, keyword, platformGuid);
                    count = GetEmployeeCountByKeyword(keyword, platformGuid);
                    break;
            }


            return Json(new { total = count, rows = employeeViewList }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 验证员工登录名唯一性
        /// </summary>
        /// <returns></returns>
        public JsonResult ValidateLoginName(string userName)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (!string.IsNullOrEmpty(userName))
            {
                Guid platformGuid = GetPlatformId();
                Employee emp = DbContext.Employees.Where(e => e.PlatformId == platformGuid && e.UserName == userName).FirstOrDefault();

                if (emp == null)
                {
                    result.Data = true;
                }
                else
                {
                    result.Data = false;
                }
                return result;
            }
            else
            {
                result.Data = false;
                return result;
            }
        }

        /// <summary>
        /// 修改员工
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Edit(Employee emp, List<Guid> roleIds)
        {

            Employee targetEmp = await UserManager.FindByIdAsync(emp.Id);
            targetEmp.Job = emp.Job;
            targetEmp.Address = emp.Address;
            targetEmp.CustomAttribute = emp.CustomAttribute;
            targetEmp.Email = emp.Email;
            targetEmp.Name = emp.Name;
            targetEmp.OrganizationId = emp.OrganizationId;
            targetEmp.PhoneNumber = emp.PhoneNumber;
            targetEmp.QQ = emp.QQ;
            targetEmp.Remark = emp.Remark;
            targetEmp.Sex = emp.Sex;
            targetEmp.Status = emp.Status;
            targetEmp.ZipCode = emp.ZipCode;
            while (targetEmp.Roles.Count > 0)
            {
                targetEmp.Roles.Remove(targetEmp.Roles.ToList()[0]);
            }
            if (roleIds.Count > 0)
            {
                foreach (var roleId in roleIds)
                {
                    targetEmp.Roles.Add(new EmployeeRole() { RoleId = roleId });
                }
            }
            var result = await UserManager.UpdateAsync(targetEmp);
            if (result.Succeeded)
            {
                if (roleIds.Count > 0 && User.Identity.GetUserId() == targetEmp.Id.ToString())
                    PermissionParticle.SetPermission();
                return Json(new { resultCode = 1, message = "修改成功" });
            }
            else
            {
                return Json(new { resultCode = 0, message = "修改失败" });
            }
        }

        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Add(Employee model, List<Guid> roleIds)
        {
            if (roleIds.Count > 0)
            {
                Guid platformId = GetPlatformId();
                model.PlatformId = platformId;

                if (roleIds.Count > 0)
                {
                    foreach (var roleId in roleIds)
                    {
                        model.Roles.Add(new EmployeeRole() { RoleId = roleId });
                    }
                }
                var result = await UserManager.CreateAsync(model, "123456a");
                if (result.Succeeded)
                {

                    return Json(new { resultCode = 1, message = "添加成功,新员工密码为“123456a”，请员工及时自行修改密码" });
                }
                else
                    return Json(new { resultCode = 0, message = "添加失败" });
            }
            else
            {
                return Json(new { resultCode = 0, message = "员工角色为必选，请选择员工角色" });
            }
        }


        /// <summary>
        /// 批量移除员工
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Remove(List<Guid> ids)
        {
            if (ids.Count > 0)
            {
                int count = 0;
                bool isError = false;
                foreach (var id in ids)
                {
                    Employee targetEmp = await UserManager.FindByIdAsync(id);
                    if (targetEmp.PlatformId == GetPlatformId())
                    {
                        try
                        {
                            var result = await UserManager.DeleteAsync(targetEmp);
                            if (result.Succeeded)
                            {
                                count++;
                            }
                        }
                        catch (Exception)
                        {
                            isError = true;
                        }
                    }
                }
                if (!isError)
                {
                    return Json(new { resultCode = 1, message = "成功删除了" + count + "个员工" });
                }
                else
                {
                    return Json(new { resultCode = 0, message = "成功删除了" + count + "个员工，为了系统信息完整，部分员工因为有相关操作数据，无法删除，您可以将无法删除的员工设为禁用状态" });
                }
            }
            else
            {
                return Json(new { resultCode = 0, message = "没有找到对应员工" });
            }



        }

        /// <summary>
        /// 批量移动员到另一个部门
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Move(List<Guid> ids, Guid parentid)
        {
            Guid platformGuid = GetPlatformId();
            if (ids.Count > 0 && parentid != Guid.Empty)
            {

                var items = from e in DbContext.Employees where (ids).Contains(e.Id) && e.PlatformId == platformGuid select e;
                var parent = from p in DbContext.Organizations where p.Id == parentid && p.PlatformId == platformGuid select p;
                if (parent != null && parent.Count() > 0)
                {
                    foreach (var item in items)
                    {
                        item.OrganizationId = parentid;
                    }
                    DbContext.SaveChanges();
                }
                return new JsonResult() { Data = new { resultCode = 1, message = "操作成功" } };
            }
            else
            {
                return new JsonResult() { Data = new { resultCode = 0, message = "非法的请求" } };
            }
        }


        /// <summary>
        ///     重置密码
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ResetPassword(List<Guid> ids)
        {
            if (ids.Count > 0)
            {
                int errCount = 0;
                int count = 0;
                foreach (var id in ids)
                {
                    Employee targetEmp = await UserManager.FindByIdAsync(id);

                    if (targetEmp.PlatformId == GetPlatformId())
                    {
                        string token = await UserManager.GeneratePasswordResetTokenAsync(targetEmp.Id);
                        var result = await UserManager.ResetPasswordAsync(targetEmp.Id, token, "123456a");
                        if (result.Succeeded)
                        {
                            count++;
                        }
                        else
                        {
                            errCount++;
                        }
                    }
                }
                if (errCount > 0)
                {

                    return Json(new { resultCode = 0, message = "已重置" + count + "个员工密码为“123456a”，" + errCount + "个失败，请员工及时自行修改密码" });

                }
                else
                {
                    return Json(new { resultCode = 1, message = "已重置" + count + "个员工密码为“123456a”，请员工及时自行修改密码" });
                }

            }
            else
            {
                return Json(new { resultCode = 0, message = "请选择要重置密码的员工" });
            }
        }


        /// <summary>
        ///     更改用户状态
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult ChangeEmployeesStatus(List<Guid> ids, EmployeeStatus status)
        {
            Guid platformGuid = GetPlatformId();

            if (ids.Count > 0)
            {
                int count = 0;
                List<Employee> editItems = (from e in DbContext.Employees where (ids).Contains(e.Id) && e.PlatformId == platformGuid select e).ToList();
                count = editItems.Count;
                foreach (var item in editItems)
                {
                    item.Status = status;
                }
                DbContext.SaveChanges();

                return Json(new { resultCode = 1, message = "已改动" + count + "个员工的账号状态。" });
            }
            else
            {
                return Json(new { resultCode = 0, message = "非法的请求" });
            }

        }

        #region 分页获取

        [NonAction]
        /// <summary>
        /// 按关键字查找
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="platformGuid"></param>
        /// <returns></returns>
        private Int32 GetEmployeeCountByKeyword(string keyword, Guid platformGuid)
        {
            if (string.IsNullOrEmpty(keyword))
            {

                return DbContext.Employees.Count(p => p.PlatformId == platformGuid);

            }
            else
            {

                return DbContext.Employees.Count(p => p.PlatformId == platformGuid && (p.Address.Contains(keyword) || p.CustomAttribute.Contains(keyword) || p.Email.Contains(keyword) || p.Job.Contains(keyword) || p.UserName.Contains(keyword) || p.Name.Contains(keyword) || p.Organization.DisplayName.Contains(keyword) || p.Remark.Contains(keyword) || p.QQ.Contains(keyword) || p.PhoneNumber.Contains(keyword) || p.ZipCode.Contains(keyword)));

            }
        }
        [NonAction]
        private List<EmployeeOrOrganizationView> GetEmployeeByPageWithKeyword(int pageIndex, int pageSize, string keyword, Guid platformGuid)
        {

            List<Employee> employeeList = new List<Employee>();

            if (string.IsNullOrEmpty(keyword))
            {
                employeeList = DbContext.Employees.Where(p => p.PlatformId == platformGuid).OrderBy(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                employeeList = DbContext.Employees.Where(p => p.PlatformId == platformGuid && (p.Address.Contains(keyword) || p.CustomAttribute.Contains(keyword) || p.Email.Contains(keyword) || p.Job.Contains(keyword) || p.UserName.Contains(keyword) || p.Name.Contains(keyword) || p.Organization.DisplayName.Contains(keyword) || p.Remark.Contains(keyword) || p.QQ.Contains(keyword) || p.PhoneNumber.Contains(keyword) || p.ZipCode.Contains(keyword))).OrderBy(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            List<EmployeeOrOrganizationView> viewEmpOrOrgList = new List<EmployeeOrOrganizationView>();
            return GetViewModelList(platformGuid, employeeList, viewEmpOrOrgList);

        }

        [NonAction]
        /// <summary>
        /// 按性别查找
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sex"></param>
        /// <param name="platformGuid"></param>
        /// <returns></returns>
        private List<EmployeeOrOrganizationView> GetEmployeeByPageWithSex(int pageIndex, int pageSize, Sex sex, Guid platformGuid)
        {

            List<Employee> employeeList = new List<Employee>();

            employeeList = DbContext.Employees.Where(p => p.PlatformId == platformGuid && (p.Sex == sex)).OrderBy(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            List<EmployeeOrOrganizationView> viewEmpOrOrgList = new List<EmployeeOrOrganizationView>();
            return GetViewModelList(platformGuid, employeeList, viewEmpOrOrgList);


        }
        [NonAction]
        private Int32 GetEmployeeCountBySex(Sex sex, Guid platformGuid)
        {

            return DbContext.Employees.Count(p => p.PlatformId == platformGuid && (p.Sex == sex));

        }
        [NonAction]
        /// <summary>
        /// 按员工状态查找
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="status"></param>
        /// <param name="platformGuid"></param>
        /// <returns></returns>
        private List<EmployeeOrOrganizationView> GetEmployeeByPageWithStatus(int pageIndex, int pageSize, EmployeeStatus status, Guid platformGuid)
        {

            List<Employee> employeeList = new List<Employee>();

            employeeList = DbContext.Employees.Where(p => p.PlatformId == platformGuid && (p.Status == status)).OrderBy(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            List<EmployeeOrOrganizationView> viewEmpOrOrgList = new List<EmployeeOrOrganizationView>();
            return GetViewModelList(platformGuid, employeeList, viewEmpOrOrgList);


        }
        private Int32 GetEmployeeCountByStatus(EmployeeStatus status, Guid platformGuid)
        {

            List<Employee> employeeList = new List<Employee>();
            return DbContext.Employees.Count(p => p.PlatformId == platformGuid && (p.Status == status));

        }

        [NonAction]
        //生成模型视图
        private List<EmployeeOrOrganizationView> GetViewModelList(Guid platformGuid, List<Employee> employeeList, List<EmployeeOrOrganizationView> viewEmpOrOrgList)
        {
            List<Guid> orgIdList = new List<Guid>();
            foreach (var item in employeeList)
            {
                if (!orgIdList.Contains(item.OrganizationId))
                {
                    orgIdList.Add(item.OrganizationId);
                }
                EmployeeOrOrganizationView viewEmpOrOrg = new EmployeeOrOrganizationView();

                viewEmpOrOrg._parentId = "-" + item.OrganizationId;
                viewEmpOrOrg.Address = item.Address;
                viewEmpOrOrg.CustomAttribute = item.CustomAttribute;
                viewEmpOrOrg.Email = item.Email;
                viewEmpOrOrg.OrganizationId = item.Id.ToString();
                viewEmpOrOrg.Id = item.Id.ToString();
                viewEmpOrOrg.Job = item.Job;
                viewEmpOrOrg.UserName = item.UserName;
                viewEmpOrOrg.Name = item.Name;
                viewEmpOrOrg.RoleIds = item.Roles.Select(p => p.RoleId).ToList();
                viewEmpOrOrg.OrganizationId = item.OrganizationId.ToString();
                viewEmpOrOrg.PlatformId = item.PlatformId;
                viewEmpOrOrg.QQ = item.QQ;
                viewEmpOrOrg.Remark = item.Remark;
                viewEmpOrOrg.Sex = item.Sex;
                viewEmpOrOrg.Status = item.Status;
                viewEmpOrOrg.OrganizationName = item.Organization.DisplayName;
                viewEmpOrOrg.PhoneNumber = item.PhoneNumber;
                viewEmpOrOrg.ZipCode = item.ZipCode;
                viewEmpOrOrgList.Add(viewEmpOrOrg);
            }

            List<Organization> orgList = DbContext.Organizations.Where(o => o.PlatformId == platformGuid && orgIdList.Contains(o.Id)).ToList();
            int orgListCount = orgList.Count;
            for (int i = 0; i < orgListCount; i++)
            {
                LoopGetEmployeeParent(orgList[i], orgList, platformGuid);
            }
            foreach (Organization org in orgList)
            {
                EmployeeOrOrganizationView viewEmpOrOrg = new EmployeeOrOrganizationView();
                viewEmpOrOrg.Id = "-" + org.Id;
                viewEmpOrOrg._parentId = (org.ParentId == null ? null : "-" + org.ParentId);
                viewEmpOrOrg.Name = org.DisplayName;
                viewEmpOrOrgList.Add(viewEmpOrOrg);
            }

            return viewEmpOrOrgList;
        }
        [NonAction]
        //递归父节点
        private void LoopGetEmployeeParent(Organization org, List<Organization> orgList, Guid platformGuid)
        {
            if (org.ParentId != null && !orgList.Exists(delegate(Organization o) { return o.Id == org.ParentId; }))
            {
                Organization parentOrg = DbContext.Organizations.Where(o => o.PlatformId == platformGuid && o.Id == org.ParentId).FirstOrDefault();
                orgList.Add(parentOrg);
                LoopGetEmployeeParent(parentOrg, orgList, platformGuid);
            }
        }
        #endregion
    }
}