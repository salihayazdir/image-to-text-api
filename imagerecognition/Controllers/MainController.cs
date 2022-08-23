using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using Google.Cloud.Vision.V1;
using Image = Google.Cloud.Vision.V1.Image;
using Google.Api;
using System;
using System.Collections;
using System.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Grpc.Auth;

namespace imagerecognition.Controllers;


[Route("/api/[controller]")]
[ApiController]
public class MainController : ControllerBase
{
    private readonly IAppConfig _appconfig;
    [ActivatorUtilitiesConstructor]
    public MainController(IAppConfig appconfig)
    {
        _appconfig = appconfig;
    }

    //private readonly ILogger<MainController> _logger;

    //public MainController(ILogger<MainController> logger)
    //{
    //    _logger = logger;
    //}

    public class VisionResponse
    {
        public bool success;
        public string? message; 
        public List<string> textContent = new List<string>();
    };

    [HttpPost]
    [Route("/api/image")]
    public async Task<ActionResult> ImageTest(string image)
    {
        VisionResponse response = new VisionResponse();

        Image image1 = Image.FromUri(image);

        //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "/Users/salihayazdir/Downloads/mystic-advice-337814-c9223049dcbe.json");
        //ImageAnnotatorClient client = ImageAnnotatorClient.Create();

        string visionCredentials = _appconfig.GetVisionCredentials();
        var credential = GoogleCredential.FromJson(visionCredentials);
        var client = new ImageAnnotatorClientBuilder
        {
            Endpoint = ImageAnnotatorClient.DefaultEndpoint,
            ChannelCredentials = credential.ToChannelCredentials()
        }.Build();

        try
        {
            IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image1);
            foreach (EntityAnnotation text in textAnnotations)
            {
                Console.WriteLine($"//////////////////////////////////////////: {text.Description}");
                response.textContent.Add(text.Description);
                response.success = true;
            }
        }
        catch (AnnotateImageException e)
        {
            AnnotateImageResponse imgResponse = e.Response;
            response.message = imgResponse.Error.ToString();
            response.success = false;
        }

        string serializedResponse = JsonConvert.SerializeObject(response);

        return Ok(serializedResponse);
    }
}











//[HttpGet(Name = "GetWeatherForecast")]
//public IEnumerable<WeatherForecast> Get()
//{
//    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
//    {
//        Date = DateTime.Now.AddDays(index),
//        TemperatureC = Random.Shared.Next(-20, 55)
//        //Summary = Summaries[Random.Shared.Next(Summaries.Length)]
//    })
//    .ToArray();
//}