using SchoolFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolMealBot
{
    [Serializable]
    internal class Util
    {

        public static Regions ConvertRegions(SchoolConfigQuery.SchoolRegions region)
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

        public static SchoolTypes ConvertSchoolTypes(SchoolConfigQuery.SchoolTypes type)
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