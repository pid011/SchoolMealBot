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
        [Prompt("주인님이 현재 다니는 교육기관의 관할지역을 골라주세요!(글로 쓰셔도 돼요!) {||}")]
        [Template(TemplateUsage.NotUnderstood, "「{0}」 👈 이런 단어는 위에 있는 목록에 없어요...")]
        public SchoolRegions SchoolRegion { get; set; }

        [Describe("교육기관 종류")]
        [Prompt("주인님이 현재 다니는 교육기관의 종류를 골라주세요!(글로 쓰셔도 돼요!) {||}")]
        [Template(TemplateUsage.NotUnderstood, "「{0}」 👈 이 단어는 위에 있는 목록에 없어요...")]
        public SchoolTypes SchoolType { get; set; }

        [Describe("교육기관 고유코드")]
        [Prompt("주인님이 현재 다니는 교육기관의 고유코드를 써주세요!")]
        public string SchoolCode { get; set; }

        public enum SchoolRegions
        {
            [Terms("서울", "서울특별시")]
            서울특별시,
            [Terms("인천", "인천광역시")]
            인천광역시,
            [Terms("서울", "서울특별시")]
            부산광역시,
            [Terms("광주", "광주광역시")]
            광주광역시,
            [Terms("대전", "대전광역시")]
            대전광역시,
            [Terms("대구", "대구광역시")]
            대구광역시,
            [Terms("세종", "세종특별자치시")]
            세종특별자치시,
            [Terms("울산", "울산광역시")]
            울산광역시,
            [Terms("경기", "경기도")]
            경기도,
            [Terms("강원", "강원도")]
            강원도,
            [Terms("충북", "충청북도")]
            충청북도,
            [Terms("충남", "충청남도")]
            충청남도,
            [Terms("경북", "경상북도")]
            경상북도,
            [Terms("경남", "경상남도")]
            경상남도,
            [Terms("전북", "전라북도")]
            전라북도,
            [Terms("전남", "전라남도")]
            전라남도,
            [Terms("제주", "제주특별자치도")]
            제주특별자치도
        }

        public enum SchoolTypes
        {
            병설유치원,
            초등학교,
            중학교,
            고등학교
        }
    }
}