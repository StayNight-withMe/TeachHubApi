using HashidsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Types.HashId
{
    public static class HashidsHelper
    {
        private static readonly Hashids _hashids = new Hashids("zNQCISFf/g#v", 8);
        public static string Encode(int id) => _hashids.Encode(id);
        public static int Decode(string hash) => _hashids.Decode(hash).FirstOrDefault();
    }
}
