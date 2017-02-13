using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolMealBot
{
    [Serializable]
    internal class Util
    {
        /// <summary>
        /// <see cref="SchoolConfigQuery.SchoolRegions"/> => <see cref="SchoolFinder.Regions"/> 변환 메소드
        /// </summary>
        /// <param name="region">변환할 값</param>
        /// <returns>변환된 값</returns>
        public static SchoolFinder.Regions ConvertRegions(SchoolConfigQuery.SchoolRegions region)
        {
            SchoolFinder.Regions result = SchoolFinder.Regions.Seoul;
            switch (region)
            {
                case SchoolConfigQuery.SchoolRegions.서울특별시:
                    result = SchoolFinder.Regions.Seoul;
                    break;
                case SchoolConfigQuery.SchoolRegions.인천광역시:
                    result = SchoolFinder.Regions.Incheon;
                    break;
                case SchoolConfigQuery.SchoolRegions.부산광역시:
                    result = SchoolFinder.Regions.Busan;
                    break;
                case SchoolConfigQuery.SchoolRegions.광주광역시:
                    result = SchoolFinder.Regions.Gwangju;
                    break;
                case SchoolConfigQuery.SchoolRegions.대전광역시:
                    result = SchoolFinder.Regions.Daejeon;
                    break;
                case SchoolConfigQuery.SchoolRegions.대구광역시:
                    result = SchoolFinder.Regions.Daegu;
                    break;
                case SchoolConfigQuery.SchoolRegions.세종특별자치시:
                    result = SchoolFinder.Regions.Sejong;
                    break;
                case SchoolConfigQuery.SchoolRegions.울산광역시:
                    result = SchoolFinder.Regions.Ulsan;
                    break;
                case SchoolConfigQuery.SchoolRegions.경기도:
                    result = SchoolFinder.Regions.Gyeonggi;
                    break;
                case SchoolConfigQuery.SchoolRegions.강원도:
                    result = SchoolFinder.Regions.Kangwon;
                    break;
                case SchoolConfigQuery.SchoolRegions.충청북도:
                    result = SchoolFinder.Regions.Chungbuk;
                    break;
                case SchoolConfigQuery.SchoolRegions.충청남도:
                    result = SchoolFinder.Regions.Chungnam;
                    break;
                case SchoolConfigQuery.SchoolRegions.경상북도:
                    result = SchoolFinder.Regions.Gyeongbuk;
                    break;
                case SchoolConfigQuery.SchoolRegions.경상남도:
                    result = SchoolFinder.Regions.Gyeongnam;
                    break;
                case SchoolConfigQuery.SchoolRegions.전라북도:
                    result = SchoolFinder.Regions.Jeonbuk;
                    break;
                case SchoolConfigQuery.SchoolRegions.전라남도:
                    result = SchoolFinder.Regions.Jeonnam;
                    break;
                case SchoolConfigQuery.SchoolRegions.제주특별자치도:
                    result = SchoolFinder.Regions.Jeju;
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// <see cref="SchoolFinder.Regions"/> => <see cref="SchoolMeal.Regions"/> 변환 메소드
        /// </summary>
        /// <param name="region">변환할 값</param>
        /// <returns>변환된 값</returns>
        public static SchoolMeal.Regions ConvertRegions(SchoolFinder.Regions region)
        {
            SchoolMeal.Regions result = SchoolMeal.Regions.Seoul;
            switch (region)
            {
                case SchoolFinder.Regions.Seoul:
                    result = SchoolMeal.Regions.Seoul;
                    break;
                case SchoolFinder.Regions.Incheon:
                    result = SchoolMeal.Regions.Incheon;
                    break;
                case SchoolFinder.Regions.Busan:
                    result = SchoolMeal.Regions.Busan;
                    break;
                case SchoolFinder.Regions.Gwangju:
                    result = SchoolMeal.Regions.Gwangju;
                    break;
                case SchoolFinder.Regions.Daejeon:
                    result = SchoolMeal.Regions.Daejeon;
                    break;
                case SchoolFinder.Regions.Daegu:
                    result = SchoolMeal.Regions.Daegu;
                    break;
                case SchoolFinder.Regions.Sejong:
                    result = SchoolMeal.Regions.Sejong;
                    break;
                case SchoolFinder.Regions.Ulsan:
                    result = SchoolMeal.Regions.Ulsan;
                    break;
                case SchoolFinder.Regions.Gyeonggi:
                    result = SchoolMeal.Regions.Gyeonggi;
                    break;
                case SchoolFinder.Regions.Kangwon:
                    result = SchoolMeal.Regions.Kangwon;
                    break;
                case SchoolFinder.Regions.Chungbuk:
                    result = SchoolMeal.Regions.Chungbuk;
                    break;
                case SchoolFinder.Regions.Chungnam:
                    result = SchoolMeal.Regions.Chungnam;
                    break;
                case SchoolFinder.Regions.Gyeongbuk:
                    result = SchoolMeal.Regions.Gyeongbuk;
                    break;
                case SchoolFinder.Regions.Gyeongnam:
                    result = SchoolMeal.Regions.Gyeongnam;
                    break;
                case SchoolFinder.Regions.Jeonbuk:
                    result = SchoolMeal.Regions.Jeonbuk;
                    break;
                case SchoolFinder.Regions.Jeonnam:
                    result = SchoolMeal.Regions.Jeonnam;
                    break;
                case SchoolFinder.Regions.Jeju:
                    result = SchoolMeal.Regions.Jeju;
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// <see cref="SchoolConfigQuery.SchoolTypes"/> => <see cref="SchoolFinder.SchoolTypes"/> 변환 메소드
        /// </summary>
        /// <param name="type">변환할 값</param>
        /// <returns>변환된 값</returns>
        public static SchoolFinder.SchoolTypes ConvertSchoolTypes(SchoolConfigQuery.SchoolTypes type)
        {
            SchoolFinder.SchoolTypes result = SchoolFinder.SchoolTypes.Elementary;
            switch (type)
            {
                case SchoolConfigQuery.SchoolTypes.초등학교:
                    result = SchoolFinder.SchoolTypes.Elementary;
                    break;
                case SchoolConfigQuery.SchoolTypes.중학교:
                    result = SchoolFinder.SchoolTypes.Middle;
                    break;
                case SchoolConfigQuery.SchoolTypes.고등학교:
                    result = SchoolFinder.SchoolTypes.High;
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// <see cref="SchoolFinder.SchoolTypes"/> => <see cref="SchoolMeal.SchoolType"/> 변환 메소드
        /// </summary>
        /// <param name="type">변환할 값</param>
        /// <returns>변환된 값</returns>
        public static SchoolMeal.SchoolType ConvertSchoolTypes(SchoolFinder.SchoolTypes type)
        {
            SchoolMeal.SchoolType result = SchoolMeal.SchoolType.Elementary;
            switch (type)
            {
                case SchoolFinder.SchoolTypes.Elementary:
                    result = SchoolMeal.SchoolType.Elementary;
                    break;
                case SchoolFinder.SchoolTypes.Middle:
                    result = SchoolMeal.SchoolType.Middle;
                    break;
                case SchoolFinder.SchoolTypes.High:
                    result = SchoolMeal.SchoolType.High;
                    break;
                default:
                    break;
            }
            return result;
        }

        public enum YesNo
        {
            그래,
            아니
        }
    }
}