using PPSAsset.Models;

namespace PPSAsset.Services
{
    /// <summary>
    /// Interface for SEO service that handles meta tags, structured data, and search engine optimization
    /// </summary>
    public interface ISeoService
    {
        /// <summary>
        /// Gets SEO metadata for a specific page
        /// </summary>
        SeoMetadata GetPageMetadata(string pageType, string pageId = null);

        /// <summary>
        /// Gets JSON-LD schema markup for structured data
        /// </summary>
        string GetJsonLdSchema(string schemaType, object data);

        /// <summary>
        /// Gets OpenGraph meta tags
        /// </summary>
        Dictionary<string, string> GetOpenGraphTags(string title, string description, string imageUrl, string pageUrl);

        /// <summary>
        /// Gets Twitter Card meta tags
        /// </summary>
        Dictionary<string, string> GetTwitterCardTags(string title, string description, string imageUrl, string pageUrl);

        /// <summary>
        /// Generates canonical URL to prevent duplicate content issues
        /// </summary>
        string GetCanonicalUrl(string baseUrl, string path);

        /// <summary>
        /// Gets project-specific SEO metadata with optimized keywords
        /// </summary>
        SeoMetadata GetProjectMetadata(ProjectViewModel project);

        /// <summary>
        /// Gets JSON-LD Organization schema
        /// </summary>
        string GetOrganizationSchema();

        /// <summary>
        /// Gets JSON-LD BreadcrumbList for navigation structure
        /// </summary>
        string GetBreadcrumbSchema(List<BreadcrumbItem> items);

        /// <summary>
        /// Gets comprehensive schema data for a project to pass to view
        /// </summary>
        SeoMetadata GetPropertyTypeMetadata(ProjectViewModel project);

        /// <summary>
        /// Gets all schema types available for a project
        /// </summary>
        Dictionary<string, string> GetAllProjectSchemas(ProjectViewModel project);
    }

    /// <summary>
    /// Data model for SEO metadata
    /// </summary>
    public class SeoMetadata
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string CanonicalUrl { get; set; }
        public string OgImage { get; set; }
        public string OgImageAlt { get; set; }
        public string OgType { get; set; } = "website";
        public bool NoIndex { get; set; } = false;
        public bool NoFollow { get; set; } = false;
    }

    /// <summary>
    /// Data model for breadcrumb items
    /// </summary>
    public class BreadcrumbItem
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int Position { get; set; }
    }
}
