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
    public class SchoolInfoSettingDialog : IDialog<SchoolInfo>
    {
        private string schoolName;
        private SchoolInfo userSchoolInfo;

#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("설정을 시작중이에요...");
            var settingFormDialog = FormDialog.FromForm(BuildSchoolInfoSettingForm, FormOptions.PromptInStart);

            context.Call(settingFormDialog, ResumeAfterSchoolInfoSettingFormDialogAsync);
        }

        private IForm<SchoolConfigQuery> BuildSchoolInfoSettingForm()
        {
            return new FormBuilder<SchoolConfigQuery>()
                .Message("제가 하라는대로 해주시면 돼요!")
                .Field(nameof(SchoolConfigQuery.SchoolRegion))
                .Field(nameof(SchoolConfigQuery.SchoolType))
                .Build();
        }

        private async Task ResumeAfterSchoolInfoSettingFormDialogAsync(IDialogContext context, IAwaitable<SchoolConfigQuery> result)
        {
            try
            {
                var queryInfo = await result;

                while (true)
                {
                    await GetSchoolNameAsync(context);

                    if (this.schoolName == null)
                    {
                        continue;
                    }
                    var search = new SchoolSearch();

                    var resultList = search.SearchSchool
                        (Util.ConvertSchoolTypes(queryInfo.SchoolType), Util.ConvertRegions(queryInfo.SchoolRegion), this.schoolName);

                    if (resultList.Count == 1)
                    {
                        this.userSchoolInfo = resultList.First();
                        break;
                    }
                    else
                    {
                        var resultMsg = context.MakeMessage();
                        resultMsg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        resultMsg.Attachments = new List<Attachment>();

                        foreach (var item in resultList)
                        {
                            HeroCard heroCard = new HeroCard()
                            {
                                Title = item.Name,
                                Text = item.Adress
                            };
                            resultMsg.Attachments.Add(heroCard.ToAttachment());
                        }
                        await context.PostAsync("여러개의 검색결과가 있어요! 정확하게 다시 알려주세요...");
                        await context.PostAsync(resultMsg);
                    }
                }                

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
                this.userSchoolInfo = null;
                await context.PostAsync(reply);
            }
            finally
            {
                context.Done(this.userSchoolInfo);
            }
        }
        
        private Task GetSchoolNameAsync(IDialogContext context)
        {
            PromptDialog.Text(context, ResumeAfterSearchSchoolFormAsync, "주인님이 현재 다니는 학교의 이름을 써주세요!");

            return Task.CompletedTask;
        }

        private async Task ResumeAfterSearchSchoolFormAsync(IDialogContext context, IAwaitable<string> input)
        {
            string text = input != null ? await input : null;

            if (text != null)
            {
                this.schoolName = text;
            }
            else
            {
                this.schoolName = null;
            }
        }
    }
}