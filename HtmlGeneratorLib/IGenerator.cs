namespace HtmlGenerator;
public interface IGenerator
{
    string CreateHtml(string template, string jsonData);
}