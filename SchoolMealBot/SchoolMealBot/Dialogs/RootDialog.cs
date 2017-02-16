using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using SchoolFinder;

namespace SchoolMealBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private SchoolInfo schoolInfo;

        private const string SettingsOption = "설정";
        private const string SchoolMealOption = "급식메뉴 보기";
        private const string RemoveUserDataOption = "유저정보 삭제";

        private readonly List<string> options = new List<string>
        {
            SchoolMealOption,
            SettingsOption,
            RemoveUserDataOption
        };
        private bool welcomed;

#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("안녕하세요! 저는 급식봇이에요. 급식메뉴를 알려준답니다.");
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (!context.ConversationData.TryGetValue(ContextConstants.SchoolConfigKey, out SchoolInfo schoolInfo))
            {
                this.welcomed = true;
                await context.PostAsync("저장되어있는 정보가 없어 설정을 시작합니다.");
                context.Call(new SchoolInfoConfigDialog(), OnConfigSchoolInfoAsync);
            }
            else
            {
                if (!this.welcomed)
                {
                    this.welcomed = true;
                    await context.PostAsync("🎉 다시 오신걸 환영합니다! 🎉");
                }
                PromptDialog.Choice(context, OnOptionSelectedAsync, this.options,
                    "무엇을 도와드릴까요?", "목록에서 원하는 작업을 선택해주세요!");
            }
        }

        private async Task OnOptionSelectedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case SchoolMealOption:
                        context.Call(new SchoolMealDialog(this.schoolInfo), AfterShowsSchoolMealListAsync);
                        break;

                    case SettingsOption:
                        context.Call(new SchoolInfoConfigDialog(), OnConfigSchoolInfoAsync);
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
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task AfterShowsSchoolMealListAsync(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
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
                this.schoolInfo = info;
                await context.PostAsync("설정을 완료했어요!");
            }
            else
            {
                await context.PostAsync("설정을 중단했어요!");
            }
            context.Wait(MessageReceivedAsync);
        }

        private async Task RemoveUserData(IDialogContext context)
        {
            var yesno = ((IEnumerable<Util.YesNo>)Enum.GetValues(typeof(Util.YesNo))).Select(x => x);
            PromptDialog.Choice(context, OnSeletedYesNoAsync, yesno, "정말로 유저정보를 삭제할까요?", "정확하게 알려주세요!");
        }

        private async Task OnSeletedYesNoAsync(IDialogContext context, IAwaitable<Util.YesNo> result)
        {
            var choiced = await result;

            switch (choiced)
            {
                case Util.YesNo.그래:
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
                                context.Wait(MessageReceivedAsync);
                            }
                        }
                        else
                        {
                            await context.PostAsync("유저정보 삭제에 실패했어요 :(");
                            context.Wait(MessageReceivedAsync);
                        }
                    }
                    break;

                case Util.YesNo.아니:
                    await context.PostAsync("유저정보 삭제를 취소했어요 :D");
                    context.Wait(MessageReceivedAsync);
                    break;

                default:
                    break;
            }
            
        }
    }
}