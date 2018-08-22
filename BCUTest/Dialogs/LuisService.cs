using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BCUTest.Dialogs
{
    [Serializable]
    public class LuisService
    {
        private readonly string luisServiceHostName;
        private readonly string luisId;
        private readonly string luisKey;

        public LuisService(string hostName, string id, string key)
        {
            luisServiceHostName = hostName;
            luisId = id;
            luisKey = key;
        }

        public string GetIntent(string query)
        {
            var client = new RestClient(luisServiceHostName + "/luis/v2.0/apps/" + luisId +
                "?subscription-key="+luisKey+ "&staging=true&q="+query);
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Deserialize the response JSON
                LuisAnswer answer = JsonConvert.DeserializeObject<LuisAnswer>(response.Content);               
                return answer.topScoringIntent.intent;
            }
            else
            {
                throw new Exception($"Luis call failed with status code {response.StatusCode} {response.StatusDescription}");
            }
        }
    }


    public class LuisAnswer
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }

        public IList<Entities> entities { get; set; }
        public SentimentAnalysis sentimentAnalysis { get; set; }
        
    }

    public class TopScoringIntent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Entities
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
    }

    public class SentimentAnalysis
    {
        public string label { get; set; }
        public double score { get; set; }
    }
}