namespace UserManagerDemo.Application.Common.Interface;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}