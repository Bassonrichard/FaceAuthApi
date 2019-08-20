
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using FaceAuth.Api.Models.Functions;
using System.Net.Http;
using FaceAuth.Api.Services;
using Microsoft.Extensions.Logging;
using FaceAuth.Api.Helper;
using System;
using System.Linq;
using FaceAuth.Api.Models.CognitiveServices;
using FaceAuth.Api.Models.CognitiveServices.Requests;

namespace FaceAuth.Api
{
    public class Register : ControllerBase
    {
        private readonly IFormatter _formatter;
        private readonly ICogniativeService _cogniativeService;

        public Register(IFormatter formatter, ICogniativeService cogniativeService)
        {
            _formatter = formatter;
            _cogniativeService = cogniativeService; 
        }

        [FunctionName("Register")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            try
            {
                var jsonString = await req.Content.ReadAsStringAsync();

                var registerRequest = JsonConvert.DeserializeObject<RegisterRequest>(jsonString);

                if (string.IsNullOrEmpty(registerRequest.DataUri))
                {
                    return BadRequest(ErrorMessages.ImageNotFound);
                }

                var createPerson = new CreatePerson()
                {
                    name = registerRequest.Email,
                    userData = registerRequest.FullName
                };

                var image = _formatter.DataUriToByteArray(registerRequest.DataUri);

                var detectedFace = await _cogniativeService.DetectFaceRequest(image);

                if (detectedFace.Count == 0)
                {
                    return NotFound(ErrorMessages.FaceNotFound);
                }
                else if (detectedFace.Count > 1)
                {
                    return BadRequest(ErrorMessages.TooManyFacesDetected);
                }

                var person = await _cogniativeService.CreatePerson(detectedFace[0].faceId, createPerson);
                await _cogniativeService.AddFace(person.personId, image);
                await _cogniativeService.TrainPersonGroup();

                return Ok(createPerson);
            }
            catch (Exception ex)
            {
                log.LogError("Technical Error: ", ex);
                return BadRequest(string.Format("Technical Error, unable to register: {0}", ex.InnerException.Message));
            }
            
        }
    }
}
