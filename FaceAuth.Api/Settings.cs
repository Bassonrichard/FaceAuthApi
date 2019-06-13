using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api
{
    public class Settings
    {
        public readonly static string StorageURL = Environment.GetEnvironmentVariable("StorageURL");
        public readonly static string CogniativeServiceUrl = Environment.GetEnvironmentVariable("CogniativeServiceUrl");
        public readonly static string CogniativeServiceKey = Environment.GetEnvironmentVariable("CogniativeServiceKey");
        public readonly static string CogniativeServicePersonGroupId = Environment.GetEnvironmentVariable("CogniativeServicePersonGroupId");
        public readonly static string CogniativeServiceRecognitionModel = Environment.GetEnvironmentVariable("CogniativeServiceRecognitionModel");
    }
}
