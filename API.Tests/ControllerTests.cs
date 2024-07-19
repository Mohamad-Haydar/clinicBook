using Xunit;

namespace API.Tests
{
    public class ControllerTests
    {
        [Theory]
        [InlineData(1,2,3)]
        [InlineData(2,3,5)]
        [InlineData(12.75,13.75,26.5)]
        public void  Test_Testing(double x, double y, double expected)
        {
            // Arrange

            // Act
            double actual = x + y;

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
