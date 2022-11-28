using Newtonsoft.Json;

namespace DS.Api.Base
{
    public class CsvExpressionOptions
    {
        [JsonProperty("delimiter")]
        public string Delimiter { get; set; }
        [JsonProperty("arraylength")]
        public int ArrayLength { get; set; }
        [JsonProperty("valueoption")]
        public ValueOption ValueOption { get; set; }
        [JsonProperty("datetimeoption")]
        public DateTimeOption DateTimeOption { get; set; }
    }
    public class ValueOption
    {
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("decimal")]
        public string Decimal { get; set; }
    }
    public class DateTimeOption
    {
        [JsonProperty("dateposition")]
        public int DatePosition { get; set; }
        [JsonProperty("timeposition")]
        public int TimePosition { get; set; }
        [JsonProperty("datetimedelimiter")]
        public string DateTimeDelimiter { get; set; }
        [JsonProperty("dateformat")]
        public string DateFormat { get; set; }
        [JsonProperty("timeformat")]
        public string TimeFormat { get; set; }
        
    }
}
