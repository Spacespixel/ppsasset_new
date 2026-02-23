# Quick Mobile Banner Reference

## TL;DR - What Changed

Your mobile banner now fits properly on mobile devices by:
1. **Reducing text size** (2.5rem → 1.8rem/1.5rem)
2. **Making banner shorter** (600px → 350px/300px)
3. **Moving text to bottom** (centered → flex-end alignment)
4. **Shrinking button** (14px 28px → 10px 20px or 8px 16px)

---

## Device Breakpoints

```
PHONE (< 480px)    │ MOBILE (481-768px)  │ TABLET (769+px)
─────────────────────────────────────────────────────────
Title: 1.5rem      │ Title: 1.8rem       │ Title: 2.5rem
Height: 300px      │ Height: 350px       │ Height: 600px
Button: 8px 16px   │ Button: 10px 20px   │ Button: 14px 28px
```

---

## Font Sizes Applied

### Title (#hero-main-title)
- **Desktop:** 2.5rem
- **Mobile & Tablet:** 1.8rem
- **Small Phone:** 1.5rem

### Subtitle (#hero-description)
- **Desktop:** 1.15rem
- **Mobile & Tablet:** 0.95rem
- **Small Phone:** 0.88rem

---

## CSS Classes Modified

| Class | Change |
|-------|--------|
| `.hero-section` | Added min-height responsive sizing |
| `.hero-section .wrapper` | Changed display: table → display: flex on mobile |
| `.hero-section .wrapper .hero-title` | Added bottom alignment (align-items: flex-end) |
| `#hero-main-title` | Added responsive font sizing |
| `#hero-description` | Added responsive font sizing |
| `.hero-cta .btn-primary-cta` | Added responsive padding/font size |

---

## Before vs After

### BEFORE (Not optimized)
```
┌────────────────────────────┐
│                            │
│  ╔────────────────────╗   │
│  ║ HERO IMAGE        ║   │ <- Small, cramped
│  ║ "เดอะริคโค้..."    ║   │ <- Too large (2.5rem)
│  ║ "ฯลฯ"            ║   │
│  ║ [Button]          ║   │ <- Cramped
│  ║ SQUEEZED TEXT     ║   │
│  ╚────────────────────╝   │
│                            │
└────────────────────────────┘
```

### AFTER (Optimized)
```
┌────────────────────────────┐
│                            │
│  ╔────────────────────╗   │
│  ║                    ║   │
│  ║ HERO IMAGE        ║   │ <- More visible (65-70%)
│  ║ (More visible)    ║   │
│  ║                    ║   │
│  ║────────────────────║   │
│  ║ "เดอะริคโค้..."    ║   │ <- Perfect size (1.8rem)
│  ║ "ฯลฯ"            ║   │
│  ║ [Button]          ║   │ <- Perfect spacing
│  ╚────────────────────╝   │
│                            │
└────────────────────────────┘
```

---

## Code Changes Summary

### Added to style-custom.css (Line 344-367)
```css
/* Mobile: Hero Title Container */
@media (max-width: 768px) {
  .hero-section .wrapper .hero-title {
    display: block !important;
    vertical-align: auto;
    padding: 0 20px 20px 20px;
  }
  /* ... more responsive styles ... */
}

@media (max-width: 480px) {
  .hero-section .wrapper .hero-title {
    padding: 0 16px 16px 16px;
  }
}
```

### Added to style-custom.css (Line 5115-5184)
```css
/* ===== MOBILE OPTIMIZATION FOR HERO BANNER ===== */

@media (max-width: 768px) {
  .hero-section {
    min-height: 350px;
    height: auto;
  }

  #hero-main-title {
    font-size: 1.8rem !important;
  }

  #hero-description {
    font-size: 0.95rem !important;
  }

  .hero-cta .btn-primary-cta {
    padding: 10px 20px !important;
    font-size: 14px !important;
  }
}

@media (max-width: 480px) {
  /* Even more compact for small phones */
  .hero-section {
    min-height: 300px;
  }

  #hero-main-title {
    font-size: 1.5rem !important;
  }
  /* ... etc ... */
}
```

---

## Testing Checklist

- [ ] View on real iPhone/Android device
- [ ] Check text isn't cut off
- [ ] Verify button is tapable (min 44x44px)
- [ ] Confirm image visibility (70% of banner)
- [ ] Test carousel transitions
- [ ] Check in both portrait & landscape
- [ ] Verify no overlapping text
- [ ] Test on iOS Safari
- [ ] Test on Chrome Mobile

---

## Browser Support

✅ All modern browsers:
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+
- Mobile browsers (iOS Safari, Chrome Mobile, Samsung Internet)

---

## Files Changed

- `/wwwroot/css/style-custom.css` - Main stylesheet with responsive CSS
- `/wwwroot/css/style.css` - Auto-compiled from style-custom.css (no manual edit)

---

## Performance Impact

- ✅ **No extra requests** - Pure CSS, no new files
- ✅ **Minimal size increase** - ~1KB additional CSS
- ✅ **No JavaScript changes** - All vanilla JS unaffected
- ✅ **Fast renders** - CSS-only = instant layout

---

## Responsive Image Recommendations

For best results, use responsive images:

```html
<img
  src="~/images/projects/ricco/hero-768.jpg"
  srcset="
    ~/images/projects/ricco/hero-480.jpg 480w,
    ~/images/projects/ricco/hero-768.jpg 768w,
    ~/images/projects/ricco/hero-1920.jpg 1920w
  "
  sizes="(max-width: 480px) 480px,
         (max-width: 768px) 768px,
         1920px"
  alt="Project Name">
```

Recommended image sizes:
- **480px wide:** 480×300px (1.6:1)
- **768px wide:** 768×350px (2.2:1)
- **1920px wide:** 1920×600px (3.2:1)

---

## Need More Details?

See the full documentation:
- `MOBILE_BANNER_OPTIMIZATION.md` - Technical details
- `MOBILE_LAYOUT_GUIDE.md` - Visual reference & diagrams
