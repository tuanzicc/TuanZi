using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.Web
{
    public static class MappingExtensions
    {
        public static TTarget MapsTo<TTarget>(this object source)
        {
            var target = source.MapTo<TTarget>();
            if (target.Id.IsEmpty())
                target.Id = CombGuid.NewGuid();

            var context = ServiceLocator.Instance.HttpContext();
            Check.NotNull(context, nameof(context));
            if (context.Request.Method == "POST")
            {
                var files = context.Request.Form.Files;
                if (files.Count > 0)
                {
                    var fileManager = ServiceLocator.Instance.GetService<FileManager>();
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            if (target.HasProperty(file.Name))
                            {
                                var value = AsyncRunner.RunSync(() => fileManager.NewAsync(file));
                                target.SetPropertyValue(file.Name, value);
                            }
                        }

                    }
                }
            }

            return target;
        }

        public static TTarget MapsTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            target = source.MapTo(target);

            var context = ServiceLocator.Instance.HttpContext();
            Check.NotNull(context, nameof(context));
            if (context.Request.Method == "POST")
            {
                var files = context.Request.Form.Files;
                if (files.Count > 0)
                {
                    var fileManager = ServiceLocator.Instance.GetService<FileManager>();
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            if (target.HasProperty(file.Name))
                            {
                                var value = AsyncRunner.RunSync(() => fileManager.NewAsync(file));
                                target.SetPropertyValue(file.Name, value);
                            }
                        }
                    }
                }
            }

            return target;
        }
    }

}
