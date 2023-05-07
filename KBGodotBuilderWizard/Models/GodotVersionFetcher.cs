using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KBAvaloniaCore.Miscellaneous;
using Path = KBAvaloniaCore.IO.Path;

namespace KBGodotBuilderWizard.Models;

internal class GodotVersionFetcher
{
    // Methods are fetched form a online directory in https://downloads.tuxfamily.org/godotengine/
    private const string s_repositoryURL = "https://downloads.tuxfamily.org/godotengine/";

    private readonly static string[] s_skipStrings = {
        "Parent Directory", "../",
    };


    public struct GodotInstallData
    {
        public GodotInstallData(string version, string fileName)
        {
            Version = version;
            FileName = fileName;
        }

        public string Version { get; }
        public string FileName { get; }
    }
    
    public static Task<IEnumerable<string>> FetchVersions()
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response = client.GetAsync(GodotVersionFetcher.s_repositoryURL).Result;
        string responseString = response.Content.ReadAsStringAsync().Result;

        List<string> versions = new List<string>();
        string[] lines = responseString.Split("\n");

        Regex regex = new Regex(@"(\d+\.)+\d+");
        foreach (string line in lines)
        {
            Match match = regex.Match(line);
            if (match.Success)
            {
                versions.Add(match.Value);
            }
        }

        // Remove last one, it is the version of the server
        versions.RemoveAt(versions.Count - 1);
        return Task.FromResult(versions.OrderByDescending(v => v).Except(GodotVersionFetcher.s_skipStrings));
    }

    public static async Task<IEnumerable<GodotInstallData>> FetchVersionDownloads(string currentVersion)
    {
        string url = GodotVersionFetcher.s_repositoryURL + $"/{currentVersion}/";
        IEnumerable<HtmlNode> links = GodotVersionFetcher._ReadHtmlNodes(url);
        return links.Select(link => link.Attributes["href"].Value).Except(GodotVersionFetcher.s_skipStrings).Select(link => new GodotInstallData(currentVersion, link));
    }

    private static IEnumerable<HtmlNode> _ReadHtmlNodes(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            // Make a GET request to the URL
            using (HttpResponseMessage response = client.GetAsync(url).Result)
            {
                // Read the content of the response
                string content = response.Content.ReadAsStringAsync().Result;

                // Load the HTML content into a document object using HtmlAgilityPack
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(content);

                // Search for all anchor tags in the document
                return doc.DocumentNode.Descendants("a");
            }
        }
    }
    
    public static async Task<Result> DownloadVersion(KBAvaloniaCore.IO.Path destinationFolder, string urlVersionPath)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                // Make a GET request to the URL
                using (HttpResponseMessage response = client.GetAsync(GodotVersionFetcher.s_repositoryURL + urlVersionPath).Result)
                {
                    // Read the content of the response
                    byte[] content = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(destinationFolder.FullPath, content);
                    return Result.CreateSuccess();
                }
            }
        }
        catch (Exception e)
        {
            return Result.CreateFailure(e);
        }
    }
}