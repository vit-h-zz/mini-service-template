namespace MiniService.Data.Domain.Core
{
    public interface IValidatableModel
    {
        public ValidateModelResult Validate();
    }
}
