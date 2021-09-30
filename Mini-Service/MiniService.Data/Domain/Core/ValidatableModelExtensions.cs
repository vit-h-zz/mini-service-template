namespace MiniService.Data.Domain.Core
{
    public static class ValidatableModelExtensions
    {
        public static void ValidateAndThrow(this IValidatableModel model)
        {
            var result = model.Validate();

            if (result.Valid)
                return;
            else
                throw new ModelValidationException(result.GetValuesFunc());
        }
    }
}
