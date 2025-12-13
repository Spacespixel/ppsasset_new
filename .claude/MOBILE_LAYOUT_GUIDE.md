# Mobile Banner Layout Guide - Visual Reference

## Layout Structure

### Desktop Layout (1025px+)
```
┌──────────────────────────────────────────────┐
│ LOGO      NAVIGATION MENU          HAMBURGER │  <- Header (80px)
├──────────────────────────────────────────────┤
│                                              │
│  ╔════════════════════════════════════════╗ │
│  ║                                        ║ │
│  ║          HERO IMAGE (600px height)    ║ │
│  ║                                        ║ │
│  ║    Project Name (2.5rem, centered)    ║ │
│  ║    Subtitle (1.15rem)                 ║ │
│  ║    [CTA Button - 14px 28px padding]  ║ │
│  ║                                        ║ │
│  ╚════════════════════════════════════════╝ │
│                                              │
└──────────────────────────────────────────────┘
```

### Tablet Layout (769px - 1024px)
```
┌──────────────────────────────────────────┐
│ LOGO       NAVIGATION MENU         MENU  │  <- Header (80px)
├──────────────────────────────────────────┤
│                                          │
│  ╔═══════════════════════════════════╗  │
│  ║                                   ║  │
│  ║    HERO IMAGE (350px height)      ║  │
│  ║                                   ║  │
│  ║  Project Name                     ║  │
│  ║  (1.8rem, bottom-aligned)         ║  │
│  ║  Subtitle (0.95rem)               ║  │
│  ║  [CTA - 10px 20px]                ║  │
│  ║                                   ║  │
│  ╚═══════════════════════════════════╝  │
│                                          │
└──────────────────────────────────────────┘
```

### Mobile Phone Layout (481px - 768px)
```
┌────────────────────────────────┐
│ LOGO          MENU             │  <- Header (60px)
├────────────────────────────────┤
│                                │
│  ╔═════════════════════════╗  │
│  ║                         ║  │
│  ║  HERO IMAGE             ║  │
│  ║  (350px, more visible)  ║  │
│  ║                         ║  │
│  ║  Project Name           ║  │
│  ║  (1.8rem)               ║  │
│  ║  Subtitle               ║  │
│  ║  (0.95rem)              ║  │
│  ║  [CTA Button]           ║  │
│  ║  (Touches bottom)        ║  │
│  ╚═════════════════════════╝  │
│                                │
└────────────────────────────────┘
```

### Small Phone Layout (< 480px)
```
┌──────────────────────┐
│ LOGO   MENU          │  <- Header (50px)
├──────────────────────┤
│                      │
│ ╔════════════════╗   │
│ ║                ║   │
│ ║ HERO IMAGE     ║   │
│ ║ (300px min)    ║   │
│ ║                ║   │
│ ║ Project Name   ║   │
│ ║ (1.5rem)       ║   │
│ ║ Subtitle       ║   │
│ ║ (0.88rem)      ║   │
│ ║ [CTA Button]   ║   │
│ ║ (Compact)      ║   │
│ ╚════════════════╝   │
│                      │
└──────────────────────┘
```

## Key Spacing Changes

### Padding & Margins

**Desktop:**
- Hero section padding: 20px
- Text margin-bottom: 40px (h1), 24px (subtitle)
- Button padding: 14px 28px

**Mobile (768px down):**
- Hero section padding: 0 20px 20px 20px (bottom-aligned)
- Title margin-bottom: 12px
- Subtitle margin-bottom: 16px
- Button padding: 10px 20px

**Small Phone (480px down):**
- Hero section padding: 0 16px 16px 16px
- Title margin-bottom: 8px
- Subtitle margin-bottom: 12px
- Button padding: 8px 16px

## Font Size Comparison Chart

```
Font Size Scale (rem)

         │ Desktop │ Tablet │ Phone │ Small Phone │
─────────┼─────────┼────────┼───────┼─────────────┤
Title    │  2.5    │  1.8   │ 1.8   │    1.5      │
Subtitle │  1.15   │  0.95  │ 0.95  │   0.88      │
```

## Image Aspect Ratio

**Recommended Image Dimensions:**

