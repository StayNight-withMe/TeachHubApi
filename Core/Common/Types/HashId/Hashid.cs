using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Common.Types.HashId
{
    [TypeConverter(typeof(Hashid))]
    public struct Hashid
    {

        public int Value { get; }
        public Hashid(int value) => Value = value;

        public static implicit operator int(Hashid hashid) => hashid.Value;
        public static implicit operator Hashid(int value) => new Hashid(value);
    }
}
