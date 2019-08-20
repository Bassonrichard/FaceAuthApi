using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api.Services.Models.CognitiveServices.Responses
{
    public class IdentifyPersonResponse
    {
        [JsonProperty("faceId")]
        public string FaceId { get; set; }

        [JsonProperty("candidates")]
        public List<Candidate> Candidates { get; set; }
    }
}
