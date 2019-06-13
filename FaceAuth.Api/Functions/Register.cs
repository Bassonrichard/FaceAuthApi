
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
    public class Register: ControllerBase
    {
        [FunctionName("Register")]
        public async  Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,"post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            var registerRequest = JsonConvert.DeserializeObject<RegisterRequest>(await req.Content.ReadAsStringAsync());


            var createPerson = new CreatePerson()
            {
                name = registerRequest.Email,
                userData = registerRequest.FullName
            };

            var image = Formatter.DataUriToByteArray(registerRequest.DataUri);
            //  var Url = await BlobStorageService.WriteImageToBlob(image,log);

            var detectedFace = await CogniativeService.DetectFaceRequest(image);

            if (detectedFace.Count < 0)
            {
                return NotFound(ErrorMessages.FaceNotFound);
            }
            else if(detectedFace.Count > 1)
            {
                return BadRequest(ErrorMessages.TooManyFacesDetected);
            }

            var response = await CogniativeService.CreatePerson(detectedFace[0].faceId, createPerson);

            return Ok(response);
        }
    }
}
