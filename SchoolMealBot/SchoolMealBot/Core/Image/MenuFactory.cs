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
using ImageShackApi;
using System.IO;

namespace SchoolMealBot.Core.Image
{
    public class MenuFactory
    {
        private const string ApiKey = "269EFIJL61c2b056e30d6c142b1714e26725e591";

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

            var filePath = Path.Combine(Environment.CurrentDirectory, "SchoolMenu", filename);
            bmp.Save(filePath, ImageFormat.Jpeg);

            UploadResult result = null;
            try
            {
                result = UploadImage(filePath);
            }
            catch (Exception)
            {
                throw;
            }

            return result.Links.ImageLink;
        }

        private static UploadResult UploadImage(string filepath)
        {
            ImageShackUploader.ApiKey = ApiKey;
            return ImageShackUploader.UploadImage(filepath);
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