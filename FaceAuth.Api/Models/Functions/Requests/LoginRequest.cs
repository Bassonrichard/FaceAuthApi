using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAuth.Api.Models.Functions
{
    public class LoginRequest
    {
        public string DataUri { get; set; }
        public string Email { get; set; }
    }
}
