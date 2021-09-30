namespace MiniService.Data.Domain.Core
{
    public interface IVersionedEntity
    {
        int Version { get; set; }
    }
}
