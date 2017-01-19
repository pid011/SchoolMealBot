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
    public class SchoolInfoConfigDialog : IDialog<SchoolInfo>
    {
        /// <summary>
        /// 클래스 리턴값
        /// </summary>
        private SchoolInfo userSchoolInfo;

#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            var configFormDialog = FormDialog.FromForm(BuildSchoolInfoConfigForm, FormOptions.PromptInStart);
            context.Call(configFormDialog, ResumeAfterSchoolInfoConfigFormAsync);
        }

        private async Task ResumeAfterSchoolInfoConfigFormAsync(IDialogContext context, IAwaitable<SchoolConfigQuery> result)
        {
            try
            {
                var query = await result;

                this.userSchoolInfo = new SchoolInfo
                {
                    SchoolType = Util.ConvertSchoolTypes(query.SchoolType),
                    Region = Util.ConvertRegions(query.SchoolRegion)
                };

            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "학교정보 설정을 취소했어요.";
                }
                else
                {
                    reply = $"이런! 오류가 발생했어요! :( {ex.InnerException.Message}";
                }

                this.userSchoolInfo = null;
                await context.PostAsync(reply);
            }
            finally
            {
                context.Done(this.userSchoolInfo);
            }
        }

        private IForm<SchoolConfigQuery> BuildSchoolInfoConfigForm()
        {
            return new FormBuilder<SchoolConfigQuery>()
                .Message("제가 하라는대로 해주시면 돼요!")
                .Field(nameof(SchoolConfigQuery.SchoolRegion))
                .Field(nameof(SchoolConfigQuery.SchoolType))
                .Build();
        }
    }
}