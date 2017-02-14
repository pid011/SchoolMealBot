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
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;

namespace SchoolMealBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string SettingsOption = "설정";
        private const string SchoolMealOption = "급식메뉴 보기";
        private const string ViewDateOption = "현재 날짜";
        private const string RemoveUserDataOption = "유저정보 삭제";

        private readonly List<string> options = new List<string>
        {
            SettingsOption,
            SchoolMealOption,
            ViewDateOption,
            RemoveUserDataOption
        };

#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            await ShowOptionsAsync(context);
        }

        private async Task ShowOptionsAsync(IDialogContext context)
        {
            var check = context.ConversationData.TryGetValue(ContextConstants.SchoolConfigKey, out SchoolInfo botInfo);
            if (!check || botInfo == null)
            {
                await context.PostAsync("먼저 지금 다니는 학교를 설정해야 해요.");
                context.Call(new SchoolInfoConfigDialog(), OnConfigSchoolInfoAsync);
            }
            else
            {
                PromptDialog.Choice(context, OnOptionSelectedAsync, this.options, 
                    "무엇을 도와드릴까요?", "목록에서 원하는 작업을 선택해주세요!", promptStyle: PromptStyle.Keyboard);
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

                    case SchoolMealOption:
                        context.Call(new SchoolMealDialog(), AfterShowsSchoolMealListAsync);
                        break;

                    case ViewDateOption:
                        await ViewCurrentDate(context);
                        break;

                    case RemoveUserDataOption:
                        await RemoveUserData(context);
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

        private async Task OnResetCompletedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await ShowOptionsAsync(context);
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

        private async Task ViewCurrentDate(IDialogContext context)
        {
            var todaysDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Today, "Korea Standard Time");
            await context.PostAsync($"현재 날짜는 {todaysDate.ToLongDateString()}입니다.");
            await ShowOptionsAsync(context);
        }

        private async Task RemoveUserData(IDialogContext context)
        {
            var yesno = ((IEnumerable<Util.YesNo>)Enum.GetValues(typeof(Util.YesNo))).Select(x => x);
            PromptDialog.Choice(context, OnSeletedYesNoAsync, yesno, "정말로 유저정보를 삭제할까요?", "정확하게 알려주세요!", promptStyle: PromptStyle.Keyboard);
        }

        private async Task OnSeletedYesNoAsync(IDialogContext context, IAwaitable<Util.YesNo> result)
        {
            if (context.Activity is Activity activity)
            {
                try
                {
                    context.ConversationData.Clear();
                    await context.PostAsync("유저정보 삭제에 성공했어요. 다시 말을걸면 봇이 재시작됩니다 :l");
                    context.Done<object>(null);
                }
                catch (Exception ex)
                {
                    await context.PostAsync("유저정보 삭제에 실패했어요 :( " + ex.Message);
                }
            }
            else
            {
                await context.PostAsync("유저정보 삭제에 실패했어요 :(");
            }
        }
    }
}