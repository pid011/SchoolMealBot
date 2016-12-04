using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolMeal;

namespace SchoolMealsBot.Models
{
    [Serializable]
    public class Settings
    {
        public Regions SchoolRegion { get; set; }
        public SchoolType SchoolType { get; set; }
        public string SchoolCode { get; set; }
    }
}