namespace WebsiteStatusApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient _httpClient;
        private readonly string _url = "https://github.com/Jd0824um"; //Put whatever URL you want here. 
        private readonly int _pingInterval = 60 * 1000;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _httpClient = new HttpClient(); // Start when the service starts
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _httpClient.Dispose(); // Properly shut down the client when the service shuts down.
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                HttpResponseMessage result = await _httpClient.GetAsync(_url);

                if (result.IsSuccessStatusCode) // If we are up, we are OK!
                {
                    _logger.LogInformation("The website is up! Status Code {StatusCode}", result.StatusCode);
                } else // Otherwise log an error 
                {
                    _logger.LogError("The website is down! Status Code {StatusCode} {ReasonPhase}", result.StatusCode, result.ReasonPhrase);
                }
                await Task.Delay(_pingInterval, stoppingToken); // Lets check based off interval {x}.
            }
        }
    }
}