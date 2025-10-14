using System.Text.Json.Serialization;

namespace NotelyAPI.Models
{
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum NoteCategory
        {
            Personal,
            Home,
            Business
        }
    
}
