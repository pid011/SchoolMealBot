using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolMeal;
using SchoolFinder;
using System.Text;

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
            PromptDialog.Choice(context, ResumeAfterChoicedAsync, this.options,
                    "원하시는 옵션을 선택해주세요!", "목록에서 원하는 옵션을 선택해주세요!");
        }

        private async Task ResumeAfterChoicedAsync(IDialogContext context, IAwaitable<string> result)
        {
            var resultType = await result;

            var mealMenu = await GetSchoolMealListAsync(context);

            if (mealMenu != null)
            {
                switch (resultType)
                {
                    case TodaysSchoolMealOption:
                        await ShowTodaysSchoolMealMenuAsync(context, mealMenu);
                        break;

                    case TomorrowsSchoolMealOption:
                        await ShowTomorrowsSchoolMealMenuAsync(context, mealMenu);
                        break;

                    case SchoolMealThisWeekOption:
                        await ShowSchoolMealThisWeekMenuAsync(context, mealMenu);
                        break;

                    case SchoolMealNextWeekOption:
                        await ShowSchoolMealNextWeekMenuAsync(context, mealMenu);
                        break;

                    default:
                        break;
                }
            }
        }

        private async Task ShowTodaysSchoolMealMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var todaysDate = DateTime.Now;
            var todayMenu = new List<MealMenu>();
            if (menus.Exists(x => x.Date == todaysDate))
            {
                todayMenu.Add(menus.Find(x => x.Date.Date == todaysDate.Date));
            }
            await ShowMessageAsync(context, todayMenu);
        }

        private async Task ShowTomorrowsSchoolMealMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var tomorrowDate = DateTime.Now;
            var tomorrowMenu = new List<MealMenu>();
            if (menus.Exists(x => x.Date == tomorrowDate))
            {
                tomorrowMenu.Add(menus.Find(x => x.Date.Date == tomorrowDate.Date));
            }
            await ShowMessageAsync(context, tomorrowMenu);
        }

        private async Task ShowSchoolMealThisWeekMenuAsync(IDialogContext context, List<MealMenu> menus)
        {
            var todayDate = DateTime.Now;
            var datesOfWeek = GetDatesOfWeek(todayDate);
            var thisWeekMenu = new List<MealMenu>();

            foreach (var date in datesOfWeek)
            {
                if (menus.Exists(x => x.Date.Date == date.Date))
                {
                    thisWeekMenu.Add(menus.Find(x => x.Date.Date == date.Date));
                }
            }
            await ShowMessageAsync(context, thisWeekMenu);
        }

        private async Task ShowSchoolMealNextWeekMenuAsync(IDialogContext context, List<MealMenu> menus)
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
            await ShowMessageAsync(context, nextWeekMenu);
        }

        private async Task<List<MealMenu>> GetSchoolMealListAsync(IDialogContext context)
        {
            List<MealMenu> menus = null;

            var meal = new Meal(Util.ConvertRegions(this.schoolInfo.Region), Util.ConvertSchoolTypes(this.schoolInfo.SchoolType), this.schoolInfo.Code);
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