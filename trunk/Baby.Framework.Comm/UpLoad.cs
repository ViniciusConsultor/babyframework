using System;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace Baby.Framework.Comm
{
    public class UpLoad
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="_Request">HttpRequest</param>
        /// <param name="_Response">HttpResponse</param>
        /// <param name="_fileName">文件名</param>
        /// <param name="_fullPath">完整路径</param>
        /// <param name="_speed">速度</param>
        /// <returns>是否下载成功</returns>
        public static bool DownFile(HttpRequest _Request, HttpResponse _Response, string _fileName, string _fullPath, long _speed)
        {
            Stream iStream = null;
            byte[] buffer = new Byte[10240];
            int length;
            long dataToRead;
            try
            {
                iStream = new FileStream(_fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _Response.Clear();
                dataToRead = iStream.Length;

                long p = 0;
                if (_Request.Headers["Range"] != null)
                {
                    _Response.StatusCode = 206;
                    p = long.Parse(_Request.Headers["Range"].Replace("bytes=", "").Replace("-", ""));
                }
                if (p != 0)
                {
                    _Response.AddHeader("Content-Range", "bytes " + p.ToString() + "-" + ((long)(dataToRead - 1)).ToString() + "/" + dataToRead.ToString());
                }
                _Response.AddHeader("Content-Length", ((long)(dataToRead - p)).ToString());
                _Response.ContentType = "application/octet-stream";
                _Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpContext.Current.Server.UrlPathEncode(_fileName));

                iStream.Position = p;
                dataToRead = dataToRead - p;

                while (dataToRead > 0)
                {
                    if (_Response.IsClientConnected)
                    {
                        length = iStream.Read(buffer, 0, 10240);

                        _Response.OutputStream.Write(buffer, 0, length);
                        _Response.Flush();

                        buffer = new Byte[10240];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        dataToRead = -1;
                    }
                }

            }
            catch
            {
                return false;
            }
            finally
            {
                if (iStream != null)
                {
                    iStream.Close();
                }
                _Response.End();
            }
            return true;
        }

        /// <summary>
        /// 上传文件并返回路径
        /// </summary>
        /// <param name="fileUpload">上传文件控件</param>
        /// <param name="allowFileStr">允许文件名 如： .jpg|.JPG|.gif|.GIF|.doc|.DOC</param>
        /// <param name="savePathStr">保存文件夹 如： ~\attachment\</param>
        /// <returns>成功返回路径，LengthErr文件超过1M，ExtErr文件不属于限制文件,string.Empty无文件</returns>
        public static string UpFile(FileUpload fileUpload,string allowFileStr,string savePathStr)
        {
            if (fileUpload.HasFile)
            {
                string sfileExt = Path.GetExtension(fileUpload.FileName);
                string sdate = DateTime.Now.ToString("yyyyMMddhhmmss");
                string savePath = System.Web.HttpContext.Current.Server.MapPath(savePathStr + sdate + sfileExt);
                float ffilelength = fileUpload.PostedFile.ContentLength / 1000;
                if (allowFileStr.Contains(sfileExt) == true)
                {
                    if (ffilelength <= 1024)
                    {
                        fileUpload.SaveAs(savePath);
                        return savePathStr.Replace('\\','/').Replace("~",string.Empty) + sdate + sfileExt;
                    }
                    else
                    {
                        return "LengthErr";
                    }
                }
                else
                {
                    return "ExtErr";
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
