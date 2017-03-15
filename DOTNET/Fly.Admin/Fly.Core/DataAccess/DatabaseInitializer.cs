using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using Fly.Core.Models;
using System.Linq;
using Fly.Core.Infrastructure;
namespace Fly.Core.DataAccess
{
    public class DatabaseInitializer : CreateDatabaseIfNotExists<FlyDbContext>
  //public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<FlyDbContext>
    {
        private PasswordHasher _passwordHasher = new PasswordHasher();
        protected override void Seed(FlyDbContext context)
        {
            //初始化xx公司平台数据           
            var sanhedai = new Platform { PlatformName = "xx公司", Domain = "xxxxx.com", Contact = "林珊", PhoneNumber = "400-xxxxxxx", Email = "sanhedai@qq.com", Address = "xx省上海市浦东区中央大道696号华友大厦", ZipCode = "351100", Remark = "xx公司（www.xxxxx.com）是汇方（xx）金融信息服务有限公司开发的P2P网络借贷平台，是一个专业的 投融资中介平台，为有资金需求的借款方、有闲置资金的投资方搭建了一座安全、便利的桥梁，满足双方的投融资需求。借款方通过在xx公司上提交材料、申请认证，获得相应的信用积分和信用等级，根据自身的资金需求发布合适的借款标的，快速、便捷地筹集资金；投资方通过xx公司投资给信用良好的个人或企业，按借款期限和利率获得相应的资金回报，省时、省力的投资理财。", CreateTime = DateTime.Now, Organizations = new List<Organization>(), };

            sanhedai.Roles = new List<Role>();
            var role0 = new Role()
            {
                Name = "测试角色1",
                PlatformId = sanhedai.Id
            };
            var role1 = new Role()
            {
                Name = "测试角色2",
                PlatformId = sanhedai.Id
            };
            sanhedai.Roles.Add(role0);
            sanhedai.Roles.Add(role1);

            //初始化组织架构
            var org0 = new Organization { DisplayName = "运营管理中心", Children = new List<Organization>(), Employees = new List<Employee>() };
            var org0_1 = new Organization { DisplayName = "风控部", Employees = new List<Employee>() };
            var org0_2 = new Organization { DisplayName = "法务部", Employees = new List<Employee>() };
            var org0_3 = new Organization { DisplayName = "市场部", Employees = new List<Employee>() };

            //初始化员工。
            Employee emp0 = new Employee { Id = Guid.NewGuid(), UserName = "admin", Name = "超级管理员", PasswordHash = _passwordHasher.HashPassword("123456a"), Job = "运营总监", PhoneNumber = "18758222975", ZipCode = "351100", Address = "xx省上海市浦东区中央大道696号华友大厦", Sex = Sex.Male, QQ = "647839700", Email = "647839700@qq.com", Status = EmployeeStatus.Normal, SecurityStamp = Guid.NewGuid().ToString() };
            org0.Employees.Add(emp0);
            //emp0.Roles.Add(role0);


         
            sanhedai.IPAddressWhiteOAs = new List<IPAddressWhiteOA>();
            sanhedai.IPAddressWhiteOAs.Add(new IPAddressWhiteOA
            {
                IPv4 = "::1",
                CreateTime = DateTime.Now,
                 Enable=true,
                Remark = "本地，真实环境务必删除"
            });


            org0_1.Employees.Add(new Employee { Id = Guid.NewGuid(), UserName = "fangsf", Name = "方善锋", PasswordHash = _passwordHasher.HashPassword("123456a"), Job = "风控主管", PhoneNumber = "13959598888", ZipCode = "351100", Address = "xx省上海市浦东区中央大道696号华友大厦", Sex = Sex.Male, QQ = "647839700", Email = "647839700@qq.com", Status = EmployeeStatus.Normal, SecurityStamp = Guid.NewGuid().ToString() });

            org0_2.Employees.Add(new Employee { Id = Guid.NewGuid(), UserName = "huangxf", Name = "黄晓凤", PasswordHash = _passwordHasher.HashPassword("123456a"), Job = "法务专员", PhoneNumber = "15260898888", ZipCode = "351100", Address = "xx省上海市浦东区中央大道696号华友大厦", Sex = Sex.Female, QQ = "647839700", Email = "647839700@qq.com", Status = EmployeeStatus.Normal, SecurityStamp = Guid.NewGuid().ToString() });

            org0_3.Employees.Add(new Employee { Id = Guid.NewGuid(), UserName = "lvxiaowei", Name = "吕晓伟", PasswordHash = _passwordHasher.HashPassword("123456a"), Job = "平台管理员", PhoneNumber = "15205948888", ZipCode = "351100", Address = "xx省上海市浦东区中央大道696号华友大厦", Sex = Sex.Male, QQ = "647839700", Email = "647839700@qq.com", Status = EmployeeStatus.Normal, SecurityStamp = Guid.NewGuid().ToString() });

            sanhedai.Organizations.Add(org0);
            org0.Children.Add(org0_1);
            org0.Children.Add(org0_2);
            org0.Children.Add(org0_3);
            context.Platforms.Add(sanhedai);
            context.SaveChanges();
            emp0.Roles.Add(new EmployeeRole { RoleId = role0.Id });
       
            //初始化产品
            context.SaveChanges();
         


            #region 添加权限组

            sanhedai.PermissionGroups = new List<PermissionGroup>();

            #region 网站管理
            PermissionGroup pg10 = new PermissionGroup
            {
                DisplayName = "网站管理",
                Tag = "nav_website",
                Url = "#",
                SN = 1,
                Headshot = "website",
                Children = new List<PermissionGroup>(),
                PlatformId = sanhedai.Id
            };
            PermissionGroup pg10_pg0 = new PermissionGroup
            {
                DisplayName = "首页",
                Tag = "nav_index",
                Url = "/Website/Index",
                SN = 1,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id
            };

          
         
            PermissionGroup pg10_pg2 = new PermissionGroup
            {
                DisplayName = "公告管理",
                Tag = "nav_website_friendlylink",
                Url = "/Website/Announcement",
                SN = 2,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id
            };
        
        

            PermissionGroup pg10_pg7 = new PermissionGroup
            {
                DisplayName = "IP黑名单",
                Tag = "nav_website_ip",
                Url = "/Website/IP",
                SN = 7,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id
            };



            pg10.Children.Add(pg10_pg0);
            pg10.Children.Add(pg10_pg2);
            pg10.Children.Add(pg10_pg7);
            #endregion

            #region 员工与部门
            PermissionGroup pg9 = new PermissionGroup
            {
                DisplayName = "员工与部门",
                Tag = "nav_platform",
                Url = "#",
                SN = 2,
                Headshot = "platform",
                Children = new List<PermissionGroup>(),
                PlatformId = sanhedai.Id
            };
            PermissionGroup pg9_pg0 = new PermissionGroup
            {
                DisplayName = "网点管理",
                Tag = "nav_platform_platform",
                Url = "/platform/index",
                SN = 1,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id
            };
            PermissionGroup pg9_pg1 = new PermissionGroup
            {
                DisplayName = "员工列表",
                Tag = "nav_platform_emplolyee",
                Url = "/Employee/Index",
                SN = 2,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id
            };
            PermissionGroup pg9_pg2 = new PermissionGroup
            {
                DisplayName = "部门管理",
                Tag = "nav_platform_organization",
                Url = "/Organization/Index",
                SN = 3,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id
            };
            PermissionGroup pg9_pg3 = new PermissionGroup
            {
                DisplayName = "IP白名单",
                Tag = "nav_platform_ip",
                Url = "/platform/IP",
                SN = 4,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id
            };
            pg9.Children.Add(pg9_pg0);
            pg9.Children.Add(pg9_pg1);
            pg9.Children.Add(pg9_pg2);
            pg9.Children.Add(pg9_pg3);
            #endregion

            #region 角色管理
            PermissionGroup pg8 = new PermissionGroup
            {
                DisplayName = "角色管理",
                Tag = "nav_role",
                Url = "#",
                SN = 3,
                Headshot = "role",
                Children = new List<PermissionGroup>(),
                PlatformId = sanhedai.Id
            };
            PermissionGroup pg8_pg0 = new PermissionGroup
            {
                DisplayName = "权限项管理",
                Tag = "nav_role_permissionline",
                Url = "/Permission/Index",
                SN = 1,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id
            };
            PermissionGroup pg8_pg1 = new PermissionGroup
            {
                DisplayName = "权限组管理",
                Tag = "nav_role_permissiongroup",
                Url = "/Permission/Group",
                SN = 2,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id
            };
            PermissionGroup pg8_pg2 = new PermissionGroup
            {
                DisplayName = "角色和权限分配",
                Tag = "nav_role_rolepermission",
                Url = "/Role/Index",
                SN = 3,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id
            };

            pg8.Children.Add(pg8_pg0);
            pg8.Children.Add(pg8_pg1);
            pg8.Children.Add(pg8_pg2);
            #endregion
            #region 工商爬虫管理
            PermissionGroup pg7 = new PermissionGroup
            {
                DisplayName = "工商数据管理",
                Tag = "nav_business",
                Url = "#",
                SN = 4,
                Headshot = "order",
                Children = new List<PermissionGroup>(),
                PlatformId = sanhedai.Id
            };
            PermissionGroup pg7_pg0 = new PermissionGroup
            {
                DisplayName = "工商数据列表",
                Tag = "nav_business_companys",
                Url = "/Business/Index",
                SN = 1,
                PermissionLines = new List<PermissionLine>(),
                PlatformId = sanhedai.Id,
                Children = new List<PermissionGroup>()
            };
        
           
            pg7.Children.Add(pg7_pg0);

            #endregion

            //#region 客户管理
            //PermissionGroup pg7 = new PermissionGroup
            //{
            //    DisplayName = "客户管理",
            //    Tag = "nav_member",
            //    Url = "#",
            //    SN = 4,
            //    Headshot = "member",
            //    Children = new List<PermissionGroup>(),
            //    PlatformId = sanhedai.Id
            //};
            //PermissionGroup pg7_pg0 = new PermissionGroup
            //{
            //    DisplayName = "客户信息管理",
            //    Tag = "nav_member_member",
            //    Url = "/Member/Index",
            //    SN = 1,
            //    PermissionLines = new List<PermissionLine>(),
            //    PlatformId = sanhedai.Id,
            //    Children = new List<PermissionGroup>()
            //};
            //PermissionGroup pg7_pg1 = new PermissionGroup
            //{
            //    DisplayName = "我的客户",
            //    Tag = "nav_member_mymember",
            //    Url = "/Member/MyMemberIndex",
            //    SN = 2,
            //    PermissionLines = new List<PermissionLine>(),
            //    PlatformId = sanhedai.Id,
            //    Children = new List<PermissionGroup>()
            //};
            //PermissionGroup pg7_pg2 = new PermissionGroup
            //{
            //    DisplayName = "认证管理",
            //    Tag = "nav_member_verify",
            //    Url = "/Member/Verify",
            //    SN = 3,
            //    PermissionLines = new List<PermissionLine>(),
            //    PlatformId = sanhedai.Id
            //};
            //PermissionGroup pg7_pg3 = new PermissionGroup
            //{
            //    DisplayName = "客服变更记录",
            //    Tag = "nav_member_servicechanged",
            //    Url = "/Member/ServiceChanged",
            //    SN = 4,
            //    PermissionLines = new List<PermissionLine>(),
            //    PlatformId = sanhedai.Id
            //};
            //PermissionGroup pg7_pg4 = new PermissionGroup
            //{
            //    DisplayName = "推荐人变更记录",
            //    Tag = "nav_member_referrerchanged",
            //    Url = "/Member/ReferrerChanged",
            //    SN = 5,
            //    PermissionLines = new List<PermissionLine>(),
            //    PlatformId = sanhedai.Id
            //};
            //PermissionGroup pg7_pg5 = new PermissionGroup
            //{
            //    DisplayName = "积分变更记录",
            //    Tag = "nav_member_scorechanged",
            //    Url = "/Member/ScoreChanged",
            //    SN = 5,
            //    PermissionLines = new List<PermissionLine>(),
            //    PlatformId = sanhedai.Id
            //};
            //pg7.Children.Add(pg7_pg0);
            //pg7.Children.Add(pg7_pg1);
            //pg7.Children.Add(pg7_pg2);
            //pg7.Children.Add(pg7_pg3);
            //pg7.Children.Add(pg7_pg4);
            //pg7.Children.Add(pg7_pg5);
            //#endregion



           // #region 案件档案

           // PermissionGroup pg6 = new PermissionGroup
           // {
           //     DisplayName = "案件档案",
           //     Tag = "nav_archive",
           //     Url = "#",
           //     SN = 5,
           //     Headshot = "archive",
           //     Children = new List<PermissionGroup>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg6_pg0 = new PermissionGroup
           // {
           //     DisplayName = "录件扫描",
           //     Tag = "nav_archive_scan",
           //     SN = 1,
           //     Url = "/Archive/Scan",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg6_pg1 = new PermissionGroup
           // {
           //     DisplayName = "邮包管理",
           //     Tag = "nav_archive_package",
           //     SN = 2,
           //     Url = "/Archive/Package",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg6_pg2 = new PermissionGroup
           // {
           //     DisplayName = "申请件整理",
           //     Tag = "nav_archive_file",
           //     SN = 3,
           //     Url = "/Archive/File",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg6_pg3 = new PermissionGroup
           // {
           //     DisplayName = "档案袋管理",
           //     Tag = "nav_archive_filepocket",
           //     SN = 4,
           //     Url = "/Archive/FilePocket",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg6_pg4 = new PermissionGroup
           // {
           //     DisplayName = "档案箱管理",
           //     SN = 5,
           //     Tag = "nav_archive_box",
           //     Url = "/Archive/Box",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg6_pg5 = new PermissionGroup
           // {
           //     DisplayName = "档案仓库管理",
           //     SN = 6,
           //     Tag = "nav_archive_repository",
           //     Url = "/Archive/Repository",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // pg6.Children.Add(pg6_pg0);
           // pg6.Children.Add(pg6_pg1);
           // pg6.Children.Add(pg6_pg2);
           // pg6.Children.Add(pg6_pg3);
           // pg6.Children.Add(pg6_pg4);
           // pg6.Children.Add(pg6_pg5);
           // #endregion

           // #region 合同管理
           // PermissionGroup pg11 = new PermissionGroup
           // {
           //     DisplayName = "合同管理",
           //     Tag = "nav_contract",
           //     Url = "#",
           //     SN = 6,
           //     Headshot = "contract",
           //     Children = new List<PermissionGroup>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg11_pg0 = new PermissionGroup
           // {
           //     DisplayName = "理财合同",
           //     Tag = "#",
           //     Url = "/Contract/Index",
           //     SN = 1,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg11_pg1 = new PermissionGroup
           // {
           //     DisplayName = "信贷合同",
           //     Tag = "#",
           //     Url = "/Contract/Borrows",
           //     SN = 2,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };

           // PermissionGroup pg11_pg2 = new PermissionGroup
           // {
           //     DisplayName = "理财应付账单",
           //     Tag = "#",
           //     Url = "/Contract/LendContractRepayments",
           //     SN = 3,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg11_pg3 = new PermissionGroup
           // {
           //     DisplayName = "信贷应收账单",
           //     Tag = "#",
           //     Url = "/Contract/BorrowContractRepayments",
           //     SN = 4,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // pg11.Children.Add(pg11_pg0);
           // pg11.Children.Add(pg11_pg1);
           // pg11.Children.Add(pg11_pg2);
           // pg11.Children.Add(pg11_pg3);
           // #endregion

           // #region 产品中心
           // PermissionGroup pg0 = new PermissionGroup
           // {
           //     DisplayName = "产品中心",
           //     Tag = "nav_product",
           //     Url = "#",
           //     SN = 7,
           //     Headshot = "product",

           //     Children = new List<PermissionGroup>(),
           //     PlatformId = sanhedai.Id
           // };

           // PermissionGroup pg0_pg0 = new PermissionGroup
           // {
           //     DisplayName = "产品设计",
           //     Tag = "nav_product_design",
           //     Url = "/Product/Index",
           //     SN = 1,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };

           // PermissionGroup pg0_pg1 = new PermissionGroup
           // {
           //     DisplayName = "产品必收材料维护",
           //     Tag = "nav_product_productfile",
           //     Url = "/ProductFile/Index",
           //     SN = 2,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg0_pg2 = new PermissionGroup
           // {
           //     DisplayName = "产品相关表单维护",
           //     Tag = "nav_product_productform",
           //     Url = "/ProductForm/Index",
           //     SN = 3,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg0_pg3 = new PermissionGroup
           // {
           //     DisplayName = "产品征信工作流配置",
           //     Tag = "nav_product_productcreditwork",
           //     SN = 4,
           //     Url = "/ProductCreditWorkflow/Index",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg0_pg4 = new PermissionGroup
           // {
           //     DisplayName = "预定义必收材料",
           //     Tag = "nav_product_file",
           //     SN = 5,
           //     Url = "/FileType/Index",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg0_pg5 = new PermissionGroup
           // {
           //     DisplayName = "预定义产品表单",
           //     Tag = "nav_product_form",
           //     Url = "/Form/Index",
           //     SN = 6,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg0_pg6 = new PermissionGroup
           // {
           //     DisplayName = "预定义征信工作流",
           //     Tag = "nav_product_creditworkflow",
           //     Url = "/CreditWorkflow/Index",
           //     SN = 7,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg0_pg7 = new PermissionGroup
           // {
           //     DisplayName = "预定义征信审查项",
           //     Tag = "nav_product_creditworkflowitem",
           //     Url = "/CreditWorkflowItem/Index",
           //     SN = 8,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // pg0.Children.Add(pg0_pg0);
           // pg0.Children.Add(pg0_pg1);
           // pg0.Children.Add(pg0_pg2);
           // pg0.Children.Add(pg0_pg3);
           // pg0.Children.Add(pg0_pg4);
           // pg0.Children.Add(pg0_pg5);
           // pg0.Children.Add(pg0_pg6);
           // pg0.Children.Add(pg0_pg7);

           // #endregion

           // #region 订单中心
           // PermissionGroup pg1 = new PermissionGroup
           // {
           //     DisplayName = "订单中心",
           //     Tag = "nav_order",
           //     Url = "#",
           //     SN = 8,
           //     Headshot = "order",
           //     Children = new List<PermissionGroup>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg1_pg0 = new PermissionGroup
           // {
           //     DisplayName = "订单管理",
           //     Tag = "nav_order_order",
           //     Url = "/Order/Index",
           //     SN = 1,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };

           // PermissionGroup pg1_pg1 = new PermissionGroup
           // {
           //     DisplayName = "二维码打印",
           //     Tag = "#",
           //     Url = "/Order/QRCode",
           //     SN = 1,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };

           // pg1.Children.Add(pg1_pg0);


           // pg1.Children.Add(pg1_pg1);

           // #endregion

           // #region 征/授信中心
           // PermissionGroup pg2 = new PermissionGroup
           //{
           //    DisplayName = "征/授信中心",
           //    Tag = "nav_credit",
           //    Url = "#",
           //    SN = 9,
           //    Headshot = "credit",
           //    Children = new List<PermissionGroup>(),
           //    PlatformId = sanhedai.Id
           //};
           // PermissionGroup pg2_pg0 = new PermissionGroup
           // {
           //     DisplayName = "征信待审案件",
           //     Tag = "nav_credit_initial",
           //     Url = "/OrderCreditWork/Index",
           //     SN = 1,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg2_pg1 = new PermissionGroup
           // {
           //     DisplayName = "征信结案案件",
           //     Tag = "nav_credit_initialclosed",
           //     SN = 2,
           //     Url = "/OrderCreditWork/InitialClosed",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg2_pg2 = new PermissionGroup
           // {
           //     DisplayName = "授信待审案件",
           //     Tag = "nav_credit_review",
           //     Url = "/OrderCreditWork/Review",
           //     SN = 3,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg2_pg3 = new PermissionGroup
           // {
           //     DisplayName = "授信结案案件",
           //     Tag = "nav_credit_reviewclosed",
           //     SN = 4,
           //     Url = "/OrderCreditWork/ReviewClosed",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // pg2.Children.Add(pg2_pg0);
           // pg2.Children.Add(pg2_pg1);
           // pg2.Children.Add(pg2_pg2);
           // pg2.Children.Add(pg2_pg3);
           // #endregion

           // #region 财富中心
           // PermissionGroup pg3 = new PermissionGroup
           // {
           //     DisplayName = "财富中心",
           //     Tag = "nav_finance",
           //     Url = "#",
           //     SN = 13,
           //     Headshot = "finance",
           //     Children = new List<PermissionGroup>(),
           //     PlatformId = sanhedai.Id
           // };


           // PermissionGroup pg3_pg3 = new PermissionGroup
           // {
           //     DisplayName = "会员资金账户管理",
           //     Tag = "nav_finance_account",
           //     Url = "/Account/Index",
           //     SN = 1,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg3_pg2 = new PermissionGroup
           // {
           //     DisplayName = "内部记账账户管理",
           //     Tag = "nav_finance_inneraccount",
           //     Url = "/Account/Inner",
           //     SN = 2,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg3_pg4 = new PermissionGroup
           // {
           //     DisplayName = "充值列表",
           //     Tag = "nav_finance_deposit",
           //     Url = "/Account/Deposit",
           //     SN = 7,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg3_pg5 = new PermissionGroup
           // {
           //     DisplayName = "提现列表",
           //     Tag = "nav_finance_withdraw",
           //     Url = "/Account/WithDrawal",
           //     SN = 8,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };

           // PermissionGroup pg3_pg6 = new PermissionGroup
           //{
           //    DisplayName = "推荐提成管理",
           //    Tag = "nav_finance_bonus",
           //    Url = "/Finance/Bonus",
           //    SN = 9,
           //    PermissionLines = new List<PermissionLine>(),
           //    PlatformId = sanhedai.Id
           //};

           // PermissionGroup pg3_pg7 = new PermissionGroup
           // {
           //     DisplayName = "标的应付管理",
           //     Tag = "#",
           //     Url = "/Finance/InvestRepayments",
           //     SN = 5,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg3_pg8 = new PermissionGroup
           // {
           //     DisplayName = "标的应收管理",
           //     Tag = "#",
           //     Url = "/Finance/BorrowRepayments",
           //     SN = 6,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg3_pg9 = new PermissionGroup
           // {
           //     DisplayName = "理财合同应付管理",
           //     Tag = "#",
           //     Url = "/Finance/LendContractRepayments",
           //     SN = 3,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg3_pg10 = new PermissionGroup
           // {
           //     DisplayName = "信贷合同应收管理",
           //     Tag = "#",
           //     Url = "/Finance/BorrowContractRepayments",
           //     SN = 4,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };


           // pg3.Children.Add(pg3_pg2);
           // pg3.Children.Add(pg3_pg3);
           // pg3.Children.Add(pg3_pg4);
           // pg3.Children.Add(pg3_pg5);
           // pg3.Children.Add(pg3_pg6);
           // pg3.Children.Add(pg3_pg7);
           // pg3.Children.Add(pg3_pg8);
           // pg3.Children.Add(pg3_pg9);
           // pg3.Children.Add(pg3_pg10);
           // #endregion

           // #region 债权匹配

           // PermissionGroup pg4 = new PermissionGroup
           // {
           //     DisplayName = "债权匹配",
           //     Tag = "nav_matching",
           //     Url = "#",
           //     SN = 14,
           //     Headshot = "matching",
           //     Children = new List<PermissionGroup>(),

           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg4_pg0 = new PermissionGroup
           // {
           //     DisplayName = "理财找债权",
           //     Tag = "nav_matching_lend",
           //     SN = 1,
           //     Url = "/Matching/LendFindBorrow",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg4_pg1 = new PermissionGroup
           // {
           //     DisplayName = "债权找理财",
           //     Tag = "nav_matching_borrow",
           //     SN = 2,
           //     Url = "/Matching/BorrowFindLend",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg4_pg2 = new PermissionGroup
           // {
           //     DisplayName = "理财标的应付账单",
           //     Tag = "#",
           //     SN = 3,
           //     Url = "/Matching/InvestRepayments",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg4_pg3 = new PermissionGroup
           // {
           //     DisplayName = "债权标的应收账单",
           //     Tag = "#",
           //     SN = 4,
           //     Url = "/Matching/BorrowRepayments",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };

           // PermissionGroup pg4_pg4 = new PermissionGroup
           // {
           //     DisplayName = "理财投标记录",
           //     Tag = "#",
           //     SN = 5,
           //     Url = "/Matching/ContractInvests",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg4_pg5 = new PermissionGroup
           // {
           //     DisplayName = "债权拆标记录",
           //     Tag = "#",
           //     SN = 6,
           //     Url = "/Matching/ContractBorrows",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // pg4.Children.Add(pg4_pg0);
           // pg4.Children.Add(pg4_pg1);
           // pg4.Children.Add(pg4_pg2);
           // pg4.Children.Add(pg4_pg3);

           // pg4.Children.Add(pg4_pg4); pg4.Children.Add(pg4_pg5);

           // #endregion

           // #region 授服中心
           // PermissionGroup pg5 = new PermissionGroup
           // {
           //     DisplayName = "授服中心",
           //     Tag = "nav_service",
           //     Url = "#",
           //     SN = 12,
           //     Headshot = "service",
           //     Children = new List<PermissionGroup>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg5_pg0 = new PermissionGroup
           // {
           //     DisplayName = "签约中心",
           //     Tag = "nav_service_sign",
           //     Url = "/Sign/Index",
           //     SN = 1,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // pg5.Children.Add(pg5_pg0);
           // #endregion

           // #region 标的管理
           // PermissionGroup pg12 = new PermissionGroup
           // {
           //     DisplayName = "标的管理",
           //     Tag = "",
           //     Url = "#",
           //     SN = 15,
           //     Headshot = "bid",
           //     Children = new List<PermissionGroup>(),
           //     PlatformId = sanhedai.Id
           // };

           // PermissionGroup pg12_pg1 = new PermissionGroup
           // {
           //     DisplayName = "发标管理",
           //     Tag = "nav_finance_borrows",
           //     Url = "/Borrow/Index",
           //     SN = 1,
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg12_pg4 = new PermissionGroup
           // {
           //     DisplayName = "投标管理",
           //     Tag = "nav_finance_invests",
           //     SN = 2,
           //     Url = "/Borrow/Invests",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg12_pg5 = new PermissionGroup
           // {
           //     DisplayName = "债权转让",
           //     Tag = "nav_finance_transfer",
           //     SN = 2,
           //     Url = "/Borrow/Transfer",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };
           // PermissionGroup pg12_pg3 = new PermissionGroup
           // {
           //     DisplayName = "标的应收账单",
           //     Tag = "nav_finance_borrowrepayment",
           //     SN = 4,
           //     Url = "/Borrow/BorrowRepayments",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };

           // PermissionGroup pg12_pg2 = new PermissionGroup
           // {
           //     DisplayName = "标的应付账单",
           //     Tag = "nav_finance_investrepayment",
           //     SN = 5,
           //     Url = "/Borrow/InvestRepayments",
           //     PermissionLines = new List<PermissionLine>(),
           //     PlatformId = sanhedai.Id
           // };


           // pg12.Children.Add(pg12_pg1);
           // pg12.Children.Add(pg12_pg4);
           // pg12.Children.Add(pg12_pg3);
           // pg12.Children.Add(pg12_pg2);
           // pg12.Children.Add(pg12_pg5);
           // #endregion


            sanhedai.PermissionGroups.Add(pg10);
            sanhedai.PermissionGroups.Add(pg9);
            sanhedai.PermissionGroups.Add(pg8);
            sanhedai.PermissionGroups.Add(pg7);
            //sanhedai.PermissionGroups.Add(pg6);
            //sanhedai.PermissionGroups.Add(pg11);
            //sanhedai.PermissionGroups.Add(pg0);
            //sanhedai.PermissionGroups.Add(pg1);
            //sanhedai.PermissionGroups.Add(pg2);
            //sanhedai.PermissionGroups.Add(pg3);
            //sanhedai.PermissionGroups.Add(pg5);
            //sanhedai.PermissionGroups.Add(pg4);
            //sanhedai.PermissionGroups.Add(pg12);
            #endregion


            #region 权限项

            #region 网站管理
            pg10_pg0.PermissionLines.Add(new PermissionLine
            {
                DisplayName = "#",
                Tag = "#",
                Url = "#"
            });
         
            pg10_pg2.PermissionLines.Add(new PermissionLine
            {
                DisplayName = "#",
                Tag = "#",
                Url = "#"
            });

         
            pg10_pg7.PermissionLines.Add(new PermissionLine
            {
                DisplayName = "#",
                Tag = "#",
                Url = "#"
            });
         
            #endregion

            #region  营业点管理
            PermissionLine pg9_pg0_pl0 = new PermissionLine
            {
                DisplayName = "添加",
                Tag = "plathform_linkbutton_add",
                Url = "/platform/Add"
            };
            PermissionLine pg9_pg0_pl1 = new PermissionLine
            {
                DisplayName = "删除",
                Tag = "plathform_linkbutton_remove",
                Url = "/platform/Remove"
            };
            PermissionLine pg9_pg0_pl2 = new PermissionLine
            {
                DisplayName = "修改",
                Tag = "#",
                Url = "/platform/Edit"
            };
            PermissionLine pg9_pg0_pl3 = new PermissionLine
            {
                DisplayName = "查看列表",
                Tag = "#",
                Url = "/platform/GetPlatforms"
            };
            PermissionLine pg9_pg0_pl4 = new PermissionLine
            {
                DisplayName = "根据ID获取网点信息",
                Tag = "#",
                Url = "/Platform/GetPlatformById"
            };
            pg9_pg0.PermissionLines.Add(pg9_pg0_pl0);
            pg9_pg0.PermissionLines.Add(pg9_pg0_pl1);
            pg9_pg0.PermissionLines.Add(pg9_pg0_pl2);
            pg9_pg0.PermissionLines.Add(pg9_pg0_pl3);
            pg9_pg0.PermissionLines.Add(pg9_pg0_pl4);
            #endregion

            #region 员工列表
            PermissionLine pg9_pg1_pl0 = new PermissionLine
            {
                DisplayName = "添加",
                Tag = "employee_linkbutton_add",
                Url = "/Employee/Add"
            };
            PermissionLine pg9_pg1_pl1 = new PermissionLine
            {
                DisplayName = "删除",
                Tag = "employee_linkbutton_delete",
                Url = "/Employee/Remove"
            };
            PermissionLine pg9_pg1_pl2 = new PermissionLine
            {
                DisplayName = "修改",
                Tag = "#",
                Url = "/Employee/Edit"
            };
            PermissionLine pg9_pg1_pl3 = new PermissionLine
            {
                DisplayName = "查看列表",
                Tag = "#",
                Url = "/Employee/GetEmployees"
            };
            PermissionLine pg9_pg1_pl4 = new PermissionLine
            {
                DisplayName = "重置密码",
                Tag = "employee_linkbutton_resetpassword",
                Url = "/Employee/ResetPassword"
            };
            PermissionLine pg9_pg1_pl5 = new PermissionLine
            {
                DisplayName = "账户状态设置",
                Tag = "#",
                Url = "/Employee/ChangeEmployeesStatus"
            };
            PermissionLine pg9_pg1_pl6 = new PermissionLine
            {
                DisplayName = "获取用户角色Combo数据",
                Tag = "#",
                Url = "/Role/GetComboData"
            };

            PermissionLine pg9_pg1_pl7 = new PermissionLine
            {
                DisplayName = "自定义输入区域",
                Tag = "#",
                Url = "/Role/GetRoleByIds"
            };
            PermissionLine pg9_pg1_pl8 = new PermissionLine
            {
                DisplayName = "移动到部门",
                Tag = "#",
                Url = "/Employee/Move"
            };
            PermissionLine pg9_pg1_pl9 = new PermissionLine
            {
                DisplayName = "获得父节点Combo数据",
                Tag = "#",
                Url = "/Organization/GetOrganizationList"
            };

            pg9_pg1.PermissionLines.Add(pg9_pg1_pl0);
            pg9_pg1.PermissionLines.Add(pg9_pg1_pl1);
            pg9_pg1.PermissionLines.Add(pg9_pg1_pl2);
            pg9_pg1.PermissionLines.Add(pg9_pg1_pl3);
            pg9_pg1.PermissionLines.Add(pg9_pg1_pl4);
            pg9_pg1.PermissionLines.Add(pg9_pg1_pl5);
            pg9_pg1.PermissionLines.Add(pg9_pg1_pl6);
            pg9_pg1.PermissionLines.Add(pg9_pg1_pl7);
            pg9_pg1.PermissionLines.Add(pg9_pg1_pl8);
            pg9_pg1.PermissionLines.Add(pg9_pg1_pl9);
            #endregion

            #region 部门管理
            PermissionLine pg9_pg2_pl0 = new PermissionLine
            {
                DisplayName = "添加",
                Tag = "#",
                Url = "/Organization/Add"
            };
            PermissionLine pg9_pg2_pl1 = new PermissionLine
            {
                DisplayName = "删除",
                Tag = "#",
                Url = "/Organization/Remove"
            };
            PermissionLine pg9_pg2_pl2 = new PermissionLine
            {
                DisplayName = "修改",
                Tag = "#",
                Url = "/Organization/Edit"
            };
            PermissionLine pg9_pg2_pl3 = new PermissionLine
            {
                DisplayName = "查看列表",
                Tag = "#",
                Url = "/Organization/GetOrganizationList"
            };
            PermissionLine pg9_pg2_pl4 = new PermissionLine
            {
                DisplayName = "部门转移",
                Tag = "#",
                Url = "/Organization/Move"
            };
            pg9_pg2.PermissionLines.Add(pg9_pg2_pl0);
            pg9_pg2.PermissionLines.Add(pg9_pg2_pl1);
            pg9_pg2.PermissionLines.Add(pg9_pg2_pl2);
            pg9_pg2.PermissionLines.Add(pg9_pg2_pl3);
            pg9_pg2.PermissionLines.Add(pg9_pg2_pl4);
            #endregion

            #region 网点IP白名单
            pg9_pg3.PermissionLines.Add(new PermissionLine
            {
                DisplayName = "#",
                Tag = "#",
                Url = "#"
            });
            #endregion

            #region 工商数据管理
            pg7_pg0.PermissionLines.Add(new PermissionLine
            {
                DisplayName = "#",
                Tag = "#",
                Url = "#"
            });
            pg7_pg0.PermissionLines.Add(new PermissionLine
            {
                DisplayName = "#",
                Tag = "#",
                Url = "/Business/GetCompanysList"
            });
    
            #endregion

            #region 权限项管理
            PermissionLine pg8_pg0_pl0 = new PermissionLine
            {
                DisplayName = "添加",
                Tag = "permissionline_linkbutton_add",
                Url = "/permission/Addpermissionline"
            };
            PermissionLine pg8_pg0_pl1 = new PermissionLine
            {
                DisplayName = "删除",
                Tag = "permissionline_linkbutton_delete",
                Url = "/permission/removepermissionlines"
            };
            PermissionLine pg8_pg0_pl2 = new PermissionLine
            {
                DisplayName = "修改",
                Tag = "#",
                Url = "/permission/editpermissionline"
            };
            PermissionLine pg8_pg0_pl3 = new PermissionLine
            {
                DisplayName = "查看列表",
                Tag = "#",
                Url = "/permission/getpermissionlines"
            };
            PermissionLine pg8_pg0_pl4 = new PermissionLine
            {
                DisplayName = "权限组Combo数据",
                Tag = "#",
                Url = "/permission/GetPermissionGroups"
            };
            PermissionLine pg8_pg0_pl5 = new PermissionLine
            {
                DisplayName = "批量转移权限组",
                Tag = "#",
                Url = "/permission/MovePermissionLines"
            };

            pg8_pg0.PermissionLines.Add(pg8_pg0_pl0);
            pg8_pg0.PermissionLines.Add(pg8_pg0_pl1);
            pg8_pg0.PermissionLines.Add(pg8_pg0_pl2);
            pg8_pg0.PermissionLines.Add(pg8_pg0_pl3);
            pg8_pg0.PermissionLines.Add(pg8_pg0_pl4);
            pg8_pg0.PermissionLines.Add(pg8_pg0_pl5);
            #endregion

            #region 权限组管理
            PermissionLine pg8_pg1_pl0 = new PermissionLine
            {
                DisplayName = "添加",
                Tag = "permissionline_linkbutton_add",
                Url = "/Permission/AddPermissionGroup"
            };
            PermissionLine pg8_pg1_pl1 = new PermissionLine
            {
                DisplayName = "删除",
                Tag = "permissionline_linkbutton_delete",
                Url = "/Permission/RemovePermissionGroups"
            };
            PermissionLine pg8_pg1_pl2 = new PermissionLine
            {
                DisplayName = "修改",
                Tag = "#",
                Url = "/permission/EditPermissionGroup"
            };
            PermissionLine pg8_pg1_pl3 = new PermissionLine
            {
                DisplayName = "查看列表",
                Tag = "#",
                Url = "/permission/getpermissiongroups"
            };
            PermissionLine pg8_pg1_pl4 = new PermissionLine
            {
                DisplayName = "转移项",
                Tag = "#",
                Url = "/Permission/MovePermissionGroups"
            };


            pg8_pg1.PermissionLines.Add(pg8_pg1_pl0);
            pg8_pg1.PermissionLines.Add(pg8_pg1_pl1);
            pg8_pg1.PermissionLines.Add(pg8_pg1_pl2);
            pg8_pg1.PermissionLines.Add(pg8_pg1_pl3);
            pg8_pg1.PermissionLines.Add(pg8_pg1_pl4);
            #endregion

            #region  权限分配管理
            pg8_pg2.PermissionLines.Add(new PermissionLine
            {
                DisplayName = "添加角色",
                Tag = "#",
                Url = "/Role/Add"
            });
            pg8_pg2.PermissionLines.Add(new PermissionLine
            {
                DisplayName = "编辑角色",
                Tag = "#",
                Url = "/Role/edit"
            });
            //pg8_pg2.PermissionLines.Add(new PermissionLine
            //{
            //    DisplayName = "编辑角色",
            //    Tag = "#",
            //    Url = "/Role/Add"
            //});
            pg8_pg2.PermissionLines.Add(new PermissionLine
           {
               DisplayName = "角色列表",
               Tag = "#",
               Url = "/Role/GetRoles"
           });
            pg8_pg2.PermissionLines.Add(new PermissionLine
           {
               DisplayName = "权限树",
               Tag = "#",
               Url = "/permission/GetPermissionLinesTree"
           });

            #endregion

            #endregion

            #region 公告
            sanhedai.Announcements = new List<Announcement>();
            sanhedai.Announcements.Add(new Announcement
            {
                Content = "<span id=\"center_spanContents\"><p>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<b style=\"line-height: 200%;\"><span style=\"font-size:12.0pt;line-height:200%;font-family:宋体\">xx公司为了答谢广大客户的支持和拥护，借双“十二”期间平台再次升级，举办“感谢有你”投资有礼大回馈活动，届时欢迎广大新老用户登陆xx公司体验新平台新功能（自动投标，债权转让等），<span lang=\"EN-US\">12</span>月<span lang=\"EN-US\">12</span>日当天投资好收益又有丰厚的礼品赠送。</span></b></p> <p style=\"line-height:200%\" class=\"MsoNormal\"><b><span style=\"font-size:12.0pt;line-height:200%;font-family:宋体\">本次活动奖励规则：</span></b><span lang=\"EN-US\" style=\"font-size:12.0pt;line-height:200%;font-family:宋体;mso-bidi-font-family: 宋体;mso-font-kerning:0pt\"><br> </span><b><span style=\"font-size:12.0pt; line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:0pt\">核算金额：</span></b><span style=\"font-size:12.0pt;line-height:200%;font-family:宋体;mso-bidi-font-family: 宋体;mso-font-kerning:0pt\">投标金额<span lang=\"EN-US\">*</span>借款标的期限月数<span lang=\"EN-US\">=</span>核算金额<span lang=\"EN-US\"><br> </span>如：投标<span lang=\"EN-US\">10</span>万元投<span lang=\"EN-US\">3</span>个月期标的，核算金额为<span lang=\"EN-US\">30</span>万元；<span lang=\"EN-US\"><br> &nbsp;&nbsp;&nbsp; </span>投标<span lang=\"EN-US\">10</span>万元投<span lang=\"EN-US\">6</span>个月期标的，核算金额为<span lang=\"EN-US\">60</span>万元；<span lang=\"EN-US\"><br> &nbsp;&nbsp;&nbsp; </span>投标<span lang=\"EN-US\">10</span>万元投<span lang=\"EN-US\">12</span>个月期标的，核算金额为<span lang=\"EN-US\">120</span>万元；</span><b><span lang=\"EN-US\" style=\"font-size:12.0pt; line-height:200%;font-family:宋体\"><o:p></o:p></span></b></p> <p style=\"line-height:200%\" class=\"MsoNormal\"><b><span lang=\"EN-US\" style=\"font-size:12.0pt;line-height:200%;font-family: 宋体;mso-bidi-font-family:宋体;mso-font-kerning:0pt\">*</span></b><b><span style=\"font-size:12.0pt;line-height: 200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:0pt\">活动当日所有标的成功完成投标的金额可以累加核算</span></b></p> <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"font-size:12.0pt;line-height:200%;font-family:宋体;mso-bidi-font-family: 宋体;mso-font-kerning:0pt\">如：账户内有<span lang=\"EN-US\">10</span>万元，其中<span lang=\"EN-US\">2</span>万元投了<span lang=\"EN-US\">3</span>个月的标的，<span lang=\"EN-US\">3</span>万元投了<span lang=\"EN-US\">6</span>个月的标的，</span></p> <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"font-size:12.0pt;line-height:200%;font-family:宋体;mso-bidi-font-family: 宋体;mso-font-kerning:0pt\"><span lang=\"EN-US\">&nbsp; &nbsp; 5</span>万元投了<span lang=\"EN-US\">12</span>个月的标的，</span><span style=\"font-size:12.0pt;line-height:200%;font-family:宋体;mso-bidi-font-family: 宋体;mso-font-kerning:0pt\">则核算金额为<span lang=\"EN-US\">2*3+3*6+5*12=84</span>万元。<span lang=\"EN-US\"><o:p></o:p></span></span></p> <p style=\"line-height:200%\" class=\"MsoNormal\"><b><span style=\"font-size:12.0pt;line-height:200%;font-family:宋体; mso-bidi-font-family:宋体;mso-font-kerning:0pt\">赠送礼品设置如下</span></b><span style=\"font-size:12.0pt;line-height:200%;font-family:宋体;mso-bidi-font-family: 宋体;mso-font-kerning:0pt\">：<span lang=\"EN-US\"><o:p></o:p></span></span></p> <table cellspacing=\"0\" cellpadding=\"0\" border=\"1\" width=\"593\" style=\"width:444.85pt;border-collapse:collapse;border:none;mso-border-alt:     solid windowtext .5pt;mso-yfti-tbllook:1184;mso-padding-alt:0cm 5.4pt 0cm 5.4pt;     mso-border-insideh:.5pt solid windowtext;mso-border-insidev:.5pt solid windowtext\" class=\"MsoNormalTable\">     <tbody>         <tr>             <td width=\"187\" valign=\"top\" style=\"width:140.1pt;border:solid windowtext 1.0pt;             mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"mso-bidi-font-size:             10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">核算金额（元）<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>             <td width=\"406\" valign=\"top\" style=\"width:304.75pt;border:solid windowtext 1.0pt;             border-left:none;mso-border-left-alt:solid windowtext .5pt;mso-border-alt:             solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"mso-bidi-font-size:             10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">礼品<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>         </tr>         <tr>             <td width=\"187\" valign=\"top\" style=\"width:140.1pt;border:solid windowtext 1.0pt;             border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;             padding:0cm 5.4pt 0cm 5.4pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">5</span><span style=\"mso-bidi-font-size:10.5pt;             line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">万以下<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>             <td width=\"406\" valign=\"top\" style=\"width:304.75pt;border-top:none;border-left:             none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;             mso-border-top-alt:solid windowtext .5pt;mso-border-left-alt:solid windowtext .5pt;             mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"mso-bidi-font-size:             10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">送xx公司精美台历<span lang=\"EN-US\">1</span>本<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>         </tr>         <tr style=\"mso-yfti-irow:2;height:26.25pt\">             <td width=\"187\" valign=\"top\" style=\"width:140.1pt;border:solid windowtext 1.0pt;             border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;             padding:0cm 5.4pt 0cm 5.4pt;height:26.25pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">5</span><span style=\"mso-bidi-font-size:10.5pt;             line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">万<span lang=\"EN-US\">---10</span>万（含）<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>             <td width=\"406\" valign=\"top\" style=\"width:304.75pt;border-top:none;border-left:             none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;             mso-border-top-alt:solid windowtext .5pt;mso-border-left-alt:solid windowtext .5pt;             mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;height:26.25pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"mso-bidi-font-size:             10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">高级保温杯<strong>或</strong>价值<span lang=\"EN-US\">100</span>元手机话费<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>         </tr>         <tr style=\"mso-yfti-irow:3;height:7.95pt\">             <td width=\"187\" valign=\"top\" style=\"width:140.1pt;border:solid windowtext 1.0pt;             border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;             padding:0cm 5.4pt 0cm 5.4pt;height:7.95pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">10</span><span style=\"mso-bidi-font-size:10.5pt;             line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">万<span lang=\"EN-US\">---25</span>万<span lang=\"EN-US\"><o:p></o:p></span></span><span style=\"font-family: 宋体;\">（含）</span></p>             </td>             <td width=\"406\" valign=\"top\" style=\"width:304.75pt;border-top:none;border-left:             none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;             mso-border-top-alt:solid windowtext .5pt;mso-border-left-alt:solid windowtext .5pt;             mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;height:7.95pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"mso-bidi-font-size:             10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">苏泊尔电磁炉<span lang=\"EN-US\">1</span>台<strong>或</strong>价值<span lang=\"EN-US\">300</span>元超市购物卡<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>         </tr>         <tr style=\"mso-yfti-irow:4;height:22.5pt\">             <td width=\"187\" valign=\"top\" style=\"width:140.1pt;border:solid windowtext 1.0pt;             border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;             padding:0cm 5.4pt 0cm 5.4pt;height:22.5pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">25</span><span style=\"mso-bidi-font-size:10.5pt;             line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">万<span lang=\"EN-US\">---50</span>万（含）<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>             <td width=\"406\" valign=\"top\" style=\"width:304.75pt;border-top:none;border-left:             none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;             mso-border-top-alt:solid windowtext .5pt;mso-border-left-alt:solid windowtext .5pt;             mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;height:22.5pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"mso-bidi-font-size:             10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">高级床上用品<span lang=\"EN-US\">1</span>套<strong>或</strong>价值<span lang=\"EN-US\">500</span>元超市购物卡<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>         </tr>         <tr style=\"mso-yfti-irow:5;height:13.2pt\">             <td width=\"187\" valign=\"top\" style=\"width:140.1pt;border:solid windowtext 1.0pt;             border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;             padding:0cm 5.4pt 0cm 5.4pt;height:13.2pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">50</span><span style=\"mso-bidi-font-size:10.5pt;             line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">万<span lang=\"EN-US\">---100</span>万（含）<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>             <td width=\"406\" valign=\"top\" style=\"width:304.75pt;border-top:none;border-left:             none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;             mso-border-top-alt:solid windowtext .5pt;mso-border-left-alt:solid windowtext .5pt;             mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;height:13.2pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"mso-bidi-font-size:             10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">海尔两厢冰箱<span lang=\"EN-US\">1</span>部<strong>或</strong>价值<span lang=\"EN-US\">1000</span>元超市购物卡<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>         </tr>         <tr style=\"mso-yfti-irow:6;height:14.7pt\">             <td width=\"187\" valign=\"top\" style=\"width:140.1pt;border:solid windowtext 1.0pt;             border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;             padding:0cm 5.4pt 0cm 5.4pt;height:14.7pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">100</span><span style=\"mso-bidi-font-size:10.5pt;             line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">万<span lang=\"EN-US\">---200</span>万（含）<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>             <td width=\"406\" valign=\"top\" style=\"width:304.75pt;border-top:none;border-left:             none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;             mso-border-top-alt:solid windowtext .5pt;mso-border-left-alt:solid windowtext .5pt;             mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;height:14.7pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"mso-bidi-font-size:             10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">小米<span lang=\"EN-US\">4</span>手机<span lang=\"EN-US\">1</span>部<strong>或</strong>价值<span lang=\"EN-US\">2000</span>元超市购物卡<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>         </tr>         <tr style=\"mso-yfti-irow:7;height:13.95pt\">             <td width=\"187\" valign=\"top\" style=\"width:140.1pt;border:solid windowtext 1.0pt;             border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;             padding:0cm 5.4pt 0cm 5.4pt;height:13.95pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">200</span><span style=\"mso-bidi-font-size:10.5pt;             line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">万<span lang=\"EN-US\">---300</span>万（含）<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>             <td width=\"406\" valign=\"top\" style=\"width:304.75pt;border-top:none;border-left:             none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;             mso-border-top-alt:solid windowtext .5pt;mso-border-left-alt:solid windowtext .5pt;             mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;height:13.95pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">Apple ipad4</span><span style=\"mso-bidi-font-size:             10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">平板电脑<span lang=\"EN-US\">1</span>部<strong>或</strong>价值<span lang=\"EN-US\">3000</span>元超市购物卡<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>         </tr>         <tr style=\"mso-yfti-irow:8;height:16.5pt\">             <td width=\"187\" valign=\"top\" style=\"width:140.1pt;border:solid windowtext 1.0pt;             border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;             padding:0cm 5.4pt 0cm 5.4pt;height:16.5pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">300</span><span style=\"mso-bidi-font-size:10.5pt;             line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">万<span lang=\"EN-US\">---400</span>万（含）<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>             <td width=\"406\" valign=\"top\" style=\"width:304.75pt;border-top:none;border-left:             none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;             mso-border-top-alt:solid windowtext .5pt;mso-border-left-alt:solid windowtext .5pt;             mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;height:16.5pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">iPhone6</span><span style=\"mso-bidi-font-size:10.5pt;             line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">（或<span lang=\"EN-US\">plus</span>）一部<strong>或</strong>价值<span lang=\"EN-US\">5000</span>元超市购物卡<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>         </tr>         <tr style=\"mso-yfti-irow:9;mso-yfti-lastrow:yes;height:15.75pt\">             <td width=\"187\" valign=\"top\" style=\"width:140.1pt;border:solid windowtext 1.0pt;             border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;             padding:0cm 5.4pt 0cm 5.4pt;height:15.75pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span lang=\"EN-US\" style=\"mso-bidi-font-size:10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:             宋体;mso-font-kerning:0pt\">400</span><span style=\"mso-bidi-font-size:10.5pt;             line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">万以上<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>             <td width=\"406\" valign=\"top\" style=\"width:304.75pt;border-top:none;border-left:             none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;             mso-border-top-alt:solid windowtext .5pt;mso-border-left-alt:solid windowtext .5pt;             mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;height:15.75pt\">             <p style=\"line-height:200%\" class=\"MsoNormal\"><span style=\"mso-bidi-font-size:             10.5pt;line-height:200%;font-family:宋体;mso-bidi-font-family:宋体;mso-font-kerning:             0pt\">佳能<span lang=\"EN-US\">EOS70D</span>单反相机一部<strong>或</strong>价值<span lang=\"EN-US\">8000</span>元超市购物卡<span lang=\"EN-US\"><o:p></o:p></span></span></p>             </td>         </tr>     </tbody> </table> <p style=\"line-height:200%\" class=\"MsoNormal\"><b><span style=\"font-size:12.0pt;line-height:200%;font-family:宋体; mso-bidi-font-family:宋体;mso-font-kerning:0pt\">xx公司根据客户投标情况核算出相应的金额，再对应到以上相应的礼品供客户选择。</span></b></p> <p class=\"MsoNormal\"><b><span style=\"font-size:12.0pt;font-family:宋体\">以上奖品只能单选一项，不能累加选择，本活动最终解释权归汇方（xx）金融信息服务有限公司所有。</span></b></p> <p class=\"MsoNormal\"><b><span style=\"font-size:12.0pt;font-family:宋体\">详情咨询</span></b>：<b><span style=\"font-size:12.0pt;font-family:宋体\">客服电话<span lang=\"EN-US\">4008-52-0594 &nbsp;&nbsp;</span></span></b><b><span style=\"font-size:12.0pt;font-family:宋体\">林小姐：0594-6731905 &nbsp; &nbsp;</span></b><b><span style=\"font-size:12.0pt;font-family:宋体\">许先生：0594-6731903 &nbsp; 张小姐:0594-6731904</span></b></p></span>",
                Title = "双十二“感谢有你”投资有礼大回馈活动",
                IsPublished = true,
                CreateTime = Convert.ToDateTime("2014-12-08 12:01:01.000"),
                EmployeeId = emp0.Id,
                PlatformId = sanhedai.Id,
                PublishedTime = Convert.ToDateTime("2014-12-08 12:01:01.000")
            });
         
            #endregion


            context.SaveChanges();
            role0.RolePermissions = new List<RolePermission>();
            foreach (var line in context.PermissionLines.ToList())
            {
                context.RolePermissions.Add(new RolePermission { PermissionLineId = line.Id, RoleId = role0.Id });
            }
            context.SaveChanges();



            //#region Business业务
            //CompanyInfo cominfo0 = new CompanyInfo()
            //{
            //    CompanyName = "中科软科技股份有限公司",
            //    CompanyNo = "110000005074238",
            //    RegisteredArea = "110000",
            //    Listed = "三板",
            //    StockCode = "430002",
            //    StockName = "中科软",
            //    NextTime = DateTime.Now,
            //    AddTime = DateTime.Now,
            //    State = 1
            //};
            //CompanyInfo cominfo1 = new CompanyInfo()
            //{
            //    CompanyName = "成都页游科技股份有限公司",
            //    CompanyNo = "510109000136214",
            //    RegisteredArea = "510000",
            //    Listed = "三板",
            //    StockCode = "430627",
            //    StockName = "页游科技",
            //    NextTime = DateTime.Now,
            //    AddTime = DateTime.Now,
            //    State = 1
            //};
            //CompanyInfo cominfo2 = new CompanyInfo()
            //{
            //    CompanyName = "上海弘陆物流设备股份有限公司",
            //    CompanyNo = "310114001822361",
            //    RegisteredArea = "310000",
            //    Listed = "三板",
            //    StockCode = "430584",
            //    StockName = "弘陆股份",
            //    NextTime = DateTime.Now,
            //    AddTime = DateTime.Now,
            //    State = 0
            //};
            //context.CompanyInfos.Add(cominfo0);
            //context.CompanyInfos.Add(cominfo1);
            //context.CompanyInfos.Add(cominfo2);
            //#endregion
            //context.SaveChanges();

        }

      
    }

}
