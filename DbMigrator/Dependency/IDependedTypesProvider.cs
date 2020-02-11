using System;

namespace DbMigrator.Dependency
{
    public interface IDependedTypesProvider
    {
        Type[] GetDependedTypes();
    }
}
