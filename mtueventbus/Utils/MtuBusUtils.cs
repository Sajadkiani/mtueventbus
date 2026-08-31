using System.Runtime.InteropServices.JavaScript;
using System.Text;

public static class MtuEventBusNameFormatter
{
    public static string ToQueueName(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        
        var serviceName = type.FullName.Split('.').First();
        var queueName = $"{DashSeparateString(serviceName)}.{DashSeparateString(type.Name)}";
        
        return queueName;
    }

    private static string DashSeparateString(string serviceName)
    {
        var stringBuilder = new StringBuilder();
        for (int i = 0; i < serviceName.Length ; i++)
        {
            if (char.IsUpper(serviceName[i]))
            {
                if (i > 0)
                {
                    stringBuilder.Append('-');
                    stringBuilder.Append(char.ToLower(serviceName[i]));
                }
            }
        }

        return stringBuilder.ToString();
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