using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api.Models.CognitiveServices.Requests
{
    public class CreatePerson
    {
        public string name { get; set; }
        public string userData { get; set; }
    }
}
