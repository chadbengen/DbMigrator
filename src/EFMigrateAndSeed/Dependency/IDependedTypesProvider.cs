using System;

namespace EFMigrateAndSeed.Dependency
{
    public interface IDependedTypesProvider
    {
        Type[] GetDependedTypes();
    }
}
