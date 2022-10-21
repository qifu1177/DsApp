using Ds.Infrastructure.Interfaces.Models;

namespace DS.Api.Base
{
    public class MemoryCaching
    {
        public static Dictionary<string, IDtValue[]> KeyValuePairs { get; private set; }
        public static Dictionary<string,string> Setting { get; private set; }
        static MemoryCaching()
        {
            KeyValuePairs = new Dictionary<string, IDtValue[]>();
            Setting = new Dictionary<string, string>();
        }
        public static bool TryGetValues(string fileName, string indexName, out IDtValue[] dtValues)
        {
            string keyStr = string.Format("{0}-{1}", fileName, indexName);
            bool b = KeyValuePairs.ContainsKey(keyStr);
            if (b)
            {
                dtValues = KeyValuePairs[keyStr];
            }
            else
                dtValues = null;
            return b;
        }
        public static void SetValues(string fileName, string indexName, IDtValue[] dtValues)
        {
            string keyStr = string.Format("{0}-{1}", fileName, indexName);
            if (!KeyValuePairs.ContainsKey(keyStr))
                KeyValuePairs[keyStr] = dtValues;
        }
    }
}
