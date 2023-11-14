using ShortUrl.Core;

var service = new ShortenUrlService(new InputValidator(), new InMemoryRepo(new KeyGenerator(keyLength: 6)));

foreach (var arg in args)
{
    Console.WriteLine("{0} : {1}", service.CreateUriAsync(arg, CancellationToken.None).GetAwaiter().GetResult(), arg);
}
