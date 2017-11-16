using Sitecore.ContentSearch.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Sitecore.Support.ContentSearch.SolrProvider.Converters
{
    public class SolrIndexFieldStorageValueFormatter : Sitecore.ContentSearch.SolrProvider.Converters.SolrIndexFieldStorageValueFormatter
    {
        public SolrIndexFieldStorageValueFormatter() : base()
        {

        }
        public override object FormatValueForIndexStorage(object value, string fieldName)
        {
            if (value == null)
            {
                return null;
            }

            var converterContext = new IndexFieldConverterContext(fieldName);
            TypeConverter converter;

            var type = value.GetType();

            if (value is IEnumerable && type.IsGenericType)
            {
                var innerType = type.GenericTypeArguments.FirstOrDefault();
                if (innerType != null)
                {
                    converter = this.Converters.GetTypeConverter(innerType);
                    var items = value as IEnumerable<object>;

                    if (items == null)
                        items = (value as IEnumerable).Cast<object>();

                    if (items != null && items.Any())
                    {
                        if (converter != null)
                        {
                            return items.Select(i => converter.ConvertToString(converterContext, i)).ToList();
                        }

                        return items.Select(x => x.ToString()).ToList();
                    }
                }
            }

            converter = this.Converters.GetTypeConverter(type);

            if (converter == null)
                return System.Convert.ToString(value, CultureInfo.InvariantCulture);

            return converter.ConvertToString(converterContext, CultureInfo.InvariantCulture, value);
        }
    }
}