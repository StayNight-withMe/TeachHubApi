using infrastructure.Utils.HashIdConverter;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Core.Common.Types.HashId;


namespace testApi.WebUtils.HashIdConverter
{
    public class HashidModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
            if (string.IsNullOrEmpty(value)) return Task.CompletedTask;

            var decoded = HashidsHelper.Decode(value);
            bindingContext.Result = ModelBindingResult.Success(new Hashid(decoded));
            return Task.CompletedTask;
        }
    }
}
