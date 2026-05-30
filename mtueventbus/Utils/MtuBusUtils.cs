using System.Text;

public static class MtuEventBusNameFormatter
{
    public static string GetQueueName(string name)
    {
        return $"{name}.service";
    }

    public static string ToRoutingKey<T>()
    {
        return ToRoutingKey(typeof(T));
    }

    public static string ToRoutingKey(Type type)
    {
        var builder = new StringBuilder();
        var name = type.Name ??
                   throw new NullReferenceException($"type {type.Name} has no fullname");

        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];

            if (i > 0 && char.IsUpper(c))
            {
                builder.Append('.');
            }

            builder.Append(char.ToLowerInvariant(c));
        }

        return builder.ToString();
    }
}