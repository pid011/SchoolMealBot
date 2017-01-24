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
                            break;
                        case ResultType.SchoolMealThisWeek:
                            break;
                        case ResultType.SchoolMealNextWeek:
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
            var menu = menus.Find(x => x.Date == tomorrowDate);
            if (menu.IsExistMenu == false)
            {
                await context.PostAsync("선택한 날짜에 해당하는 급식메뉴가 없네요.");
                context.Done<object>(null);
            }
            else
            {
                await ShowMessageAsync(context, new List<MealMenu>() { menu });
            }
        }

        private async Task ShowSchoolMealThisWeekMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var todayDate = DateTime.Today;
            var datesOfWeek = GetDatesOfWeek(todayDate);

            foreach (var date in datesOfWeek)
            {
                // TODO: 메뉴리스트에서 일주일 날짜를 걸러내서 리스트에 따로 빼기
            }
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
                await context.PostAsync("선택하신 날의 급식메뉴가 존재하지 않네요...");
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