using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CoreFra.Repository
{
    public class DapperHelper
    {
        #region PrivateProperties

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties =
            new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties =
            new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ComputedProperties =
            new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TableNames =
            new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly string Prefix = string.Empty;
        private const string Suffix = "s";

        #endregion

        public static string GetColumnsStringSqlServer(IEnumerable<PropertyInfo> properties, string tablePrefix = null)
        {
            return string.Join(", ", properties.Select(property => $"{tablePrefix}[{property.Name}]"));
        }

        public static string GetTableName(Type type)
        {
            if (TableNames.TryGetValue(type.TypeHandle, out var name))
            {
                return name;
            }

            var tableAttr =
                type.GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute") as
                    dynamic;
            if (tableAttr != null)
            {
                name = tableAttr.Name;
            }
            else
            {
                name = type.IsInterface && type.Name.StartsWith("I")
                    ? type.Name.Substring(1)
                    : type.Name;
                name = Prefix + name + Suffix;
            }

            TableNames[type.TypeHandle] = name;
            return name;
        }

        public static List<PropertyInfo> ComputedPropertiesCache(Type type)
        {
            if (ComputedProperties.TryGetValue(type.TypeHandle, out var cachedProps))
            {
                return cachedProps.ToList();
            }

            var computedProperties =
                TypePropertiesCache(type)
                    .Where(p => p.GetCustomAttributes(true).Any(a => a.GetType().Name == "ComputedAttribute"))
                    .ToList();
            ComputedProperties[type.TypeHandle] = computedProperties;
            return computedProperties;
        }

        public static List<PropertyInfo> TypePropertiesCache(Type type)
        {
            if (TypeProperties.TryGetValue(type.TypeHandle, out var cachedProps))
            {
                return cachedProps.ToList();
            }
            var properties = type.GetProperties().Where(ValidateProperty);
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }

        public static bool ValidateProperty(PropertyInfo prop)
        {
            bool result = prop.CanWrite;
            result = result && (prop.GetSetMethod(true)?.IsPublic ?? false);
            result = result && (!prop.GetGetMethod()?.IsVirtual ?? false);
            //result = result && (!prop.PropertyType.IsClass || prop.PropertyType == typeof(string));
            result = result && prop.GetCustomAttributes(true).All(a => a.GetType().Name != "NotMappedAttribute");

            return result;
        }

        public static List<PropertyInfo> KeyPropertiesCache(Type type)
        {
            if (KeyProperties.TryGetValue(type.TypeHandle, out var cachedProps))
            {
                return cachedProps.ToList();
            }

            var allProperties = TypePropertiesCache(type);
            var keyProperties =
                allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a.GetType().Name == "KeyAttribute"))
                    .ToList();

            if (keyProperties.Count == 0)
            {
                var idProp =
                    allProperties.Find(p => string.Equals(p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
                if (idProp != null)
                {
                    keyProperties.Add(idProp);
                }
            }

            KeyProperties[type.TypeHandle] = keyProperties;
            return keyProperties;
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> data, string tableName, IList<PropertyInfo> properties)
        {
            var dataTable = new DataTable(tableName);
            foreach (var prop in properties)
            {
                var dataType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (dataType == typeof(DateTimeOffset))
                    dataType = typeof(DateTime);

                dataTable.Columns.Add(prop.Name, dataType);
            }

            //var typeCasts = new Type[properties.Count];
            //for (var i = 0; i < properties.Count; i++)
            //{
            //    var isEnum = properties[i].PropertyType.IsEnum;
            //    if (isEnum)
            //    {
            //        typeCasts[i] = Enum.GetUnderlyingType(properties[i].PropertyType);
            //    }
            //    //else if (properties[i].PropertyType == typeof(Guid))
            //    //{
            //    //    typeCasts[i] = 
            //    //}
            //    else
            //    {
            //        typeCasts[i] = null;
            //    }
            //}

            foreach (var item in data)
            {
                var values = new object[properties.Count];
                for (var i = 0; i < properties.Count; i++)
                {
                    var value = properties[i].GetValue(item, null);
                    if (dataTable.Columns[i].DataType == typeof(DateTime))
                        values[i] = ((DateTimeOffset) value).DateTime;
                    else
                        values[i] = value;

                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static PropertyContainer ParseProperties<T>(T obj)
        {
            var propertyContainer = new PropertyContainer();

            var typeName = typeof(T).Name;
            var validKeyNames = new[]
            {
                "Id",
                string.Format("{0}Id", typeName), string.Format("{0}_Id", typeName)
            };

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                // Skip reference types (but still include string!)
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null)
                    continue;

                // Skip methods specifically ignored
                if (property.IsDefined(typeof(DapperIgnore), false))
                    continue;

                var name = "[" + property.Name + "]";
                var value = typeof(T).GetProperty(property.Name).GetValue(obj, null);

                if (property.IsDefined(typeof(DapperKey), false) || validKeyNames.Contains(name))
                {
                    propertyContainer.AddId(name, value);
                }
                //else
                //{
                propertyContainer.AddValue(name, value);
                //}
            }

            return propertyContainer;
        }

        public static string GetSqlPairs
            (IEnumerable<string> keys, string separator = ", ")
        {
            var pairs =
                keys.Select(key => string.Format("{0}=@{1}", key, key.Replace("[", "").Replace("]", ""))).ToList();
            return string.Join(separator, pairs);
        }
    }
}