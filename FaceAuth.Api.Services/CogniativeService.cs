
using FaceAuth.Api.Services.Models.CognitiveServices.Requests;
using FaceAuth.Api.Services.Models.CognitiveServices.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaceAuth.Api.Services
{ 
    public interface ICogniativeService
    {
         Task<List<DetectedFace>> DetectFaceRequest(byte[] Image);
         Task<CreatePersonResponse> CreatePerson(string FaceId, CreatePerson createPerson);
         Task AddFace(string personId, byte[] Image);
         Task TrainPersonGroup();
        Task<string> IdentifyPerson(string faceId);
        Task<Person> GetPerson(string personId);

    }

    public class CogniativeService : ICogniativeService
    {
        private readonly RestClient cogniativeServiceClient = new RestClient(Settings.CogniativeServiceUrl);
        public  async Task<List<DetectedFace>> DetectFaceRequest(byte[] Image)
        {
                cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

                var request = new RestRequest("detect");
                request.AddQueryParameter("returnFaceId", "true");
                request.AddQueryParameter("recognitionModel", Settings.CogniativeServiceRecognitionModel);

                request.AddParameter("application/octet-stream", Image, ParameterType.RequestBody);

                var response = await cogniativeServiceClient.PostAsync<List<DetectedFace>>(request);

                return response;
        }

        public  async Task<CreatePersonResponse> CreatePerson(string FaceId, CreatePerson createPerson)
        {
                cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

                var request = new RestRequest("persongroups/{persongroupid}/persons");
                request.AddJsonBody(createPerson);
                request.AddUrlSegment("persongroupid", Settings.CogniativeServicePersonGroupId);

                var response = await cogniativeServiceClient.PostAsync<CreatePersonResponse>(request);

                return response;
        }

        public  async Task AddFace(string personId, byte[] Image)
        {
                cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

                var request = new RestRequest("persongroups/{persongroupid}/persons/{personId}/persistedFaces");
                request.AddUrlSegment("persongroupid", Settings.CogniativeServicePersonGroupId);
                request.AddUrlSegment("personId", personId);

                request.AddParameter("application/octet-stream", Image, ParameterType.RequestBody);

                await cogniativeServiceClient.PostAsync<AddFaceResponse>(request);
        }

        public  async Task TrainPersonGroup()
        {
                var cancellationTokenSource = new CancellationTokenSource();
                cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

                var request = new RestRequest("persongroups/{persongroupid}/train");
                request.AddUrlSegment("persongroupid", Settings.CogniativeServicePersonGroupId);

                await cogniativeServiceClient.ExecuteTaskAsync(request, cancellationTokenSource.Token, Method.POST);
        }

        public  async Task<string> IdentifyPerson(string faceId)
        {
                cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

                var request = new RestRequest("identify");

                var identifyRequest = new IdentifyPerson()
                {
                    personGroupId = Settings.CogniativeServicePersonGroupId,
                    faceIds = new List<string>()
                    {
                        faceId
                    }
                };

                request.AddJsonBody(identifyRequest);

                var response = await cogniativeServiceClient.PostAsync<List<IdentifyPersonResponse>>(request);

                if (response.Count > 0)
                {
                    var Candidates = response.FirstOrDefault().Candidates;
                    if (Candidates.Count > 0)
                    {
                        var candidate = Candidates.FirstOrDefault();
                        return candidate.PersonId;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }                               
        }

        public  async Task<Person> GetPerson(string personId)
        {
                cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

                var request = new RestRequest("persongroups/{persongroupid}/persons/{personId}");
                request.AddUrlSegment("persongroupid", Settings.CogniativeServicePersonGroupId);
                request.AddUrlSegment("personId", personId);

                var response = await cogniativeServiceClient.GetAsync<Person>(request);

                return response;
        }
    }
}
