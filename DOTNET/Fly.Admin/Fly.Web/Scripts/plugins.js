

/* 平台的一些脚本扩展，因为用到jquery和easyui，请放在两者折后引入 */
(function ($) {
    /* 
    *String 格式化: 
    * String.Format("{0} is {1}","Tom","a man")
    * 输出："Tom is a man"
    */
    String.Format = function () {
        var args = arguments;
        return arguments[0].replace(/\{(\d+)\}/g,
            function (m, i) {
                return args[parseInt(i) + 1];
            });
    }


    /*数组是否包含指定元素*/
    Array.prototype.contains = function (obj) {
        var i = this.length;
        while (i--) {
            if (this[i] === obj) {
                return true;
            }
        }
        return false;
    }

    /*将表单转化为object对象*/
    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

    /* 对Date的扩展，将 Date 转化为指定格式的String   
    * 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符，   
    * 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)   
    * 例子：   
    * (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423   
    * (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18   */
    Date.prototype.Format = function (fmt) { //author: meizz   
        var o = {
            "M+": this.getMonth() + 1,                 //月份   
            "d+": this.getDate(),                    //日   
            "h+": this.getHours(),                   //小时   
            "m+": this.getMinutes(),                 //分   
            "s+": this.getSeconds(),                 //秒   
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
            "S": this.getMilliseconds()             //毫秒   
        };
        if (/(y+)/.test(fmt))
            fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt))
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    }


    /* 对Date对象的属性进行扩展，加或减月份
    * 逢日超过月份最大日的，日取结果月份最大日
    * new Date('2015/1/31 10:10:10').AddMonth(1)  ==>  2015/2/28 10:10:10
    * new Date('2015/1/31 10:10:10').AddMonth(-10)  ==>  2014/3/31 10:10:10
    */
    Date.prototype.AddMonth = function (intMonth) {
        var currDate = this;
        var currDay = currDate.getDate();
        var currMonth = currDate.getMonth() + 1;
        var currYear = currDate.getFullYear();
        var ModMonth = currMonth + intMonth;
        if (Math.abs(ModMonth) > 12) {
            currYear += ModMonth / 12;
        }
        if (ModMonth > 0) {
            currMonth = ModMonth % 12;
        } else {
            currMonth = 12 - (Math.abs(ModMonth) % 12);
        }
        var lDate = lastDate(currYear, currMonth);
        if (currDay > lDate) {
            currDay = lDate;
        }
        return new Date(currYear, currMonth - 1, currDay, this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
        function lastDate(year, month) {
            if ([1, 3, 5, 7, 8, 10, 12].contains(month)) {
                return 31;
            } else {
                if (month == 2) {
                    if ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0) {
                        return 29;
                    } else {
                        return 28;
                    }
                } else {
                    return 30;
                }
            }
        }


    }

    $.extend({
        isEmpty: function (val) {
            return (val == null || val == '' || val == undefined)
        },
        fixDateNumber: function (val) {
            if (val < 10) { return "0" + val } else { return val; }
        },
        updateTimeLabel: function () {
            var week = { 0: '星期一', 1: '星期二', 2: '星期三', 3: '星期四', 4: '星期五', 5: '星期六', 6: '星期日' }
            var date = new Date();
            var y = date.getFullYear();
            var mo = $.fixDateNumber(date.getMonth() + 1);
            var d = $.fixDateNumber(date.getDate());
            var w = week[date.getDay()];
            var h = $.fixDateNumber(date.getHours());
            var mi = $.fixDateNumber(date.getMinutes());
            var s = $.fixDateNumber(date.getSeconds());
            $('.time').text(String.Format("{0}/{1}/{2} {3} {4}:{5}:{6}", y, mo, d, w, h, mi, s))
        },
        /*Ajax请求开始时调用，可显示progress条*/
        ajaxStart: function () {
            this.progressStart('请求处理中...')
        },
        /*Ajax请求结束时调用，关闭progress条*/
        ajaxEnd: function () {
            $.messager.progress('close');
        },
        progressStart: function (message) {
            $.messager.progress({ text: '<span style="color:#2d78b9;">' + message + '</span>', interval: 100 });
        },
        progressEnd: function () {
            $.messager.progress('close');
        },


        ///*初始化菜单*/
        setStarMenu: function () {
            var timer;
            $('.start').click(function (e) {
                $(this).toggleClass('active');
                e.stopPropagation();
                $('.startmenu').toggleClass('active');
                $('.nav-submenu').hide();
                $('.startmenu .nav-item').removeClass('active');
                setTimeout(function () {
                    $('#mainlayout').unbind('contextmenu').contextmenu(function () {
                        $('.start').removeClass('active');
                        $('.startmenu').removeClass('active');
                        $(this).unbind('click').unbind('contextmenu');
                    })
                    $('#mainlayout').unbind('click').bind('click', function () {
                        $('.startmenu').removeClass('active'); $('.start').removeClass('active');
                        $(this).unbind('click').unbind('contextmenu');
                    })
                }, 10)
            })
            $('.startmenu .nav-item').mouseenter(function (e) {
                timer = setTimeout(function () {
                    $('.nav-submenu').hide();
                    $('.startmenu .nav-item').removeClass('active');
                    $(e.target).addClass('active');
                    $('.nav-submenu:eq(' + $(e.target).index() + ')').show();
                }, 50);
            })
            $('.startmenu .nav-item').mouseleave(function (e) {
                clearTimeout(timer);
            });
        },


        formatJsonResult: function (jsonDateString) {
            return jsonDateString.replace(/\/Date\(\d*\)\//g, function (m, i) {
                return $.formatJsonDate(m);
            })
        },
        getDate: function (jsonDate) {
            return new Date(parseInt(jsonDate.replace("/Date(", "").replace(")/", ""), 10));
        },
        /*根据格式化字符串将JSON格式的时间转化成字符串*/
        formatJsonDate: function (jsonDate, formatterString) {
            try {
                var date = $.getDate(jsonDate);
                var year = date.getFullYear();
                var month = $.fixDateNumber(date.getMonth() + 1);
                var day = $.fixDateNumber(date.getDate());
                var hours = $.fixDateNumber(date.getHours());
                var minutes = $.fixDateNumber(date.getMinutes());
                var seconds = $.fixDateNumber(date.getSeconds());
                var milliseconds = date.getMilliseconds();
                if (formatterString != undefined) {
                    return String.Format(formatterString, year, month, day, hours, minutes, seconds);
                } else {

                    return String.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hours, minutes, seconds);
                }

            } catch (ex) {
                return "";
            }
        },
        /*右下角滑出提示*/
        slideMessage: function (title, message) {
            $.messager.show({
                title: title,
                msg: message,
                timeout: 2000,
                showType: 'slide'
            });
        },
        /*初始化申请表单*/
        initFillForm: function (formId, eleConfig, controller) {
            $('#orderform_' + formId + ' form').prop('autocomplete', 'off')
            $('#orderform_' + formId + ' form').attr('action', String.Format('/' + controller + '/FillForm/{0}', formId)).css('overflow', 'hidden')
            //关闭自动完成
            $('#orderform_' + formId + ' form>table').css('margin', 'auto');
            for (var i = 0; i < elesConfig.length; i++) {

                switch (elesConfig[i].Style) {
                    case 0:
                        //文本
                        $('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]').attr('Value', elesConfig[i].OptionValues);
                        if (elesConfig[i].Value != null && elesConfig[i].Value != "") {

                            $('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]').attr('Value', elesConfig[i].Value.toString());
                        }

                        break;
                    case 1:
                        //select列表
                        var optionValues = elesConfig[i].OptionValues.split('|');


                        if ($('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]').length > 0) {
                            $('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]').parent().append('<span class="forprint"></span>');
                            if ($('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]')[0].type == "radio") {
                                $('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]').each(function (i, ele) {
                                    $(ele.nextSibling).wrap('<span class="noprint"></span>')
                                });
                                $('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]').wrap('<span class="noprint"></span>')
                                $('input:radio[name="' + elesConfig[i].IdentityString + '"][value="' + elesConfig[i].Value + '"]').prop('checked', true)
                            } else {
                                $('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]').wrap('<span class="noprint"></span>')
                                if ($('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"] option').length == 0) {
                                    $('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]').append('<option value="">---未填---</option>')
                                    for (var k = 0; k < optionValues.length; k++) {
                                        $('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]').append('<option value="' + optionValues[k] + '">' + optionValues[k] + '</option>')
                                    }
                                }
                                $('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"] [value="' + elesConfig[i].Value + '"]').attr('selected', true)
                            }
                        }
                        break;
                    case 2:
                        //颜色
                        try {
                            var optionValues = elesConfig[i].OptionValues.split('|');
                            var comboPanel = $('<div id="orderform_' + formId + '_' + elesConfig[i].IdentityString + '" ></div>');
                            for (var k = 0; k < optionValues.length; k++) {
                                comboPanel.append($('<div style="position: relative;"><input type="radio" name="orderform_' + formId + '_' + elesConfig[i].IdentityString + '" data-value="' + optionValues[k] + '" data-text="' + optionValues[k].split('#')[0] + '"><div style="background:' + optionValues[k].split('#')[1] + ';background:#' + optionValues[k].split('#')[1] + ';height:17px;display:inline-block;width:20px;vertical-align: sub;margin-right:5px;"></div><span style="display:inline-block;">' + optionValues[k].split('#')[0] + '</span></div>'))
                            }
                            var combo = $('#orderform_' + formId + ' [name="' + elesConfig[i].IdentityString + '"]').combo({ editable: false });
                            var identityString = elesConfig[i].IdentityString;
                            var oldValue = elesConfig[i].Value == null ? "" : elesConfig[i].Value;
                            var oldText = $.isEmptyObject(elesConfig[i].Value) ? "" : elesConfig[i].Value.split('#')[0];
                            $('#orderform_' + formId + ' form').bind('reset', function () {
                                setTimeout(function () {
                                    combo.combo('setValue', '').combo('setText', '');
                                    combo.combo('setValue', oldValue).combo('setText', oldText);
                                    $('#orderform_' + formId + '_' + identityString + '  [name="orderform_' + formId + '_' + identityString + '"]:checked').prop('checked', false)
                                    $('#orderform_' + formId + '_' + identityString + '  [name="orderform_' + formId + '_' + identityString + '"][data-value="' + oldValue + '"]').prop('checked', true)
                                }, 0)
                            })
                            comboPanel.find('input').click(function () {
                                combo.combo('setValue', $(this).data('value')).combo('setText', $(this).data('text'));
                                combo.combo('hidePanel');
                            })
                            comboPanel.appendTo(combo.combo('panel'))
                            combo.combo('setValue', oldValue).combo('setText', oldText);
                            $('#orderform_' + formId + '_' + identityString + '  [name="orderform_' + formId + '_' + identityString + '"][data-value="' + oldValue + '"]').prop('checked', true)
                        } catch (e) {

                        }

                        break;
                }
            }

        },
        /*获取用户名*/
        setUserName: function () {
            if ($.cookie != undefined) {
                if ($.isEmpty($.cookie('name'))) {
                    $.get('/Employee/GetEmployeeName', {}, function (data) {
                        $.cookie('name', data.name, {
                            path: '/'
                        })
                        $('.welcome b').text(data.name);
                    }, 'json')
                } else {
                    $('.welcome b').text($.cookie('name'));
                }
            }
        }

    })
    //$.extend($.fn.treegrid.methods, {
    //    getSelectionsValues: function (jq, fieldName) {

    //    }
    //})
    $.extend($.fn.datagrid.methods, {
        getSelectionsValues: function (jq, fieldName) {
            var rows = jq.datagrid('getSelections');
            var values = [];
            for (var i = 0; i < rows.length; i++) {

                values.push(rows[i][fieldName]);
            }
            return values;
        }
    })

})(jQuery)
var showAjaxErrorAlert = true;
$(function () {

    if (!$.support.leadingWhitespace) {
        if (!$.cookie('bsie')) {
            alert('检测到您的IE版本较旧，请配合IE9以上版本或者其他现代浏览器使用该系统，以获得最佳体验。')
        }
        $.cookie('bsie', true, { path: '/' });
    }

    $.updateTimeLabel();
    setInterval($.updateTimeLabel, 1000);
    $.setStarMenu();
    $.setUserName();
    $(document).ajaxError(function (event, request, settings) {
        if (request.status != 200 && settings.url != "" && showAjaxErrorAlert) {
            $.messager.alert("通讯出错", "在与页面:\"" + settings.url + "\"通讯时出现【" + request.status + "】错误", "error", function () { $.ajaxEnd() });
        }
    });

    $(document).ajaxSuccess(function (event, request, settings) {
        if (showAjaxErrorAlert) {
            if (request.responseText != undefined) {
                try {
                    var data = JSON.parse(request.responseText);
                    if (data.resultCode == -1) {
                        $.messager.alert("授权出错", "在与页面:\"" + settings.url + "\"通讯时出现权限错误，您没有访问该路径的权限", "error", function () { $.ajaxEnd() });
                        //$.messager.alert("授权出错", "无权进行该操作", "error", function () { $.ajaxEnd() });
                        return;
                    }
                } catch (e) {

                }
            }
            if (request.responseJSON != undefined && request.responseJSON.resultCode == -1) {
                $.messager.alert("授权出错", "在与页面:\"" + settings.url + "\"通讯时出现权限错误，您没有访问该路径的权限", "error", function () { $.ajaxEnd() });
                //$.messager.alert("授权出错", "无权进行该操作", "error", function () { $.ajaxEnd() });
            }
        }
    });
    //ajax开始时显示progress条
    //$(document).ajaxStart(function () { $.ajaxStart(); });
    //$(document).ajaxComplete(function () { $.ajaxEnd();})
})


window.onload = function () {
    setTimeout(function () { $('.loadding').hide(); }, 0);
}

function CloseWest() {
    $('#mainlayout').layout('collapse', 'west');
}

function changeMinePassword() {
    if ($('#password_dialog_change form').form('validate')) {
        $('#password_dialog_change form').ajaxSubmit({
            type: 'post',
            url: '/Employee/ChangeMinePassword',
            success: function (data) {
                if (data.resultCode == 1) {
                    $.messager.alert('提示', '密码修改成功，请重新登陆', 'info', function () {
                        location.href = '/Employee/Logout';
                    })
                } else {
                    $.messager.alert('错误', data.message, 'warning');
                }
            }
        })
    }
}

window.onbeforeunload = function () {
    showAjaxErrorAlert = false;
}
