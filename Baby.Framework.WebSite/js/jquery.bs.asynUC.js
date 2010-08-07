///    <summary>
///    jQuery实例扩展，Ajax加载封装用户控件(*.ascx)，输出Html，仅适用于Asp.Net。
///     依赖 $.ajaxWebService(url, dataMap, fnSuccess)
///    </summary>
///    <param name="control" type="String">
///     需要加载的用户控件的相对路径
///     </param>
///    <param name="webService" type="String">
///     输出控件Html片段的WebService。可选，缺省值为 UC/UCService.asmx。
///     </param>
$.fn.loadUserControl = function (control, webService) {
    var $dom = this;
    if (page == "" || page == null) {
        page = "/UC/UCService.asmx";
    }
    page += "/RenderUserControl"; //RenderUserControl是UCService中的方法，不要轻易修改
    $.ajaxWebService(page, "{control:'" + control + "'}", function (result) {
        $dom.html(result.d);
    });

}