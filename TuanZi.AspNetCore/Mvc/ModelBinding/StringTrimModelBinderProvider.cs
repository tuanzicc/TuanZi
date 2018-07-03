using Microsoft.AspNetCore.Mvc.ModelBinding;
using TuanZi.Data;

namespace TuanZi.AspNetCore.Mvc.ModelBinding
{
    public class StringTrimModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            Check.NotNull(context, nameof(context));

            if (context.Metadata.UnderlyingOrModelType == typeof(string))
            {
                return new StringTrimModelBinder();
            }
            return null;
        }
    }
}