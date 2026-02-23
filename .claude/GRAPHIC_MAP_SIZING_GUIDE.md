# Graphic Map Sizing Guide - PPS Asset

## Current Status

Your graphic maps are detailed, complex location maps showing:
- ✅ Project boundaries and layout
- ✅ Nearby amenities (schools, hospitals, shops)
- ✅ Transportation routes (BTS, roads)
- ✅ Community information
- ✅ Multiple colored annotations

**Current Files:**
- `Residence-Ramintra-Hathairat-map.png` - 657KB
- `Residence-Ramintra-Chatuchot-map.png` - 479KB

---

## Optimal Graphic Map Sizes

### For Display on Website

Your maps need to be optimized for web display while maintaining readability of text labels.

#### Desktop Display (1920px width)
```
Recommended Size:   1920px × 1080px (16:9 aspect ratio)
File Format:        WebP or PNG
File Size:          80-120KB (compressed)
DPI:                96 DPI (web standard)
Quality:            High (95% quality)
```

#### Tablet Display (768px width)
```
Recommended Size:   1024px × 576px (16:9 aspect ratio)
File Format:        WebP or PNG
File Size:          40-60KB (compressed)
DPI:                96 DPI
Quality:            High (90% quality)
```

#### Mobile Display (375px width)
```
Recommended Size:   768px × 432px (16:9 aspect ratio)
File Format:        WebP or PNG
File Size:          25-40KB (compressed)
DPI:                96 DPI
Quality:            Medium-High (85% quality)
```

### Summary Table

| Device | Width | Height | Aspect | Format | Size |
|--------|-------|--------|--------|--------|------|
| **Desktop** | 1920px | 1080px | 16:9 | WebP | 80-120KB |
| **Laptop** | 1366px | 768px | 16:9 | WebP | 60-90KB |
| **Tablet** | 1024px | 576px | 16:9 | WebP | 40-60KB |
| **Mobile** | 768px | 432px | 16:9 | WebP | 25-40KB |
| **Phone** | 480px | 270px | 16:9 | WebP | 15-25KB |

---

## Current Map Characteristics

### Hathairat Map (657KB)
**Analysis:**
- Dimensions: Appears to be ~1600px × 1200px (4:3 ratio)
- File size: 657KB (quite large for web)
- Content: Very detailed with many text labels
- Issue: Too large, text may be unreadable on small screens

**Recommendations:**
- Reduce to 1920×1080px or smaller
- Compress with WebP format
- Target 80-100KB file size
- Use responsive image technique

### Chatuchot Map (479KB)
**Analysis:**
- Dimensions: Appears to be ~1400px × 1000px (4:3 ratio)
- File size: 479KB (large for web)
- Content: Detailed location and amenity information
- Issue: Oversize, slow loading

**Recommendations:**
- Reduce to 1024×768px or 1366×768px
- Convert to WebP format
- Target 60-80KB file size
- Consider responsive image strategy

---

## Recommended Image Optimization Strategy

### Step 1: Create Responsive Image Set

For optimal performance, create multiple sizes:

```
Graphic Maps Directory:
├── graphic-map-2560.webp   (2560×1440 - retina desktop)
├── graphic-map-1920.webp   (1920×1080 - desktop)
├── graphic-map-1366.webp   (1366×768 - laptop)
├── graphic-map-1024.webp   (1024×576 - tablet)
├── graphic-map-768.webp    (768×432 - mobile)
└── graphic-map-480.webp    (480×270 - small phone)
```

### Step 2: Use Responsive Image Syntax

Update HTML to use `srcset`:

```html
<img
  src="~/images/projects/{id}/graphic-map-1366.webp"
  srcset="
    ~/images/projects/{id}/graphic-map-480.webp 480w,
    ~/images/projects/{id}/graphic-map-768.webp 768w,
    ~/images/projects/{id}/graphic-map-1024.webp 1024w,
    ~/images/projects/{id}/graphic-map-1366.webp 1366w,
    ~/images/projects/{id}/graphic-map-1920.webp 1920w,
    ~/images/projects/{id}/graphic-map-2560.webp 2560w
  "
  sizes="
    (max-width: 480px) 100vw,
    (max-width: 768px) 100vw,
    (max-width: 1024px) 100vw,
    (max-width: 1366px) 100vw,
    1920px
  "
  alt="Location Map - @Model.NameTh"
  loading="lazy"
/>
```

