using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolMeal;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Text;

namespace SchoolMealBot.Core.Image
{
    public class MenuFactory
    {
        public static string MakeImage(string userId, MealMenu menu)
        {
            var filename = GetRandomFileName(userId);

            List<string> menuStrings = Regex.Split(menu.ToString(), "\r\n|\r|\n").ToList();

            Bitmap bmp = new Bitmap(100, 200);

            RectangleF rectf = new RectangleF(70, 90, 90, 50);

            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            // fix this code
            g.DrawString("yourText", new Font(@"Assets\Fonts\NanumGothic.ttf#나눔고딕", 10), Brushes.White, rectf);

            g.Flush();

            bmp.Save(@"Results\Menu\" + filename, ImageFormat.Png);
            //////////////////////////////////////////////////////////////////
            return "";
        }

        private static string GetRandomFileName(string userId)
        {
            string filename = Regex.Replace(userId, @"[^a-zA-Z0-9가-힣]", "", RegexOptions.Singleline) + "-";
            Random rnd = new Random();
            StringBuilder rs = new StringBuilder();
            var charPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            int length = 10;
            while (length-- != 0)
            {
                rs.Append(charPool[(int)(rnd.NextDouble() * charPool.Length)]);
            }
            filename += rs.ToString();

            return filename;
        }
    }
}