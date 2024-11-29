namespace Boom.Infrastructure.Data.Entities;

public interface IEntity
{
    long Id { get; set; }
    
    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}