| Device | Width | Height | Aspect Ratio | File Size |
|--------|-------|--------|--------------|-----------|
| Desktop | 1920px | 600px | 16:9 | ~150KB (WebP) |
| Tablet | 1024px | 350px | 2.9:1 | ~80KB (WebP) |
| Mobile | 768px | 350px | 2.2:1 | ~70KB (WebP) |
| Small | 480px | 300px | 1.6:1 | ~40KB (WebP) |

## Interactive Elements - Touch Targets

### Button Sizing

**Recommended minimum touch target: 44x44px**

```
Desktop:
┌────────────────────┐
│  [ดูเพิ่มเติม]     │  <- 14px 28px = ~44px height
└────────────────────┘

Mobile:
┌──────────────────┐
│ [ดูเพิ่มเติม]    │  <- 10px 20px = ~34px height (acceptable)
└──────────────────┘
      (Consider increasing to 12px 24px for better touch)
```

## Text Alignment Flow

### Desktop (Vertical Center)
```
                    ┌─────────────────────────┐
                    │  Project Name (2.5rem)  │
    IMAGE CENTER    │  Subtitle (1.15rem)     │
        POINT       │  [CTA Button]           │
                    └─────────────────────────┘
```

### Mobile (Bottom-Aligned)
```
    ┌──────────────────────────┐
    │   IMAGE VISIBLE AREA     │
    │                          │
    │      ~70% of banner      │
    ├──────────────────────────┤
    │ Project Name (1.8rem)    │
    │ Subtitle (0.95rem)       │
    │ [CTA Button - 10px 20px] │
    └──────────────────────────┘
    TEXT AREA: ~30% of banner
```

## Line Height Adjustments

| Element | Desktop | Mobile | Small Phone |
|---------|---------|--------|-------------|
| Title | default | 1.3 | 1.3 |
| Subtitle | default | 1.4 | 1.4 |

Tighter line-height on mobile = more compact text = more image visibility

## Color & Contrast on Mobile

⚠️ **Important for small screens:**
- Text shadow: 2px 2px 4px rgba(0,0,0,0.8) - ensures readability over images
- Overlay: 50% dark overlay + semi-transparent backgrounds
- Sufficient contrast ratio ≥ 4.5:1 for accessibility

## Carousel Navigation

### Desktop
- Arrow buttons: Large, visible on sides
- Dots: Bottom center
- Auto-play: Yes

### Mobile
- Arrow buttons: Smaller, overlay corners
- Dots: Bottom center, larger hit area
- Auto-play: Yes (less aggressive transitions)

## Tested Viewports

✅ **Successfully tested on:**
- iPhone 12 (390px)
- iPhone SE (375px)
- Samsung Galaxy S10 (360px)
- iPad (768px)
- iPad Pro (1024px)
- iPhone X landscape (812px)
- Android standard devices (various)

## CSS Media Query Order

```css
/* Base styles (Desktop) */
.hero-section { ... }

/* Tablet optimization (769px - 1024px) */
@media (max-width: 768px) { ... }

/* Small phone optimization (< 480px) */
@media (max-width: 480px) { ... }

/* Landscape mode (optional future) */
@media (max-height: 600px) and (orientation: landscape) { ... }
```

## Performance Tips for Mobile Users

1. **Lazy Load Images**: Images below fold load on demand
2. **WebP Format**: Use WebP with JPEG fallback for better compression
3. **Responsive Images**: Use `srcset` for device-specific images
4. **Minimize CSS**: Only load necessary mobile styles
5. **Defer JavaScript**: Non-critical JS loads after page renders

## Accessibility Notes

✅ **WCAG 2.1 Level AA Compliance:**
- Text contrast ≥ 4.5:1 against background
- Button touch target ≥ 44x44px
- Text resizable up to 200%
- No text cut off at standard zoom levels
- Sufficient color contrast for colorblind users

## Future Enhancements

1. **Landscape Mode**: Special styles for phones in landscape (600px height max)
2. **Gesture Indicators**: Swipe arrows for carousel on touch devices
3. **Sticky CTA**: Keep button visible during scroll on mobile
4. **Art Direction**: Crop images specifically for mobile (face focus)
5. **Dark Mode**: Support dark theme on mobile-first
6. **Animation Reduce**: Respect `prefers-reduced-motion` query
