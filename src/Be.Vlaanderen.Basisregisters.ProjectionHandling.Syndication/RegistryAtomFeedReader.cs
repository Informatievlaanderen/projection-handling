namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.Extensions.Logging;
    using Microsoft.SyndicationFeed;
    using Microsoft.SyndicationFeed.Atom;
    using System.Web;

    public interface IRegistryAtomFeedReader
    {
        /// <summary>
        /// Reads the entries of an Atom feed at the provided feedUrl.
        /// </summary>
        /// <returns>A list of <see cref="IAtomEntry "/>.</returns>
        Task<IEnumerable<IAtomEntry>> ReadEntriesAsync(Uri feedUrl, long? from);
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

        public async Task<IEnumerable<IAtomEntry>> ReadEntriesAsync(Uri feedUrl, long? from)
        {
            var entries = new List<IAtomEntry>();

            if (from.HasValue)
                feedUrl = AddQueryParameter(feedUrl, "from", from.Value.ToString());

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

        private static Uri AddQueryParameter(Uri uri, string key, string value)
        {
            var uriBuilder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[key] = value;

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }
    }
}
