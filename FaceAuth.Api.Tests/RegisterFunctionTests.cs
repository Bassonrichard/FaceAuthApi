using FaceAuth.Api.Functions;
using FaceAuth.Api.Helper;
using FaceAuth.Api.Models.Functions;
using FaceAuth.Api.Models.Functions.Response;
using FaceAuth.Api.Services;
using FaceAuth.Api.Services.Models.CognitiveServices.Requests;
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
    public class RegisterFunction
    {

        [Test]
        public async Task SuccessFulRegister()
        {
            #region Assume
            string dataUri = System.IO.File.ReadAllText(@"./TestData/Base64Image.txt");
            string personId = "a75e036d-48fa-42a6-85ca-dd2bb0d9db11";
            string faceId = "bd02056c-d0e7-4b6e-adef-0383ba343aff";

            var Body = new RegisterRequest()
            {
                DataUri = dataUri,
                Email = "basoich@gmail.com",
                FullName = "Richard"
            };

            var createPerson = new CreatePerson()
            {
                name = "basoich@gmail.com",
                userData = "Richard"
            };


            var DetectedFaces = new List<DetectedFace>()
            {
                new DetectedFace()
                {
                    faceId = faceId
                }
            };

            var CreateResponsePerson = new CreatePersonResponse()
            {
                personId = personId
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
                .Setup(m => m.CreatePerson(It.IsAny<string>(), It.IsAny<CreatePerson>()))
                .Returns(Task.FromResult(CreateResponsePerson));

            cogniativeService
              .Setup(m => m.AddFace(It.IsAny<string>(), It.IsAny<byte[]>()))
              .Returns(Task.CompletedTask);

            cogniativeService
             .Setup(m => m.TrainPersonGroup())
             .Returns(Task.CompletedTask);

            ILogger<Register> logger = new Mock<ILogger<Register>>().Object;
            var controller = new Register(formatter.Object, cogniativeService.Object);

            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Body)))
            };
            #endregion

            #region Act
            var actionResult = await controller.Run(request, logger);

            var returnResult = actionResult as OkObjectResult;
            var resultData = returnResult?.Value as CreatePerson;
            #endregion

            #region Assert
            Assert.AreEqual(createPerson.name, resultData.name);
            Assert.AreEqual(createPerson.userData, resultData.userData);
            #endregion
        }
    }
}