### Step 3: Compression Guidelines

**For PNG → WebP Conversion:**
```bash
# Using ImageMagick or similar tool
convert input-map.png -quality 85 output-map.webp

# File Size Targets:
2560×1440 (2x retina) → 200-250KB
1920×1080 (desktop)   → 80-120KB
1366×768 (laptop)     → 60-90KB
1024×576 (tablet)     → 40-60KB
768×432 (mobile)      → 25-40KB
480×270 (small phone) → 15-25KB
```

---

## Aspect Ratio Considerations

### Current Maps
- **Current Aspect Ratio:** 4:3 (1600×1200 or 1400×1000)
- **Issue:** Doesn't match 16:9 responsive design

### Recommended Approach

**Option 1: Maintain 4:3 Ratio (Preserve Detail)**
```
If keeping 4:3 ratio:
- Desktop: 1920×1440 (wider to show full detail)
- Tablet:  1024×768
- Mobile:  768×576

Pros: Shows all map details
Cons: Taller container, less balanced with 16:9 design
```

**Option 2: Convert to 16:9 Ratio (Recommended)**
```
Convert to 16:9 for consistency:
- Desktop: 1920×1080 (crop/expand to 16:9)
- Tablet:  1024×576
- Mobile:  768×432

Pros: Consistent with design system, balanced layout
Cons: May need to crop some edges

Implementation:
1. Add white/colored padding to sides
2. OR slightly crop top/bottom
3. Ensure key location info visible
```

---

## Detailed Sizing Specifications

### Desktop (1920px width)
```
Display Size:   1920px × 1080px (16:9)
Actual Render:  450px max-height (with aspect ratio)
Calculate:      450px ÷ 9 × 16 = 800px width
File:           WebP, 85-100KB
Quality:        95% (high detail)
Purpose:        Full context view with all details
```

### Tablet (768px width)
```
Display Size:   1024px × 576px (16:9)
Actual Render:  350px max-height
Calculate:      350px ÷ 9 × 16 = 622px width
File:           WebP, 40-50KB
Quality:        90% (good detail)
Purpose:        Balanced display, readable text
```

### Mobile (375px width)
```
Display Size:   768px × 432px (16:9)
Actual Render:  250px max-height
Calculate:      250px ÷ 9 × 16 = 444px width
File:           WebP, 25-30KB
Quality:        85% (acceptable detail)
Purpose:        Quick reference, key locations visible
```

### Small Phone (320px width)
```
Display Size:   480px × 270px (16:9)
Actual Render:  200px max-height
Calculate:      200px ÷ 9 × 16 = 356px width
File:           WebP, 15-20KB
Quality:        80% (minimal detail)
Purpose:        General location reference
```

---

## Image Optimization Checklist

### For Current Maps

- [ ] **Reduce file size**
  - Current: 657KB → Target: 100KB (desktop version)
  - Current: 479KB → Target: 80KB (desktop version)

- [ ] **Convert format**
  - From: PNG
  - To: WebP (30-40% smaller)
  - Fallback: JPEG as alternative

- [ ] **Create responsive sizes**
  - [ ] 2560×1440 (retina desktop)
  - [ ] 1920×1080 (desktop)
  - [ ] 1366×768 (laptop)
  - [ ] 1024×576 (tablet)
  - [ ] 768×432 (mobile)
  - [ ] 480×270 (small phone)

- [ ] **Update HTML**
  - Use `srcset` attribute
  - Use `sizes` attribute
  - Add `loading="lazy"`
  - Provide `alt` text

- [ ] **Test display quality**
  - [ ] Desktop: All details readable
  - [ ] Tablet: Text legible
  - [ ] Mobile: Key locations visible
  - [ ] Load time < 2 seconds

---

## Tools for Image Optimization

### Online Tools (Free)
- **TinyPNG/TinyJPG** - Reduce PNG/JPEG size by 60-80%
- **Squoosh** (Google) - WebP conversion and optimization
- **ImageOptim** - Mac app for batch optimization
- **FileOptimizer** - Windows batch optimization

### Command Line Tools
```bash
# Using ImageMagick:
convert input.png -resize 1920x1080 -quality 85 output.webp

# Using ffmpeg:
ffmpeg -i input.png -vf scale=1920:-1 output.webp

# Using cwebp (WebP dedicated):
cwebp -q 85 input.png -o output.webp
```

