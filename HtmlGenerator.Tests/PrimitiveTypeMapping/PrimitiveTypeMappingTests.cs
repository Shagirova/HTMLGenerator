using System.Reflection;
using System.Text;

namespace HtmlGenerator.Tests.PrimitiveTypeMapping;

public class PrimitiveTypeMappingTests
{
    [Theory]
    [InlineData("{{ObjectName|TypeName }}{{ ObjectName }}", "ObjectName", "TestValue", "TestValueTestValue")]
    [InlineData("{{ AnotherObject }}{{ AnotherObject|price }}", "AnotherObject", "TestValue", "TestValue$TestValue")]
    public void ProcessPrimitiveType_ReplacesMatchingPattern(string inputTemplate, string objectName, string primitiveValue, string expectedOutput)
    {
        // Arrange
        var template = new StringBuilder(inputTemplate);

        var methodInfo = typeof(Generator).GetMethod("ProcessPrimitiveType", BindingFlags.Static | BindingFlags.NonPublic);
        var parameters = new object[] { template, objectName, primitiveValue };

        // Act
        methodInfo.Invoke(null, parameters);

        // Assert
        Assert.Equal(expectedOutput, template.ToString());
    }

    [Theory]
    [InlineData("price", "10", "$10")]
    [InlineData("paragraph", "Test", "<p>Test</p>")]
    [InlineData("unknownType", "Value", "Value")]
    [InlineData("", "EmptyValue", "EmptyValue")]
    public void ProcessPrimitiveType_AppliesTypeCorrectly(string? typeName, string input, string expectedOutput)
    {
        // Arrange
        var objectName = "PrimitiveName";

        var template = new StringBuilder("{{" + objectName + " | " + typeName + " }}");

        var methodInfo = typeof(Generator).GetMethod("ProcessPrimitiveType", BindingFlags.Static | BindingFlags.NonPublic);

        // Act
        methodInfo.Invoke(null, new object[] { template, objectName, input });

        // Assert
        Assert.Equal(expectedOutput, template.ToString());
    }
}
