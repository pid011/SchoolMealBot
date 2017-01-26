using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using SchoolMeal;
using System.Threading;
using SchoolFinder;

namespace SchoolMealBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string SettingsOption = "설정";
        private const string TodaysSchoolMealOption = "오늘급식";
        private const string TomorrowsSchoolMealOption = "내일급식";
        private const string SchoolMealThisWeekOption = "이번주급식";
        private const string SchoolMealNextWeekOption = "다음주급식";

        private readonly List<string> options = new List<string>
        {
            SettingsOption,
            TodaysSchoolMealOption,
            TomorrowsSchoolMealOption,
            SchoolMealThisWeekOption,
            SchoolMealNextWeekOption
        };

#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("안녕하세요! 저는 급식메뉴를 알려주는 봇이에요!");
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            await ShowOptionsAsync(context);
        }

        private async Task ShowOptionsAsync(IDialogContext context)
        {
            if (!context.ConversationData.TryGetValue(ContextConstants.SchoolConfigKey, out SchoolInfo botInfo))
            {
                await context.PostAsync("먼저 지금 다니는 학교를 설정해야 해요.");
                context.Call(new SchoolInfoConfigDialog(), OnConfigSchoolInfoAsync);
            }
            else
            {
                PromptDialog.Choice(context, OnOptionSelectedAsync, this.options, "무엇을 도와드릴까요?", "존재하지 않는 명령목록이에요.");
            }
        }


        private async Task OnOptionSelectedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case SettingsOption:
                        context.Call(new SchoolInfoConfigDialog(), OnConfigSchoolInfoAsync);
                        break;

                    case TodaysSchoolMealOption:
                        context.Call(new SchoolMealDialog(SchoolMealDialog.ResultType.TodaysSchoolMeal), AfterShowsSchoolMealListAsync);
                        break;

                    case TomorrowsSchoolMealOption:
                        context.Call(new SchoolMealDialog(SchoolMealDialog.ResultType.TomorrowsSchoolMeal), AfterShowsSchoolMealListAsync);
                        break;

                    case SchoolMealThisWeekOption:
                        context.Call(new SchoolMealDialog(SchoolMealDialog.ResultType.SchoolMealThisWeek), AfterShowsSchoolMealListAsync);
                        break;

                    case SchoolMealNextWeekOption:
                        context.Call(new SchoolMealDialog(SchoolMealDialog.ResultType.SchoolMealNextWeek), AfterShowsSchoolMealListAsync);
                        break;

                    default:
                        break;
                }
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("시도횟수가 너무 많아요. 다시 한번 시도해주세요. :<");
                await ShowOptionsAsync(context);
            }
        }

        private async Task AfterShowsSchoolMealListAsync(IDialogContext context, IAwaitable<object> result)
        {
            await ShowOptionsAsync(context);
        }

        private async Task OnConfigSchoolInfoAsync(IDialogContext context, IAwaitable<SchoolInfo> result)
        {
            var info = await result;
            context.Call(new SearchSchoolDialog(info), OnSearchSchoolAsync);
        }

        private async Task OnSearchSchoolAsync(IDialogContext context, IAwaitable<SchoolInfo> result)
        {
            var info = await result;
            if (info != null)
            {
                context.ConversationData.SetValue(ContextConstants.SchoolConfigKey, info);
                await context.PostAsync("설정을 완료했어요!");
                await ShowOptionsAsync(context);
            }
            else
            {
                await context.PostAsync("설정을 중단했어요!");
                await context.PostAsync("저에게 다시 말을 걸어주세요!");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}