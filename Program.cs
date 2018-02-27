using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SplitToJson.Model;

namespace SplitToJson
{
    enum InputFomatMode
    {
        team_activity,
        team_prepare
    }

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
            string[] lines = sr.ReadToEnd().Split('\n');
            foreach (var line in lines)
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
            }
            #endregion
            
            #region Create activity           
            sr = new StreamReader(rootPath + fileNames[1]); 
            lines = sr.ReadToEnd().Split('\n');            
            activities.AddRange(ActiivityHelper(lines, InputFomatMode.team_activity, teams));

            sr = new StreamReader(rootPath + fileNames[2]);
            lines = sr.ReadToEnd().Split('\n');
            activities.AddRange(ActiivityHelper(lines, InputFomatMode.team_prepare, teams));
            #endregion            

            #region Mapping to user
            sr = new StreamReader(rootPath + fileNames[3]);
            lines = sr.ReadToEnd().Split('\n');
            foreach (var line in lines)
            {
                string[] info = line.Split(',');
                User user = new User()
                {
                    Name = info[0],
                    Mail = info[1]
                };
                var acts = from activity in activities
                           where activity.OwnerName.Contains(user.Name)
                           select activity;
                user.Activities = acts.ToArray();
                users.Add(user);
            }
            #endregion

            Console.WriteLine(users[0].Name);
        }
        
        static string[] DateHelper(string d)
        {            
            string[] date = d.Split('/');
            string entity = string.Format("2018-{0}-{1}", date[0], date[1]);            
            return new string[] { entity + "T10:00:00", entity + "T18:00:00" };
        }
        
        static IEnumerable<Activity> ActiivityHelper(string[] data, InputFomatMode mode, IEnumerable<Team> teams)
        {
            List<Activity> acts = new List<Activity>();
            foreach (var item in data)
            {
                string[] acs = item.Split(',');
                string[] date = DateHelper(acs[0]);            
                string title = acs[1];                
                string teamName = acs[3];
                var teamMembers = (from t in teams
                                 where t.Name.Equals(teamName)
                                 select t.Members).Single();
                
                foreach (var owner in teamMembers)
                {
                    Activity activity = new Activity()
                    {
                        StartDate = date[0],
                        EndDate = date[1],                        
                        Title = title,
                        OwnerName = owner
                    };

                    switch (mode)
                    {
                        case InputFomatMode.team_activity:
                            // for location.
                            activity.Location = acs[2];
                            activity.Description = "";
                            break;
                        case InputFomatMode.team_prepare:
                            // for description.
                            activity.Location = "";
                            activity.Description = acs[2];
                            break;                            
                        default:
                            break;
                    }

                    acts.Add(activity);
                }                    
            }

            return acts;
        }
    }
}
