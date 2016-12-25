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
    public class SetSchoolConfigDialog : IDialog<object>
    {
#pragma warning disable CS1998
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("설정을 시작중이에요...");
            var settingFormDialog = FormDialog.FromForm(BuildSetSchoolConfigForm, FormOptions.PromptInStart);

            context.Call(settingFormDialog, ResumeAfterSchoolMealFormDialogAsync);
        }

        private IForm<SchoolConfigQuery> BuildSetSchoolConfigForm()
        {
            OnCompletionAsyncDelegate<SchoolConfigQuery> processSetSchoolConfig = async (context, state) =>
            {
                await context.PostAsync("설정을 끝냈어요. \\(●⁰౪⁰●\\)(//●⁰౪⁰●)//");
            };

            return new FormBuilder<SchoolConfigQuery>()
                .Message("제가 하라는대로 해주시면 돼요!")
                .Field(nameof(SchoolConfigQuery.SchoolRegion))
                .Field(nameof(SchoolConfigQuery.SchoolType))
                .Field(nameof(SchoolConfigQuery.SchoolCode))
                .Message(async state => 
                    {
                        return new PromptAttribute($"주인님이 설정한 학교의 관할지역은 {state.SchoolRegion}이고 종류는 {state.SchoolType}, 고유코드는 {state.SchoolCode} 이에요. (*´･∀･)");
                    })
                .AddRemainingFields()
                .OnCompletion(processSetSchoolConfig)
                .Build();
        }

        private async Task ResumeAfterSchoolMealFormDialogAsync(IDialogContext context, IAwaitable<SchoolConfigQuery> result)
        {
            try
            {
                var schoolQuery = await result;

                var settings = new SchoolInfo()
                {
                    Region = ConvertRegions(schoolQuery.SchoolRegion),
                    SchoolType = ConvertSchoolTypes(schoolQuery.SchoolType),
                    Code = schoolQuery.SchoolCode
                };

                context.ConversationData.SetValue(ContextConstants.SchoolConfigKey, settings);
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

        private Regions ConvertRegions(SchoolConfigQuery.SchoolRegions region)
        {
            Regions result = Regions.Seoul;
            switch (region)
            {
                case SchoolConfigQuery.SchoolRegions.서울특별시:
                    result = Regions.Seoul;
                    break;
                case SchoolConfigQuery.SchoolRegions.인천광역시:
                    result = Regions.Incheon;
                    break;
                case SchoolConfigQuery.SchoolRegions.부산광역시:
                    result = Regions.Busan;
                    break;
                case SchoolConfigQuery.SchoolRegions.광주광역시:
                    result = Regions.Gwangju;
                    break;
                case SchoolConfigQuery.SchoolRegions.대전광역시:
                    result = Regions.Daejeon;
                    break;
                case SchoolConfigQuery.SchoolRegions.대구광역시:
                    result = Regions.Daegu;
                    break;
                case SchoolConfigQuery.SchoolRegions.세종특별자치시:
                    result = Regions.Sejong;
                    break;
                case SchoolConfigQuery.SchoolRegions.울산광역시:
                    result = Regions.Ulsan;
                    break;
                case SchoolConfigQuery.SchoolRegions.경기도:
                    result = Regions.Gyeonggi;
                    break;
                case SchoolConfigQuery.SchoolRegions.강원도:
                    result = Regions.Kangwon;
                    break;
                case SchoolConfigQuery.SchoolRegions.충청북도:
                    result = Regions.Chungbuk;
                    break;
                case SchoolConfigQuery.SchoolRegions.충청남도:
                    result = Regions.Chungnam;
                    break;
                case SchoolConfigQuery.SchoolRegions.경상북도:
                    result = Regions.Gyeongbuk;
                    break;
                case SchoolConfigQuery.SchoolRegions.경상남도:
                    result = Regions.Gyeongnam;
                    break;
                case SchoolConfigQuery.SchoolRegions.전라북도:
                    result = Regions.Jeonbuk;
                    break;
                case SchoolConfigQuery.SchoolRegions.전라남도:
                    result = Regions.Jeonnam;
                    break;
                case SchoolConfigQuery.SchoolRegions.제주특별자치도:
                    result = Regions.Jeju;
                    break;
                default:
                    break;
            }

            return result;
        }

        private SchoolTypes ConvertSchoolTypes(SchoolConfigQuery.SchoolTypes type)
        {
            SchoolTypes result = SchoolTypes.Elementary;
            switch (type)
            {
                case SchoolConfigQuery.SchoolTypes.초등학교:
                    result = SchoolTypes.Elementary;
                    break;
                case SchoolConfigQuery.SchoolTypes.중학교:
                    result = SchoolTypes.Middle;
                    break;
                case SchoolConfigQuery.SchoolTypes.고등학교:
                    result = SchoolTypes.High;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}