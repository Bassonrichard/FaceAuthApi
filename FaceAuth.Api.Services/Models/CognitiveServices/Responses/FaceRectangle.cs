using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api.Services.Models.CognitiveServices.Responses
{
    public class FaceRectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}
