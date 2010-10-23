using System;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Diagnostics;

namespace Baby.Framework.Net
{
    public class HttpHelper
    {
        /// <summary>
        /// 获取页面字符串
        /// </summary>
        /// <param name="HttpUrl">页面地址</param>
        /// <param name="CharSet">是否使用默认编码</param>
        /// <returns>页面HTML代码</returns>
        public static string GetHttpPage(string HttpUrl, bool CharSet)
        {
            string PageHtml = string.Empty;
            WebClient wc = null;
            try
            {
                wc = new WebClient();
                byte[] PageData = wc.DownloadData(HttpUrl);

                if (CharSet)
                    PageHtml = Encoding.Default.GetString(PageData);
                else
                    PageHtml = Encoding.UTF8.GetString(PageData);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (wc != null)
                {
                    wc.Dispose();
                }
            }
            return PageHtml;
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="Constr">需要截取的字符串</param>
        /// <param name="StartStr">开始字符串</param>
        /// <param name="OverStr">结束字符串</param>
        /// <param name="IncluL">包含左侧标签</param>
        /// <param name="IncluR">包含右侧标签</param>
        /// <returns>截取出来的字符串</returns>
        public static string GetBody(string Constr, string StartStr, string OverStr, bool IncluL, bool IncluR)
        {
            string ConstrTemp;
            int Start, Over;
            ConstrTemp = Constr.ToLower();
            StartStr = StartStr.ToLower();
            OverStr = OverStr.ToLower();

            Start = ConstrTemp.IndexOf(StartStr) + StartStr.Length;
            if (IncluL)
            {
                Start -= StartStr.Length;
            }
            ConstrTemp = ConstrTemp.Substring(Start);

            Over = ConstrTemp.IndexOf(OverStr);
            if (IncluR)
            {
                Over += OverStr.Length;
            }
            ConstrTemp = ConstrTemp.Substring(0, Over);

            return ConstrTemp;
        }

        /// <summary>
        /// 使用MSXML3.dll来获取远程代码
        /// </summary>
        /// <param name="Url">页面地址</param>
        /// <returns>页面地址</returns>
        public static string GetRemoteHtmlCode(string Url)
        {
            string s = null;
            try
            {
                MSXML2.XMLHTTP _xmlhttp = new MSXML2.XMLHTTP();
                _xmlhttp.open("GET", Url, false, null, null);
                _xmlhttp.send("");
                if (_xmlhttp.readyState == 4)
                {
                    s = System.Text.Encoding.Default.GetString((byte[])_xmlhttp.responseBody);
                }
            }
            catch
            {
                throw;
            }
            return s;
        }

        /// <summary>
        /// 根据正则表达式匹配字符串
        /// </summary>
        /// <param name="htmlStr">需要匹配字符串的HTML</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>匹配出来的字符串 返回string.Empty则匹配失败</returns>
        public static string MatchHtmlByRegex(string htmlStr, string regexStr)
        {
            string matchHtml = string.Empty;
            Regex regex = new Regex(regexStr, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match match = regex.Match(htmlStr);
            if (match.Success)
            {
                matchHtml = match.Groups[0].ToString();
            }

            return matchHtml;
        }

        #region 可以自动获取出所有类型页面的编码

        /// <summary>
        /// 获取页面编码
        /// </summary>
        /// <param name="url">页面地址</param>
        /// <returns>页面编码</returns>
        public static string GetEncoding(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 20000;
                request.AllowAutoRedirect = false;
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK && response.ContentLength < 1024 * 1024)
                {
                    if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                        reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
                    else
                        reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII);

                    string html = reader.ReadToEnd();
                    Regex reg_charset = new Regex(@"charset\b\s*=\s*(?<charset>[^""]*)");

                    if (reg_charset.IsMatch(html))
                    {
                        return reg_charset.Match(html).Groups["charset"].Value;
                    }
                    else if (response.CharacterSet != string.Empty)
                    {
                        return response.CharacterSet;
                    }
                    else
                        return Encoding.Default.BodyName;
                }
            }
            catch
            {
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (reader != null)
                    reader.Close();
                if (request != null)
                    request = null;
            }
            return Encoding.Default.BodyName;
        }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <param name="url">页面地址</param>
        /// <param name="encoding">页面编码</param>
        /// <returns>页面html代码</returns>
        public static string GetAllEncodeHtml(string url, Encoding encoding)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 20000;
                request.AllowAutoRedirect = false;
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK && response.ContentLength < 1024 * 1024)
                {
                    if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                        reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress), encoding);
                    else
                        reader = new StreamReader(response.GetResponseStream(), encoding);

                    string html = reader.ReadToEnd();
                    return html;
                }
            }
            catch
            {
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }

                if (reader != null)
                    reader.Close();
                if (request != null)

                    request = null;
            }

            return string.Empty;
        }

        #endregion

        #region html无输出charset则无法识别 (代码较简洁)

        /// <summary>
        /// 获取网页的HTML内容，根据网页的charset自动判断Encoding
        /// </summary>
        /// <param name="url">页面地址</param>
        /// <returns>页面内容</returns>
        public static string GetCommonHtml(string url)
        {
            return GetCommonHtml(url, null);
        }

        /// <summary>
        /// 获取网页的HTML内容，指定Encoding
        /// </summary>
        /// <param name="url">页面地址</param>
        /// <param name="encoding">页面编码</param>
        /// <returns>页面内容</returns>
        public static string GetCommonHtml(string url, Encoding encoding)
        {
            WebClient myWebClient = new WebClient();
            //string agentStr = " Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.0.3705;MyIE2; Alexa Toolbar; mxie; .NET CLR 1.1.4322";
            myWebClient.Headers.Add(HttpRequestHeader.UserAgent, " Mozilla/5.0 (Windows; U; Windows NT 5.2; zh-CN; rv:1.9.0.3) Gecko/2008092417 Firefox/3.0.3");
            myWebClient.Headers.Add(HttpRequestHeader.KeepAlive, "FALSE");
            byte[] buf;
            try { buf = myWebClient.DownloadData(url); }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
            finally
            {
                myWebClient.Dispose();
            }
            if (encoding != null) return encoding.GetString(buf);
            string html = Encoding.UTF8.GetString(buf);
            encoding = GetEncodingByCharset(html);
            if (encoding == null || encoding == Encoding.UTF8) return html;
            return encoding.GetString(buf);
        }

        /// <summary>
        /// 根据网页的HTML内容提取网页的Encoding
        /// </summary>
        /// <param name="html">页面内容</param>
        /// <returns>页面编码</returns>
        public static Encoding GetEncodingByCharset(string html)
        {
            string pattern = @"(?i)\bcharset=(?<charset>[-a-zA-Z_0-9]+)";
            string charset = Regex.Match(html, pattern).Groups["charset"].Value;
            try { return Encoding.GetEncoding(charset); }
            catch (ArgumentException) { return null; }
        }

        /// <summary>
        /// 根据网页的HTML内容提取网页的Title
        /// </summary>
        /// <param name="html">页面代码</param>
        /// <returns>Title</returns>
        public static string GetTitle(string html)
        {
            string pattern = @"(?si)<title(?:\s+(?:""[^""]*""|'[^']*'|[^""'>])*)?>(?<title>.*?)</title>";
            return Regex.Match(html, pattern).Groups["title"].Value.Trim();
        }
        #endregion

        #region 多线程时使用TcpClient获取html更加好，报错率低

        /// <summary>
        /// 使用TcpClient获取Html(编码为UTF-8)
        /// </summary>
        /// <param name="URL">地址</param>
        /// <returns>Html源码</returns>
        public static string GetHTMLByTCP(string URL)
        {
            return GetHTMLByTCP(URL, Encoding.UTF8);
        }

        /// <summary>
        /// 使用TcpClient获取Html
        /// </summary>
        /// <param name="URL">地址</param>
        /// <param name="encoding">编码</param>
        /// <returns>Html源码</returns>
        public static string GetHTMLByTCP(string URL, Encoding encoding)
        {
            string strHTML = "";//用来保存获得的HTML代码
            TcpClient clientSocket = new TcpClient();
            Stream readStream = null;
            try
            {
                Uri URI = new Uri(URL);
                clientSocket.Connect(URI.Host, URI.Port);
                StringBuilder RequestHeaders = new StringBuilder();//用来保存HTML协议头部信息
                RequestHeaders.AppendFormat("{0} {1} HTTP/1.1\r\n", "GET", URI.PathAndQuery);
                RequestHeaders.AppendFormat("Connection:close\r\n");
                RequestHeaders.AppendFormat("Host:{0}\r\n", URI.Host);
                RequestHeaders.AppendFormat("Accept:*/*\r\n");
                RequestHeaders.AppendFormat("Accept-Language:zh-cn\r\n");
                RequestHeaders.AppendFormat("User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)\r\n\r\n");
                //Encoding encoding = Encoding.UTF8;
                byte[] request = encoding.GetBytes(RequestHeaders.ToString());
                clientSocket.Client.Send(request);
                //获取要保存的网络流
                readStream = clientSocket.GetStream();
                StreamReader sr = new StreamReader(readStream, encoding);
                strHTML = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                //Debug.WriteLine("发生了错误：" + ex.Source + " 信息：" + ex.Message);
                throw ex;
            }
            finally
            {
                if (readStream != null)
                    readStream.Close();
                if (clientSocket.Connected)
                    clientSocket.Close();
            }

            return strHTML;
        }

        #endregion
    }
}
