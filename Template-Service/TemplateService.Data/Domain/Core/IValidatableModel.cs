namespace TemplateService.Data.Domain.Core
{
    public interface IValidatableModel
    {
        public ValidateModelResult Validate();
    }
}
