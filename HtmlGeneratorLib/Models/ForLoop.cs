namespace HtmlGenerator.Models;
public class ForLoop
{
    public required string ArrayName { get; set; }
    public required string ObjectName { get; set; }
    public int OuterStartIndex { get; set; }
    public int? OuterEndIndex { get; set; }
    public int InnerStartIndex { get; set; }
    public int? InnerEndIndex { get; set; }
    public string? PlaceHolder { get; set; }
    public string? ReplacableText { get; set; }

}