using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using SchoolFinder;
using Microsoft.Bot.Connector;

namespace SchoolMealBot.Dialogs
{
    [Serializable]
    public class ResetDialog : IDialog<object>
    {
#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            var yesno = ((IEnumerable<Util.YesNo>)Enum.GetValues(typeof(Util.YesNo))).Select(x => x);
            PromptDialog.Choice(context, OnSeletedYesNoAsync, yesno, "정말로 학교정보를 초기화 할까요?", "정확하게 알려주세요!", promptStyle: PromptStyle.Keyboard);
        }

        private async Task OnSeletedYesNoAsync(IDialogContext context, IAwaitable<Util.YesNo> result)
        {
            var choiced = await result;

            if (choiced == Util.YesNo.그래)
            {
                if (context.ConversationData.RemoveValue(ContextConstants.SchoolConfigKey))
                {
                    await context.PostAsync("학교정보 초기화에 성공했어요.");
                }
                else
                {
                    await context.PostAsync("학교정보 초기화에 실패했어요. 저장된 학교정보가 없어요.");
                }
            }
            else
            {
                await context.PostAsync("학교정보 초기화를 취소했어요.");
            }

            context.Done<object>(null);
        }
    }
}