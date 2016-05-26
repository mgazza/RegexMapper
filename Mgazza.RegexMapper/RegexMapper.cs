using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Mgazza.RegexMapper
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RegexMapper : Attribute
    {
        protected Regex Mapping { get; set; }

        public RegexMapper(string patternGroup)
        {
            Mapping = new Regex(patternGroup);
        }

        public Dictionary<string, string> Match(string line)
        {
            var groups = Mapping.Match(line).Groups;

            return Mapping.GetGroupNames().ToDictionary(groupName => groupName, groupName => groups[(string)groupName].Value);
        }

        public void Apply<T>(string line, T result, IList<PropertyInfo> properties)
        {
            var keyValuePairs = this.Match(line);

            foreach (var keyValuePair in keyValuePairs)
            {
                var property = properties.FirstOrDefault(p => p.Name.Equals(keyValuePair.Key));

                if (property == null) continue;

                //we can only map int and string atm
                if (property.PropertyType.IsAssignableFrom(typeof(int)))
                {
                    if (!string.IsNullOrEmpty(keyValuePair.Value))
                    {
                        property.SetValue(result, int.Parse(keyValuePair.Value), null);
                    }
                }
                else if (property.PropertyType.IsAssignableFrom(typeof(string)))
                {
                    property.SetValue(result, keyValuePair.Value, null);
                }

            }
        }

        public static T Map<T>(string line) where T : new()
        {
            var result = new T();
            var typeInfo = typeof(T);

            var attributes = typeInfo.GetCustomAttributes(typeof(RegexMapper), false).Cast<RegexMapper>();
            var properties = typeof(T).GetProperties();


            foreach (var attribute in attributes)
            {
                attribute.Apply(line, result, properties);
            }

            return result;
        }
    }
}