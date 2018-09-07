using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TuanZi.Collections;
using TuanZi.Core.Systems;
using TuanZi.Data;
using TuanZi.Extensions;


namespace TuanZi.Entity
{
    public static class EntityHashExtensions
    {


        public static bool CheckSyncByHash(this IEnumerable<IEntityHash> entityHashes, IServiceProvider provider, ILogger logger)
        {
            IEntityHash[] hashes = entityHashes as IEntityHash[] ?? entityHashes.ToArray();
            if (hashes.Length == 0)
            {
                return false;
            }
            string hash = hashes.Select(m => m.GetHash()).ExpandAndToString().ToMd5Hash();
            IKeyValueStore store = provider.GetService<IKeyValueStore>();
            string entityType = hashes[0].GetType().FullName;
            string key = $"TuanZi.Initialize.SyncToDatabaseHash-{entityType}";
            var keyValue = store.GetKeyValue(key);
            if (keyValue != null && keyValue.Value?.ToString() == hash)
            {
                logger.LogInformation($"Content signature of {hashes.Length} basic data '{entityType}' {hash} Same as last time, cancel database synchronization");
                return false;
            }
            OperationResult result = store.CreateOrUpdateKeyValue(key, hash).Result;
            logger.LogInformation($"Content signature of {hashes.Length} basic data '{entityType}' {hash} Unlike the last {keyValue?.Value}, database synchronization will be performed");
            return true;
        }

        public static string GetHash(this IEntityHash entity)
        {
            Type type = entity.GetType();
            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(m => m.CanWrite && m.Name != "Id"))
            {
                sb.Append(property.GetValue(entity));
            }
            return sb.ToString().ToMd5Hash();
        }
    }
}