using AbiturientDirectory.Models;

namespace AbiturientDirectory.Tests;

public class CompetitionTests
{
    [Fact]
    public void HasAnyForm_FalseWhenAllNull()
    {
        var c = new Competition();
        Assert.False(c.HasAnyForm);
    }

    [Fact]
    public void HasAnyForm_TrueWhenOneSet()
    {
        var c = new Competition { Evening = 2.5m };
        Assert.True(c.HasAnyForm);
    }

    [Theory]
    [InlineData(StudyForm.FullTime, 7.5)]
    [InlineData(StudyForm.Evening, null)]
    [InlineData(StudyForm.PartTime, 1.2)]
    public void ByForm_ReturnsMatchingValue(StudyForm form, double? expected)
    {
        var c = new Competition { FullTime = 7.5m, Evening = null, PartTime = 1.2m };
        Assert.Equal((decimal?)expected, c.ByForm(form));
    }
}
