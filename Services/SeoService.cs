using System.Text.Json;
using PPSAsset.Models;

namespace PPSAsset.Services
{
    /// <summary>
    /// SEO service implementation for managing meta tags, structured data, and search engine optimization
    /// </summary>
    public class SeoService : ISeoService
    {
        private readonly ILogger<SeoService> _logger;
        private const string CompanyName = "PPS Asset";
        private const string CompanyUrl = "https://www.ppsasset.com";
        private const string CompanyEmail = "info@ppsasset.com";
        private const string CompanyPhone = "+66 (0)2-XXX-XXXX";

        public SeoService(ILogger<SeoService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets SEO metadata for a specific page type
        /// </summary>
        public SeoMetadata GetPageMetadata(string pageType, string pageId = null)
        {
            return pageType?.ToLower() switch
            {
                "home" => GetHomePageMetadata(),
                "about" => GetAboutPageMetadata(),
                "contact" => GetContactPageMetadata(),
                "project" => GetProjectPageMetadata(pageId),
                "singlehouse" => GetPropertyTypeMetadata("บ้านเดี่ยว", "Single House"),
                "townhome" => GetPropertyTypeMetadata("ทาวน์โฮม", "Townhome"),
                "twin" => GetPropertyTypeMetadata("บ้านแฝด", "Twin House"),
                _ => GetDefaultMetadata()
            };
        }

        /// <summary>
        /// Gets home page SEO metadata
        /// </summary>
        private SeoMetadata GetHomePageMetadata()
        {
            return new SeoMetadata
            {
                Title = "PPS Asset | บ้านเดี่ยว ทาวน์โฮม กรุงเทพ - เดอะริคโค้ เรสซิเดนซ์",
                Description = "ค้นหาบ้านเดี่ยว ทาวน์โฮม และบ้านแฝด ในกรุงเทพ จากเดอะริคโค้ เรสซิเดนซ์ โครงการที่ดีที่สุดในหทัยราษฎร์ จตุโชติ วงแหวน เรามีโครงการมากมายให้คุณเลือก",
                Keywords = "บ้านเดี่ยว กรุงเทพ, ทาวน์โฮม กรุงเทพ, เดอะริคโค้, บ้านใหม่ กรุงเทพ, โครงการบ้าน, ซื้อบ้าน, หทัยราษฎร์, จตุโชติ, วงแหวน",
                OgType = "website",
                OgImage = "/images/og-home.jpg",
                OgImageAlt = "PPS Asset - บ้านเดี่ยว ทาวน์โฮม กรุงเทพ"
            };
        }

        /// <summary>
        /// Gets about page SEO metadata
        /// </summary>
        private SeoMetadata GetAboutPageMetadata()
        {
            return new SeoMetadata
            {
                Title = "เกี่ยวกับ PPS Asset | ผู้พัฒนาโครงการที่อยู่อาศัยชั้นนำ",
                Description = "เรียนรู้เพิ่มเติมเกี่ยวกับ PPS Asset นักพัฒนาอสังหาริมทรัพย์ที่มีความเชี่ยวชาญในการสร้างชุมชนที่มีคุณภาพในกรุงเทพและปริมณฑล",
                Keywords = "PPS Asset, ผู้พัฒนาบ้าน, นักพัฒนาอสังหาริมทรัพย์, โครงการบ้าน, ประวัติ, ทีม",
                OgType = "website",
                OgImage = "/images/og-about.jpg",
                OgImageAlt = "เกี่ยวกับ PPS Asset"
            };
        }

        /// <summary>
        /// Gets contact page SEO metadata
        /// </summary>
        private SeoMetadata GetContactPageMetadata()
        {
            return new SeoMetadata
            {
                Title = "ติดต่อ PPS Asset | ข้อมูลติดต่อและที่อยู่",
                Description = "ติดต่อ PPS Asset เพื่อซื้อบ้าน ทาวน์โฮม หรือสอบถามข้อมูลเพิ่มเติมเกี่ยวกับโครงการของเรา",
                Keywords = "ติดต่อ, ข้อมูลติดต่อ, PPS Asset, หมายเลขโทรศัพท์, อีเมล, ที่อยู่",
                OgType = "website"
            };
        }

        /// <summary>
        /// Gets project page SEO metadata
        /// </summary>
        private SeoMetadata GetProjectPageMetadata(string projectId)
        {
            return new SeoMetadata
            {
                Title = "โครงการบ้าน | PPS Asset",
                Description = "สำรวจโครงการบ้านและทาวน์โฮมของเรา พบสิ่งอำนวยความสะดวก ราคา และตำแหน่งที่เหมาะสม",
                Keywords = "โครงการบ้าน, ทาวน์โฮม, บ้านเดี่ยว, PPS Asset",
                OgType = "product"
            };
        }

        /// <summary>
        /// Gets property type page SEO metadata
        /// </summary>
        private SeoMetadata GetPropertyTypeMetadata(string thaiType, string englishType)
        {
            return new SeoMetadata
            {
                Title = $"{thaiType} | {englishType} - PPS Asset กรุงเทพ",
                Description = $"ค้นหา{thaiType}ใหม่ในกรุงเทพและปริมณฑล จากเดอะริคโค้ เรสซิเดนซ์ ราคาดี ติดต่อสอบถามได้ทุกวัน",
                Keywords = $"{thaiType}, {englishType}, กรุงเทพ, ซื้อบ้าน, โครงการใหม่, บ้านราคาดี",
                OgType = "website"
            };
        }

        /// <summary>
        /// Gets default SEO metadata
        /// </summary>
        private SeoMetadata GetDefaultMetadata()
        {
            return new SeoMetadata
            {
                Title = "PPS Asset | บ้านเดี่ยว ทาวน์โฮม กรุงเทพ",
                Description = "ค้นหาบ้านและโครงการที่อยู่อาศัยมีคุณภาพจาก PPS Asset",
                Keywords = "บ้าน, ทาวน์โฮม, กรุงเทพ, อสังหาริมทรัพย์",
                OgType = "website"
            };
        }

        /// <summary>
        /// Gets project-specific SEO metadata with optimized keywords
        /// </summary>
        public SeoMetadata GetProjectMetadata(ProjectViewModel project)
        {
            if (project == null)
                return GetDefaultMetadata();

            var thaiName = project.NameTh;
            var englishName = project.NameEn;
            var location = project.Details?.Location ?? "กรุงเทพ";
            var area = project.Location?.District ?? "";

            return new SeoMetadata
            {
                Title = $"{thaiName} | {englishName} - {location} {area} | PPS Asset",
                Description = $"สำรวจ{thaiName} ({englishName}) ที่{location} {area} ราคาดี มีสิ่งอำนวยความสะดวกครบครัน ติดต่อขายและเช่าได้",
                Keywords = $"{thaiName}, {englishName}, {location}, {area}, บ้าน, โครงการใหม่, ซื้อบ้าน, ราคาบ้าน",
                OgType = "product",
                OgImage = project.Images?.Hero,
                OgImageAlt = $"{thaiName} - {englishName}"
            };
        }

        /// <summary>
        /// Gets JSON-LD schema markup for structured data
        /// </summary>
        public string GetJsonLdSchema(string schemaType, object data)
        {
            return schemaType?.ToLower() switch
            {
                "organization" => GetOrganizationSchema(),
                "breadcrumb" => GetBreadcrumbSchema(data as List<BreadcrumbItem>),
                "property" => GetPropertySchema(data as ProjectViewModel),
                "product" => GetProductSchema(data as ProjectViewModel),
                "apartmentcomplex" => GetApartmentComplexSchema(data as ProjectViewModel),
                "house" => GetHouseSchema(data as ProjectViewModel),
                _ => "{}"
            };
        }

        /// <summary>
        /// Gets OpenGraph meta tags
        /// </summary>
        public Dictionary<string, string> GetOpenGraphTags(string title, string description, string imageUrl, string pageUrl)
        {
            return new Dictionary<string, string>
            {
                { "og:title", title },
                { "og:description", description },
                { "og:image", imageUrl },
                { "og:image:alt", title },
                { "og:url", pageUrl },
                { "og:type", "website" },
                { "og:site_name", CompanyName }
            };
        }

        /// <summary>
        /// Gets Twitter Card meta tags
        /// </summary>
        public Dictionary<string, string> GetTwitterCardTags(string title, string description, string imageUrl, string pageUrl)
        {
            return new Dictionary<string, string>
            {
                { "twitter:card", "summary_large_image" },
                { "twitter:title", title },
                { "twitter:description", description },
                { "twitter:image", imageUrl },
                { "twitter:url", pageUrl }
            };
        }

        /// <summary>
        /// Generates canonical URL to prevent duplicate content issues
        /// </summary>
        public string GetCanonicalUrl(string baseUrl, string path)
        {
            if (string.IsNullOrEmpty(path))
                return baseUrl;

            // Remove trailing slashes and query parameters
            path = path.TrimEnd('/').Split('?')[0];
            return $"{baseUrl?.TrimEnd('/')}{path}";
        }

        /// <summary>
        /// Gets JSON-LD Organization schema
        /// </summary>
        public string GetOrganizationSchema()
        {
            var organization = new
            {
                @context = "https://schema.org",
                @type = "RealEstateAgent",
                name = CompanyName,
                url = CompanyUrl,
                email = CompanyEmail,
                telephone = CompanyPhone,
                address = new
                {
                    @type = "PostalAddress",
                    addressCountry = "TH",
                    addressRegion = "Bangkok"
                },
                sameAs = new[]
                {
                    "https://www.facebook.com/ppsasset",
                    "https://www.instagram.com/ppsasset"
                }
            };

            return JsonSerializer.Serialize(organization, new JsonSerializerOptions { WriteIndented = false });
        }

        /// <summary>
        /// Gets JSON-LD BreadcrumbList for navigation structure
        /// </summary>
        public string GetBreadcrumbSchema(List<BreadcrumbItem> items)
        {
            if (items == null || items.Count == 0)
                return "{}";

            var breadcrumb = new
            {
                @context = "https://schema.org",
                @type = "BreadcrumbList",
                itemListElement = items.Select(item => new
                {
                    @type = "ListItem",
                    position = item.Position,
                    name = item.Name,
                    item = item.Url
                }).ToList()
            };

            return JsonSerializer.Serialize(breadcrumb, new JsonSerializerOptions { WriteIndented = false });
        }

        /// <summary>
        /// Gets JSON-LD Property/House schema
        /// </summary>
        private string GetPropertySchema(ProjectViewModel project)
        {
            if (project == null)
                return "{}";

            var propertySchema = new
            {
                @context = "https://schema.org",
                @type = "SingleFamilyResidence",
                name = project.NameTh,
                description = project.Description,
                image = project.Images?.Hero,
                address = new
                {
                    @type = "PostalAddress",
                    addressCountry = "TH",
                    addressRegion = "Bangkok",
                    addressLocality = project.Location?.District,
                    streetAddress = project.Details?.Location
                },
                geo = new
                {
                    @type = "GeoCoordinates",
                    latitude = project.Location?.Latitude ?? (decimal)13.7563,
                    longitude = project.Location?.Longitude ?? (decimal)100.5018
                }
            };

            return JsonSerializer.Serialize(propertySchema, new JsonSerializerOptions { WriteIndented = false });
        }

        /// <summary>
        /// Gets JSON-LD Product schema for real estate listing
        /// </summary>
        private string GetProductSchema(ProjectViewModel project)
        {
            if (project == null)
                return "{}";

            var productSchema = new
            {
                @context = "https://schema.org",
                @type = "Product",
                name = project.NameTh,
                description = project.Description,
                image = project.Images?.Hero,
                brand = new { @type = "Brand", name = CompanyName },
                offers = new
                {
                    @type = "AggregateOffer",
                    availability = "https://schema.org/InStock",
                    price = project.Details?.PriceRange,
                    priceCurrency = "THB"
                }
            };

            return JsonSerializer.Serialize(productSchema, new JsonSerializerOptions { WriteIndented = false });
        }

        /// <summary>
        /// Gets JSON-LD ApartmentComplex schema for multi-unit residential properties
        /// </summary>
        private string GetApartmentComplexSchema(ProjectViewModel project)
        {
            if (project == null)
                return "{}";

            var apartmentSchema = new
            {
                @context = "https://schema.org",
                @type = "ApartmentComplex",
                name = project.NameTh,
                description = project.Description,
                image = project.Images?.Hero,
                address = new
                {
                    @type = "PostalAddress",
                    addressCountry = "TH",
                    addressRegion = "Bangkok",
                    addressLocality = project.Location?.District,
                    streetAddress = project.Details?.Location
                },
                numberOfRooms = project.HouseTypes?.Count ?? 0,
                amenityFeature = project.Facilities?.Select(f => new
                {
                    @type = "LocationFeatureSpecification",
                    name = f.Name,
                    description = f.Description
                }).ToList(),
                geo = new
                {
                    @type = "GeoCoordinates",
                    latitude = project.Location?.Latitude ?? (decimal)13.7563,
                    longitude = project.Location?.Longitude ?? (decimal)100.5018
                },
                priceRange = project.Details?.PriceRange
            };

            return JsonSerializer.Serialize(apartmentSchema, new JsonSerializerOptions { WriteIndented = false });
        }

        /// <summary>
        /// Gets JSON-LD House schema for residential properties
        /// </summary>
        private string GetHouseSchema(ProjectViewModel project)
        {
            if (project == null)
                return "{}";

            var features = new List<string>();
            if (project.HouseTypes != null && project.HouseTypes.Count > 0)
            {
                var firstType = project.HouseTypes.First();
                features.AddRange(firstType.Features ?? new List<string>());
            }

            var houseSchema = new
            {
                @context = "https://schema.org",
                @type = "House",
                name = project.NameTh,
                description = project.Description,
                image = project.Images?.Hero,
                address = new
                {
                    @type = "PostalAddress",
                    addressCountry = "TH",
                    addressRegion = "Bangkok",
                    addressLocality = project.Location?.District,
                    streetAddress = project.Details?.Location,
                    postalCode = project.Location?.PostalCode
                },
                floorSize = new
                {
                    @type = "QuantitativeValue",
                    value = project.Details?.UsableArea,
                    unitCode = "SQM"
                },
                numberOfRooms = project.HouseTypes?.FirstOrDefault()?.Bedrooms ?? 0,
                numberOfBathroomsTotal = project.HouseTypes?.FirstOrDefault()?.Bathrooms ?? 0,
                petsAllowed = false,
                features = features,
                amenityFeature = project.Facilities?.Select(f => new
                {
                    @type = "LocationFeatureSpecification",
                    name = f.Name
                }).ToList(),
                geo = new
                {
                    @type = "GeoCoordinates",
                    latitude = project.Location?.Latitude ?? (decimal)13.7563,
                    longitude = project.Location?.Longitude ?? (decimal)100.5018
                },
                offers = new
                {
                    @type = "AggregateOffer",
                    availability = "https://schema.org/InStock",
                    price = project.Details?.PriceRange,
                    priceCurrency = "THB"
                }
            };

            return JsonSerializer.Serialize(houseSchema, new JsonSerializerOptions { WriteIndented = false });
        }

        /// <summary>
        /// Gets comprehensive schema data for a project to pass to view
        /// </summary>
        public SeoMetadata GetPropertyTypeMetadata(ProjectViewModel project)
        {
            if (project == null)
                return GetDefaultMetadata();

            return GetProjectMetadata(project);
        }

        /// <summary>
        /// Gets all schema types available for a project
        /// </summary>
        public Dictionary<string, string> GetAllProjectSchemas(ProjectViewModel project)
        {
            if (project == null)
                return new Dictionary<string, string>();

            var schemas = new Dictionary<string, string>
            {
                { "organization", GetOrganizationSchema() },
                { "property", GetPropertySchema(project) },
                { "product", GetProductSchema(project) },
                { "house", GetHouseSchema(project) }
            };

            // Add ApartmentComplex schema only if it's an apartment project
            if (project.Type.ToString().ToLower().Contains("apartment"))
            {
                schemas.Add("apartmentcomplex", GetApartmentComplexSchema(project));
            }

            return schemas;
        }
    }
}
