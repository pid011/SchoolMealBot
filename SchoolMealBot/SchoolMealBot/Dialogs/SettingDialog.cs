using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using SchoolMeal;
using SchoolMealsBot.Models;
using Microsoft.Bot.Connector;

namespace SchoolMealsBot.Dialogs
{
    [Serializable]
    public class SettingDialog : IDialog<object>
    {
#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("급식봇 설정을 시작중이에요...");
            var settingFormDialog = FormDialog.FromForm(BuildSettingForm, FormOptions.PromptInStart);

            context.Call(settingFormDialog, ResumeAfterSchoolMealsFormDialogAsync);
        }

        private IForm<SchoolMealsQuery> BuildSettingForm()
        {
            OnCompletionAsyncDelegate<SchoolMealsQuery> processSchoolMealsSetting = async (context, state) =>
            {
            };

            return new FormBuilder<SchoolMealsQuery>()
                .Message("급식봇을 설정할게요. 주인님!")
                .Field(nameof(SchoolMealsQuery.SchoolRegion))
                .Field(nameof(SchoolMealsQuery.SchoolType))
                .Field(nameof(SchoolMealsQuery.SchoolCode))
                .Confirm(async state => 
                    {
                        return new PromptAttribute($"주인님이 설정한 교육기관의 관할지역은 {state.SchoolRegion}이고 종류는 {state.SchoolType}, 고유코드는 {state.SchoolCode} 이에요. 맞나요? 맞으면 [yes], 아니면 [no]를 입력해주세요. (*´･∀･)");
                    })
                .AddRemainingFields()
                .OnCompletion(processSchoolMealsSetting)
                .Build();
        }

        private async Task ResumeAfterSchoolMealsFormDialogAsync(IDialogContext context, IAwaitable<SchoolMealsQuery> result)
        {
            try
            {
                var schoolMealsQuery = await result;

                var settings = new Settings()
                {
                    SchoolRegion = (Regions) schoolMealsQuery.SchoolRegion,
                    SchoolType = (SchoolType) schoolMealsQuery.SchoolType,
                    SchoolCode = schoolMealsQuery.SchoolCode
                };

                context.ConversationData.SetValue(ContextConstants.SchoolMealSettingKey, settings);
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