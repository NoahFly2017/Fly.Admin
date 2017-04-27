/*
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.filebrowserBrowseUrl = '/Scripts/ckfinder/ckfinder.html'; //上传文件时浏览服务文件夹
    //上传图片时浏览服务文件夹
    config.filebrowserImageBrowseUrl = '/Scripts/ckfinder/ckfinder.html?Type=Images';
    //上传Flash时浏览服务文件夹
    config.filebrowserFlashBrowseUrl = '/Scripts/ckfinder/ckfinder.html?Type=Flash';
    //上传文件按钮(标签)
    config.filebrowserUploadUrl = '/Scripts/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files';
    //上传图片按钮(标签)
    config.filebrowserImageUploadUrl = '/Scripts/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images';
    //上传Flash按钮(标签)
    config.filebrowserFlashUploadUrl = '/Scripts/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash';
    config.height = 300;
    config.toolbarCanCollapse = true;

    config.font_names = '宋体/宋体;黑体/黑体;仿宋/仿宋_GB2312;楷体/楷体_GB2312;隶书/隶书;幼圆/幼圆;微软雅黑/微软雅黑;' + config.font_names;
    config.toolbar_Full = [
    {
        name: 'document', items: ['Source', '-',
          //'Save',
          'NewPage', 'DocProps', 'Preview', 'Print', '-', 'Templates']
    },
    { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
    { name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] },
    { name: 'tools', items: ['Maximize', 'ShowBlocks', '-', 'About'] },
    {
        name: 'forms', items: ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton',
             'HiddenField']
    },
    { name: 'insert', items: ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', 'Iframe'] },
    '/',
    { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
    {
        name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv',
        '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl']
    },
    { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },

    '/',
    { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
    { name: 'colors', items: ['TextColor', 'BGColor'] }

    ];
    config.toolbar_Normal = [
    {
        name: 'document', items: ['Source', '-',
          //'Save',
          'NewPage', 'DocProps', 'Preview', 'Print', '-', 'Templates']
    },
    { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
    { name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] },
    { name: 'tools', items: ['Maximize', 'ShowBlocks', '-', 'About'] }, '/',
    { name: 'insert', items: ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', 'Link', 'Unlink'] },
   {
       name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv',
       '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl']
   },
    { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
    { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
    { name: 'colors', items: ['TextColor', 'BGColor'] }

    ];
    config.toolbar_Basic = [
         {
             name: 'document', items: ['Source', '-',
               //'Save',
               'NewPage', 'DocProps', 'Templates']
         }, { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] }, { name: 'colors', items: ['TextColor', 'BGColor'] }, { name: 'tools', items: ['Maximize', 'ShowBlocks'] }, '/', { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] }, {
             name: 'paragraph', items: ['NumberedList', 'BulletedList',  'Outdent', 'Indent', 'Blockquote', 
             'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock',  'BidiLtr', 'BidiRtl']
         }, { name: 'insert', items: ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'Link', 'Unlink'] }
    ]
};
