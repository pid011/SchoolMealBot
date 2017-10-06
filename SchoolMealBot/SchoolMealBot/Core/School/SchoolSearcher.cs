using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolFinder;

namespace SchoolMealBot.Core.School
{
    [Serializable]
    public class SchoolSearcher
    {
        private SchoolSearch searcher = new SchoolSearch();

        public List<SchoolInfo> Search(SchoolTypes type, Regions region, string name)
        {
            var result = searcher.SearchSchool(type, region, name);
            var list = new List<SchoolInfo>();
            result.ForEach(x => list.Add(new SchoolInfo(x.SchoolType, x.Region, x.Name, x.Adress, x.Code)));
            return list;
        }
    }
}