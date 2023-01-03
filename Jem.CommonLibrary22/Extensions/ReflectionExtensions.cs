namespace Jem.CommonLibrary22;

public static class ReflectionExtensions
{
    public static FieldInfo[] GetPublicFields(this Type type, bool includeNonPublic = false)
    {
        var flags = includeNonPublic
            ? BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            : BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance;

        if (type.IsInterface)
        {
            var fieldInfos = new List<FieldInfo>();

            var considered = new List<Type>();
            var queue = new Queue<Type>();
            considered.Add(type);
            queue.Enqueue(type);
            while (queue.Count > 0)
            {
                var subType = queue.Dequeue();
                foreach (var subInterface in subType.GetInterfaces())
                {
                    if (considered.Contains(subInterface)) continue;

                    considered.Add(subInterface);
                    queue.Enqueue(subInterface);
                }

                var typeFields = subType.GetFields(flags);

                var newFieldsInfos = typeFields
                    .Where(x => !fieldInfos.Contains(x));

                fieldInfos.InsertRange(0, newFieldsInfos);
            }

            return fieldInfos.ToArray();
        }

        return type.GetFields(flags);
    }

    public static PropertyInfo[] GetPublicProperties(this Type type, bool includeNonPublic = false)
    {
        var flags = includeNonPublic
            ? BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            : BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance;

        if (type.IsInterface)
        {
            var propertyInfos = new List<PropertyInfo>();

            var considered = new List<Type>();
            var queue = new Queue<Type>();
            considered.Add(type);
            queue.Enqueue(type);
            while (queue.Count > 0)
            {
                var subType = queue.Dequeue();
                foreach (var subInterface in subType.GetInterfaces())
                {
                    if (considered.Contains(subInterface)) continue;

                    considered.Add(subInterface);
                    queue.Enqueue(subInterface);
                }

                var typeProperties = subType.GetProperties(flags);

                var newPropertyInfos = typeProperties
                    .Where(x => !propertyInfos.Contains(x));

                propertyInfos.InsertRange(0, newPropertyInfos);
            }

            return propertyInfos.ToArray();
        }

        return type.GetProperties(flags);
    }

    public static MethodInfo[] GetPublicMethods(this Type type, bool includeNonPublic = false)
    {
        var flags = includeNonPublic
            ? BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            : BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance;

        if (type.IsInterface)
        {
            var methodInfos = new List<MethodInfo>();

            var considered = new List<Type>();
            var queue = new Queue<Type>();
            considered.Add(type);
            queue.Enqueue(type);
            while (queue.Count > 0)
            {
                var subType = queue.Dequeue();
                foreach (var subInterface in subType.GetInterfaces())
                {
                    if (considered.Contains(subInterface)) continue;

                    considered.Add(subInterface);
                    queue.Enqueue(subInterface);
                }

                var typeMethods = subType.GetMethods(flags);

                var newMethodInfos = typeMethods
                    .Where(x => !methodInfos.Contains(x));

                methodInfos.InsertRange(0, newMethodInfos);
            }

            return methodInfos.ToArray();
        }

        return type.GetMethods(flags);
    }
}