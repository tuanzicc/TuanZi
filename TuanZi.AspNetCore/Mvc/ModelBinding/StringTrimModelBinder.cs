using System;

using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using TuanZi.Data;

namespace TuanZi.AspNetCore.Mvc.ModelBinding
{
    public class StringTrimModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Check.NotNull(bindingContext, nameof(bindingContext));

            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            Type underlyingOrModelType = bindingContext.ModelMetadata.UnderlyingOrModelType;
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            try
            {
                string firstValue = valueProviderResult.FirstValue;
                object model;
                if (string.IsNullOrWhiteSpace(firstValue))
                {
                    model = null;
                }
                else
                {
                    if (underlyingOrModelType != typeof(string))
                    {
                        throw new MulticastNotSupportedException();
                    }
                    model = firstValue.Trim();
                }
                bindingContext.Result = ModelBindingResult.Success(model);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Exception exception = ex;
                if (!(exception is FormatException) && exception.InnerException != null)
                    exception = ExceptionDispatchInfo.Capture(exception.InnerException).SourceException;
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, exception, bindingContext.ModelMetadata);
                return Task.CompletedTask;
            }
        }
    }
}