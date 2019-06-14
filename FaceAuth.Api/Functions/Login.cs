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
    public class Login:ControllerBase
    {
        [FunctionName("Login")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            try
            {
                var loginRequest = JsonConvert.DeserializeObject<LoginRequest>(await req.Content.ReadAsStringAsync());

                var image = Formatter.DataUriToByteArray(loginRequest.DataUri);
                //  var Url = await BlobStorageService.WriteImageToBlob(image,log);

                var detectedFace = await CogniativeService.DetectFaceRequest(image);

                if (detectedFace.Count < 0)
                {
                    return NotFound(ErrorMessages.FaceNotFound);
                }
                else if (detectedFace.Count > 1)
                {
                    return BadRequest(ErrorMessages.TooManyFacesDetected);
                }

                var personId = await CogniativeService.IdentifyPerson(detectedFace[0].faceId);
                var person = await CogniativeService.GetPerson(personId);

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
                return BadRequest(string.Format("Technical Error, unable to register: {0}", ex.InnerException.Message));
            }
        }
    }
}
