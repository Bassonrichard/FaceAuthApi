using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api.Models.Functions
{
    public class RegisterRequest
    {
        public string DataUri { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}
