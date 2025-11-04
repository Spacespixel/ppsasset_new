# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview
This is a complete website redesign project for PPS Asset, a Thai real estate development company. The project focuses on creating a modern, mobile-first website showcasing residential properties in Bangkok and surrounding areas.

## Technology Stack
- **Backend**: ASP.NET Core 8.0 (C#) with MVC pattern
- **Frontend**: Razor Views with Bootstrap, custom CSS/JS
- **Database**: MySQL with Dapper ORM
- **Target Framework**: .NET 8.0
- **Architecture**: Hybrid data service architecture with database + static fallback

## Development Commands

### Build and Run
```bash
# Build the project
dotnet build

# Run in development mode
dotnet run

# Run with specific URL
dotnet run --urls="http://localhost:5000"

# Run with specific environment
dotnet run --environment Development
```

### Project Structure
```
├── Controllers/          # MVC Controllers
│   └── HomeController.cs # Main controller with Index, About, Contact, Project actions
├── Models/              # Data models and ViewModels
│   ├── ErrorViewModel.cs # Error handling model
│   └── ProjectModels.cs # Comprehensive project data models
├── Services/            # Business logic layer with dependency injection
│   ├── IProjectService.cs      # Service interface
│   ├── StaticProjectService.cs # Static data fallback service
│   ├── DatabaseProjectService.cs # MySQL database service
│   └── HybridProjectService.cs   # Primary service combining both sources
├── Data/                # Database operations and migrations
│   └── DatabaseMigration.cs # Data migration utilities
├── Views/               # Razor view templates
│   ├── Home/           # Home controller views
│   └── Shared/         # Shared view components and layouts
├── wwwroot/            # Static files (CSS, JS, images, fonts)
└── wireframe/          # Design wireframes and templates
```

Project Sub Menu
โครงการ
├── บ้านเดี่ยว
    ├──เดอะริคโค้ เรสซิเดนซ์ ไพร์ม วงแหวนฯ-หทัยราษฎร์
    ├──เดอะริคโค้ เรสซิเดนซ์ ไพร์ม วงแหวนฯ-จตุโชติ
    ├──เดอะริคโค้ เรสซิเดนซ์ รามอินทรา-จตุโชติ
    ├──เดอะริคโค้ เรสซิเดนซ์ รามอินทรา-หทัยราษฎร์
    ├──เอริกได้ เรสซ์เดนซ์ วงแหวนฯ-หกัยราษฎร์  (SOLD OUT)
    ├──เดอะริคโค้ เรสซิเดนซ์ วงแหวนฯ-จตุโชติ (SOLD OUT)
├── ทาวน์โฮม
    ├──เดอะริคโค้ ทาวน์ พหลโยธิน-สายไหม 53
    ├──เดอะริคโค้ ทาวน์ วงแหวนฯ-ลำลูกกา
    ├──เดอะริคโค้ ทาวน์ พหลโยริน-วัชรพล(SOLD OUT)
    ├──เดอะริคโค้ ทาวน์ วงแหวนฯ-หทัยราษฎร์ (SOLD OUT)
    ├──เดอะริคโค้ ทาวน์ รามอินทรา - วัชรพล (SOLD OUT)
    ├──เดอะริคโค้ ทาวน์ วัชรพล (SOLD OUT)
    ├──เดอะริคโค้ ทาวน์ พหลโยธิน - สายไหม (SOLD OUT)
├── บ้านแฝด
    ├──เดอะริคโค้ ทาวน์ วงแหวนฯ-ลำลูกกา


## Architecture Notes

### Hybrid Data Service Pattern
The application uses a **HybridProjectService** that implements a resilient data architecture:
- **Primary**: DatabaseProjectService (MySQL with Dapper ORM)
- **Fallback**: StaticProjectService (hardcoded data for zero downtime)
- **Benefits**: Ensures application availability during database issues or maintenance

### Dependency Injection Services
Registered in `Program.cs`:
- `DatabaseProjectService` - MySQL database operations
- `StaticProjectService` - Static fallback data
- `HybridProjectService (implements `IProjectService`) - Primary service with auto-fallback
- `DatabaseMigration` - Database schema and data migration utilities

### Key Features Implemented
- **Multi-page structure**: Index, About, Contact, Project pages
- **Advanced URL routing**: Supports both legacy (`/Project/{id}`) and PPS-style (`/{projectType}/{projectName}/{location}`) URLs
- **Database migration endpoint**: `/MigrateDatabase (development only)
- **Service monitoring**: `/ServiceStatus` endpoint for health checks
- **Static asset management**: Organized CSS, JS, images, and fonts

### Design Resources
- **Wireframes**: Located in `/wireframe/` directory
- **Templates**: HTML templates in `/wireframe/template/`
- **Project requirements**: Detailed specifications in existing `claude.md`

## Development Guidelines

### Adding New Pages
1. Create action method in appropriate controller (usually HomeController)
2. Add corresponding view in `Views/[Controller]/` directory
3. Update navigation/routing as needed

### Static Assets
- Place CSS files in `wwwroot/css/`
- Place JavaScript in `wwwroot/js/`
- Place images in `wwwroot/images/`
- Reference assets using `~/` prefix in views

### Theme and Color System

#### Universal Brand Colors
- **Brand Identity Green**: #365523 - Primary navigation, brand elements
- **Footer Charcoal**: #61676d - Footer background, consistent across all projects
- **Pure White**: #FFFFFF - Main backgrounds, card backgrounds

#### Dynamic Project-Specific Color Themes
Each project uses vibrant, distinctive colors to match marketing materials:

**Ricco Residence Hathairat:**
- URL: https://www.ppsasset.com/singlehouse/thericcoresidence/ramindrahathairat/index#UserReg 
- Primary: #C2185B (Vibrant Magenta)
- Secondary: #E91E63 (Lighter Magenta)
- Light Background: #FCE4EC (Ultra-light Pink)

**Ricco Residence Chatuchot:**
- URL: https://www.ppsasset.com/singlehouse/thericcoresidence/ramindrachatuchot/index#UserReg
- Primary: #B71C1C (Deep Red)
- Secondary: #D32F2F (Lighter Red)
- Light Background: #FFEBEE (Ultra-light Red)

**Ricco Residence Prime Chatuchot:**
- URL: https://www.ppsasset.com/singlehouse/thericcoresidenceprime/wongwaenchatuchot/index#UserReg
- Primary: #334199  
- Secondary: #07354b  
- Light Background: #FFEBEE (Ultra-light Red)

**Ricco Residence Prime Hathairat:**
- URL: https://www.ppsasset.com/singlehouse/thericcoresidenceprime/wongwaenhathairat/index#UserReg
- Primary: #580709  
- Secondary: #b9834c
- Light Background: #E3F2FD (Ultra-light Blue)

**Ricco Town Saimai 53:**
- URL: https://www.ppsasset.com/townhome/thericcotown/phahonyothin_saimai53/index#Register
- Primary: #8D1537 (Maroon)
- Secondary: #AD1457 (Lighter Maroon)
- Light Background: #F8BBD9 (Ultra-light Pink)

**Wongwaen Lamlukka**
- URL: https://www.ppsasset.com/townhome/thericcotown/wongwaen_lumlukka/index#Register
- Primary: #8D1537 (Maroon)
- Secondary: #AD1457 (Lighter Maroon)
- Light Background: #F8BBD9 (Ultra-light Pink)

#### Implementation Structure
- **CSS Variables**: Dynamic color system using CSS custom properties
- **Project Themes**: Colors injected per project page via CSS classes or inline styles
- **File Location**: `/wwwroot/css/style-custom.css` - Main stylesheet with theme variables
- **Controller Integration**: Project colors passed from `HomeController.cs` to views
- **Usage**: Navigation tabs, buttons, section backgrounds use project-specific colors

#### Color Psychology
- **Vibrant Colors**: Each project has distinct brand identity matching marketing materials
- **Brand Consistency**: Universal green maintained for main navigation/header
- **User Experience**: Visual cues help users differentiate between projects
- **Marketing Alignment**: Colors match promotional banners and advertising materials

### Project-Specific Requirements
- **Target Audience**: Thai homebuyers and real estate investors
- **Language Support**: Thai/English bilingual requirements
- **Mobile-First**: Responsive design is critical
- **Performance**: Target <3 seconds load time
- **SEO**: Focus on property search terms and local SEO

### Data Models Architecture
The application uses comprehensive models in `Models/ProjectModels.cs`:
- **ProjectViewModel**: Complete project data with nested models
- **HouseType**: Property types with floor plans and pricing
- **Facility**: Property amenities and features
- **LocationInfo**: Geographic data and nearby places
- **ContactInfo**: Sales and contact information

### URL Routing Patterns
1. **Legacy format**: `/Project/{id} (e.g., `/Project/ricco-residence-hathairat`)
2. **PPS Asset format**: `/{projectType}/{projectName}/{location}`
   - Example: `/singlehouse/ricco-residence-ramintra/hathairat`
   - Mapped to project IDs in `HomeController.ConvertPpsUrlToProjectId()` method

### Database Integration
- **MySQL**: Primary data storage with Dapper ORM
- **Migration endpoint**: `/MigrateDatabase (development environment only)
- **Health monitoring**: `/ServiceStatus` for service health checks
- **Automatic fallback**: HybridService ensures zero downtime during database issues

### Development Workflow
1. Review wireframes in `/wireframe/` before implementing new features
2. Follow existing MVC patterns and naming conventions
3. Ensure mobile responsiveness for all new components
4. Test performance impact of image-heavy content
5. Consider bilingual content structure in view design

## SEO Strategy & Competitive Analysis (Updated 2024-11-04)

### SEO Goals
- **Primary Target**: Top 3 rankings for "[Project Name] + Bangkok" keywords
- **Organic Traffic**: 150% increase within 6 months
- **Core Web Vitals**: LCP <2.5s, FID <100ms, CLS <0.1
- **Mobile Performance**: PageSpeed score >85/100
- **Local Focus**: Bangkok suburban districts (หทัยราษฎร์, จตุโชติ, สายไหม, ลำลูกกา)

### Target Keywords
- **Primary**: "บ้านเดี่ยว + [Location]", "ทาวน์โฮม + [District]"
- **Long-tail**: "บ้านราคาดี หทัยราษฎร์", "โครงการใหม่ วงแหวน"
- **Decision Intent**: "ซื้อบ้านใหม่ กรุงเทพ 2024"
- **Brand Terms**: "เดอะริคโค้ เรสซิเดนซ์" + variants

### Competitor Analysis

#### Direct Competitors
1. **Sansiri** (www.sansiri.com)
   - **Strengths**: 40-year established brand, comprehensive content, multiple languages
   - **Weaknesses**: Generic content, limited schema markup, slow performance
   - **Opportunity**: Developer-direct authentic content vs. corporate generic messaging

2. **Craftwork** (craftwork.co.th)
   - **Strengths**: Local Pattaya focus, Next.js framework, clear project structure
   - **Weaknesses**: Limited schema markup, performance issues, narrow geographic focus
   - **Opportunity**: Bangkok market focus vs. their Pattaya specialization

3. **Peace & Living** (peaceandliving.co.th)
   - **Strengths**: Good organization schema implementation, clear site structure
   - **Weaknesses**: Limited scale and market reach
   - **Opportunity**: Broader project portfolio and better content strategy

#### Indirect Competitors
4. **AP Thai** (apthai.com/th)
   - **Strengths**: Major property listing portal, wide market coverage
   - **Weaknesses**: Basic technical implementation, generic listing approach
   - **Opportunity**: Specialized developer focus vs. generic property portal

### SEO Implementation Strategy

#### Phase 1 (Months 1-2): Technical Foundation
- **Core Web Vitals optimization**: Image compression, lazy loading, CSS/JS bundling
- **Schema markup**: Real estate structured data for all project pages
- **Meta optimization**: Project-specific titles, descriptions, and keywords
- **Mobile-first**: Responsive design improvements and performance optimization

#### Phase 2 (Months 3-4): Content Authority
- **Location pages**: Bangkok district-specific landing pages
- **Property guides**: Buyer decision trees and area comparisons
- **FAQ implementation**: Schema-enhanced answers for common queries
- **Internal linking**: Strategic link architecture for authority distribution

#### Phase 3 (Months 5-6): Market Domination
- **Voice search optimization**: Natural language query targeting
- **Video SEO**: Virtual tour and project video optimization
- **Featured snippets**: Content optimized for position zero
- **Local citations**: Bangkok real estate directory listings

### Technical SEO Advantages
- **ASP.NET Core Performance**: Superior to competitors' average 3.5-4.2s load times
- **Hybrid Architecture**: Database + static fallback ensures 100% uptime
- **Mobile-First Design**: Critical for Thai market where mobile usage dominates
- **Schema Implementation**: Comprehensive real estate structured data vs. competitors' basic markup

### Success Metrics & Tracking
- **Google Search Console**: Primary keyword tracking and performance monitoring
- **Core Web Vitals**: LCP, FID, CLS measurements via PageSpeed Insights
- **Local Rankings**: Bangkok district-specific position tracking
- **Organic CTR**: Target 8-12% for top 3 positions
- **Conversion Tracking**: Lead generation from organic traffic

### Competitive Advantage
- **Developer Direct**: Authentic content vs. generic property portals
- **Local Expertise**: Bangkok suburban focus vs. broad market coverage
- **Technical Superior**: Modern ASP.NET stack vs. competitors' legacy systems
- **Content Strategy**: Project-specific optimization vs. generic approaches
- **Performance Edge**: <2.5s load times vs. competitors' 3.5-4.2s average