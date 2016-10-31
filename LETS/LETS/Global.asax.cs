using LETS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LETS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static Dictionary<string, Dictionary<string, string>> referenceDataDictionary = new Dictionary<string, Dictionary<string, string>>();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Adding the Category attribute to the Html markup for reference data lookup.
            ModelMetadataProviders.Current = new MetadataProvider();

            //Getting the reference data for the Html elements from the backend.
            ReferenceDataController referenceDataController = new ReferenceDataController();
            referenceDataDictionary = referenceDataController.getReferenceData();
        }
    }
}
