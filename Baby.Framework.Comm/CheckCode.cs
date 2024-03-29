﻿using System;
using System.Web;

namespace Baby.Framework.Comm
{
    public class CheckCode
    {
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="randomNum">生成几位验证码</param>
        /// <returns>返回随机的字符串</returns>
        private string GetCheckCodeStr(int randomNum)
        {
            int number;
            char code;
            string checkCodeStr = string.Empty;
            Random random = new Random();
            for (int i = 0; i < randomNum; i++)
            {
                number = random.Next();
                if (number % 2 == 0)
                {
                    code = (char)('0' + (char)(number % 10));
                }
                else
                {
                    code = (char)('A' + (char)(number % 26));
                }
                checkCodeStr += code.ToString();
            }
            return checkCodeStr;
        }

        /// <summary>
        /// 生成验证图片，默认生成4位，Session保存
        /// </summary>
        public void CreateCheckCodeImage(out string checkCode, int codeNum = 4)
        {
            checkCode = GetCheckCodeStr(codeNum);
            this.CreateCheckCodeImage(checkCode);
        }

        /// <summary>
        /// 生成验证图片
        /// </summary>
        /// <param name="checkCode">验证码字符串</param>
        public void CreateCheckCodeImage(string checkCode)
        {
            if (checkCode == null || checkCode.Trim() == string.Empty)
            {
                return;
            }
            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 12.5)), 22);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(System.Drawing.Color.White);
                //画图片的背景噪音线
                for (int i = 0; i < 2; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Black), x1, y1, x2, y2);
                }
                System.Drawing.Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.Color.Blue, System.Drawing.Color.DarkRed, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);
                //画图片的前景噪音点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, System.Drawing.Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                System.IO.MemoryStream myStream = new System.IO.MemoryStream();
                image.Save(myStream, System.Drawing.Imaging.ImageFormat.Gif);
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ContentType = "image/Gif";
                HttpContext.Current.Response.BinaryWrite(myStream.ToArray());
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}
