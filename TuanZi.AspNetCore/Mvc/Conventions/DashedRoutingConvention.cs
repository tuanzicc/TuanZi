using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;


namespace TuanZi.AspNetCore.Mvc.Conventions
{
    public class DashedRoutingConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var hasRouteAttribute = controller.Selectors.Any(seletor => seletor.AttributeRouteModel != null);
            if (hasRouteAttribute)
            {
                return;
            }
            foreach (ActionModel action in controller.Actions)
            {
                foreach (SelectorModel model in action.Selectors.Where(m=>m.AttributeRouteModel == null))
                {
                    List<string> parts = new List<string>();
                    foreach (var attribute in controller.Attributes)
                    {
                        if (attribute is AreaAttribute area)
                        {
                            parts.Add(PascalToKebabCase(area.RouteValue));
                            break;
                        }
                    }

                    if (parts.Count == 0 && controller.ControllerName == "Home" && action.ActionName == "Index")
                    {
                        continue;
                    }
                    parts.Add(PascalToKebabCase(controller.ControllerName));

                    if (action.ActionName != "Index")
                    {
                        parts.Add(PascalToKebabCase(action.ActionName));
                    }
                    
                    string template = string.Join("/", parts);
                    model.AttributeRouteModel = new AttributeRouteModel() { Template = template };
                }
            }
        }

        private static string PascalToKebabCase(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return Regex.Replace(value, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1", RegexOptions.Compiled).Trim().ToLower();
        }
    }
}