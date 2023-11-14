namespace ShortUrl.Tests.Integration;

public class ShortUrlTests
{
    [Theory]
    [InlineData(@"http://google.com/")]
    [InlineData(@"https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service.com")]
    [InlineData(@"https://charbelnemnom.com/mount-and-persist-azure-file-share-with-windows/")]
    [InlineData(@"https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-deadlock")]
    public async Task Create_Tests(string uriString)
    {
        var service = new ShortenUrlService(new InputValidator(), new InMemoryRepo(new KeyGenerator(keyLength: 6)));
        
        var result = await service.CreateUriAsync(uriString, CancellationToken.None);

        result.Should().NotBeNull();
        result.AbsoluteUri.Should().StartWith("https://zip.ly/");
        result.AbsolutePath.Should().HaveLength(6 + 1);
    }
    
    [Theory]
    [InlineData("https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service.com")]
    [InlineData("https://charbelnemnom.com/mount-and-persist-azure-file-share-with-windows/")]
    [InlineData("https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-deadlock")]
    public async Task Retrieve_Existing_Tests(string uriString)
    {
        var service = new ShortenUrlService(new InputValidator(), new InMemoryRepo(new KeyGenerator(keyLength: 6)));
        var create = await service.CreateUriAsync(uriString, CancellationToken.None);
        
        var result = await service.RetrieveUriAsync(create.ToString(), CancellationToken.None);

        result.GetValueOrThrow().AbsoluteUri.Should().BeSameAs(uriString);
    }
    
    [Theory]
    [InlineData(@"https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service.com")]
    [InlineData(@"https://charbelnemnom.com/mount-and-persist-azure-file-share-with-windows/")]
    [InlineData(@"https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-deadlock")]
    public async Task Retrieve_NonExistent_Tests(string uriString)
    {
        var service = new ShortenUrlService(new InputValidator(), new InMemoryRepo(new KeyGenerator(keyLength: 6)));
        
        var result = await service.RetrieveUriAsync(uriString, CancellationToken.None);

        result.HasNoValue.Should().BeTrue();
    }
    
    [Theory]
    [InlineData("https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service.com")]
    [InlineData("https://charbelnemnom.com/mount-and-persist-azure-file-share-with-windows/")]
    [InlineData("https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-deadlock")]
    public async Task Retrieve_IncreasesCounter_Tests(string uriString)
    {
        var repo = new InMemoryRepo(new KeyGenerator(6));
        var service = new ShortenUrlService(new InputValidator(), repo);
        var create = await service.CreateUriAsync(uriString, CancellationToken.None);
        
        await service.RetrieveUriAsync(create.ToString(), CancellationToken.None);
        var result = await repo.GetCounterAsync(create.AbsolutePath, CancellationToken.None);
        result.Should().Be(1);
        
        await service.RetrieveUriAsync(create.ToString(), CancellationToken.None);
        result = await repo.GetCounterAsync(create.AbsolutePath, CancellationToken.None);
        result.Should().Be(2);
    }
    
    [Theory]
    [InlineData("https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service.com", "/abc", true)]
    [InlineData("https://charbelnemnom.com/mount-and-persist-azure-file-share-with-windows/", "/123123123", true)]
    [InlineData("https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-deadlock", "/mayBEthisLONGER/", true)]
    [InlineData("https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service.com", "a-p", false)]
    [InlineData("https://charbelnemnom.com/mount-and-persist-azure-file-share-with-windows/", "/1231:23123", false)]
    [InlineData("https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-deadlock", "+notValid", false)]
    public async Task Add_Tests(string longUri, string shortPath, bool expected)
    {
        var service = new ShortenUrlService(new InputValidator(), new InMemoryRepo(new KeyGenerator(keyLength: 6)));
        
        var result = await service.TryAddShortUriAsync(shortPath, longUri, CancellationToken.None);
        if (expected)
        {
            result.HasValue.Should().BeTrue();
            var retrieved = await service.RetrieveUriAsync(result.GetValueOrThrow().AbsoluteUri, CancellationToken.None);
            retrieved.GetValueOrThrow().AbsoluteUri.Should().BeSameAs(longUri);
        }
        else
        {
            result.HasValue.Should().BeFalse();
        }
    }
}