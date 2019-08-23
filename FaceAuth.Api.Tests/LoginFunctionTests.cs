using FaceAuth.Api.Functions;
using FaceAuth.Api.Helper;
using FaceAuth.Api.Models.Functions;
using FaceAuth.Api.Models.Functions.Response;
using FaceAuth.Api.Services;
using FaceAuth.Api.Services.Models.CognitiveServices.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FaceAuth.Api.Tests
{
    public class LoginFunction
    {

        [Test]
        public async Task SuccessFulLogin()
        {
            #region Assume
            string dataUri = System.IO.File.ReadAllText(@".\TestData\Base64Image.txt");
            string personId = "a75e036d-48fa-42a6-85ca-dd2bb0d9db11";
            string faceId = "bd02056c-d0e7-4b6e-adef-0383ba343aff";

            var Body = new LoginRequest()
            {
                DataUri = dataUri,
                Email = "basoich@gmail.com"
            };

            var DetectedFaces = new List<DetectedFace>()
            {
                new DetectedFace()
                {
                    faceId = faceId
                }
            };

            var Person = new Person()
            {
                Name = "basoich@gmail.com",
                PersonId = personId,
                UserData = "Richard",
                PersistedFaceIds = new List<string>()
                {
                    faceId
                }
            };

            var LoginResponse = new LoginResponse()
            {
                name = "basoich@gmail.com",
                userData = "Richard"
            };
            #endregion

            #region Mock
            var formatter = new Mock<IFormatter>();
            formatter
                .Setup(m => m.DataUriToByteArray(It.IsAny<string>()))
                .Returns(new byte[0]);


            var cogniativeService = new Mock<ICogniativeService>();

            cogniativeService
                .Setup(m => m.DetectFaceRequest(It.IsAny<byte[]>()))
                .Returns(Task.FromResult(DetectedFaces));

            cogniativeService
                .Setup(m => m.IdentifyPerson(It.IsAny<string>()))
                .Returns(Task.FromResult(personId));

            cogniativeService
              .Setup(m => m.GetPerson(It.IsAny<string>()))
              .Returns(Task.FromResult(Person));

            ILogger<Login> logger = new Mock<ILogger<Login>>().Object;
            var controller = new Login(formatter.Object, cogniativeService.Object);

            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Body)))
            };
            #endregion

            #region Action
            var actionResult = await controller.Run(request, logger);

            var returnResult = actionResult as OkObjectResult;
            var resultData = returnResult?.Value as LoginResponse;
            #endregion

            #region Assert
            Assert.AreEqual(LoginResponse.name, resultData.name);
            Assert.AreEqual(LoginResponse.userData, resultData.userData);
            #endregion

        }
    }
}