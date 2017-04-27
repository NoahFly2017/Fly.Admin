
$.extend($.fn.validatebox.defaults.rules, {
    minLength: { // 判断最小长度
        validator: function (value, param) {
            return value.length >= param[0];
        },
        message: '最少输入 {0} 个字符。'
    },
    length: {
        validator: function (value, param) {
            var len = $.trim(value).length;
            return len >= param[0] && len <= param[1];
        },
        message: "输入内容长度必须介于{0}和{1}之间."
    },
    phone: {// 验证电话号码
        validator: function (value) {
            return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
        },
        message: '格式不正确,请使用下面格式:020-88888888'
    },
    mobile: {// 验证手机号码
        validator: function (value) {
            return /^(13|15|18)\d{9}$/i.test(value);
        },
        message: '手机号码格式不正确'
    },
    idcard: {// 验证身份证
        validator: function (value) {
            return /^\d{15}(\d{2}[A-Za-z0-9])?$/i.test(value);
        },
        message: '身份证号码格式不正确'
    },
    intOrFloat: {// 验证整数或小数
        validator: function (value) {
            return /^\d+(\.\d+)?$/i.test(value);
        },
        message: '请输入数字，并确保格式正确'
    },
    currency: {// 验证货币
        validator: function (value) {
            return /^\d+(\.\d+)?$/i.test(value);
        },
        message: '货币格式不正确'
    },
    qq: {// 验证QQ,从10000开始
        validator: function (value) {
            return /^[1-9]\d{4,9}$/i.test(value);
        },
        message: 'QQ号码格式不正确'
    },
    integer: {// 验证整数
        validator: function (value) {
            return /^[+]?[1-9]+\d*$/i.test(value);
        },
        message: '请输入整数'
    },
    chinese: {// 验证中文
        validator: function (value) {
            return /^[\u0391-\uFFE5]+$/i.test(value);
        },
        message: '请输入中文'
    },
    english: {// 验证英语
        validator: function (value) {
            return /^[A-Za-z]+$/i.test(value);
        },
        message: '请输入英文'
    },
    unnormal: {// 验证是否包含空格和非法字符
        validator: function (value) {
            return /.+/i.test(value);
        },
        message: '输入值不能为空和包含其他非法字符'
    },
    username: {// 验证用户名
        validator: function (value) {
            return /^[a-zA-Z][a-zA-Z0-9_]{2,15}$/i.test(value);
        },
        message: '用户名不合法（字母开头，允许3-16字节，允许字母数字下划线）'
    },
    faxno: {// 验证传真
        validator: function (value) {
            //            return /^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$/i.test(value);
            return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
        },
        message: '传真号码不正确'
    },
    zip: {// 验证邮政编码
        validator: function (value) {
            return /^[1-9]\d{5}$/i.test(value);
        },
        message: '邮政编码格式不正确'
    },
    ip: {// 验证IP地址
        validator: function (value) {
            return /d+.d+.d+.d+/i.test(value);
        },
        message: 'IP地址格式不正确'
    },
    name: {// 验证姓名，可以是中文或英文
        validator: function (value) {
            return /^[\u0391-\uFFE5]+$/i.test(value) | /^\w+[\w\s]+\w+$/i.test(value);
        },
        message: '请输入姓名'
    },
    carNo: {
        validator: function (value) {
            return /^[\u4E00-\u9FA5][\da-zA-Z]{6}$/.test(value);
        },
        message: '车牌号码无效（例：粤J12350）'
    },
    carenergin: {
        validator: function (value) {
            return /^[a-zA-Z0-9]{16}$/.test(value);
        },
        message: '发动机型号无效(例：FG6H012345654584)'
    },
    email: {
        validator: function (value) {
            return /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(value);
        },
        message: '请输入有效的电子邮件账号(例：abc@126.com)'
    },
    msn: {
        validator: function (value) {
            return /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(value);
        },
        message: '请输入有效的msn账号(例：abc@hotnail(msn/live).com)'
    }, same: {
        validator: function (value, param) {
            if ($("#" + param[0]).val() != "" && value != "") {
                return $("#" + param[0]).val() == value;
            } else {
                return true;
            }
        },
        message: '两次输入的密码不一致！'
    },
    lessthan: {
        validator: function (value, param) {
            if ($("#" + param[0]).val() != "" && value != "" && $.isNumeric($("#" + param[0]).val()) && $.isNumeric(value)) {
                return ((parseFloat($("#" + param[0]).val()) - parseFloat(value)) >= 0 || Math.abs(parseFloat($("#" + param[0]).val()) - parseFloat(value))<0.005);
            } else {
                return false;
            }
        },
        message: '输入的数值过大请调整。'
    },
    password: {
        validator: function (value, param) {
            var result = false;
            if (value.length > 5) {
                result = true;
            }
            return result;
        },
        message: '密码至少6位'
    },
    checkuser: {
        validator: function (value, param) {
            var result = false;
            $.ajax({
                type: "POST",
                url: "/Employee/ValidateLoginName",
                cache: false,
                async: false,
                data: {
                    "UserName": value
                },
                dataType: "json",
                success: function (data) {
                    result = data
                }
            })
            return result;
        },
        message: '您输入的用户名重复，请重新输入'
    },
    number: {
        validator: function (value, param) { return /^\d+$/.test(value); },
        message: '请输入数字'
    },
    checkchannelid: {
        validator: function (value, param) {
            var result = false;
            $.ajax({
                type: "POST",
                url: "/admin/service/channel/checkchannelid.aspx",
                cache: false,
                async: false,
                data: {
                    "ChannelID": value
                },
                dataType: "text",
                success: function (data) {
                    if (data == "true") {
                        result = true;
                    } else if (data == "false") {
                        result = false;
                    }
                }
            })
            return result;
        },
        message: '您输入的频道ID重复，请重新输入'
    },
    checkchanneltypeattrunique: {
        validator: function (value, param) {
            var result = false;
            $.ajax({
                type: "POST",
                url: "/admin/service/channeltype/checkchanneltypeattrunique.aspx",
                cache: false,
                async: false,
                data: {
                    "AttrUnique": value
                },
                dataType: "text",
                success: function (data) {
                    if (data == "true") {
                        result = true;
                    } else if (data == "false") {
                        result = false;
                    }
                }
            })
            return result;
        },
        message: '您输入的唯一标识重复，请重新输入'
    },
    identityproduct: {
        validator: function (value, param) {
            var result = false;
            $.ajax({
                type: "POST",
                url: "/Product/CheckIdentityString",
                cache: false,
                async: false,
                data: {
                    "identitystring": value
                },
                dataType: "json",
                success: function (data) {
                    result = data;
                }
            })
            return result;
        },
        message: '您输入的产品唯一标识重复，请重新输入'
    },
    checkmark: {// 验证标识
        validator: function (value) {
            return /^[a-zA-Z0-9_("*)]{2,15}$/i.test(value);
        },
        message: '该标识不合法（允许2-15字节，允许字母数字下划线）'
    },
    domain: {
        validator: function (value) {
            return /^([a-z0-9][a-z0-9\-]*?\.(?:com|cn|net|org|gov|info|la|cc|co)(?:\.(?:cn|jp))?)$/.test(value);
        },
        message: '请输入正确的域名'
    },
    customattribute: {
        validator: function (value) {
            return /^([^`|]*|([^`]+\|[^`]+)*)$/.test(value);
        },
        message: '请输入正确的自定义属性，多个自定义属性之间以“|”分开，属性名不能包含英文符号“`”。'
    },
    ipvfour: {
        validator: function (value) {
            return /^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$/.test(value);
        },
        message: 'IPv4地址有误'
    },
    ipvsix: {
        validator: function (value) {
            try {
                var isIpv6 = false;
                if (value.match(/:/g).length <= 7 && /::/.test(value) ? /^([\da-f]{1,4}(:|::)){1,6}[\da-f]{1,4}$/i.test(value) : /^([\da-f]{1,4}:){7}[\da-f]{1,4}$/i.test(value)) {
                    isIpv6 = true;
                }
                return isIpv6;
            } catch (e) {
                return false;
            }
         
        },
        message: 'IPv6地址有误'
    },
    checkprevious: {
        validator: function (value, param) {
            var value = $(param[0]).combobox('getValue');
            var id = $(param[1]).val();
            if (value == id) {
                return false
            } else {
                return true
            }

        },
        message: '依赖环节不能是自身'
    },
    memberloginname: {
        validator: function (value) {
            var result;
            $.ajax({
                url: '/Member/CheckMemberLoginName',
                data: { lgn: value },
                async: false,
                dataType: 'json',
                type: 'post',
                success: function (data) {
                    result = data;
                }
            })
            return result;
        }, message: '客户登陆名已注册，请更换其他登录名。'
    },
    memberidcard: {
        validator: function (value, params) {
            var result;
            var memberType = $(params[0]).combobox('getValue');
            $.ajax({
                url: '/Member/CheckMemberIDCard',
                data: { idcard: value, memberType: memberType },
                async: false,
                dataType: 'json',
                type: 'post',
                success: function (data) {
                    result = data;
                }
            })
            return result;
        }, message: '客户身份证已注册，请更换其他身份证。'
    },
    membermobile: {
        validator: function (value) {
            var result;
            $.ajax({
                url: '/Member/CheckMemberMobile',
                data: { mobile: value },
                async: false,
                dataType: 'json',
                type: 'post',
                success: function (data) {
                    result = data;
                }
            })
            return result;
        }, message: '客户手机号已注册，请更换其他手机号。'
    },
    date: {
        validator: function (value) {
            return /^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$/.test(value);
        }, message: '请输入正确的日期,如“2014-12-10”。'
    },
    month: {
        validator: function (value) {
            return /^(((1[6-9]|[2-9]\d)\d{2})-(0?[123456789]|1[012]))$/.test(value);
        }, message: '请输入正确的年月,如“2014-12”。'
    },
    telormobile: {
        validator: function (value) {
            return /^((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)/.test(value);
        }, message: '请输入固定电话或手机号码'
    },
    owners: {
        validator: function (value) {
            return /^([\u0391-\uFFE5]*|([\u0391-\uFFE5]+\|[\u0391-\uFFE5]+)*)$/.test(value);

        },
        message: '请输入中文姓名，如有多个人，之间以“|”分开。'
    },
    nospace: {
        validator: function (value) {
            return !/ /.test(value);
        },
        message: '请勿输入空格'
    },
    bankcard: {
        validator: function (value) {
            return /^[0-9]{15,19}$/.test(value);

        },
        message: '请输入正确的银行卡号。'
    }

});
