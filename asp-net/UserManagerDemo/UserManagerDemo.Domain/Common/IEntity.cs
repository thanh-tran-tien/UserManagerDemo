namespace UserManagerDemo.Domain.Common;

public interface IEntity<TPrimaryKey>

{
    TPrimaryKey Id { get; set; }
}