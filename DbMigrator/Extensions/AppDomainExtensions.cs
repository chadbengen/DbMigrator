using System;
using System.Linq;

namespace DbMigrator.Extensions
{
    public static class AppDomainExtensions
    {
        /// <summary>
        /// Gets the first type that matches the fullname.
        /// </summary>
        /// <param name="appDomain"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type GetTypeByFullName(this AppDomain appDomain, string className)
        {
            return appDomain
                .GetAssemblies()
                .Where(a => className.StartsWith(a.GetName().Name))
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.FullName == className);
        }
    }
}
