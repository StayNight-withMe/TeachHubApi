using System.ComponentModel;

namespace Core.Common.Types.HashId
{
    //[TypeConverter(typeof(Hashid))]
    [TypeConverter(typeof(HashidTypeConverter))]
    public struct Hashid
    {
        public int Value { get; }
        public Hashid(int value) => Value = value;

        public static implicit operator int(Hashid hashid) => hashid.Value;
        public static implicit operator Hashid(int value) => new Hashid(value);
    }
}
