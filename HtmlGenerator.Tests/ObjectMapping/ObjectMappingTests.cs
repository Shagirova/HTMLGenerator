using Newtonsoft.Json.Linq;
using System.Text;
using System.Reflection;

namespace HtmlGenerator.Tests;
public class ObjectMappingTests
{
    [Fact]
    public void ProcessObject_ShouldReplacePlaceholders_WhenObjectContainsProperties()
    {
        // Arrange
        var generator = new Generator();
        var template = new StringBuilder("{{ObjectName.Property1}} {{ObjectName.Property2}}");
        var objectName = "ObjectName";
        var props = new JObject
        {
            { "Property1", "Value1" },
            { "Property2", "Value2" }
        };

        var methodInfo = typeof(Generator).GetMethod("ProcessObject", BindingFlags.NonPublic | BindingFlags.Instance );
        var parameters = new object[] { template, objectName, props };
        
        // Act
        methodInfo.Invoke(generator, parameters);

        // Assert
        Assert.Equal("Value1 Value2", template.ToString());
    }    
}