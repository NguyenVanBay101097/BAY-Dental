using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.ModelBinders
{
    public class CustomDateTimeModelBinder : IModelBinder
    {
        public static readonly Type[] SUPPORTED_DATETIME_TYPES =
          new Type[] { typeof(DateTime), typeof(DateTime?) };

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (!SUPPORTED_DATETIME_TYPES.Contains(bindingContext.ModelType))
            {
                return Task.CompletedTask;
            }

            var modelName = bindingContext.ModelName;

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var dateTimeToParse = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(dateTimeToParse))
            {
                return Task.CompletedTask;
            }

            var formattedDateTime = ParseDateTime(dateTimeToParse);

            bindingContext.Result = ModelBindingResult.Success(formattedDateTime);
            return Task.CompletedTask;
        }

        static DateTime? ParseDateTime(string date)
        {
            if (DateTime.TryParse(date, Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out DateTime validDate))
            {
                return validDate;
            }

            return null;
        }
    }
}
