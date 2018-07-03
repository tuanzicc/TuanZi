using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Extensions;

namespace TuanZi.AspNetCore.Mvc
{
    public static class ModelStateExtensions
    {
        public static string FirstError(this ModelStateDictionary modelState)
        {
            var errorText = string.Empty;
            foreach (var state in modelState.Values)
            {
                if (state != null && state.Errors.Count > 0)
                {
                    var error = state.Errors[0];
                    errorText = error.ErrorMessage;
                    if (errorText.IsNullOrEmpty() && error.Exception != null)
                        errorText = state.Errors[0].Exception.Message;
                    break;
                }
            }


            return errorText;
        }

        public static List<string> Errors(this ModelStateDictionary modelState)
        {
            var errors = new List<string>();
            foreach (var state in modelState.Values)
            {
                if (state != null && state.Errors.Count > 0)
                {
                    var error = state.Errors[0];
                    var errorText = error.ErrorMessage;
                    if (errorText.IsNullOrEmpty() && error.Exception != null)
                        errorText = state.Errors[0].Exception.Message;
                    errors.Add(errorText);
                }
            }


            return errors;
        }
    }
}
