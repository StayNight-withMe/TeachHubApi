using infrastructure.Utils.HashIdConverter;
using System.ComponentModel;
using System.Globalization;

namespace Core.Common.Types.HashId
{
    public class HashidTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringValue)
            {
                if (string.IsNullOrWhiteSpace(stringValue)) return default(Hashid);

                return new Hashid(HashidsHelper.Decode(stringValue));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
