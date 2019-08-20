using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using FaceAuth.Api.Models.Functions;
using FaceAuth.Api.Helper;
using FaceAuth.Api.Services;
using FaceAuth.Api.Models.Functions.Response;

namespace FaceAuth.Api.Functions
{
    public class Login : ControllerBase
    {
        private readonly IFormatter _formatter;
        private readonly ICogniativeService _cogniativeService;

        public Login(IFormatter formatter, ICogniativeService cogniativeService)
        {
            _formatter = formatter;
            _cogniativeService = cogniativeService;
        }

        [FunctionName("Login")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            try
            {
                var jsonString = await req.Content.ReadAsStringAsync();
                var loginRequest = JsonConvert.DeserializeObject<LoginRequest>(jsonString);

                if (string.IsNullOrEmpty(loginRequest.DataUri))
                {
                    return BadRequest(ErrorMessages.ImageNotFound);
                }


                var image = _formatter.DataUriToByteArray(loginRequest.DataUri);

                var detectedFace = await _cogniativeService.DetectFaceRequest(image);

                if (detectedFace.Count == 0)
                {
                    return NotFound(ErrorMessages.FaceNotFound);
                }
                else if (detectedFace.Count > 1)
                {
                    return BadRequest(ErrorMessages.TooManyFacesDetected);
                }

                var personId = await _cogniativeService.IdentifyPerson(detectedFace[0].faceId);

                if (string.IsNullOrEmpty(personId))
                {
                    return BadRequest(ErrorMessages.NotRegistered);
                }

                var person = await _cogniativeService.GetPerson(personId);

                if (person == null)
                {
                    return NotFound(ErrorMessages.PersonNotFound);
                }

                if (loginRequest.Email.ToLower() == person.Name.ToLower())
                {
                    var loginResponse = new LoginResponse()
                    {
                        name = person.Name,
                        userData = person.UserData
                    };

                    return Ok(loginResponse);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                log.LogError("Technical Error: ", ex);
                return BadRequest(string.Format("Technical Error, unable to login: {0}", ex.InnerException.Message));
            }
        }
    }
}
