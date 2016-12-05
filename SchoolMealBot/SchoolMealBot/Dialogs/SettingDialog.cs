using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using SchoolMeal;
using SchoolMealBot.Models;
using Microsoft.Bot.Connector;

namespace SchoolMealBot.Dialogs
{
    [Serializable]
    public class SettingDialog : IDialog<object>
    {
#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("설정을 시작중이에요...");
            var settingFormDialog = FormDialog.FromForm(BuildSettingForm, FormOptions.PromptInStart);

            context.Call(settingFormDialog, ResumeAfterSchoolMealFormDialogAsync);
        }

        private IForm<SchoolMealQuery> BuildSettingForm()
        {
            OnCompletionAsyncDelegate<SchoolMealQuery> processSchoolMealSetting = async (context, state) =>
            {
                await context.PostAsync("설정을 끝냈어요. \\(●⁰౪⁰●\\)(//●⁰౪⁰●)//");
            };

            return new FormBuilder<SchoolMealQuery>()
                .Message("제가 하라는대로 해주시면 돼요!")
                .Field(nameof(SchoolMealQuery.SchoolRegion))
                .Field(nameof(SchoolMealQuery.SchoolType))
                .Field(nameof(SchoolMealQuery.SchoolCode))
                .Message(async state => 
                    {
                        return new PromptAttribute($"주인님이 설정한 교육기관의 관할지역은 {state.SchoolRegion}이고 종류는 {state.SchoolType}, 고유코드는 {state.SchoolCode} 이에요. (*´･∀･)");
                    })
                .AddRemainingFields()
                .OnCompletion(processSchoolMealSetting)
                .Build();
        }

        private async Task ResumeAfterSchoolMealFormDialogAsync(IDialogContext context, IAwaitable<SchoolMealQuery> result)
        {
            try
            {
                var SchoolMealQuery = await result;

                var settings = new Settings()
                {
                    SchoolRegion = (Regions)((int)SchoolMealQuery.SchoolRegion - 1),
                    SchoolType = (SchoolType)((int)SchoolMealQuery.SchoolType - 1),
                    SchoolCode = SchoolMealQuery.SchoolCode
                };

                context.ConversationData.SetValue(ContextConstants.SchoolMealettingKey, settings);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "설정 작업을 취소했어요. 설정작업을 나갈게요.";
                }
                else
                {
                    reply = $"이런! 문제가 발생했어요! :( \n원인: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }
    }
}