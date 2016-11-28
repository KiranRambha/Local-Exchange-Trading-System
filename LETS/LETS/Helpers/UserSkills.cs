using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LETS.Helpers
{
    public class UserSkills
    {
        public string[] GetUserSkills()
        {
            var skills = new List<string>();
            using (var skillDataFile = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\Helpers\\SkillsList.txt"))
            {
                while (!skillDataFile.EndOfStream)
                {
                    var skill = skillDataFile.ReadLine();
                    skills.Add(skill);
                }
            }
            return skills.ToArray();
        }
    }
}