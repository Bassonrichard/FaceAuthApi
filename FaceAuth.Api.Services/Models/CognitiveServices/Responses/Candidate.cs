using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api.Services.Models.CognitiveServices.Responses
{
    public class Candidate
    {
        [JsonProperty("personId")]
        public string PersonId { get; set; }

        [JsonProperty("confidence")]
        public float Confidence { get; set; }
    }
}
