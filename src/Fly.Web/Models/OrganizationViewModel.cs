using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fly.Web.Models
{
    public class OrganizationViewModel
    {
        public object Id { get; set; }

        /// <summary>
        ///     组织架构的友好名称。
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        ///     父级组织架构，顶级为-1/0/NULL。

        public Guid? ParentId { get; set; }


        /// <summary>
        /// EasyUI用于构建树形的父节点标记，属性名固定且区分大小写，勿改动
        /// </summary>

        public Guid? _parentId { get; set; }

    }
}