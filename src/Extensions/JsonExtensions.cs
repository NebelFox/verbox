using System;
using System.Linq;
using System.Text.Json;

namespace Verbox.Extensions
{
    internal static class JsonExtensions
    {
        public static JsonElement GetProperty(this JsonElement container,
                                              string propertyName,
                                              string containerName,
                                              params JsonValueKind[] expectedValueKind)
        {
            if (container.AssertValueKind(containerName, JsonValueKind.Object)
                         .TryGetProperty(propertyName, out JsonElement element)
             == false)
                throw new FormatException($"{containerName} missed mandatory property {propertyName}");
            return element.AssertValueKind("propertyName", expectedValueKind);
        }

        public static string GetPropertyString(this JsonElement container,
                                               string propertyName,
                                               string containerName)
        {
            return container.GetProperty(propertyName, containerName, JsonValueKind.String).GetString();
        }

        public static string GetOptionalPropertyString(this JsonElement container,
                                                       string name,
                                                       string defaultValue = null)
        {
            return container.TryGetProperty(name, out JsonElement value)
                ? value.AssertValueKind(name, JsonValueKind.String).GetString()
                : defaultValue;
        }

        public static JsonElement AssertValueKind(this JsonElement element,
                                                  string name,
                                                  params JsonValueKind[] expectedValueKind)
        {
            if (expectedValueKind.All(kind => kind != element.ValueKind))
                throw BuildInvalidTypeException(name, expectedValueKind, element.ValueKind);
            return element;
        }

        private static FormatException BuildInvalidTypeException(object name,
                                                                 object expectedType,
                                                                 object actualType)
        {
            return new FormatException(
                $"{name} expected to be an instance of {expectedType}, but actually is {actualType}");
        }

        public static bool GetOptionalBoolProperty(this JsonElement container,
                                                   string name,
                                                   bool defaultValue = false)
        {
            if (container.TryGetProperty(name, out JsonElement value) == false)
                return defaultValue;
            return value.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => throw new FormatException($"{name} property expected to be a boolean, but got {{value.ValueKind}}")
            };
        }

        public static bool TryGetAliasedProperty(this JsonElement container,
                                                      out JsonProperty value,
                                                      params string[] names)
        {
            JsonProperty[] match = container.AssertValueKind("container with aliased property",
                                                            JsonValueKind.Object)
                                           .EnumerateObject()
                                           .Where(p => names.Contains(p.Name))
                                           .ToArray();
            switch (match.Length)
            {
            case > 1:
                throw new ArgumentException($"Multiple properties matched: {string.Join(',', match)}");
            case < 1:
                value = new JsonProperty();
                return false;
            default:
                value = match[0];
                return true;
            }
        }

        public static JsonProperty GetAliasedProperty(this JsonElement container,
                                                      string containerName,
                                                      params string[] names)
        {
            if (container.TryGetAliasedProperty(out JsonProperty property, names) == false)
                throw new FormatException(
                    $"A {containerName} element must contain one of {string.Join('|', names)} property");
            return property;
        }
    }
}
