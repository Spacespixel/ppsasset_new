# PPS Asset Amenity Icon System

## Overview
A comprehensive minimal icon system designed specifically for real estate location features and nearby amenities. Built with modern web standards, accessibility, and Thai real estate context in mind.

---

## Design Specifications

### Core Principles
- **Style**: Modern, minimal, flat design with line-based iconography
- **Base Size**: 64×64px (fully scalable vector)
- **Stroke Width**: 2px for visual consistency
- **Color Philosophy**: Brand magenta (#C2185B) with intelligent neutral states
- **Accessibility**: WCAG 2.1 AA compliant with keyboard navigation support

### Technical Details
- **Format**: SVG (Scalable Vector Graphics)
- **Optimization**: Hand-coded for minimal file size
- **Rendering**: Uses `shape-rendering="geometricPrecision"` for crisp display
- **Browser Support**: All modern browsers (Chrome, Firefox, Safari, Edge)

---

## Icon Inventory

### 1. Transportation (การเดินทาง)
**Icon ID**: `icon-transportation`
**Use Cases**: BTS, MRT, bus stops, taxi stands, public transit
**Design Elements**:
- Train car with windows
- Circular wheels
- Rail track at base
- Top antenna for electric train reference

**Context**: Perfect for Bangkok's extensive public transit system

---

### 2. Education (สถาบันการศึกษา)
**Icon ID**: `icon-education`
**Use Cases**: Schools, universities, tutoring centers, international schools
**Design Elements**:
- Graduation cap (mortarboard) at top
- Classical building columns
- Academic foundation base
- Tassel detail for graduation symbolism

**Context**: Critical for Thai families prioritizing education proximity

---

### 3. Hospital/Health (โรงพยาบาล)
**Icon ID**: `icon-hospital`
**Use Cases**: Hospitals, clinics, pharmacies, medical centers
**Design Elements**:
- Medical cross symbol
- Circular container (universally recognized)
- Optional pulse line accent
- Clean, clinical aesthetic

**Context**: Essential for family safety and healthcare access

---

### 4. Shopping (ห้างสรรพสินค้า/ตลาด)
**Icon ID**: `icon-shopping`
**Use Cases**: Malls, shopping centers, markets, retail districts
**Design Elements**:
- Shopping bag with handles
- Price tag detail
- Fold lines for depth
- Consumer-friendly aesthetic

**Context**: Bangkok's shopping culture is a major lifestyle factor

---

### 5. Dining (ร้านอาหาร)
**Icon ID**: `icon-dining`
**Use Cases**: Restaurants, food courts, street food, dining districts
**Design Elements**:
- Fork and spoon (international dining symbols)
- Plate/bowl with food
- Steam lines indicating freshness
- Appetizing visual language

**Context**: Thai food culture makes dining proximity highly valuable

---

### 6. Parks/Recreation (สวนสาธารณะ)
**Icon ID**: `icon-parks`
**Use Cases**: Parks, green spaces, recreation areas, exercise spaces
**Design Elements**:
- Tree with foliage
- Grass and ground details
- Park bench
- Nature-centric design

**Context**: Increasingly important for Bangkok's urban quality of life

---

## Implementation Guide

### Method 1: SVG Symbol + Use (Recommended)

**Benefits**: Best performance, easy color control, minimal code duplication

```html
<!-- 1. Include the SVG sprite once in your layout (before </body>) -->
<svg style="display: none;">
  <use href="/images/icons/amenity-icons.svg#icon-transportation"></use>
</svg>

<!-- 2. Use icons anywhere in your HTML -->
<div class="amenity-item">
  <div class="amenity-icon-wrapper amenity-icon-wrapper--large amenity-icon-wrapper--circle">
    <svg class="amenity-icon amenity-icon--active" width="64" height="64">
      <use href="/images/icons/amenity-icons.svg#icon-transportation"></use>
    </svg>
  </div>
  <h4 class="amenity-item__title">BTS อนุสาวรีย์ชัยสมรภูมิ</h4>
  <p class="amenity-item__distance">1.2 กม.</p>
</div>
```

### Method 2: Inline SVG

**Benefits**: No external HTTP request, full control

```html
<div class="amenity-icon-wrapper amenity-icon-wrapper--medium">
  <svg class="amenity-icon" viewBox="0 0 64 64" width="48" height="48">
    <!-- Paste icon path data here -->
  </svg>
</div>
```

### Method 3: CSS Background (Not Recommended)

Only use for static, non-interactive icons where color changes aren't needed.

---

## CSS Integration

### Include the stylesheet:
```html
<link rel="stylesheet" href="/css/amenity-icons.css">
```

### Available CSS Classes

#### Size Modifiers
```css
.amenity-icon-wrapper--small   /* 32×32px */
.amenity-icon-wrapper--medium  /* 48×48px */
.amenity-icon-wrapper--large   /* 64×64px (default) */
.amenity-icon-wrapper--xl      /* 80×80px */
```

#### Color States
```css
.amenity-icon--default    /* Neutral gray (#737373) */
.amenity-icon--active     /* Brand magenta (#C2185B) */
.amenity-icon--hover      /* Lighter magenta (#E91E63) */
.amenity-icon--disabled   /* Light gray with opacity */
```

#### Background Variants
```css
.amenity-icon-wrapper--circle   /* Circular background */
.amenity-icon-wrapper--rounded  /* Rounded square */
.amenity-icon-wrapper--filled   /* Filled magenta background */
```

#### Category-Specific Colors (Optional)
```css
.amenity-icon--transportation  /* Blue (#3b82f6) */
.amenity-icon--education      /* Purple (#8b5cf6) */
.amenity-icon--hospital       /* Red (#ef4444) */
.amenity-icon--shopping       /* Amber (#f59e0b) */
.amenity-icon--dining         /* Orange (#f97316) */
.amenity-icon--parks          /* Green (#10b981) */
```

---

## Complete HTML Example

```html
<!-- Nearby Amenities Section -->
<section class="nearby-amenities">
  <h2>สิ่งอำนวยความสะดวกใกล้เคียง</h2>

  <div class="amenity-grid">

    <!-- Transportation -->
    <div class="amenity-item">
      <div class="amenity-icon-wrapper amenity-icon-wrapper--large amenity-icon-wrapper--circle">
        <svg class="amenity-icon amenity-icon--transportation" width="64" height="64" aria-hidden="true">
          <use href="/images/icons/amenity-icons.svg#icon-transportation"></use>
        </svg>
      </div>
      <h4 class="amenity-item__title">BTS อนุสาวรีย์ชัยสมรภูมิ</h4>
      <p class="amenity-item__distance">1.2 กม.</p>
      <span class="amenity-item__badge">5 นาที</span>
    </div>

    <!-- Education -->
    <div class="amenity-item">
      <div class="amenity-icon-wrapper amenity-icon-wrapper--large amenity-icon-wrapper--circle">
        <svg class="amenity-icon amenity-icon--education" width="64" height="64" aria-hidden="true">
          <use href="/images/icons/amenity-icons.svg#icon-education"></use>
        </svg>
      </div>
      <h4 class="amenity-item__title">โรงเรียนนานาชาติ</h4>
      <p class="amenity-item__distance">2.5 กม.</p>
    </div>

    <!-- Hospital -->
    <div class="amenity-item">
      <div class="amenity-icon-wrapper amenity-icon-wrapper--large amenity-icon-wrapper--circle">
        <svg class="amenity-icon amenity-icon--hospital" width="64" height="64" aria-hidden="true">
          <use href="/images/icons/amenity-icons.svg#icon-hospital"></use>
        </svg>
      </div>
      <h4 class="amenity-item__title">โรงพยาบาลเซนต์หลุยส์</h4>
      <p class="amenity-item__distance">3.0 กม.</p>
    </div>

    <!-- Shopping -->
    <div class="amenity-item">
      <div class="amenity-icon-wrapper amenity-icon-wrapper--large amenity-icon-wrapper--circle">
        <svg class="amenity-icon amenity-icon--shopping" width="64" height="64" aria-hidden="true">
          <use href="/images/icons/amenity-icons.svg#icon-shopping"></use>
        </svg>
      </div>
      <h4 class="amenity-item__title">ห้างฟิวเจอร์พาร์ค</h4>
      <p class="amenity-item__distance">1.8 กม.</p>
    </div>

    <!-- Dining -->
    <div class="amenity-item">
      <div class="amenity-icon-wrapper amenity-icon-wrapper--large amenity-icon-wrapper--circle">
        <svg class="amenity-icon amenity-icon--dining" width="64" height="64" aria-hidden="true">
          <use href="/images/icons/amenity-icons.svg#icon-dining"></use>
        </svg>
      </div>
      <h4 class="amenity-item__title">ถนนคนเดินตลาดเก่า</h4>
      <p class="amenity-item__distance">0.8 กม.</p>
    </div>

    <!-- Parks -->
    <div class="amenity-item">
      <div class="amenity-icon-wrapper amenity-icon-wrapper--large amenity-icon-wrapper--circle">
        <svg class="amenity-icon amenity-icon--parks" width="64" height="64" aria-hidden="true">
          <use href="/images/icons/amenity-icons.svg#icon-parks"></use>
        </svg>
      </div>
      <h4 class="amenity-item__title">สวนลุมพินี</h4>
      <p class="amenity-item__distance">4.2 กม.</p>
    </div>

  </div>
</section>
```

---

## Color Palette Reference

### Brand Colors
```css
--color-brand-magenta:       #C2185B  /* Primary brand color */
--color-brand-magenta-light: #E91E63  /* Hover/active states */
--color-brand-magenta-dark:  #880E4F  /* Emphasis */
--color-brand-green:         #365523  /* Navigation/header */
```

### Neutral Palette
```css
--color-neutral-900: #1a1a1a  /* Headings, primary text */
--color-neutral-700: #404040  /* Body text */
--color-neutral-500: #737373  /* Secondary text, default icons */
--color-neutral-300: #d4d4d4  /* Borders, disabled states */
--color-neutral-100: #f5f5f5  /* Backgrounds */
```

### State Colors
```css
--color-success: #10b981  /* Positive actions */
--color-info:    #3b82f6  /* Informational */
--color-warning: #f59e0b  /* Caution */
```

---

## Accessibility Guidelines

### 1. Color Contrast
- **Icon + Background**: Minimum 3:1 contrast ratio (WCAG AA)
- **Text + Background**: Minimum 4.5:1 contrast ratio
- All default color combinations meet WCAG 2.1 AA standards

### 2. Semantic HTML
```html
<!-- ✅ GOOD: Proper semantic structure -->
<div class="amenity-item" role="article" aria-labelledby="amenity-1">
  <svg aria-hidden="true" focusable="false">...</svg>
  <h4 id="amenity-1">BTS อนุสาวรีย์ชัยสมรภูมิ</h4>
  <p>1.2 กม.</p>
</div>

<!-- ❌ BAD: No semantic meaning -->
<div>
  <svg>...</svg>
  <div>BTS อนุสาวรีย์ชัยสมรภูมิ</div>
  <div>1.2 กม.</div>
</div>
```

### 3. Screen Reader Support
```html
<!-- Always add aria-hidden to decorative icons -->
<svg aria-hidden="true" focusable="false">...</svg>

<!-- Use descriptive text alongside icons -->
<h4 class="amenity-item__title">BTS อนุสาวรีย์ชัยสมรภูมิ</h4>
```

### 4. Keyboard Navigation
- All interactive elements have `:focus` styles
- Logical tab order maintained
- Skip links available for long lists

### 5. Motion Sensitivity
```css
/* Respects user's motion preferences */
@media (prefers-reduced-motion: reduce) {
  .amenity-icon,
  .amenity-item {
    transition: none;
  }
}
```

---

## Responsive Design

### Breakpoint Strategy

#### Desktop (>768px)
- Grid: 3-6 columns (auto-fit)
- Icon size: 64×64px
- Font size: 14px title, 13px distance

#### Tablet (768px)
- Grid: 2-3 columns
- Icon size: 48×48px
- Font size: 13px title, 12px distance

#### Mobile (<480px)
- Grid: 2 columns
- Icon size: 40×48px
- Reduced padding and spacing
- Touch-friendly targets (min 44×44px)

### Mobile Optimization
```css
@media (max-width: 768px) {
  .amenity-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 12px;
  }

  .amenity-icon-wrapper--large {
    width: 48px;
    height: 48px;
  }
}
```

---

## Performance Optimization

### File Size
- **SVG Sprite**: ~8KB (uncompressed)
- **CSS File**: ~12KB (uncompressed)
- **Gzipped Total**: ~4KB

### Loading Strategy
```html
<!-- Critical CSS: Inline above-the-fold icons -->
<style>
  /* Inline critical icon styles here */
</style>

<!-- Deferred CSS: Load full stylesheet asynchronously -->
<link rel="preload" href="/css/amenity-icons.css" as="style" onload="this.onload=null;this.rel='stylesheet'">
<noscript><link rel="stylesheet" href="/css/amenity-icons.css"></noscript>
```

### Caching Headers
```
Cache-Control: public, max-age=31536000, immutable
```

### CDN Recommendations
- Place icons on CDN for global distribution
- Use versioned URLs for cache busting
- Enable Brotli compression

---

## Browser Support

### Modern Browsers (Full Support)
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+
- Opera 76+

### Mobile Browsers
- iOS Safari 14+
- Chrome Mobile 90+
- Samsung Internet 14+

### Fallback Strategy
```html
<!-- SVG not supported fallback -->
<svg>
  <image href="/images/icons/fallback/transportation.png" alt="Transportation" />
</svg>
```

---

## Customization Guide

### Changing Icon Colors

**Method 1: CSS Variables (Recommended)**
```css
:root {
  --color-brand-magenta: #FF5722; /* Your custom color */
}
```

**Method 2: Direct Class Override**
```css
.amenity-icon--active {
  color: #FF5722;
}
```

### Adjusting Stroke Width
```css
.amenity-icon svg {
  stroke-width: 3px; /* Heavier weight */
}
```

### Custom Background Styles
```css
.amenity-icon-wrapper--custom {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 16px;
  padding: 16px;
}
```

---

## Advanced Use Cases

### Animated Icons
```css
@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-8px); }
}

.amenity-icon--featured {
  animation: bounce 2s infinite;
}
```

### Icon Badges
```html
<div class="amenity-icon-wrapper" style="position: relative;">
  <svg class="amenity-icon">...</svg>
  <span class="icon-badge">NEW</span>
</div>

<style>
.icon-badge {
  position: absolute;
  top: -4px;
  right: -4px;
  background: #ef4444;
  color: white;
  font-size: 10px;
  padding: 2px 6px;
  border-radius: 8px;
  font-weight: 700;
}
</style>
```

### Interactive Maps
```javascript
// Add click handlers for interactive maps
document.querySelectorAll('.amenity-item').forEach(item => {
  item.addEventListener('click', () => {
    const lat = item.dataset.lat;
    const lng = item.dataset.lng;
    openMap(lat, lng);
  });
});
```

---

## Testing Checklist

### Visual Testing
- [ ] Icons render correctly at all sizes (32px, 48px, 64px, 80px)
- [ ] Colors match brand guidelines
- [ ] Hover states work smoothly
- [ ] Icons are crisp on retina displays
- [ ] Print styles render appropriately

### Accessibility Testing
- [ ] Keyboard navigation works (Tab, Enter, Space)
- [ ] Screen reader announces content correctly
- [ ] Color contrast meets WCAG AA (3:1 for icons)
- [ ] Focus indicators are visible
- [ ] Works with reduced motion preferences

### Performance Testing
- [ ] Icons load within 1 second
- [ ] No layout shift (CLS = 0)
- [ ] SVG sprite is cached properly
- [ ] No console errors or warnings

### Cross-Browser Testing
- [ ] Chrome/Edge (Windows + Mac)
- [ ] Firefox (Windows + Mac)
- [ ] Safari (Mac + iOS)
- [ ] Mobile browsers (iOS Safari, Chrome Mobile)

### Responsive Testing
- [ ] Desktop (1920px, 1440px, 1024px)
- [ ] Tablet (768px)
- [ ] Mobile (480px, 375px, 320px)

---

## Troubleshooting

### Icons Not Displaying

**Problem**: Icons show as blank squares
**Solution**: Check SVG path and ensure sprite is loaded
```html
<!-- Verify this is in your HTML -->
<svg style="display: none;"><use href="/images/icons/amenity-icons.svg"></use></svg>
```

### Color Not Changing

**Problem**: Icon color doesn't respond to CSS
**Solution**: Ensure you're using `currentColor` or CSS variables
```css
.amenity-icon {
  color: var(--color-brand-magenta); /* Not fill or stroke */
}
```

### Icons Too Small on Mobile

**Problem**: Icons difficult to tap on mobile
**Solution**: Increase wrapper size for touch targets
```css
@media (max-width: 768px) {
  .amenity-icon-wrapper {
    min-width: 44px;  /* iOS minimum tap target */
    min-height: 44px;
  }
}
```

### Blurry Icons

**Problem**: Icons appear blurry or pixelated
**Solution**: Ensure SVG viewBox is correct and use integer dimensions
```html
<svg viewBox="0 0 64 64" width="64" height="64">...</svg>
```

---

## Version History

### v1.0.0 (2024-11-20)
- Initial release with 6 core amenity icons
- Complete CSS system with color states
- Full accessibility implementation
- Responsive grid layout
- Documentation and examples

---

## Future Roadmap

### Planned Icons (v1.1.0)
- [ ] Gas stations (ปั๊มน้ำมัน)
- [ ] Banks/ATMs (ธนาคาร)
- [ ] Convenience stores (7-Eleven, Family Mart)
- [ ] Community spaces (ชุมชน)
- [ ] Entertainment (โรงภาพยนตร์, บันเทิง)
- [ ] Sports facilities (สนามกีฬา)

### Planned Features
- [ ] Animated icon variants
- [ ] Dark mode optimizations
- [ ] Icon picker component
- [ ] Figma design tokens export
- [ ] React/Vue component library

---

## Credits & License

**Designed by**: Claude (Anthropic) for PPS Asset
**Design Date**: November 20, 2024
**Version**: 1.0.0
**License**: Proprietary (PPS Asset exclusive use)

### Design Influences
- Material Design Icons (Google)
- Heroicons (Tailwind Labs)
- Feather Icons (Cole Bemis)
- Thai cultural context and local real estate conventions

---

## Support & Contact

For questions, customization requests, or issues:
- **Project Repository**: /Users/horizon/Documents/dev/ppsasset_new
- **Icon Files**: `/wwwroot/images/icons/`
- **CSS Files**: `/wwwroot/css/amenity-icons.css`
- **Documentation**: This file (ICON_SYSTEM_README.md)

---

## Quick Start (TL;DR)

```html
<!-- 1. Add this to your <head> -->
<link rel="stylesheet" href="/css/amenity-icons.css">

<!-- 2. Add this before </body> -->
<svg style="display: none;">
  <use href="/images/icons/amenity-icons.svg"></use>
</svg>

<!-- 3. Use icons like this -->
<div class="amenity-icon-wrapper amenity-icon-wrapper--large amenity-icon-wrapper--circle">
  <svg class="amenity-icon amenity-icon--active" width="64" height="64" aria-hidden="true">
    <use href="/images/icons/amenity-icons.svg#icon-transportation"></use>
  </svg>
</div>
```

**That's it!** You're ready to use the icon system.

---

**Last Updated**: November 20, 2024
**Status**: Production Ready ✅
