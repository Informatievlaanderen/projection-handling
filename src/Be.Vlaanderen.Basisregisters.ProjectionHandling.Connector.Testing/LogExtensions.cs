namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Newtonsoft.Json;

    public static class LogExtensions
    {
        public static readonly JsonSerializerSettings LogSerializerSettings = new JsonSerializerSettings();

        public static string ToLogStringLimited<T>(
            this IEnumerable<T> objects,
            Formatting formatting = Formatting.Indented,
            int max = 5)
        {
            var objectsList = objects.ToList();
            return objectsList.Count < max
                ? JsonConvert.SerializeObject(objectsList.Select(x => x?.ToAnonymousWithTypeInfo()), formatting, LogSerializerSettings)
                : "...";
        }

        public static dynamic ToAnonymousWithTypeInfo(this object @object)
            => @object.GetType().IsOfTypeAnonymous()
                ? @object
                : new
                {
                    _type = @object.GetType().Name,
                    _payload = @object
                };

        private static bool IsOfTypeAnonymous(this Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false) &&
                   type.IsGenericType && type.Name.Contains("AnonymousType") &&
                   (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$")) &&
                   (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}
