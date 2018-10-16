using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FacialSimilarities
{
  class Program
  {
    const string subscriptionKey = "8974ba41ee8d4db9ac713e22d580937d";

    const string uriBase = "https://westcentralus.api.cognitive.microsoft.com";

    private static readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new DelegatingHandler[] { });

    static void Main(string[] args)
    {
      //MainAsync(args).Wait();

      CallThing().Wait();
    }

    static async Task CallThing()
    {
      var test = await TryFaceClient();
    }

    static async Task<IList<DetectedFace>> TryFaceClient()
    {
      string firstImagePath = @"C:\dev\isdal-woman\FacialSimilarities\FacialSimilarities\isdal-woman-sketch.jpg";
      string secondImagePath = @"C:\dev\isdal-woman\FacialSimilarities\FacialSimilarities\MRT.jpg";

      faceClient.Endpoint = $"{uriBase}";
      // The list of Face attributes to return.
      IList<FaceAttributeType> faceAttributes =
          new FaceAttributeType[]
          {
            FaceAttributeType.Gender , FaceAttributeType.Age,
            FaceAttributeType.Smile  , FaceAttributeType.Emotion,
            FaceAttributeType.Glasses, FaceAttributeType.Hair,
            FaceAttributeType.Occlusion
          };

      try
      {
        using (var imageFileStream = File.OpenRead(firstImagePath))
        {
          // The second argument specifies to return the faceId, while
          // the third argument specifies not to return face landmarks.
          IList<DetectedFace> faceList =
              await faceClient.Face.DetectWithStreamAsync(
                  imageFileStream, true, true, faceAttributes);

          return faceList;
        }
      }
      catch(APIErrorException f)
      {
        Console.WriteLine(f.Message);
        Console.ReadKey();
      }

      return new List<DetectedFace>();
    }
 
    static async Task MainAsync(string[] args)
    {
      // Get the path and filename to process from the user.
      Console.WriteLine("Detect faces:");
      Console.Write(
          "Enter the path to an image with faces that you wish to analyze: ");

      string firstImagePath = @"C:\dev\isdal-woman\FacialSimilarities\FacialSimilarities\isdal-woman-sketch.jpg";
      string secondImagePath = @"C:\dev\isdal-woman\FacialSimilarities\FacialSimilarities\MRT.jpg";

      if (File.Exists(firstImagePath) && File.Exists(secondImagePath))
      {
        // Execute the REST API call.
        try
        {
          var faceOneResponse = await GetFace(firstImagePath);
          var faceTwoResponse = await GetFace(secondImagePath);

          var faceOne = JsonConvert.DeserializeObject<FaceDetectResponse>(faceOneResponse);
          var faceTwo = JsonConvert.DeserializeObject<FaceDetectResponse>(faceTwoResponse);


          var verify = await GetVerification(faceOne.FaceId, faceTwo.FaceId);

          var test = JsonConvert.DeserializeObject<VerifyResponse>(verify);

          Console.WriteLine("\nWait a moment for the results to appear.\n");
        }
        catch (Exception e)
        {
          Console.WriteLine("\n" + e.Message + "\nPress Enter to exit...\n");
        }
      }
      else
      {
        Console.WriteLine("\nInvalid file path.\nPress Enter to exit...\n");
      }
      Console.ReadLine();
    }

    /// <summary>
    /// Gets the analysis of the specified image by using the Face REST API.
    /// </summary>
    /// <param name="imageFilePath">The image file.</param>
    static async void MakeAnalysisRequest(string imageFilePath)
    {
      HttpClient client = new HttpClient();

      // Request headers.
      client.DefaultRequestHeaders.Add(
          "Ocp-Apim-Subscription-Key", subscriptionKey);

      // Request parameters. A third optional parameter is "details".
      string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
          "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
          "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

      // Assemble the URI for the REST API Call.
      string uri = uriBase + "/detect" + "?" + requestParameters;

      HttpResponseMessage response;

      // Request body. Posts a locally stored JPEG image.
      byte[] byteData = GetImageAsByteArray(imageFilePath);

      using (ByteArrayContent content = new ByteArrayContent(byteData))
      {
        // This example uses content type "application/octet-stream".
        // The other content types you can use are "application/json"
        // and "multipart/form-data".
        content.Headers.ContentType =
            new MediaTypeHeaderValue("application/octet-stream");

        // Execute the REST API call.
        response = await client.PostAsync(uri, content);

        // Get the JSON response.
        string contentString = await response.Content.ReadAsStringAsync();

        // Display the JSON response.
        Console.WriteLine("\nResponse:\n");
        Console.WriteLine(JsonPrettyPrint(contentString));
        Console.WriteLine("\nPress Enter to exit...");
      }
    }

    static async Task<string> GetFace(string imageFilePath)
    {
      var contentString = string.Empty;

      HttpClient client = new HttpClient();

      // Request headers.
      client.DefaultRequestHeaders.Add(
          "Ocp-Apim-Subscription-Key", subscriptionKey);

      // Request parameters. A third optional parameter is "details".
      string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
          "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
          "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

      // Assemble the URI for the REST API Call.
      string uri = uriBase + "/detect" + "?" + requestParameters;

      HttpResponseMessage response;

      // Request body. Posts a locally stored JPEG image.
      byte[] byteData = GetImageAsByteArray(imageFilePath);

      using (ByteArrayContent content = new ByteArrayContent(byteData))
      {
        // This example uses content type "application/octet-stream".
        // The other content types you can use are "application/json"
        // and "multipart/form-data".
        content.Headers.ContentType =
            new MediaTypeHeaderValue("application/octet-stream");

        // Execute the REST API call.
        response = await client.PostAsync(uri, content);

        contentString = await response.Content.ReadAsStringAsync();
      }

      return contentString;
    }

    static async Task<string> GetVerification(string faceOneId, string faceTwoId)
    {
      var contentString = string.Empty;

      HttpClient client = new HttpClient();

      // Request headers.
      client.DefaultRequestHeaders.Add(
          "Ocp-Apim-Subscription-Key", subscriptionKey);

      client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

      string uri = uriBase + "/verify";

      var requestContent = new VerifyRequest
      {
        faceId1 = faceOneId,
        faceId2 = faceTwoId
      };

      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
      {
        Content = new StringContent("{\"faceId1\":\"78b67095-0007-4a40-a7b9-b9d08274768e\",\"faceId2\":\"78b67095-0007-4a40-a7b9-b9d08274768e\"}", Encoding.UTF8, "application/json")
      };

      var response = await client.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }

    internal class VerifyRequest
    {
      public string faceId1 { get; set; }
      public string faceId2 { get; set; }
    }

    /// <summary>
    /// Returns the contents of the specified file as a byte array.
    /// </summary>
    /// <param name="imageFilePath">The image file to read.</param>
    /// <returns>The byte array of the image data.</returns>
    static byte[] GetImageAsByteArray(string imageFilePath)
    {
      using (FileStream fileStream =
          new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
      {
        BinaryReader binaryReader = new BinaryReader(fileStream);
        return binaryReader.ReadBytes((int)fileStream.Length);
      }
    }

    /// <summary>
    /// Formats the given JSON string by adding line breaks and indents.
    /// </summary>
    /// <param name="json">The raw JSON string to format.</param>
    /// <returns>The formatted JSON string.</returns>
    static string JsonPrettyPrint(string json)
    {
      if (string.IsNullOrEmpty(json))
        return string.Empty;

      json = json.Replace(Environment.NewLine, "").Replace("\t", "");

      StringBuilder sb = new StringBuilder();
      bool quote = false;
      bool ignore = false;
      int offset = 0;
      int indentLength = 3;

      foreach (char ch in json)
      {
        switch (ch)
        {
          case '"':
            if (!ignore) quote = !quote;
            break;
          case '\'':
            if (quote) ignore = !ignore;
            break;
        }

        if (quote)
          sb.Append(ch);
        else
        {
          switch (ch)
          {
            case '{':
            case '[':
              sb.Append(ch);
              sb.Append(Environment.NewLine);
              sb.Append(new string(' ', ++offset * indentLength));
              break;
            case '}':
            case ']':
              sb.Append(Environment.NewLine);
              sb.Append(new string(' ', --offset * indentLength));
              sb.Append(ch);
              break;
            case ',':
              sb.Append(ch);
              sb.Append(Environment.NewLine);
              sb.Append(new string(' ', offset * indentLength));
              break;
            case ':':
              sb.Append(ch);
              sb.Append(' ');
              break;
            default:
              if (ch != ' ') sb.Append(ch);
              break;
          }
        }
      }

      return sb.ToString().Trim();
    }
  }
}
