using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolMeal;
using SchoolMealBot.Core.School;

namespace SchoolMealBot.Core.Menu
{
    [Serializable]
    public class MenuGenerator
    {
        private SchoolInfo schoolInfo;
        private List<MealMenu> menus = null;

        public MenuGenerator(SchoolInfo schoolInfo)
        {
            this.schoolInfo = schoolInfo;
        }


        /// <exception cref="SchoolMeal.Exception.FaildToParseException"/>
        public List<MealMenu> GetSchoolMealMenu(List<DateTime> dates)
        {
            if (menus == null)
            {
                var meal = new Meal(Util.ConvertRegions(schoolInfo.SchoolRegion), Util.ConvertSchoolTypes(schoolInfo.SchoolType), schoolInfo.SchoolCode);
                try
                {
                    menus = meal.GetMealMenu();
                }
                catch (SchoolMeal.Exception.FaildToParseException ex)
                {
                    throw ex;
                }
            }

            List<MealMenu> result = null;

            foreach (var date in dates)
            {
                if (menus.Exists(x => x.Date.Date == date))
                {
                    if (result == null)
                    {
                        result = new List<MealMenu>();
                    }
                    result.Add(menus.Find(x => x.Date.Date == date));
                }
            }

            return result;
        }
    }
}