using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolMeal;
using System.Text;
using Microsoft.Bot.Connector;
using SchoolMealBot.Core.Menu;
using SchoolMealBot.Core.School;
using System.Linq;

namespace SchoolMealBot.Dialogs
{
    [Serializable]
    public class SchoolMealDialog : IDialog<object>
    {
        private MenuGenerator generator = null;

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
            if (schoolInfo != null)
            {
                generator = new MenuGenerator(schoolInfo);
            }
            
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
            var replyMsg = context.MakeMessage();

            List<Attachment> attachments = null;

            if (generator != null)
            {
                switch (resultType)
                {
                    case TodaysSchoolMealOption:
                        attachments = await GetTodaysSchoolMealMenuAsync(context);
                        break;

                    case TomorrowsSchoolMealOption:
                        attachments = await GetTomorrowsSchoolMealMenuAsync(context);
                        break;

                    case SchoolMealThisWeekOption:
                        attachments = await GetSchoolMealThisWeekMenuAsync(context);
                        break;

                    case SchoolMealNextWeekOption:
                        attachments = await GetSchoolMealNextWeekMenuAsync(context);
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

        private async Task<List<Attachment>> GetTodaysSchoolMealMenuAsync(IDialogContext context)
        {
            var todaysDate = DateTime.Now;

            var todayMenu = (await GetSchoolMealListAsync(context, new List<DateTime>() { todaysDate })).First();

            return todayMenu != null ? CreateAttachmentsAsync(context, new List<MealMenu> { todayMenu }) : null;
        }

        private async Task<List<Attachment>> GetTomorrowsSchoolMealMenuAsync(IDialogContext context)
        {
            var tomorrowDate = DateTime.Now.AddDays(1);
            var tomorrowMenu = (await GetSchoolMealListAsync(context, new List<DateTime>() { tomorrowDate })).First();

            return tomorrowMenu != null ? CreateAttachmentsAsync(context, new List<MealMenu> { tomorrowMenu }) : null;
        }

        private async Task<List<Attachment>> GetSchoolMealThisWeekMenuAsync(IDialogContext context)
        {
            var todayDate = DateTime.Now;
            var datesOfWeek = GetDatesOfWeek(todayDate);
            var thisWeekMenu = await GetSchoolMealListAsync(context, datesOfWeek);

            return thisWeekMenu != null ? CreateAttachmentsAsync(context, thisWeekMenu) : null;
        }

        private async Task<List<Attachment>> GetSchoolMealNextWeekMenuAsync(IDialogContext context)
        {
            var NextWeekDate = DateTime.Now.AddDays(7);
            var datesOfWeek = GetDatesOfWeek(NextWeekDate);
            var nextWeekMenu = await GetSchoolMealListAsync(context, datesOfWeek);

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
                Name = Util.GetStringOfDate(menu.Date.Date)
            };
        }
        

        private async Task<List<MealMenu>> GetSchoolMealListAsync(IDialogContext context, List<DateTime> dates)
        {
            List<MealMenu> menus = null;
            try
            {
                menus = generator.GetSchoolMealMenu(dates);
            }
            catch (SchoolMeal.Exception.FaildToParseException ex)
            {
                await context.PostAsync("급식정보를 가져오는 도중에 문제가 발생 했어요 :( \n" + ex.Message);
                context.Done<object>(null);
            }

            return menus;
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