using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.Result;

public class XmlToJsonResultFilter(ILogger<XmlToJsonResultFilter> logger) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        logger.LogInformation("{filterName}.{methodName} - before preparing the result", nameof(XmlToJsonResultFilter), nameof(OnResultExecutionAsync));
        
        if (context.Result is ObjectResult { Value: string xmlContent } objectResult &&
            IsXml(xmlContent))
        {
            // Convert the XML content to JSON
            var jsonContent = ConvertXmlToJson(xmlContent);

            // Update the result with JSON content
            objectResult.Value = jsonContent;

            // Update the content type to application/json
            objectResult.ContentTypes.Clear();
            objectResult.ContentTypes.Add("application/json");
        }
        
        await next();
        
        logger.LogInformation("{filterName}.{methodName} - after preparing the result", nameof(XmlToJsonResultFilter), nameof(OnResultExecutionAsync));
    }
    
    private static bool IsXml(string content)
    {
        try
        {
            var xDocument = XDocument.Parse(content);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string ConvertXmlToJson(string xmlContent)
    {
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xmlContent);

        // Convert XML to JSON using System.Text.Json
        var node = XElement.Parse(xmlContent);
        var json = JsonSerializer.Serialize(ToDictionary(node));
        return json;
    }

    private static Dictionary<string, object> ToDictionary(XElement element)
    {
        // Recursively convert XElement to a dictionary-like object
        var result = new Dictionary<string, object>();
        foreach (var child in element.Elements())
        {
            if (child.HasElements)
            {
                result[child.Name.LocalName] = ToDictionary(child);
            }
            else
            {
                result[child.Name.LocalName] = child.Value;
            }
        }

        return result;
    }
}