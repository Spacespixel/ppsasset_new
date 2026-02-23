namespace PPSAsset.Models
{
    public class ProjectViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameTh { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Concept { get; set; } = string.Empty;
        public ProjectType Type { get; set; }
        public ProjectStatus Status { get; set; }
        public int SortOrder { get; set; } = 0;
        public ProjectDetails Details { get; set; } = new();
        public ProjectImages Images { get; set; } = new();
        public List<HouseType> HouseTypes { get; set; } = new();
        public List<Facility> Facilities { get; set; } = new();
        public LocationInfo Location { get; set; } = new();
        public ContactInfo Contact { get; set; } = new();
        public List<ConceptFeature> ConceptFeatures { get; set; } = new();
        public string? GtmId { get; set; }
    }

    public class ProjectDetails
    {
        public string Location { get; set; } = string.Empty;
        public string ProjectSize { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public int TotalUnits { get; set; }
        public string LandSize { get; set; } = string.Empty;
        public string UsableArea { get; set; } = string.Empty;
        public string PriceRange { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public DateTime? LaunchDate { get; set; }
        public DateTime? CompletionDate { get; set; }
    }

    public class ProjectImages
    {
        public string Hero { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string MasterPlan { get; set; } = string.Empty;
        public string FacilityMain { get; set; } = string.Empty;
        public List<string> Gallery { get; set; } = new();
        public List<string> Facilities { get; set; } = new();
        public string LocationMap { get; set; } = string.Empty;
    }

    public class HouseType
    {
        public int HouseTypeID { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Parking { get; set; }
        public string LandSize { get; set; } = string.Empty;
        public string UsableArea { get; set; } = string.Empty;
        public string PriceRange { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
        public List<FloorPlan> FloorPlans { get; set; } = new();
        public List<string> Features { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }

    public class FloorPlan
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FloorType Type { get; set; }
    }

    public class Facility
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public FacilityCategory Category { get; set; }
    }

    public class ConceptFeature
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class LocationInfo
    {
        public string Address { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<NearbyPlace> NearbyPlaces { get; set; } = new();
        public TransportationInfo Transportation { get; set; } = new();
    }

    public class NearbyPlace
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Distance { get; set; } = string.Empty;
        public string TravelTime { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class TransportationInfo
    {
        public List<string> BusRoutes { get; set; } = new();
        public List<string> NearbyStations { get; set; } = new();
        public List<string> Highways { get; set; } = new();
    }

    public class ContactInfo
    {
        public string SalesCenter { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LineId { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public OfficeHours Hours { get; set; } = new();
    }

    public class OfficeHours
    {
        public string Weekdays { get; set; } = string.Empty;
        public string Weekends { get; set; } = string.Empty;
        public string Holidays { get; set; } = string.Empty;
    }

    public enum ProjectType
    {
        SingleHouse,
        Townhouse,
        TwinHouse,
        Condominium
    }

    public enum ProjectStatus
    {
        NewProject,
        Available,
        SoldOut,
        ComingSoon,
        UnderConstruction,
        Completed
    }

    public enum FloorType
    {
        Facade,
        GroundFloor,
        SecondFloor,
        ThirdFloor,
        Roof,
        SitePlan
    }

    public enum FacilityCategory
    {
        Recreation,
        Security,
        Parking,
        Landscaping,
        Fitness,
        Community,
        Utilities
    }

    public class ProjectTheme
    {
        public string ThemeName { get; set; } = "default";
        public string PrimaryColor { get; set; } = "#365523";
        public string SecondaryColor { get; set; } = "#4A7030";
        public string LightBackground { get; set; } = "#F8FAF7";
        public string CssClass { get; set; } = "theme-default";
    }

    /// <summary>
    /// Request model for updating project status
    /// </summary>
    public class UpdateProjectStatusRequest
    {
        public string ProjectId { get; set; } = string.Empty;
        public ProjectStatus NewStatus { get; set; }
        public string? ChangedBy { get; set; }
        public string? Reason { get; set; }
    }

    /// <summary>
    /// GTM Configuration model for storing Google Tag Manager settings
    /// </summary>
    public class GtmConfiguration
    {
        public int Id { get; set; }
        public string ConfigKey { get; set; } = string.Empty;
        public string GtmId { get; set; } = string.Empty;
        public string? ProjectId { get; set; }
        public string Environment { get; set; } = "Production";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? Description { get; set; }
    }
}