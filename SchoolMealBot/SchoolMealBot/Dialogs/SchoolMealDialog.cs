using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolMeal;
using SchoolFinder;
using System.Text;
using Microsoft.Bot.Connector;
using SchoolMealBot.Core.Image;

namespace SchoolMealBot.Dialogs
{
    [Serializable]
    public class SchoolMealDialog : IDialog<object>
    {
        private SchoolInfo schoolInfo;

        private const string TodaysSchoolMealOption = "오늘급식";
        private const string TomorrowsSchoolMealOption = "내일급식";
        private const string SchoolMealThisWeekOption = "이번주급식";
        private const string SchoolMealNextWeekOption = "다음주급식";

        private readonly List<string> options = new List<string>
        {
            TodaysSchoolMealOption,
            TomorrowsSchoolMealOption,
            SchoolMealThisWeekOption,
            SchoolMealNextWeekOption
        };

        public SchoolMealDialog(SchoolInfo schoolInfo)
        {
            this.schoolInfo = schoolInfo;
        }

#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(context, ResumeAfterChoicedAsync, options,
                    "원하시는 옵션을 선택해주세요!", "목록에서 원하는 옵션을 선택해주세요!");
        }

        private async Task ResumeAfterChoicedAsync(IDialogContext context, IAwaitable<string> result)
        {
            var resultType = await result;

            var mealMenu = await GetSchoolMealListAsync(context);

            var replyMsg = context.MakeMessage();

            List<Attachment> attachments = null;

            if (mealMenu != null && schoolInfo != null)
            {
                switch (resultType)
                {
                    case TodaysSchoolMealOption:
                        attachments = GetTodaysSchoolMealMenuAsync(context, mealMenu);
                        break;

                    case TomorrowsSchoolMealOption:
                        attachments = GetTomorrowsSchoolMealMenuAsync(context, mealMenu);
                        break;

                    case SchoolMealThisWeekOption:
                        attachments = GetSchoolMealThisWeekMenuAsync(context, mealMenu);
                        break;

                    case SchoolMealNextWeekOption:
                        attachments = GetSchoolMealNextWeekMenuAsync(context, mealMenu);
                        break;

                    default:
                        break;
                }

                if (attachments != null)
                {
                    if (attachments.Count == 0)
                    {
                        await context.PostAsync("해당 날짜와 일치하는 급식메뉴가 없어요 :O");
                    }
                    else
                    {
                        replyMsg.Attachments = attachments;

                        await context.PostAsync(replyMsg);
                    }
                }
                else
                {
                    await context.PostAsync("급식메뉴를 가져오는데 실패했어요 :(");
                }
                
                context.Done<object>(null);
            }
        }

        private List<Attachment> GetTodaysSchoolMealMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var todaysDate = DateTime.Now;
            MealMenu todayMenu = null;
            if (menus.Exists(x => x.Date.Date == todaysDate.Date))
            {
                todayMenu = menus.Find(x => x.Date.Date == todaysDate.Date);
            }

            return todayMenu != null ? CreateAttachmentsAsync(context, new List<MealMenu> { todayMenu }) : null;
        }

        private List<Attachment> GetTomorrowsSchoolMealMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var tomorrowDate = DateTime.Now.AddDays(1);
            MealMenu tomorrowMenu = null;
            if (menus.Exists(x => x.Date.Date == tomorrowDate.Date))
            {
                tomorrowMenu = menus.Find(x => x.Date.Date == tomorrowDate.Date);
            }

            return tomorrowMenu != null ? CreateAttachmentsAsync(context, new List<MealMenu> { tomorrowMenu }) : null;
        }

        private List<Attachment> GetSchoolMealThisWeekMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var todayDate = DateTime.Now;
            var datesOfWeek = GetDatesOfWeek(todayDate);
            var thisWeekMenu = new List<MealMenu>();

            foreach (var date in datesOfWeek)
            {
                if (menus.Exists(x => x.Date.Date.Date == date.Date.Date))
                {
                    thisWeekMenu.Add(menus.Find(x => x.Date.Date == date.Date));
                }
            }

            return thisWeekMenu != null ? CreateAttachmentsAsync(context, thisWeekMenu) : null;
        }

        private List<Attachment> GetSchoolMealNextWeekMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var NextWeekDate = DateTime.Now.AddDays(7);
            var datesOfWeek = GetDatesOfWeek(NextWeekDate);
            var nextWeekMenu = new List<MealMenu>();

            foreach (var date in datesOfWeek)
            {
                if (menus.Exists(x => x.Date.Date == date.Date))
                {
                    nextWeekMenu.Add(menus.Find(x => x.Date.Date == date.Date));
                }
            }

            return nextWeekMenu != null ? CreateAttachmentsAsync(context, nextWeekMenu) : null;
        }

        private List<Attachment> CreateAttachmentsAsync(IDialogContext context, List<MealMenu> menus)
        {
            var attachments = new List<Attachment>();
            foreach (var menu in menus)
            {
                if (menu.IsExistMenu)
                {
                    attachments.Add(CreateAttachmentAsync(context, menu));
                }
            }

            return attachments;
        } 

        private Attachment CreateAttachmentAsync(IDialogContext context, MealMenu menu)
        {
            var imageBytes = MenuFactory.MakeImage(context.Activity.Conversation.Id, menu);
            var image64 = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);

            return new Attachment
            {
                ContentUrl = image64,
                ContentType = "image/jpeg",
                Name = GetDateString(menu.Date.Date)
            };
        }

        private async Task<List<MealMenu>> GetSchoolMealListAsync(IDialogContext context)
        {
            List<MealMenu> menus = null;

            var meal = new Meal(Util.ConvertRegions(schoolInfo.Region), Util.ConvertSchoolTypes(schoolInfo.SchoolType), schoolInfo.Code);
            try
            {
                menus = meal.GetMealMenu();
            }
            catch (SchoolMeal.Exception.FaildToParseException ex)
            {
                await context.PostAsync("급식정보를 가져오는 도중에 문제가 발생 했어요 :( " + ex.Message);
                context.Done<object>(null);
            }

            return menus;
        }

        /// <summary>
        /// 매개변수로 받는 급식메뉴 리스트를 가공해서 메시지로 보낸 뒤 해당 Dialog를 끝냅니다.
        /// 매개변수의 리스트가 비어있다면 리스트를 가공하지 않고 급식메뉴가 없다는 메시지를 보냅니다.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="menus">메시지로 보낼 급식메뉴</param>
        /// <returns></returns>
        private async Task ShowMessageAsync(IDialogContext context, List<MealMenu> menus)
        {
            List<string> menu_list = new List<string>();
            foreach (var menu in menus)
            {
                if (menu.IsExistMenu)
                {
                    var resultMenu = new StringBuilder();
                    resultMenu.AppendLine(GetDateString(menu.Date.Date));
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
                        resultMenu.AppendLine("[점심]");
                        foreach (var lunchMenu in menu.Lunch)
                        {
                            resultMenu.AppendLine(lunchMenu);
                        }
                    }
                    if (menu.Dinner != null)
                    {
                        resultMenu.AppendLine("[저녁]");
                        foreach (var dinnerMenu in menu.Dinner)
                        {
                            resultMenu.AppendLine(dinnerMenu);
                        }
                    }
                    menu_list.Add(resultMenu.ToString());
                }
                
            }

            if (menu_list.Count == 0)
            {
                await context.PostAsync("선택한 날짜에 해당하는 급식메뉴가 없네요.");
            }
            else
            {
                foreach (var menu in menu_list)
                {
                    await context.PostAsync(menu);
                }
            }

            context.Done<object>(null);
        }

        private string GetDateString(DateTime date)
        {
            string dateString = $"{date.Month}월 {date.Day}일 ";
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    dateString += "일요일";
                    break;
                case DayOfWeek.Monday:
                    dateString += "월요일";
                    break;
                case DayOfWeek.Tuesday:
                    dateString += "화요일";
                    break;
                case DayOfWeek.Wednesday:
                    dateString += "수요일";
                    break;
                case DayOfWeek.Thursday:
                    dateString += "목요일";
                    break;
                case DayOfWeek.Friday:
                    dateString += "금요일";
                    break;
                case DayOfWeek.Saturday:
                    dateString += "토요일";
                    break;
                default:
                    break;
            }

            return dateString;
        }

        private List<DateTime> GetDatesOfWeek(DateTime date)
        {
            List<DateTime> dates = new List<DateTime> { date };

            int num = (int)DateTime.Now.DayOfWeek;
            for (int i = 1; i <= num; i++)
            {
                dates.Add(date.Subtract(new TimeSpan(i, 0, 0, 0, 0)));
            }
            for (int i = 1; i <= 6 - num; i++)
            {
                dates.Add(date.AddDays(i));
            }

            dates.Sort((x, y) => x.CompareTo(y));

            return dates;
        }
    }
}