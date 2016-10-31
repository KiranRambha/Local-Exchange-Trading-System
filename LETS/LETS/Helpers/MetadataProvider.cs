using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LETS.Helpers
{
    public class MetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            var additionalValues = attributes.OfType<CategoryLookupAttribute>().FirstOrDefault();
            if (additionalValues != null)
            {
                metadata.AdditionalValues.Add("Category", additionalValues.Category);
            }
            return metadata;
        }
    }
}