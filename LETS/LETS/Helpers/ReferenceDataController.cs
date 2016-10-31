using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LETS.Helpers
{
    public class ReferenceDataController
    {
        protected Dictionary<string, Dictionary<string, string>> referenceDataDictionary = new Dictionary<string, Dictionary<string, string>>();

        internal Dictionary<string, Dictionary<string, string>> getReferenceData()
        {
            using (var referenceDataFile = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\Helpers\\referencedata.csv"))
            {
                string category = string.Empty;
                Dictionary<string, string> referenceData;
                while(!referenceDataFile.EndOfStream)
                {
                    referenceData = new Dictionary<string, string>();
                    var dataSet = referenceDataFile.ReadLine().Split('/');
                    category = dataSet[0];
                    for (int i = 1; i < dataSet.Length; i++)
                    {
                        var values = dataSet[i].Substring(1, dataSet[i].Length - 2).Split('|');
                        referenceData.Add(values[0], values[1]);
                    }
                    referenceDataDictionary.Add(category, referenceData);
                }
            }
            return referenceDataDictionary;
        }
    }
}