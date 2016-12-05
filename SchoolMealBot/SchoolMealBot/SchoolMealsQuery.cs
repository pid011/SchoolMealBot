using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using SchoolMeal;

namespace SchoolMealBot
{
    [Serializable]
    public class SchoolMealQuery
    {
        [Describe("관할지역")]
        [Prompt("주인님이 현재 다니는 학교의 관할지역을 골라주세요!(글로 쓰셔도 돼요!) {||}")]
        [Template(TemplateUsage.NotUnderstood, "「{0}」 👈 이런 단어는 위에 있는 목록에 없어요...")]
        public SchoolRegions SchoolRegion { get; set; }

        [Describe("학교 종류")]
        [Prompt("주인님이 현재 다니는 학교의 종류를 골라주세요!(글로 쓰셔도 돼요!) {||}")]
        [Template(TemplateUsage.NotUnderstood, "「{0}」 👈 이 단어는 위에 있는 목록에 없어요...")]
        public SchoolTypes SchoolType { get; set; }

        [Describe("학교 고유코드")]
        [Prompt("주인님이 현재 다니는 학교의 고유코드를 써주세요!")]
        public string SchoolCode { get; set; }

        public enum SchoolRegions
        {
            [Terms("서울", "서울특별시")]
            서울특별시 = 1,
            [Terms("인천", "인천광역시")]
            인천광역시 = 2,
            [Terms("서울", "부산광역시")]
            부산광역시 = 3,
            [Terms("광주", "광주광역시")]
            광주광역시 = 4,
            [Terms("대전", "대전광역시")]
            대전광역시 = 5,
            [Terms("대구", "대구광역시")]
            대구광역시 = 6,
            [Terms("세종", "세종특별자치시")]
            세종특별자치시 = 7,
            [Terms("울산", "울산광역시")]
            울산광역시 = 8,
            [Terms("경기", "경기도")]
            경기도 = 9,
            [Terms("강원", "강원도")]
            강원도 = 10,
            [Terms("충북", "충청북도")]
            충청북도 = 11,
            [Terms("충남", "충청남도")]
            충청남도 = 12,
            [Terms("경북", "경상북도")]
            경상북도 = 13,
            [Terms("경남", "경상남도")]
            경상남도 = 14,
            [Terms("전북", "전라북도")]
            전라북도 = 15,
            [Terms("전남", "전라남도")]
            전라남도 = 16,
            [Terms("제주", "제주특별자치도")]
            제주특별자치도 = 17
        }

        public enum SchoolTypes
        {
            초등학교 = 2,
            중학교 = 3,
            고등학교 = 4
        }
    }
}