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

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

            string requestParameters = "returnFaceId=true";

            string uri = Settings.CogniativeServiceUrl+ "detect?" + requestParameters;

            HttpResponseMessage response;

            using (ByteArrayContent content = new ByteArrayContent(Image))
            {
                content.Headers.ContentType =new MediaTypeHeaderValue("application/octet-stream");

                response = await client.PostAsync(uri, content);

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var DetectedFace = JsonConvert.DeserializeObject<List<DetectedFace>>(jsonResponse);
         
                return DetectedFace;
            }
        }

        public static async Task<CreatePersonResponse> CreatePerson(string FaceId, CreatePerson createPerson)
        {
            cogniativeServiceClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

            var request = new RestRequest("persongroups/{persongroupid}/persons");
            request.AddJsonBody(createPerson);
            request.AddUrlSegment("persongroupid", Settings.CogniativeServicePersonGroupId);

            var response = await cogniativeServiceClient.PostAsync<CreatePersonResponse>(request);

            return response;
        }

        public static async Task<AddFaceResponse> AddFace(string personId, byte[] Image)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Settings.CogniativeServiceKey);

            string requestParameters = "returnFaceId=true";

            string uri = Settings.CogniativeServiceUrl + "detect?" + requestParameters;

            HttpResponseMessage response;

            using (ByteArrayContent content = new ByteArrayContent(Image))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                response = await client.PostAsync(uri, content);

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var DetectedFace = JsonConvert.DeserializeObject<AddFaceResponse>(jsonResponse);

                return DetectedFace;
            }
        }
    }
}
