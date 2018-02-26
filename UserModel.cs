using System;

namespace SplitToJson.Model
{
    public class User
    {
        public string Name { get; set; }
        public string Mail { get; set; }
        public Activity[] Activities { get; set; }
    }
}