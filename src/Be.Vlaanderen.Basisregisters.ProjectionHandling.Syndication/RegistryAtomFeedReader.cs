namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.Extensions.Logging;
    using Microsoft.SyndicationFeed;
    using Microsoft.SyndicationFeed.Atom;

    public interface IRegistryAtomFeedReader
    {
        /// <summary>
        /// Reads the entries of an Atom feed at the provided feedUrl.
        /// </summary>
        /// <returns>A list of <see cref="IAtomEntry "/>.</returns>
        Task<IEnumerable<IAtomEntry>> ReadEntriesAsync(Uri feedUrl, long? from, string feedUserName = "", string feedPassword = "", bool embedEvent = true, bool embedObject = true);
    }

    public class RegistryAtomFeedReader : IRegistryAtomFeedReader
    {
        public const string HttpClientName = "registryFeedClient";

        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public RegistryAtomFeedReader(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RegistryAtomFeedReader>();
            _httpClient = httpClientFactory.CreateClient(HttpClientName);
        }

        public async Task<IEnumerable<IAtomEntry>> ReadEntriesAsync(
            Uri feedUrl,
            long? from,
            string feedUserName = "",
            string feedPassword = "",
            bool embedEvent = true,
            bool embedObject = true)
        {
            var entries = new List<IAtomEntry>();

            var embedString = string.Empty;
            if (embedObject && embedEvent)
                embedString = "embed: \"event,object\"";
            else if (embedObject)
                embedString = "embed: \"object\"";
            else if (embedEvent)
                embedString = "embed: \"event\"";

            _httpClient.DefaultRequestHeaders.Remove("X-Filtering");
            if (from.HasValue)
            {
                var filter = string.IsNullOrEmpty(embedString) ? $"{{ position: {@from} }}" : $"{{ position: {@from}, {embedString} }}";
                _httpClient.DefaultRequestHeaders.Add("X-Filtering", filter);
            }
            else
                _httpClient.DefaultRequestHeaders.Add("X-Filtering", $"{{ {embedString} }}");


            if (!string.IsNullOrEmpty(feedUserName) && !string.IsNullOrEmpty(feedPassword))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{feedUserName}:{feedPassword}")));

            try
            {
                using (var response = await _httpClient.GetAsync(feedUrl))
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                using (var xmlReader = XmlReader.Create(responseStream, new XmlReaderSettings { Async = true }))
                {
                    var atomReader = new AtomFeedReader(xmlReader);
                    while (await atomReader.Read())
                    {
                        if (atomReader.ElementType == SyndicationElementType.Item)
                            entries.Add(await atomReader.ReadEntry());
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);

                throw;
            }

            return entries;
        }
    }
}
