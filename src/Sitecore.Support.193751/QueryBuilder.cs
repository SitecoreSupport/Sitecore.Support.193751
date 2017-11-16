using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Sitecore.Support.ContentSearch.Utilities
{
    class QueryBuilder : Sitecore.ContentSearch.Utilities.QueryBuilder
    {
        protected override object ConvertIndexFieldValue(IProviderSearchContext context, string fieldName, string value, object defaultValue)
        {
            string str;
            if (string.IsNullOrEmpty(fieldName))
            {
                return defaultValue;
            }
            if (context == null)
            {
                return defaultValue;
            }
            if (context.Index == null)
            {
                return defaultValue;
            }
            if (context.Index.Configuration == null)
            {
                return defaultValue;
            }
            IFieldMap fieldMap = context.Index.Configuration.FieldMap;
            if (fieldMap == null)
            {
                return defaultValue;
            }
            AbstractSearchFieldConfiguration fieldConfiguration = fieldMap.GetFieldConfiguration(fieldName);
            if (fieldConfiguration == null)
            {
                return defaultValue;
            }
            if (!fieldConfiguration.Attributes.TryGetValue("type", out str))
            {
                return defaultValue;
            }
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }
            Type conversionType = Type.GetType(str, false);
            if (conversionType == null)
            {
                Log.Warn($"Type '{str}' mapped to the index field '{value}' is undefined.", typeof(LinqHelper));
                return defaultValue;
            }
            if (conversionType == typeof(string))
            {
                return value;
            }
            try
            {
                return System.Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);

            }
            catch (Exception exception)
            {
                Log.Warn($"Cannot convert value '{value}' to type '{str}' mapped to the index field '{fieldName}': {exception.Message}", typeof(LinqHelper));
                return defaultValue;
            }
        }
    }
}