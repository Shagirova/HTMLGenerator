using HtmlGenerator.Models;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlGenerator
{
    public class Generator : IGenerator
    {
        private IList<ForLoop> forLoops = new List<ForLoop>();
        private const string endForPattern = @"{% endfor %}";
        public string CreateHtml(string template, string jsonData)
        {
            var initialObject = JObject.Parse(jsonData);
            return ProcessTemplate(initialObject, template);
        }

        static IList<ForLoop> GetLoopBoundaries(string template)
        {
            var loopStartIndexes = FindStartLoopIndexes(template);
            var loopEndIndexes = FindEndLoopIndexes(template);

            if (loopEndIndexes.Count != loopStartIndexes.Count)
            {
                throw new ArgumentException("Invalid template content. Starting and ending of for loop are not equal");
            }

            var startLoopStack = new Stack<ForLoop>();
            int i = 0, j = 0;
            while (i < loopStartIndexes.Count && j < loopEndIndexes.Count)
            {
                if (loopStartIndexes[i].InnerStartIndex < loopEndIndexes[j])
                {
                    startLoopStack.Push(loopStartIndexes[i]);
                    i++;
                }
                else
                {
                    var current = startLoopStack.Pop();
                    current.InnerEndIndex = loopEndIndexes[j];
                    current.OuterEndIndex = loopEndIndexes[j] + endForPattern.Length;
                    current.PlaceHolder = template.Substring(current.InnerStartIndex, (int)current.InnerEndIndex - current.InnerStartIndex).TrimEnd();
                    current.ReplacableText = template.Substring(current.OuterStartIndex, (int)current.OuterEndIndex - current.OuterStartIndex);
                    j++;
                }
            }

            while (j < loopEndIndexes.Count)
            {
                var current = startLoopStack.Pop();
                current.InnerEndIndex = loopEndIndexes[j];
                current.OuterEndIndex = loopEndIndexes[j] + endForPattern.Length;
                current.PlaceHolder = template.Substring(current.InnerStartIndex, (int)current.InnerEndIndex - current.InnerStartIndex).TrimEnd();
                current.ReplacableText = template.Substring(current.OuterStartIndex, (int)current.OuterEndIndex - current.OuterStartIndex);
                j++;
            }

            return loopStartIndexes;
        }

        static IList<ForLoop> FindStartLoopIndexes(string template)
        {
            var result = new List<ForLoop>();
            var startForLoopPattern = @"[\s\n]*{%\s*for\s+(\w+.)\s+in\s+([\w.]+)\s*%}";
            var startRegex = new Regex(startForLoopPattern, RegexOptions.Singleline);

            var startMatches = startRegex.Matches(template);
            foreach (var match in startMatches.Cast<Match>())
            {
                result.Add(
                    new ForLoop()
                    {
                        ArrayName = match.Groups[2].Value.Trim(),
                        ObjectName = match.Groups[1].Value.Trim(),
                        OuterStartIndex = match.Groups[0].Index,
                        InnerStartIndex = match.Index + match.Length,
                    });
            }
            return result;
        }

        static IList<int> FindEndLoopIndexes(string template)
        {
            var indexes = new List<int>();
            int index = template.IndexOf(endForPattern);

            while (index != -1)
            {
                indexes.Add(index);
                index = template.IndexOf(endForPattern, index + 1);
            }

            return indexes;
        }

        private string ProcessTemplate(JObject rootObject, string template)
        {
            var strTemplate = new StringBuilder(template);
            forLoops = GetLoopBoundaries(strTemplate.ToString());

            foreach (var prop in rootObject)
            {
                if (prop.Value is JArray)
                {
                    var loop = forLoops.FirstOrDefault(l => l.ArrayName == prop.Key);
                    ProcessLoop(strTemplate, loop, prop.Value);
                }
            }

            foreach (var prop in rootObject)
            {
                if (prop.Value is JObject jObject)
                {
                    ProcessObject(strTemplate, prop.Key, jObject);
                }
                else if (prop.Value is not JArray)
                {
                    ProcessPrimitiveType(strTemplate, prop.Key, prop.Value.ToString());
                }
            }           

            return strTemplate.ToString();
        }
        private void ProcessLoop(StringBuilder template, ForLoop loop, JToken items)
        {
            var loopContent = new StringBuilder();
            foreach (var item in items)
            {
                var placeHolder = new StringBuilder(loop.PlaceHolder);
                if (item is JObject jObject)
                {
                    ProcessObject(placeHolder, loop.ObjectName, jObject);
                }
                else
                {
                    ProcessPrimitiveType(placeHolder, loop.ObjectName, item.Value<string>());
                }
                loopContent.Append(placeHolder.ToString());
            }

            template.Replace(loop.ReplacableText, loopContent.ToString());
        }

        private void ProcessObject(StringBuilder template, string objectName, JObject props)
        {
            var pattern = $@"\{{\{{\s*{Regex.Escape(objectName)}\.([^|{{}}]+)(\|*)([^|{{}}]+)?\s*\}}\}}";
            var regex = new Regex(pattern);
            var matches = regex.Matches(template.ToString());

            foreach (var match in matches.Cast<Match>())
            {
                var matchObjectName = match.Groups[0].Value.Trim();
                var propertyName = match.Groups[1].Value.Trim();
                string? typeName = match.Groups[3].Value.Trim();

                if (props.ContainsKey(propertyName))
                {
                    var propValue = props[propertyName];
                    if (propValue is JObject propertyObject)
                    {
                        ProcessObject(template, propertyName, propertyObject);
                    }
                    else
                    {
                        template.Replace(matchObjectName, ApplyType(propValue.ToString(), typeName));
                    }
                }
            }

            foreach (var prop in props.Properties())
            {
                var innerLoop = forLoops.FirstOrDefault(l => string.Equals(l.ArrayName, $"{objectName}.{prop.Name}"));
                if (innerLoop != null)
                {
                    ProcessLoop(template, innerLoop, prop.Value);
                }
            }
        }

        private static void ProcessPrimitiveType(StringBuilder template, string objectName, string? primitiveValue)
        {
            var pattern = $@"\{{\{{\s*{Regex.Escape(objectName)}\s*(\|*)([^|{{}}]+)?\s*\}}\}}";
            var regex = new Regex(pattern);
            var matches = regex.Matches(template.ToString());

            foreach (var match in matches.Cast<Match>())
            {
                string? typeName = match.Groups[2].Value.Trim();
                template.Replace(match.Value, ApplyType(primitiveValue, typeName));
            }
        }

        static string? ApplyType(string? input, string typeName)
        {
            return (typeName?.ToLowerInvariant()) switch
            {
                "price" => $"${input}",
                "paragraph" => $"<p>{input}</p>",
                _ => input,
            };
        }
    }
}
