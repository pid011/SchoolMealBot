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
    public class SearchSchoolDialog : IDialog<SchoolInfo>
    {
        private SchoolInfo userSchoolInfo;

        public SearchSchoolDialog(SchoolInfo info)
        {
            this.userSchoolInfo = info;
        }

#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Text(context, OnGetSchoolNameAsync, "현재 다니는 학교의 이름을 알려주세요! ([ exit ] => 설정 종료)");
        }

        private async Task OnGetSchoolNameAsync(IDialogContext context, IAwaitable<string> result)
        {
            var msg = await result;

            if (msg == "exit" || msg == null)
            {
                await context.PostAsync("학교 검색을 나가는 중이에요!");
                context.Done<SchoolInfo>(null);
            }
            else
            {
                var schoolName = msg;

                var search = new SchoolSearch();
                var searchResults = search.SearchSchool(this.userSchoolInfo.SchoolType, this.userSchoolInfo.Region, schoolName);

                if (searchResults.Count == 1)
                {
                    // TODO: 학교정보 확인
                    context.Done(searchResults.First());
                }
                else if (searchResults.Count > 1)
                {
                    await context.PostAsync("여러개의 검색결과가 있어요! 정확하게 다시 알려주세요...");

                    var resultMsg = context.MakeMessage();
                    resultMsg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    resultMsg.Attachments = new List<Attachment>();
                    foreach (var item in searchResults)
                    {
                        HeroCard heroCard = new HeroCard
                        {
                            Title = item.Name,
                            Text = item.Adress
                        };
                        resultMsg.Attachments.Add(heroCard.ToAttachment());
                    }
                    await context.PostAsync(resultMsg);
                    PromptDialog.Text(context, OnGetSchoolNameAsync, "현재 다니는 학교의 이름을 알려주세요! ([ /exit ] => 설정 종료)");
                }
                else if (searchResults.Count < 1)
                {
                    await context.PostAsync("검색결과가 존재하지 않네요... 다시한번 정확히 알려주세요!");
                    PromptDialog.Text(context, OnGetSchoolNameAsync, "현재 다니는 학교의 이름을 알려주세요! ([ /exit ] => 설정 종료)");
                }
                else
                {
                    await context.PostAsync("검색 중 문제가 발생했어요! :(");
                    context.Done<SchoolInfo>(null);
                }
            }

            
        }
    }
}