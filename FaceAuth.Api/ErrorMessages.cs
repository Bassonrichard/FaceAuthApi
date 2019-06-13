using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api
{
    public class ErrorMessages
    {
        public readonly static string FaceNotFound = "No face was detected , please try stand in better lighting.";
        public readonly static string TooManyFacesDetected = "Too many faces detected, please make sure you are alone.";
    }
}
