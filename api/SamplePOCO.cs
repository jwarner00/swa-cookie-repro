using Newtonsoft.Json;

public class SamplePOCO
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("age")]
    public int Age { get; set; }
}