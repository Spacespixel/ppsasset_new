using PPSAsset.Models;

namespace PPSAsset.Services
{
    public interface IProjectService
    {
        ProjectViewModel? GetProject(string id);
        List<ProjectViewModel> GetAllProjects();
        List<ProjectViewModel> GetProjectsByType(ProjectType type);
        List<ProjectViewModel> GetAvailableProjects();
        Task<bool> UpdateProjectStatusAsync(string projectId, ProjectStatus newStatus, string changedBy = "system", string reason = "Status update");
        Task<List<dynamic>> GetProjectStatusSummaryAsync();
        Task<string> GetProjectStatusHistoryAsync(string projectId);
    }

    public class StaticProjectService : IProjectService
    {
        private readonly Dictionary<string, ProjectViewModel> _projects;

        public StaticProjectService()
        {
            _projects = InitializeProjects();
        }

        public ProjectViewModel? GetProject(string id)
        {
            _projects.TryGetValue(id, out var project);
            return project;
        }

        public List<ProjectViewModel> GetAllProjects()
        {
            return _projects.Values.ToList();
        }

        public List<ProjectViewModel> GetProjectsByType(ProjectType type)
        {
            return _projects.Values.Where(p => p.Type == type).ToList();
        }

        public List<ProjectViewModel> GetAvailableProjects()
        {
            return _projects.Values.Where(p => p.Status == ProjectStatus.NewProject || p.Status == ProjectStatus.Available).ToList();
        }

        public async Task<bool> UpdateProjectStatusAsync(string projectId, ProjectStatus newStatus, string changedBy = "system", string reason = "Status update")
        {
            await Task.Delay(1);
            if (_projects.TryGetValue(projectId, out var project))
            {
                project.Status = newStatus;
                return true;
            }
            return false;
        }

        public async Task<List<dynamic>> GetProjectStatusSummaryAsync()
        {
            await Task.Delay(1);
            return _projects.Values.Select(p => new
            {
                ProjectID = p.Id,
                ProjectName = p.Name,
                ProjectNameEN = p.NameEn,
                ProjectType = p.Type.ToString(),
                ProjectStatus = p.Status.ToString(),
                ModifiedDate = DateTime.Now,
                TotalInquiries = 0,
                LastStatusChange = DateTime.Now
            }).Cast<dynamic>().ToList();
        }

        public async Task<string> GetProjectStatusHistoryAsync(string projectId)
        {
            await Task.Delay(1);
            return "[]";
        }

