using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolFinder;

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
            this.userSchoolInfo = new SchoolInfo();
            var regions = ((IEnumerable<SchoolConfigQuery.SchoolRegions>)Enum.GetValues(typeof(SchoolConfigQuery.SchoolRegions))).Select(x => x);

            PromptDialog.Choice(context, OnSchoolRegionSelectedAsync, regions, 
                "현재 다니는 학교의 관할지역을 골라주세요!", "다시 선택해주세요.", promptStyle: PromptStyle.PerLine);
        }

        private async Task OnSchoolRegionSelectedAsync(IDialogContext context, IAwaitable<SchoolConfigQuery.SchoolRegions> result)
        {
            var choiced = await result;
            this.userSchoolInfo.Region = Util.ConvertRegions(choiced);

            var types = ((IEnumerable<SchoolConfigQuery.SchoolTypes>)Enum.GetValues(typeof(SchoolConfigQuery.SchoolTypes))).Select(x => x);

            PromptDialog.Choice(context, OnSchoolTypeSelectedAsync, types,
                "현재 다니는 학교의 종류를 골라주세요!", "다시 선택해주세요.", promptStyle: PromptStyle.Auto);
        }

        private async Task OnSchoolTypeSelectedAsync(IDialogContext context, IAwaitable<SchoolConfigQuery.SchoolTypes> result)
        {
            var choiced = await result;
            this.userSchoolInfo.SchoolType = Util.ConvertSchoolTypes(choiced);

            context.Done(this.userSchoolInfo);
        }
    }
}