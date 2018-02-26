using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SplitToJson.Model;

namespace SplitToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootPath = @"D:\Share\";
            string[] fileNames = { "team.txt", "team_activity.txt", "team_prepare.txt", "mapping.txt" };            
            List<Team> teams = new List<Team>();
            List<User> users = new List<User>();
            List<Activity> activities = new List<Activity>();

            foreach (var item in fileNames)
            {
                if (!File.Exists(rootPath + item))
                {
                    Console.WriteLine("Cannot find file at {0}", rootPath + item);
                    return;
                }
            }
            StreamReader sr;

            #region Create team       
            sr = new StreamReader(rootPath + fileNames[0]);                 
            string line = sr.ReadLine();
            while (line != null || !line.Equals(""))
            {
                string[] info = line.Split(',');

                Team team = new Team()
                {
                    Name = info[0]
                };

                List<string> ms = new List<string>();
                for (int i = 1; i < info.Length; i++)
                {
                    ms.Add(info[i]);
                }

                teams.Add(team);
                line = sr.ReadLine();
            }
            #endregion
            
            #region Create activity           
            sr = new StreamReader(rootPath + fileNames[1]); 
            line = sr.ReadLine();
            while (line != null || !line.Equals(""))
            {
                string[] acs = line.Split(",");
            }
            #endregion
        }
    }
}