---

## Current Map Analysis

### Hathairat Map Issues
1. **File Size:** 657KB (Too large)
   - Should be 80-120KB for desktop version
   - 40-60KB for tablet version
   - 25-40KB for mobile version

2. **Dimensions:** Appears 4:3 aspect ratio (not 16:9)
   - Conflict with responsive design
   - May display with padding or cropping

3. **Format:** PNG (larger than WebP)
   - Converting to WebP saves 30-40%
   - Would reduce 657KB to ~400KB

4. **Detail Level:** Very high
   - Good for desktop, may not scale well to small screens
   - Text labels difficult to read on mobile

### Chatuchot Map Issues
1. **File Size:** 479KB (Still quite large)
   - Should be 60-90KB for desktop
   - 40-60KB for tablet
   - 25-40KB for mobile

2. **Format:** PNG (larger than WebP)
   - WebP would save 30-40%
   - Would reduce to ~300KB

3. **Clarity:** Good presentation
   - Clear labeling
   - Professional design

---

## Implementation Recommendation

### Immediate Actions

**1. Optimize Existing Maps**
```
Current: Hathairat 657KB, Chatuchot 479KB
Target: 100KB desktop + 50KB tablet + 30KB mobile each

Action: Use TinyPNG or Squoosh to compress
Estimated Savings: 50-60% file size reduction
```

**2. Convert to WebP**
```
Format: PNG → WebP
Savings: 30-40% additional reduction
Tool: ffmpeg or cwebp
Result: Smaller files, better quality
```

**3. Create Responsive Set**
```
Create 3-6 versions per map
Tool: ImageMagick batch processing
Time: 30 minutes per map
Benefit: Optimal display on all devices
```

### Long-term Strategy

**1. Responsive Image Implementation**
- Implement `srcset` in Project.cshtml
- Add lazy loading
- Use WebP with PNG fallback

**2. Performance Monitoring**
- Track map load times
- Monitor image cache hit rates
- Measure user engagement with maps

**3. Future Maps**
- Ensure all new maps created at appropriate sizes
- Use WebP format by default
- Create responsive image sets upfront

---

## Size Recommendations Summary

### Minimum Sizes Needed
```
Per Map:
- Desktop (1920×1080):  80-120KB WebP
- Tablet (1024×576):    40-60KB WebP
- Mobile (768×432):     25-40KB WebP

Total Per Project: 3 maps × 3 sizes = 9 files
Total Storage: ~500KB per project (vs. ~1.1MB current)
```

### Quality vs. Performance
```
Desktop (95% quality):     Heavy detail, full labels
Tablet (90% quality):      Good detail, readable text
Mobile (85% quality):      Basic detail, key info only
```

---

## Testing the Optimized Maps

### Desktop (1920px)
- ✅ All text labels legible
- ✅ Amenity icons visible
- ✅ Road/street names clear
- ✅ Project boundaries defined
- ✅ Load time < 2s

### Tablet (768px)
- ✅ Important text readable
- ✅ Main amenities visible
- ✅ Clear project location
- ✅ No excessive cropping
- ✅ Load time < 1.5s

### Mobile (375px)
- ✅ Basic location info visible
- ✅ Nearby amenities identifiable
- ✅ Map not too cramped
- ✅ Text minimally readable
- ✅ Load time < 1s

---

## Next Steps

1. **Measure Current Performance**
   - Check actual image dimensions
   - Monitor load times
   - Test on real devices

2. **Create Optimization Plan**
   - List all maps needing optimization
   - Prioritize by file size
   - Set target file sizes

3. **Implement Optimization**
   - Convert to WebP
   - Create responsive sizes
   - Test on all devices

4. **Deploy**
   - Update HTML with srcset
   - Monitor performance
   - Gather user feedback

---

## Reference Dimensions

### Quick Copy-Paste Sizes

**Desktop:**
- Original: 1920×1080 @ 95% quality
- File size: 100KB WebP

**Tablet:**
- 1024×576 @ 90% quality
- File size: 50KB WebP

**Mobile:**
- 768×432 @ 85% quality
- File size: 30KB WebP

**Small Phone:**
- 480×270 @ 80% quality
- File size: 20KB WebP

---

**Status:** Recommendation Complete
**Date:** November 19, 2025
**Next Action:** Optimize current maps using TinyPNG or Squoosh
