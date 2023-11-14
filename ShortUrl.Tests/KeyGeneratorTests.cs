namespace ShortUrl.Tests;

public class KeyGeneratorTests
{
    private const int KEYLENGTH = 6;
    private readonly KeyGenerator _sut = new KeyGenerator(KEYLENGTH);
    
    [Fact]
    public void Test()
    {
        var result = Enumerable.Range(0, 100000).Select(x => _sut.GetKey()).ToList();

        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(x =>
        {
            x.Length.Should().Be(KEYLENGTH + 1);
            x.Should().StartWith("/");
        });
        result.Distinct().Count().Should().Be(100000);
    }
}