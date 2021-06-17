using System;
using System.Text.RegularExpressions;

namespace Craigslist
{
    public partial class CraigslistListingRequest
    {
        private static readonly Regex _urlRegex = 
            new Regex(@"https?\://(\w+)\.craigslist\.org/(\w{3})(/(\w{3}))?(/d/[-\w]+)?(/(\d+).html)", RegexOptions.Compiled);

        public string Site { get; set; }
        public string? Area { get; set; }
        public string Category { get; set; }
        public string Id { get; set; }

        public Uri Uri => CreateRequestUrl();

        public CraigslistListingRequest(string site, string category, string id)
            : this(site, default, category, id) { }

        public CraigslistListingRequest(string site, string? area, string category, string id) =>
            (Site, Area, Category, Id) = (site, area, category, id);

        public CraigslistListingRequest(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ArgumentException("Invalid url", nameof(url));

            var match = _urlRegex.Match(url);

            if (match.Success == false)
                throw new ArgumentException("Unable to parse url.", nameof(url));

            Site = match.Groups[1].Value;

            if (match.Groups[3].Success == false)
            {
                Category = match.Groups[2].Value;
            }
            else
            {
                Area = match.Groups[2].Value;
                Category = match.Groups[4].Value;
            }
            
            Id = match.Groups[7].Value;
        }

        private Uri CreateRequestUrl()
        {
            var builder = new UriBuilder()
            {
                Scheme = "http",
                Host = $"{Site}.craigslist.org"
            };

            if (Area == default)
            {
                builder.Path = $"{Category}/{Id}.html";
            }
            else
            {
                builder.Path = $"{Area}/{Category}/{Id}.html";
            }

            return builder.Uri;
        }
    }
}