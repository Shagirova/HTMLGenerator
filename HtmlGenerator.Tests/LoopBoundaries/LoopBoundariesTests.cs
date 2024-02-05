using HtmlGenerator.Models;

namespace HtmlGenerator.Tests.LoopBoundaries
{
    public class LoopBoundariesTests
    {
        private readonly Helper _helper;

        public LoopBoundariesTests()
        {
            _helper = new Helper();
        }

        [Theory]
        [InlineData("simple_correct_template.dat", "values", "value", 5, 36)]
        public void GetLoopBoundaries_ShouldReturnCorrectBoundaries(string templateFileName, string arrayName, string objectName, int outerStartIndex, int innerStartIndex)
        {
            // Arrange
            var generator = new Generator();
            var template = _helper.LoadTemplateFromFile(templateFileName, "LoopBoundaries/Data");

            var expectedLoopBoundaries = new List<ForLoop>
            {
                new ForLoop
                {
                    ArrayName = arrayName,
                    ObjectName = objectName,
                    OuterStartIndex = outerStartIndex,
                    InnerStartIndex = innerStartIndex
                }
            };

            // Act
            var actualLoopBoundaries = _helper.InvokePrivateMethod<List<ForLoop>>(generator, "GetLoopBoundaries", template);

            // Assert
            Assert.Equal(expectedLoopBoundaries.Count, actualLoopBoundaries.Count);

            for (int i = 0; i < expectedLoopBoundaries.Count; i++)
            {
                Assert.Equal(expectedLoopBoundaries[i].ArrayName, actualLoopBoundaries[i].ArrayName);
                Assert.Equal(expectedLoopBoundaries[i].ObjectName, actualLoopBoundaries[i].ObjectName);
                Assert.Equal(expectedLoopBoundaries[i].OuterStartIndex, actualLoopBoundaries[i].OuterStartIndex);
                Assert.Equal(expectedLoopBoundaries[i].InnerStartIndex, actualLoopBoundaries[i].InnerStartIndex);
            }
        }
    }
}
