using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using SchoolMeal;
using SchoolFinder;
using System.Text;

namespace SchoolMealBot.Dialogs
{
    [Serializable]
    public class SchoolMealDialog : IDialog<object>
    {
        private ResultType resultType;

        public SchoolMealDialog(ResultType type)
        {
            this.resultType = type;
        }

#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            try
            {
                var mealMenu = await GetSchoolMealListAsync(context);

                if (mealMenu != null)
                {
                    switch (this.resultType)
                    {
                        case ResultType.TodaysSchoolMeal:
                            await ShowTodaysSchoolMealMenuAsync(context, mealMenu);
                            break;

                        case ResultType.TomorrowsSchoolMeal:
                            await ShowTomorrowsSchoolMealMenuAsync(context, mealMenu);
                            break;

                        case ResultType.SchoolMealThisWeek:
                            await ShowSchoolMealThisWeekMenuAsync(context, mealMenu);
                            break;

                        case ResultType.SchoolMealNextWeek:
                            await ShowSchoolMealNextWeekMenuAsync(context, mealMenu);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (SchoolMeal.Exception.FaildToParseException ex)
            {
                await context.PostAsync($"급식메뉴를 가져오는중 오류가 발생했어요 :( {ex.InnerException.Message}");
                context.Done<object>(null);
            }
            
        }

        private async Task ShowTodaysSchoolMealMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var todaysDate = DateTime.Today;
            var list = new List<MealMenu>
            {
                menus.Find(x => x.Date == todaysDate)
            };
            await ShowMessageAsync(context, list);
        }

        private async Task ShowTomorrowsSchoolMealMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var tomorrowDate = DateTime.Today.AddDays(1);
            var tomorrowMenu = new List<MealMenu>();
            if (menus.Exists(x => x.Date == tomorrowDate))
            {
                tomorrowMenu.Add(menus.Find(x => x.Date == tomorrowDate));
            }
            await ShowMessageAsync(context, tomorrowMenu);
        }

        private async Task ShowSchoolMealThisWeekMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var todayDate = DateTime.Today;
            var datesOfWeek = GetDatesOfWeek(todayDate);
            var thisWeekMenu = new List<MealMenu>();

            foreach (var date in datesOfWeek)
            {
                if (menus.Exists(x => x.Date == date))
                {
                    thisWeekMenu.Add(menus.Find(x => x.Date == date));
                }
            }
            await ShowMessageAsync(context, thisWeekMenu);
        }

        private async Task ShowSchoolMealNextWeekMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var NextWeekDate = DateTime.Today.AddDays(7);
            var datesOfWeek = GetDatesOfWeek(NextWeekDate);
            var nextWeekMenu = new List<MealMenu>();

            foreach (var date in datesOfWeek)
            {
                if (menus.Exists(x => x.Date == date))
                {
                    nextWeekMenu.Add(menus.Find(x => x.Date == date));
                }
            }
            await ShowMessageAsync(context, nextWeekMenu);
        }

        private async Task<List<MealMenu>> GetSchoolMealListAsync(IDialogContext context)
        {
            List<MealMenu> menus = null;
            if (!context.ConversationData.TryGetValue(ContextConstants.SchoolConfigKey, out SchoolInfo schoolInfo))
            {
                await context.PostAsync("저장되어있는 학교정보가 없어요!");
            }
            else
            {
                var meal = new Meal(Util.ConvertRegions(schoolInfo.Region), Util.ConvertSchoolTypes(schoolInfo.SchoolType), schoolInfo.Code);
                menus = meal.GetMealMenu();
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
            var resultMsg = context.MakeMessage();
            resultMsg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMsg.Attachments = new List<Attachment>();

            foreach (var menu in menus)
            {
                if (menu.IsExistMenu)
                {
                    var resultMenu = new StringBuilder();
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

                    HeroCard heroCard = new HeroCard
                    {
                        Title = $"{menu.Date.Month}월 {menu.Date.Day}일",
                        Text = resultMenu.ToString()
                    };
                    resultMsg.Attachments.Add(heroCard.ToAttachment());
                }
                
            }

            if (resultMsg.Attachments.Count == 0)
            {
                await context.PostAsync("선택한 날짜에 해당하는 급식메뉴가 없네요.");
            }
            else
            {
                await context.PostAsync(resultMsg);
            }

            context.Done<object>(null);
        }

        private List<DateTime> GetDatesOfWeek(DateTime date)
        {
            List<DateTime> dates = new List<DateTime> { date };
            int num = 0;

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    num = 0;
                    break;
                case DayOfWeek.Tuesday:
                    num = 1;
                    break;
                case DayOfWeek.Wednesday:
                    num = 2;
                    break;
                case DayOfWeek.Thursday:
                    num = 3;
                    break;
                case DayOfWeek.Friday:
                    num = 4;
                    break;
                case DayOfWeek.Saturday:
                    num = 5;
                    break;
                case DayOfWeek.Sunday:
                    num = 6;
                    break;
                default:
                    break;
            }

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

        public enum ResultType
        {
            TodaysSchoolMeal,
            TomorrowsSchoolMeal,
            SchoolMealThisWeek,
            SchoolMealNextWeek
        }
    }
}