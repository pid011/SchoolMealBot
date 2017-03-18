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

        public static byte[] MakeImage(string userId, MealMenu menu)
        {
            List<string> menuStrings = Regex.Split(MenuToString(menu), "\r\n|\r|\n").ToList();

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

            var height = menuStrings.Count * 24;

            Bitmap bitmap = new Bitmap(width, height); //load the image file

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.DarkGreen, 0, 0, width, height);
                using (Font writeFont = new Font("Malgun Gothic", 12))
                {
                    float yPoint = 10f;
                    foreach (var str in menuStrings)
                    {
                        graphics.DrawString(str, writeFont, Brushes.White, new PointF(15f, yPoint));
                        yPoint += 20f;
                    }
                }
            }

            var imageArray = (byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[]));

            return imageArray;
        }

        private static string MenuToString(MealMenu menu)
        {
            var resultMenu = new StringBuilder();
            if (menu.IsExistMenu)
            {
                resultMenu.Append($"{menu.Date.Month}월 {menu.Date.Day}일 ");
                switch (menu.Date.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        resultMenu.AppendLine("일요일");
                        break;
                    case DayOfWeek.Monday:
                        resultMenu.AppendLine("월요일");
                        break;
                    case DayOfWeek.Tuesday:
                        resultMenu.AppendLine("화요일");
                        break;
                    case DayOfWeek.Wednesday:
                        resultMenu.AppendLine("수요일");
                        break;
                    case DayOfWeek.Thursday:
                        resultMenu.AppendLine("목요일");
                        break;
                    case DayOfWeek.Friday:
                        resultMenu.AppendLine("금요일");
                        break;
                    case DayOfWeek.Saturday:
                        resultMenu.AppendLine("토요일");
                        break;
                    default:
                        break;
                }
                resultMenu.AppendLine("=============");
                resultMenu.AppendLine();

                if (menu.Breakfast != null)
                {
                    resultMenu.AppendLine("[아침]");
                    foreach (var breakfastMenu in menu.Breakfast)
                    {
                        resultMenu.AppendLine(breakfastMenu);
                    }
                }
                if (menu.Lunch != null)
                {
                    resultMenu.AppendLine();
                    resultMenu.AppendLine("[점심]");
                    foreach (var lunchMenu in menu.Lunch)
                    {
                        resultMenu.AppendLine(lunchMenu);
                    }
                }
                if (menu.Dinner != null)
                {
                    resultMenu.AppendLine();
                    resultMenu.AppendLine("[저녁]");
                    foreach (var dinnerMenu in menu.Dinner)
                    {
                        resultMenu.AppendLine(dinnerMenu);
                    }
                }

            }
            return resultMenu.ToString();
        }
    }
}