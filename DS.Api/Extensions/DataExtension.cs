using DS.Api.Services;
using System.Globalization;

namespace DS.Api.Extensions
{
    public static class DataExtension
    {
        public static DateTime JSZeroDt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static string ToJsonStr(this DateTimeOffset dt)
        {
            double v= (dt - DataExtension.JSZeroDt).TotalMilliseconds;
            return string.Format("{0:0}", v);
        }
        public static DateTimeOffset ToDateTimeOffset(this long value)
        {
            return new DateTimeOffset(JSZeroDt.AddMilliseconds(value));
        }
    }
}
