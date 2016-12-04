using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using SchoolMeal;
using SchoolMealBot.Models;
using System.Threading;

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

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (!context.ConversationData.TryGetValue(ContextConstants.SchoolMealettingKey, out Settings settings))
            {
                await context.PostAsync("안녕하세요! 처음뵈는 주인님!  ヾ(｡･ω･)ｼ");
                context.Call(new SettingDialog(), ResumeAfterSettingDialogAsync);
            }
            else
            {
                await context.Forward(new SchoolMealDialog(), ResumeAfterSchoolMealDialogAsync, message, CancellationToken.None);
                await context.PostAsync($"1: {settings.SchoolCode}, 2: {settings.SchoolRegion.ToString()}, 3: {settings.SchoolType}");
            }
        }

        private async Task ResumeAfterSettingDialogAsync(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
                await context.PostAsync("설정작업이 끝났어요. 다시 말걸어주세요.");
            }
            catch (Exception ex)
            {
                await context.PostAsync("알수없는 이유로 작업을 실패했어요...");
                await context.PostAsync($"원인: {ex.Message}");
            }
            finally
            {
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSchoolMealDialogAsync(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
        }
    }
}