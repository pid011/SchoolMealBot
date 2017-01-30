using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace SchoolMealBot
{
    [Serializable]
    public class SchoolConfigQuery
    {
        /*
        [Describe("관할지역")]
        [Prompt("현재 다니는 학교의 관할지역을 골라주세요! {||}")]
        [Template(TemplateUsage.NotUnderstood, "[ {0} ] 이런 단어는 위에 있는 목록에 없어요...")]
        public SchoolRegions SchoolRegion { get; set; }

        [Describe("학교종류")]
        [Prompt("현재 다니는 학교의 종류를 골라주세요! {||}")]
        [Template(TemplateUsage.NotUnderstood, "[ {0} ] 이런 단어는 위에 있는 목록에 없어요...")]
        public SchoolTypes SchoolType { get; set; }

        public string SchoolCode { get; set; }
        */
        public enum SchoolRegions
        {
            서울특별시 = 1,
            인천광역시 = 2,
            부산광역시 = 3,
            광주광역시 = 4,
            대전광역시 = 5,
            대구광역시 = 6,
            세종특별자치시 = 7,
            울산광역시 = 8,
            경기도 = 9,
            강원도 = 10,
            충청북도 = 11,
            충청남도 = 12,
            경상북도 = 13,
            경상남도 = 14,
            전라북도 = 15,
            전라남도 = 16,
            제주특별자치도 = 17
        }

        public enum SchoolTypes
        {
            초등학교 = 1,
            중학교 = 2,
            고등학교 = 3
        }
    }
}