using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api
{
    public class ErrorMessages
    {
        public readonly static string FaceNotFound = "No face was detected , please try stand in better lighting.";
        public readonly static string TooManyFacesDetected = "Too many faces detected, please make sure you are alone.";
        public readonly static string NotRegistered = "You are not registered, please register to log in. ";
        public readonly static string PersonNotFound = "We were unable to find you in our database.";
        public readonly static string ImageNotFound = "We were not able to get the image.";
    }
}
