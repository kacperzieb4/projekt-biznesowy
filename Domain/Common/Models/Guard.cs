using System;

namespace AttractionCatalog.Domain.Common.Models
{
    public static class Guard
    {
        public static void AgainstNull(object value, string name)
        {
            if (value == null) throw new ArgumentNullException(name);
        }

        public static void AgainstEmptyString(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{name} cannot be empty.");
        }

        public static void AgainstNegative(double value, string name)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name);
        }
    }
}
