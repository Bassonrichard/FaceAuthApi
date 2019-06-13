using FaceAuth.Api.Helper;
using FaceAuth.Api.Models.CognitiveServices;
using FaceAuth.Api.Models.CognitiveServices.Requests;
using FaceAuth.Api.Models.CognitiveServices.Responses;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaceAuth.Api.Services
{
    public class CogniativeService
    {
        public static RestClient cogniativeServiceClient = new RestClient(Settings.CogniativeServiceUrl);
        public static async Task<List<DetectedFace>> DetectFaceRequest(byte[] Image)
        {
            try
            {
                cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

                var request = new RestRequest("detect");
                request.AddQueryParameter("returnFaceId", "true");
                request.AddQueryParameter("recognitionModel", Settings.CogniativeServiceRecognitionModel);

                request.AddParameter("application/octet-stream", Image, ParameterType.RequestBody);

                var response = await cogniativeServiceClient.PostAsync<List<DetectedFace>>(request);

                return response;
            }
            catch
            {
                throw;
            }



        }

        public static async Task<CreatePersonResponse> CreatePerson(string FaceId, CreatePerson createPerson)
        {
            try
            {
                cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

                var request = new RestRequest("persongroups/{persongroupid}/persons");
                request.AddJsonBody(createPerson);
                request.AddUrlSegment("persongroupid", Settings.CogniativeServicePersonGroupId);

                var response = await cogniativeServiceClient.PostAsync<CreatePersonResponse>(request);

                return response;
            }
            catch
            {
                throw;
            }

        }

        public static async Task AddFace(string personId, byte[] Image)
        {
            try
            {
                cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

                var request = new RestRequest("persongroups/{persongroupid}/persons/{personId}/persistedFaces");
                request.AddUrlSegment("persongroupid", Settings.CogniativeServicePersonGroupId);
                request.AddUrlSegment("personId", personId);

                request.AddParameter("application/octet-stream", Image, ParameterType.RequestBody);

                await cogniativeServiceClient.PostAsync<AddFaceResponse>(request);
            }
            catch
            {
                throw;
            }
        }

        public static async Task TrainPersonGroup()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

                var request = new RestRequest("persongroups/{persongroupid}/train");
                request.AddUrlSegment("persongroupid", Settings.CogniativeServicePersonGroupId);

                await cogniativeServiceClient.ExecuteTaskAsync(request, cancellationTokenSource.Token, Method.POST);
            }
            catch
            {
                throw;
            }

        }
    }
}