        private Dictionary<string, ProjectViewModel> InitializeProjects()
        {
            var projects = new Dictionary<string, ProjectViewModel>();

            // Rico Residence Ramintra-Hathairat (Current project)
            projects.Add("ricco-residence-hathairat", new ProjectViewModel
            {
                Id = "ricco-residence-hathairat",
                Name = "เดอะ ริกโค้ เรสซิเดนซ์ รามอินทรา-หทัยราษฎร์",
                NameTh = "เดอะ ริกโค้ เรสซิเดนซ์ รามอินทรา-หทัยราษฎร์",
                NameEn = "The Ricco Residence Ramintra-Hathairat",
                Subtitle = "Perfect Balance ความสมดุลที่ลงตัวในการดำเนินชีวิตและความสะดวกสบาย",
                Description = "บ้านเดี่ยว 2 ชั้น ออกแบบสำหรับครอบครัวสมัยใหม่ บนทำเลยุทธศาสตร์ที่เข้าถึงสิ่งอำนวยความสะดวกได้อย่างครบครัน",
                Concept = "\"Perfect Balance\" ความสมดุลที่ลงตัวในการดำเนินชีวิตและความสะดวกสบาย",
                Type = ProjectType.SingleHouse,
                Status = ProjectStatus.Available,
                SortOrder = 3,

                Details = new ProjectDetails
                {
                    Location = "ถนนหทัยราษฎร์ แขวงสามวาตะวันตก เขตคลองสามวา จังหวัดกรุงเทพฯ",
                    ProjectSize = "13-2-58.5 ไร่",
                    PropertyType = "บ้านเดี่ยว 2 ชั้น",
                    TotalUnits = 58,
                    LandSize = "50.3 ตารางวา",
                    UsableArea = "152 ตารางเมตร",
                    Developer = "พูลผลทรัพย์ จำกัด",
                    PriceRange = "8-15 ล้านบาท"
                },

                Images = new ProjectImages
                {
                    Hero = "Ricco-Residence-Ramintra-Hathairat.jpg",
                    Logo = "ricco-residence-hathatrat-logo.png",
                    FacilityMain = "Rico-Residence-Hathairath-Facility-1.png",
                    Gallery = new List<string>()
                },
 

                Facilities = new List<Facility>
                {
                    new Facility
                    {
                        Id = "swimming-pool",
                        Name = "คลับเฮาส์สระว่ายน้ำ",
                        Description = "สระว่ายน้ำขนาดใหญ่ พร้อมศูนย์ฟิตเนสครบครัน เพื่อสุขภาพที่แข็งแรงของทุกคนในครอบครัว",
                        Icon = "fa fa-tint",
                        Category = FacilityCategory.Recreation
                    },
                    new Facility
                    {
                        Id = "garden",
                        Name = "สวนส่วนกลาง",
                        Description = "มาพร้อมส่วนกลางครบครัน รองรับทุกกิจกรรมผ่อนคลาย",
                        Icon = "fa fa-tree",
                        Category = FacilityCategory.Landscaping
                    },
                    new Facility
                    {
                        Id = "security",
                        Name = "ระบบรักษาความปลอดภัย 24 ชั่วโมง",
                        Description = "ระบบรักษาความปลอดภัย 24 ชั่วโมง พร้อม Key Card Access สำหรับความสงบสุขของครอบครัว",
                        Icon = "fa fa-shield",
                        Category = FacilityCategory.Security
                    }
                },

                ConceptFeatures = new List<ConceptFeature>
                {
                    new ConceptFeature
                    {
                        Title = "Modern Natural Style",
                        Description = "การออกแบบที่เน้นความทันสมัยและฟังก์ชันการใช้งาน"
                    },
                    new ConceptFeature
                    {
                        Title = "ทำเลสะดวก",
                        Description = "ทำเลที่ล้อมรอบไปด้วยแหล่งอำนวยความสะดวก ลดเวลาเดินทาง เพิ่มเวลาการใช้ชีวิต"
                    },
                    new ConceptFeature
                    {
                        Title = "คุณภาพชีวิต",
                        Description = "พื้นที่สีเขียวในโครงการและรอบบ้าน เพื่อคุณภาพชีวิตที่ดีขึ้น"
                    }
                },

                
                Contact = new ContactInfo
                {
                    Phone = "02-xxx-xxxx",
                    Email = "sales@ppsasset.com",
                    LineId = "@ppsasset",
                    Facebook = "PPS Asset",
                    Hours = new OfficeHours
                    {
                        Weekdays = "09:00-18:00 น.",
                        Weekends = "09:00-19:00 น.",
                        Holidays = "09:00-17:00 น."
                    }
                }
            });

            // Rico Residence Prime Chatuchot
            projects.Add("ricco-residence-chatuchot", new ProjectViewModel
            {
                Id = "ricco-residence-chatuchot",
                Name = "เดอะ ริกโค้ เรสซิเดนซ์ รามอินทรา-จตุโชติ",
                NameTh = "เดอะ ริกโค้ เรสซิเดนซ์ รามอินทรา-จตุโชติ",
                NameEn = "The Ricco Residence Ramintra-Chatuchot",
                Subtitle = "Modern Living ชีวิตสมัยใหม่ในทำเลยุทธศาสตร์",
                Type = ProjectType.SingleHouse,
                Status = ProjectStatus.Available,
                SortOrder = 4,
                Concept = "Modern Living in Strategic Location",
                Description = "บ้านเดี่ยว 2 ชั้น ในทำเลยุทธศาสตร์ใกล้จตุจักร",
                
              
                ConceptFeatures = new List<ConceptFeature>
                {
                    new ConceptFeature { Title = "Strategic Location", Description = "ใกล้ตลาดจตุจักร และแหล่งช้อปปิ้ง" },
                    new ConceptFeature { Title = "Modern Design", Description = "ดีไซน์โมเดิร์นสไตล์ร่วมสมัย" },
                    new ConceptFeature { Title = "Convenient Access", Description = "เข้าออกได้หลายเส้นทาง" }
                },

            });

            // Rico Town Phahonyothin
            projects.Add("ricco-town-phahonyothin-saimai53", new ProjectViewModel
            {
                Id = "ricco-town-phahonyothin-saimai53",
                Name = "เดอะ ริกโค้ ทาวน์ พหลโยธิน-สายไหม 53",
                NameTh = "เดอะ ริกโค้ ทาวน์ พหลโยธิน-สายไหม 53",
                NameEn = "The Ricco Town Phahonyothin-Saimai 53",
                Subtitle = "Premium Townhome ทาวน์โฮมไพร์มที่บ้านคุณสาเหตุ",
                Type = ProjectType.Townhouse,
                Status = ProjectStatus.Available,
                SortOrder = 5,
                Concept = "Premium Townhome Living",
                Description = "ทาวน์โฮม 3 ชั้น ในทำเลพหลโยธิน",
                Details = new ProjectDetails
                {
                    Location = "ถนนพหลโยธิน แขวงสายไหม เขตสายไหม จังหวัดกรุงเทพฯ",
                    PropertyType = "ทาวน์โฮม 3 ชั้น",
                    TotalUnits = 68,
                    LandSize = "20.0 ตารางวา",
                    UsableArea = "140 ตารางเมตร"
                },
               
                ConceptFeatures = new List<ConceptFeature>
                {
                    new ConceptFeature { Title = "3-Story Design", Description = "ทาวน์โฮม 3 ชั้น เพิ่มพื้นที่ใช้สอย" },
                    new ConceptFeature { Title = "Phahonyothin Access", Description = "ติดถนนพหลโยธิน เดินทางสะดวก" },
                    new ConceptFeature { Title = "Complete Facilities", Description = "สิ่งอำนวยความสะดวกครบครัน" }
                }
            });

            // Rico Residence Prime Wong Waen-Hathairat
            projects.Add("ricco-residence-prime-hathairat", new ProjectViewModel
            {
                Id = "ricco-residence-prime-hathairat",
                Name = "เดอะ ริกโค้ เรสซิเดนซ์ ไพร์ม วงแหวนฯ-หทัยราษฎร์",
                NameTh = "เดอะ ริกโค้ เรสซิเดนซ์ ไพร์ม วงแหวนฯ-หทัยราษฎร์",
                NameEn = "The Ricco Residence Prime Wong Waen-Hathairat",
                Type = ProjectType.SingleHouse,
                Status = ProjectStatus.NewProject,
                SortOrder = 1,
                Concept = "Modern Classic Living",
                Description = "บ้านเดี่ยว 2 ชั้น สไตล์ Modern Classic บนทำเลวงแหวน",
                Details = new ProjectDetails
                {
                    Location = "ถนนไทยรามัณ แขวงสามวาตะวันตก เขตคลองสามวา จังหวัดกรุงเทพฯ",
                    ProjectSize = "24-2-38.4 ไร่",
                    PropertyType = "บ้านเดี่ยว 2 ชั้น",
                    TotalUnits = 103,
                    LandSize = "50.90 ตารางวา",
                    UsableArea = "162-218 ตารางเมตร",
                    Developer = "พูลผลทรัพย์ จำกัด",
                    PriceRange = "8-18 ล้านบาท"
                },
              
               
                Facilities = new List<Facility>
                {
                    new Facility
                    {
                        Id = "clubhouse",
                        Name = "คลับเฮาส์",
                        Description = "คลับเฮาส์พร้อมสระว่ายน้ำและศูนย์ฟิตเนส",
                        Icon = "fa fa-building",
                        Category = FacilityCategory.Recreation
                    },
                    new Facility
                    {
                        Id = "garden",
                        Name = "สวนสไตล์อังกฤษ",
                        Description = "สวนสวยสไตล์อังกฤษโมเดิร์น",
                        Icon = "fa fa-tree",
                        Category = FacilityCategory.Landscaping
                    },
                    new Facility
                    {
                        Id = "security",
                        Name = "ระบบรักษาความปลอดภัย 24 ชั่วโมง",
                        Description = "ระบบรักษาความปลอดภัย 24 ชั่วโมง พร้อมระบบจดจำป้ายทะเบียน",
                        Icon = "fa fa-shield",
                        Category = FacilityCategory.Security
                    }
                },
                ConceptFeatures = new List<ConceptFeature>
                {
                    new ConceptFeature { Title = "Modern Classic Design", Description = "การผสมผสานสถาปัตยกรรมยุโรปกับความทันสมัย" },
                    new ConceptFeature { Title = "Strategic Location", Description = "ทำเลยุทธศาสตร์ใกล้วงแหวนรอบนอก" },
                    new ConceptFeature { Title = "Premium Facilities", Description = "สิ่งอำนวยความสะดวกระดับพรีเมียม" }
                }
            });

            // Rico Residence Prime Wong Waen-Chatuchot
            projects.Add("ricco-residence-prime-chatuchot", new ProjectViewModel
            {
                Id = "ricco-residence-prime-chatuchot",
                Name = "เดอะ ริกโค้ เรสซิเดนซ์ ไพร์ม วงแหวนฯ-จตุโชติ",
                NameTh = "เดอะ ริกโค้ เรสซิเดนซ์ ไพร์ม วงแหวนฯ-จตุโชติ",
                NameEn = "The Ricco Residence Prime Wong Waen-Chatuchot",
                Type = ProjectType.SingleHouse,
                Status = ProjectStatus.NewProject,
                SortOrder = 2,
                Concept = "Modern Classic Elegance",
                Description = "บ้านเดี่ยว 2 ชั้น สไตล์ Modern Classic ใกล้จตุจักร",
                Details = new ProjectDetails
                {
                    Location = "ใกล้วงแหวนรอบนอก-จตุโชติ กรุงเทพมหานคร",
                    ProjectSize = "37-1-92.3 ไร่",
                    PropertyType = "บ้านเดี่ยว 2 ชั้น",
                    TotalUnits = 156,
                    LandSize = "50.90-55.80 ตารางวา",
                    UsableArea = "162-218 ตารางเมตร",
                    Developer = "พูลผลทรัพย์ จำกัด",
                    PriceRange = "8-20 ล้านบาท"
                },
               
             
                Facilities = new List<Facility>
                {
                    new Facility
                    {
                        Id = "clubhouse",
                        Name = "คลับเฮาส์พร้อมสระว่ายน้ำ",
                        Description = "คลับเฮาส์พร้อมสระว่ายน้ำและศูนย์ฟิตเนส",
                        Icon = "fa fa-building",
                        Category = FacilityCategory.Recreation
                    },
                    new Facility
                    {
                        Id = "garden",
                        Name = "สวนสไตล์อังกฤษโมเดิร์น",
                        Description = "สวนสวยสไตล์อังกฤษโมเดิร์น",
                        Icon = "fa fa-tree",
                        Category = FacilityCategory.Landscaping
                    },
                    new Facility
                    {
                        Id = "security",
                        Name = "ระบบรักษาความปลอดภัย 24 ชั่วโมง",
                        Description = "ระบบรักษาความปลอดภัย 24 ชั่วโมง พร้อมระบบจดจำป้ายทะเบียน",
                        Icon = "fa fa-shield",
                        Category = FacilityCategory.Security
                    }
                },
                ConceptFeatures = new List<ConceptFeature>
                {
                    new ConceptFeature { Title = "European Influence", Description = "สถาปัตยกรรมได้แรงบันดาลใจจากยุโรป" },
                    new ConceptFeature { Title = "Chatuchot Proximity", Description = "ใกล้ตลาดจตุจักร แหล่งช้อปปิ้งชื่อดัง" },
                    new ConceptFeature { Title = "Modern Classic Style", Description = "ดีไซน์ผสมผสานคลาสสิคและโมเดิร์น" }
                }
            });

            // Rico Town Wong Waen-Lamlukka
            projects.Add("ricco-town-wongwaen-lamlukka", new ProjectViewModel
            {
                Id = "ricco-town-wongwaen-lamlukka",
                Name = "เดอะ ริกโค้ ทาวน์ วงแหวนฯ-ลำลูกกา",
                NameTh = "เดอะ ริกโค้ ทาวน์ วงแหวนฯ-ลำลูกกา",
                NameEn = "The Ricco Town Wong Waen-Lamlukka",
                Subtitle = "Modern Contemporary ทาวน์โฮมและบ้านแฝดสมัยใหม่",
                Type = ProjectType.Townhouse,
                Status = ProjectStatus.NewProject,
                SortOrder = 6,
                Concept = "Modern Contemporary Living",
                Description = "ทาวน์โฮมและบ้านแฝด 2 ชั้น ดีไซน์โมเดิร์น คอนเทมโพรารี่",
                Details = new ProjectDetails
                {
                    Location = "ใกล้ถนนสายไหวประชารัฐ ลาดสวาย ลำลูกกา ปทุมธานี",
                    ProjectSize = "43 ไร่",
                    PropertyType = "ทาวน์โฮม และบ้านแฝด 2 ชั้น",
                    TotalUnits = 331,
                    LandSize = "19.2-35.8 ตารางวา",
                    UsableArea = "119-125 ตารางเมตร",
                    Developer = "พูลผลแอสเซท จำกัด",
                    PriceRange = "3-6 ล้านบาท"
                },
               
             
                Facilities = new List<Facility>
                {
                    new Facility
                    {
                        Id = "clubhouse",
                        Name = "คลับเฮาส์",
                        Description = "คลับเฮาส์พร้อมสระว่ายน้ำและศูนย์ฟิตเนส",
                        Icon = "fa fa-building",
                        Category = FacilityCategory.Recreation
                    },
                    new Facility
                    {
                        Id = "garden",
                        Name = "สวนส่วนกลางขนาดใหญ่",
                        Description = "สวนส่วนกลางขนาดใหญ่เพื่อการพักผ่อน",
                        Icon = "fa fa-tree",
                        Category = FacilityCategory.Landscaping
                    },
                    new Facility
                    {
                        Id = "security",
                        Name = "ระบบรักษาความปลอดภัย 24 ชั่วโมง",
                        Description = "ระบบรักษาความปลอดภัย 24 ชั่วโมง พร้อมระบบ Key Card",
                        Icon = "fa fa-shield",
                        Category = FacilityCategory.Security
                    }
                },
                ConceptFeatures = new List<ConceptFeature>
                {
                    new ConceptFeature { Title = "Modern Contemporary", Description = "ดีไซน์โมเดิร์น คอนเทมโพรารี่ เน้นความสบายและแสงธรรมชาติ" },
                    new ConceptFeature { Title = "Diverse Housing Options", Description = "ทั้งทาวน์โฮมและบ้านแฝด ตอบสนองทุกความต้องการ" },
                    new ConceptFeature { Title = "Strategic Location", Description = "ทำเลยุทธศาสตร์ใกล้ลำลูกกา เดินทางสะดวก" }
                }
            });

            return projects;
        }
    }
}