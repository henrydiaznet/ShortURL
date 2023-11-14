namespace ShortUrl.Tests;

public class ValidatorTests
{
    private readonly InputValidator _sut = new();

    [Theory]
    [InlineData(@"http://google.com/", true)]
    [InlineData(@"https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service.com", true)]
    [InlineData(@"http://charbelnemnom.com/mount-and-persist-azure-file-share-with-windows/", true)]
    [InlineData(@"https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-deadlock", true)]
    [InlineData(@"https://blablacom", true)]
    [InlineData(@"data://youpoop.com", false)]
    [InlineData(@"something://charbelnemnom.com//", false)]
    [InlineData(@"https://learn.microso    ft.com/", false)]
    [InlineData("http:\\google.com/", false)]
    [InlineData(@"www.contoso.com/path/file", false)]
    public void ValidateUri(string uriString,bool expected)
    {
        var result = _sut.ValidateUrl(uriString, out var url);

        result.Should().Be(expected);
        
        if (expected)
        {
            url.Should().NotBeNull();
        }
    }
    
    [Theory]
    [InlineData(@"abC", true)]
    [InlineData(@"/Ha35ak5/", true)]
    [InlineData(@"/blablacom", true)]
    [InlineData(@"", false)]
    [InlineData(@"/core/diagnostics", false)]
    [InlineData(@"not-true", false)]
    [InlineData(@"something:com", false)]
    [InlineData(@"learn.to", false)]
    public void ValidatePath(string maybePath,bool expected)
    {
        var result = _sut.ValidatePath(maybePath, out var url);

        result.Should().Be(expected);
        
        if (expected)
        {
            url.Should().NotBeNull();
        }
    }
}