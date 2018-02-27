using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SplitToJson.Model;

namespace SplitToJson
{
    enum PairMode
    {
        Team,
        Parson
    }

    enum InpuMode
    {
        Execute,
        Prepare
    }

    class Program
    {
        static void Main(string[] args)
        {
            string rootPath = @"D:\Share\";                  
            // string[] fileNames = { "team.txt", "mapping.txt", "team_activity.txt", "team_prepare.txt" };                        
            string[] fileNames = { "fake_team.txt", "fake_mapping.txt", "fake_team_activity.txt", "fake_team_prepare.txt" };         
            
            Dictionary<string, string> teamPairs = PairHelper(ReadFileAllLine(rootPath + fileNames[0]), PairMode.Team);
            Dictionary<string, string> parsonPairs = PairHelper(ReadFileAllLine(rootPath + fileNames[1]), PairMode.Parson);            

            string[] lines = ReadFileAllLine(rootPath + fileNames[2]);            
            List<Activity> activities = new List<Activity>();
            activities.AddRange(CreateEvent(lines, teamPairs, parsonPairs, InpuMode.Execute));

            lines = ReadFileAllLine(rootPath + fileNames[3]);            
            activities.AddRange(CreateEvent(lines, teamPairs, parsonPairs, InpuMode.Prepare));

            string jsonString = JsonConvert.SerializeObject(activities);
            HttpClient hc = new HttpClient();
            var respone = hc.PostAsync("https://prod-30.eastasia.logic.azure.com:443/workflows/ec859b609596444487af6c41096f4aa3/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=FVo6BA4v5qY418qaqedwvoCUyfl94WGQwQ0bnqejjpg", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Console.WriteLine(jsonString);
            Console.WriteLine(respone.Result.StatusCode);           
        }   

        static string[] ReadFileAllLine(string path)
        {
            StreamReader sr = new StreamReader(path);
            return sr.ReadToEnd().Split('\n');                        
        }
        static Dictionary<string, string> PairHelper(string[] lines, PairMode mode)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            char splitWord;
            if (mode == PairMode.Parson)
            {
                splitWord = ',';
            }
            else
            {
                splitWord = ':';
            }

            foreach (var line in lines)
            {
                var pair = line.Split(splitWord);
                result.Add(pair[0], pair[1]);
            }

            return result;
        }   

        static string[] DateHelper(string date)
        {
            var item = date.Split('/');
            string result = $"2018-{item[0]}-{item[1]}";
            return new string[] { result + "T10:00:00", result + "T18:00:00" };
        }

        static string GetMailListFromTeam(string input, Dictionary<string, string> people)
        {            
            string[] members = input.Split(',');
            string result = "";
            foreach (var item in members)
            {                
                string parson;
                if (people.TryGetValue(item, out parson))
                {
                    result += parson + ";";
                }
                else
                {
                    Console.WriteLine("Cannot find parson name {0}", item);
                }
            }

            return result;
        }
        static IEnumerable<Activity> CreateEvent(string[] lines, Dictionary<string, string> teams, Dictionary<string, string> people, InpuMode mode)
        {
            List<Activity> result = new List<Activity>();
            foreach (var item in lines)
            {
                string[] act = item.Split(',');
                string[] date = DateHelper(act[0]);
                Activity activity = new Activity()
                {
                    StartDate = date[0],
                    EndDate = date[1],
                    Title = act[1],
                    Location = "",
                    Description = "",
                    Owners = ""
                };

                if (mode == InpuMode.Execute)
                {
                    activity.Location = act[2];
                }
                else
                {
                    activity.Description = act[2];
                }

                string members;
                if (teams.TryGetValue(act[3], out members))
                {
                    // go get mail list.
                    activity.Owners = GetMailListFromTeam(members, people);
                }
                else
                {
                    // get by parson.                    
                    string[] temp = act[3].Split('、');
                    string parson;
                    if (temp.Count() == 0)
                    {                   
                        if (people.TryGetValue(temp[0], out parson))
                        {
                            activity.Owners = parson;
                        }                   
                        else
                        {
                            Console.WriteLine("Cannot find parson name {0}", item);
                        }          
                    }
                    else
                    {
                        string tempResult = "";
                        foreach (var tempItem in temp)
                        {
                            if (people.TryGetValue(tempItem, out parson))
                            {
                                tempResult += parson + ";";
                            }
                            else
                            {
                                Console.WriteLine("Cannot find parson name {0}", item);
                            }
                        }
                        activity.Owners = tempResult;
                    }
                }

                result.Add(activity);
            }

            return result;
        }
    }
}
