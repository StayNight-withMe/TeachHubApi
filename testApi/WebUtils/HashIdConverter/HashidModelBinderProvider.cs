using infrastructure.Utils.HashIdConverter;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace testApi.WebUtils.HashIdConverter
{
    public class HashidModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(Hashid))
            {
                return new BinderTypeModelBinder(typeof(HashidModelBinder));
            }
            return null;
        }
    }
}
