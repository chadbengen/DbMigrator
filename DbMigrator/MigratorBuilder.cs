using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DbMigrator
{
    public class MigratorBuilder
    {
        private Migrator _migrator;

        private Dictionary<string, bool> _jsonFiles = new Dictionary<string, bool>();

        private string _basePath;

        private List<Action<IServiceCollection, IConfigurationRoot>> _registrations = new List<Action<IServiceCollection, IConfigurationRoot>>();

        private MigratorBuilder()
        {
            _migrator = new Migrator();
        }

        public static MigratorBuilder Create()
        {
            return new MigratorBuilder();
        }

        public MigratorBuilder SetBasePath(string path)
        {
            var migratorDirectory = AppContext.BaseDirectory;

            var contentRootPath = Path.Combine(migratorDirectory, path);

            _basePath = contentRootPath;

            return this;
        }

        public MigratorBuilder AddJsonFile(string path, bool optional)
        {
            _jsonFiles.Add(path, optional);

            return this;
        }

        public MigratorBuilder Register(Action<IServiceCollection, IConfigurationRoot> register)
        {
            _registrations.Add(register);

            return this;
        }

        public async Task Execute<T>() where T : class
        {
            if (!string.IsNullOrWhiteSpace(_basePath))
            {
                _migrator.ConfigurationBuilder.SetBasePath(_basePath);
            }

            foreach (var jsonFile in _jsonFiles)
            {
                _migrator.ConfigurationBuilder.AddJsonFile(jsonFile.Key, jsonFile.Value);
            }

            _migrator.BuildConfiguration<T>();

            _migrator.ConfigureLogging();

            _migrator.InitializeServices();

            foreach(var registration in _registrations)
            {
                registration(_migrator.Services, _migrator.ConfigurationRoot);
            }
            
            await _migrator.Execute();
        }
    }
}
