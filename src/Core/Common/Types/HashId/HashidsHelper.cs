using HashidsNet;


namespace Core.Common.Types.HashId
{
    public static class HashidsHelper
    {
        private static readonly Hashids _hashids = new Hashids("zNQCISFf/g#v", 8);
        public static string Encode(int id) => _hashids.Encode(id);
        public static int Decode(string hash) => _hashids.Decode(hash).FirstOrDefault();
    }
}
