using Core.Common.Types.HashId;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;


namespace testApi.WebUtils.HashIdConverter
{
    public class HashidModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var resultByModelName = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var resultByProperty = bindingContext.ModelMetadata.PropertyName != null
                ? bindingContext.ValueProvider.GetValue(bindingContext.ModelMetadata.PropertyName)
                : ValueProviderResult.None;

           

            string? rawValue = resultByModelName.FirstValue ?? resultByProperty.FirstValue;

            if (string.IsNullOrEmpty(rawValue))
            {
                return Task.CompletedTask;
            }

            try
            {
 
                int decoded = HashidsHelper.Decode(rawValue);         
                bindingContext.Result = ModelBindingResult.Success(new Hashid(decoded));
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid Hashid format.");
            }

            return Task.CompletedTask;
        }
    }
}
