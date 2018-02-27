using System;

namespace SplitToJson.Model
{
    public class Activity
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Owners { get; set; } 
    }
}