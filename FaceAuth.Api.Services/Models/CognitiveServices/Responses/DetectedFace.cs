﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api.Services.Models.CognitiveServices.Responses
{
    public class DetectedFace
    {
        public string faceId { get; set; }
        public FaceRectangle faceRectangle { get; set; }
    }
}
