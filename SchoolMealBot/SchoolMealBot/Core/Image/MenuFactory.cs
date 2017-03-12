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
using System.Threading.Tasks;

namespace SchoolMealBot.Core.Image
{
    public class MenuFactory
    {
        private const string ApiKey = "269EFIJL61c2b056e30d6c142b1714e26725e591";

        public static async Task<string> MakeImage(string userId, MealMenu menu)
        {
            var filename = GetRandomFileName(userId) + ".jpg";
            var directoryPath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SchoolMealMenu");
            var filePath = Path.Combine(directoryPath, filename);

            List<string> menuStrings = Regex.Split(menu.ToString(), "\r\n|\r|\n").ToList();

            var width = 0;
            var defaultWidth = 500;
            foreach (var str in menuStrings)
            {
                if (str.Length > width)
                {
                    width = str.Length;
                }
            }
            width += 40;
            if (width < defaultWidth)
            {
                width = defaultWidth;
            }

            var height = menuStrings.Count * 20;

            Bitmap bitmap = new Bitmap(width, height); //load the image file

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (Font writeFont = new Font("Malgun Gothic", 12))
                {
                    float yPoint = 10f;
                    foreach (var str in menuStrings)
                    {
                        graphics.DrawString(str, writeFont, Brushes.White, new PointF(10f, yPoint));
                        yPoint += 15f;
                    }
                }
            }

            await Task.Run(() =>
            {
                Directory.CreateDirectory(directoryPath);
                bitmap.Save(filePath, ImageFormat.Jpeg); //save the image file
            });

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