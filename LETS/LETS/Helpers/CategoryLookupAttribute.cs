using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LETS.Helpers
{
    [AttributeUsage(AttributeTargets.All)]
    public class CategoryLookupAttribute : Attribute
    {
        public string Category { get; set; }

        public CategoryLookupAttribute(string category)
        {
            this.Category = category;
        }
        
        public IDictionary<string, object> HtmlAttributes()
        {
            IDictionary<string, object> htmlatts = new Dictionary<string, object>();
            htmlatts.Add("Category", Category);
            return htmlatts;
        }
    }
}