using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ApplicationConfig_FeatureFunctionViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public List<object> Requireds { get; set; }
    }

    public class ApplicationConfig_FeatureObjectViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public List<ApplicationConfig_FeatureFunctionViewModel> Functions { get; set; }
        public List<string> Packages { get; set; } = new List<string>();
    }

    public class ApplicationConfig_FeatureChildViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public List<ApplicationConfig_FeatureObjectViewModel> Objects { get; set; } = new List<ApplicationConfig_FeatureObjectViewModel>();
        public List<ApplicationConfig_FeatureFunctionViewModel> Functions { get; set; }
        public List<string> Packages { get; set; } = new List<string>();
    }

    public class ApplicationConfig_FeatureViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public List<ApplicationConfig_FeatureObjectViewModel> Objects { get; set; } = new List<ApplicationConfig_FeatureObjectViewModel>();
        public List<ApplicationConfig_FeatureChildViewModel> Children { get; set; } = new List<ApplicationConfig_FeatureChildViewModel>();
        public List<string> Packages { get; set; } = new List<string>();
    }
}
