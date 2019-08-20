using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api.Services.Models.CognitiveServices.Requests
{
    public class IdentifyPerson
    {
        public string personGroupId { get; set; }
        public List<string> faceIds { get; set; }
        public int maxNumOfCandidatesReturned { get; set; } = 1;
        public float confidenceThreshold { get; set; } = 0.5f;
    }
}
