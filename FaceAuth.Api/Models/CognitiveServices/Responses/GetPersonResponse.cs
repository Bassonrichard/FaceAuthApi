using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api.Models.CognitiveServices.Responses
{
    public class GetPersonResponse
    {
        [JsonProperty("personId")]
        public string PersonId { get; set; }

        [JsonProperty("persistedFaceIds")]
        public List<string> PersistedFaceIds { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("userData")]
        public string UserData { get; set; }
    }
}
