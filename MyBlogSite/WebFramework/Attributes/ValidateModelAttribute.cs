using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using MyBlogSite.Constants;
using MyBlogSite.Dtos.Response;

namespace MyBlogSite.WebFramework.Attributes
{
    /// <summary>
    /// Model validasyonu için kullanılan action filter.
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Action methodu çalışmadan önce model validasyonu yapılır.
        /// </summary>
        /// <param name="context">ActionExecutingContext.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Action'a geçirilen argümanları alır ve null olmayanları filtreler.
            var arguments = context.ActionArguments.Values.Where(argument => argument != null);

            // Her bir argüman için validator bulur ve validasyon yapar.
            var validationErrors = arguments
                .Select(argument =>
                {
                    var validatorType = typeof(IValidator<>).MakeGenericType(argument!.GetType());
                    var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                    return validator?.Validate(new ValidationContext<object>(argument));
                })
                .Where(validationResult => validationResult != null && !validationResult.IsValid)
                .SelectMany(validationResult => validationResult!.Errors)
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToList());

            // Eğer validasyon hataları varsa hata yanıtı oluşturur.
            if (validationErrors.Any())
            {
                context.Result = ResponseDto.BadRequest("Validation Error", new { Errors = validationErrors }, ErrorConstants.ValidationError);
                return;
            }

            // Validasyon başarılı ise işlem devam ettirilir.
            base.OnActionExecuting(context);
        }
    }
}
