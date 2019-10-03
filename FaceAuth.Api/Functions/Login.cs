using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FaceAuth.Api.Models.Functions;
using FaceAuth.Api.Helper;
using FaceAuth.Api.Models.Functions.Response;
using FaceAuth.Api.Services;
using Microsoft.AspNetCore.Http;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using System.Net;

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            [RequestBodyType(typeof(LoginRequest), "Login Request")]HttpRequest req, ILogger log)
        {
            try
            {
                var jsonString = await req.ReadAsStringAsync();
                var loginRequest = JsonConvert.DeserializeObject<LoginRequest>(jsonString);

                if (string.IsNullOrEmpty(loginRequest.DataUri))
                {
                    log.LogError("Picture not recvieved to backend for e-mail : {0}", loginRequest.Email);
                    return BadRequest(ErrorMessages.ImageNotFound);
                }


                var image = _formatter.DataUriToByteArray(loginRequest.DataUri);

                var detectedFace = await _cogniativeService.DetectFaceRequest(image);

                if (detectedFace.Count == 0)
                {
                    log.LogError("Face not found in the image: {0}", loginRequest.DataUri);
                    return NotFound(ErrorMessages.FaceNotFound);
                }
                else if (detectedFace.Count > 1)
                {
                    log.LogError("Too many faces detected in image: {0}", loginRequest.DataUri);
                    return BadRequest(ErrorMessages.TooManyFacesDetected);
                }

                var personId = await _cogniativeService.IdentifyPerson(detectedFace[0].faceId);

                if (string.IsNullOrEmpty(personId))
                {
                    log.LogError("PersonId not found with FaceId: {0}", detectedFace[0].faceId);
                    return BadRequest(ErrorMessages.NotRegistered);
                }

                var person = await _cogniativeService.GetPerson(personId);

                if (person == null)
                {
                    log.LogError("PersonId not found with PersonId: {0}", personId);
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
                    log.LogError("Invalid login for: {0}", loginRequest.Email);
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Technical Error: {ex.Message}");
                return BadRequest(string.Format("Technical Error, unable to login: {0}", ex.InnerException.Message));
            }
        }
    }
}
