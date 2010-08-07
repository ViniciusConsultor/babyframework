using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.IO;

/// <summary>
///异步解释用户自定义控件
/// </summary>
[WebService(Namespace = "http://babysuper.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
[System.Web.Script.Services.ScriptService]
public class UCService : System.Web.Services.WebService {

    public UCService () {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    /// <summary>
    /// 解释用户自定义控件
    /// </summary>
    /// <param name="controlPath">自定义控件路径</param>
    /// <returns>返回解释后的html</returns>
    [WebMethod]
    public string RenderUserControl(string controlPath)
    {
        Page page = new Page();
        UserControl uc = (UserControl)page.LoadControl("~/" + controlPath);
        page.Controls.Add(uc);
        StringWriter writer = new StringWriter();
        HttpContext.Current.Server.Execute(page, writer, false);
        return writer.ToString();
    }
}
