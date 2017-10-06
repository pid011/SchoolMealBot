using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolFinder;

namespace SchoolMealBot.Core.School
{
    [Serializable]
    public class SchoolInfo
    {
        public SchoolTypes SchoolType { get; set; }
        public Regions SchoolRegion { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAdress { get; set; }
        public string SchoolCode { get; set; }
        

        public SchoolInfo()
        {

        }

        public SchoolInfo(SchoolTypes type, Regions region, string name, string adress, string code)
        {
            SchoolType = type;
            SchoolRegion = region;
            SchoolName = name;
            SchoolAdress = adress;
            SchoolCode = code;
        }
    }
}