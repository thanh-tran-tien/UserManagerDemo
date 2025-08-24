namespace UserManagerDemo.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class UnitOfWorkAttribute : Attribute
{
    public bool Enabled { get; }
    public bool RollbackOnException { get; }

    public UnitOfWorkAttribute(bool enabled = true, bool rollbackOnException = true)
    {
        Enabled = enabled;
        RollbackOnException = rollbackOnException;
    }
}