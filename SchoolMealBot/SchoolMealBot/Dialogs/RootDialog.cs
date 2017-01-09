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

#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (!context.ConversationData.TryGetValue(ContextConstants.SchoolConfigKey, out SchoolInfo botInfo))
            {
                await context.PostAsync("안녕하세요! 처음뵈는 주인님!  ヾ(｡･ω･)ｼ");
                await context.PostAsync("저장되어있는 학교정보가 없으니 설정을 시작할게요 (* ^ ω ^)");
                context.Call(new SchoolInfoConfigDialog(), OnConfigSchoolInfoAsync);
            }
            else
            {
                await context.PostAsync($"1: {botInfo.Code}, 2: {botInfo.Region.ToString()}, 3: {botInfo.SchoolType}");
                context.Wait(MessageReceivedAsync);
            }

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
            }
            else
            {
                await context.PostAsync("설정을 중단했어요! :<");
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}