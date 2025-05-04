using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ParentsApp.Helpers
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .Name ?? enumValue.ToString();
        }

        public static TEnum ParseFromDisplayName<TEnum>(string displayName)
    where TEnum : Enum
        {
            foreach (var field in typeof(TEnum).GetFields())
            {
                var attr = field.GetCustomAttribute<DisplayAttribute>();
                if ((attr != null && attr.Name == displayName)
                    || field.Name == displayName)
                {
                    return (TEnum)field.GetValue(null)!;
                }
            }
            throw new ArgumentException($"Nieprawidłowy display name '{displayName}' dla enuma {typeof(TEnum).Name}");
        }
    }
}
