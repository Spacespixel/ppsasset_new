# Typography System - PPS Asset Website

## Overview
This document defines the complete typography system for PPS Asset website, ensuring consistent font sizing and weights across all sections.

## Font Stack

### Primary Font Family (Thai-Optimized)
```css
font-family: 'Kanit', 'Prompt', 'Sarabun', 'Montserrat', sans-serif;
```

The font stack is designed to:
1. **Kanit** - Primary Thai font, excellent readability
2. **Prompt** - Secondary Thai font, elegant alternative
3. **Sarabun** - Tertiary Thai font, clean and modern
4. **Montserrat** - English/Latin fallback for maximum compatibility
5. **sans-serif** - Final fallback for system fonts

## Font Size Scale

### Desktop (16px base)
All sizes scale from a 16px base font size on desktop browsers.

| Element | Size | Pixels | rem | Usage |
|---------|------|--------|-----|-------|
| h1 | 3.5rem | 56px | 3.5 | Page Title, Main Heading |
| h2 | 2.75rem | 44px | 2.75 | Section Title, Major Heading |
| h3 | 2rem | 32px | 2 | Subsection Title, Feature Title |
| h4 | 1.5rem | 24px | 1.5 | Card Title, Smaller Heading |
| h5 | 1.25rem | 20px | 1.25 | Small Title |
| h6 | 1rem | 16px | 1 | Mini Title |
| p (body) | 1rem | 16px | 1 | Paragraph Text |
| p.text-large | 1.125rem | 18px | 1.125 | Hero/Feature Text |
| p.text-small | 0.875rem | 14px | 0.875 | Helper/Meta Text |
| p.text-xs | 0.75rem | 12px | 0.75 | Labels/Captions |

### Tablet (max-width: 768px)
Base font: 15px

| Element | Size | Pixels | rem |
|---------|------|--------|-----|
| h1 | 2.5rem | 40px | 2.5 |
| h2 | 2rem | 32px | 2 |
| h3 | 1.625rem | 26px | 1.625 |
| h4 | 1.25rem | 20px | 1.25 |
| h5 | 1.125rem | 18px | 1.125 |
| h6 | 1rem | 16px | 1 |
| p (body) | 0.9375rem | 15px | 0.9375 |
| p.text-large | 1rem | 16px | 1 |
| p.text-small | 0.8125rem | 13px | 0.8125 |

### Mobile (max-width: 480px)
Base font: 14px

| Element | Size | Pixels | rem |
|---------|------|--------|-----|
| h1 | 2rem | 32px | 2 |
| h2 | 1.625rem | 26px | 1.625 |
| h3 | 1.375rem | 22px | 1.375 |
| h4 | 1.125rem | 18px | 1.125 |
| h5 | 1rem | 16px | 1 |
| h6 | 0.875rem | 14px | 0.875 |
| p (body) | 0.875rem | 14px | 0.875 |
| p.text-large | 0.9375rem | 15px | 0.9375 |
| p.text-small | 0.75rem | 12px | 0.75 |

## Font Weights

| Class | Weight | Usage |
|-------|--------|-------|
| .fw-light | 300 | Subtle text, light emphasis |
| .fw-normal | 400 | Default body text |
| .fw-medium | 500 | Slightly emphasized text |
| .fw-semibold / strong / b | 600 | Important text, labels |
| .fw-bold | 700 | Headings, strong emphasis |

### Semantic Weight Usage
- **Headings (h1-h6)**: Always use `font-weight: 700` (bold)
- **Body Text (p)**: Default `font-weight: 400` (normal)
- **Labels/Labels**: Use `font-weight: 600` (semibold)
- **Emphasis (strong, b)**: Automatically `font-weight: 600`

## Line Heights

| Class | Value | Usage |
|-------|-------|-------|
| .lh-tight | 1.2 | Headings, titles |
| .lh-normal | 1.6 | Body text, paragraphs |
| .lh-relaxed | 1.8 | Blockquotes, longer text |

### Default Line Heights by Element
- **h1-h6**: 1.2 (tight)
- **p, .text-large, .text-small**: 1.6 (normal)
- **blockquote**: 1.8 (relaxed)
- **li, td, th**: 1.6 (normal)

## Letter Spacing

| Class | Value | Usage |
|-------|-------|-------|
| --ls-tight | -0.5px | Headings, compressed spacing |
| --ls-normal | 0 | Default spacing |
| --ls-wide | 0.5px | Special emphasis, wide spacing |

## Component Typography Examples

### Hero Section
```html
<h1 class="heading-xl">Main Title</h1>
<p class="text-large">Hero subtitle text</p>
```
**Expected Rendering:**
- h1: 56px, 700 weight, Kanit font
- p: 18px, 400 weight, Kanit font

### Concept Section
```html
<h3>Concept Title</h3>
<p>Concept description text</p>
```
**Expected Rendering (Desktop):**
- h3: 32px, 700 weight, Kanit font
- p: 16px, 400 weight, Kanit font

**Expected Rendering (Tablet):**
- h3: 26px, 700 weight
- p: 16px, 400 weight

**Expected Rendering (Mobile):**
- h3: 22px, 700 weight
- p: 14px, 400 weight

### Registration Form
```html
<label for="name">Full Name</label>
<input type="text" id="name" />
```
**Expected Rendering:**
- label: 14px, 500 weight, Kanit font
- input: 16px, 400 weight, Kanit font

### Feature Cards
```html
<h4>Feature Title</h4>
<p>Feature description</p>
```
**Expected Rendering:**
- h4: 24px, 600 weight, Kanit font
- p: 16px, 400 weight, Kanit font

## CSS Classes for Direct Use

### Text Size Classes
```html
<p class="text-xl">Extra Large Text (32px)</p>
<p class="text-lg">Large Text (24px)</p>
<p class="text-md">Medium Text (16px)</p>
<p class="text-sm">Small Text (14px)</p>
<p class="text-xs">Extra Small Text (12px)</p>
```

### Heading Classes
```html
<p class="heading-xl">56px Heading</p>
<p class="heading-lg">44px Heading</p>
<p class="heading-md">32px Heading</p>
<p class="heading-sm">24px Heading</p>
<p class="heading-xs">20px Heading</p>
```

### Font Weight Classes
```html
<p class="fw-light">Light (300)</p>
<p class="fw-normal">Normal (400)</p>
<p class="fw-medium">Medium (500)</p>
<p class="fw-semibold">Semibold (600)</p>
<p class="fw-bold">Bold (700)</p>
```

### Line Height Classes
```html
<p class="lh-tight">Tight line height (1.2)</p>
<p class="lh-normal">Normal line height (1.6)</p>
<p class="lh-relaxed">Relaxed line height (1.8)</p>
```

## Implementation in Views

### HTML Semantic Tags
```html
<!-- Automatic styling without additional classes -->
<h1>Page Title</h1>
<h2>Section Title</h2>
<h3>Subsection Title</h3>
<p>Paragraph text</p>
<strong>Bold text</strong>
```

### With Utility Classes
```html
<!-- Override default sizing with utility classes -->
<h3 class="text-xl">Large subtitle using text-xl class</h3>
<p class="text-sm">Small paragraph text</p>
```

### Combined Utilities
```html
<!-- Combine multiple utilities -->
<h4 class="fw-bold lh-tight text-lg">Bold large heading with tight spacing</h4>
```

## Ensuring Consistency Across Sections

### In Project Details Section
- **Titles (h3)**: Always 32px on desktop, 26px on tablet, 22px on mobile
- **Descriptions (p)**: Always 16px on desktop, 16px on tablet, 14px on mobile
- **DO NOT** use inline `style="font-family:..."` attributes
- **DO NOT** mix different font families in same section

### In Forms
```html
<label>Field Label</label>  <!-- 14px, 500 weight -->
<input type="text" />       <!-- 16px, 400 weight -->
<small>Helper text</small>  <!-- 12px, 400 weight -->
```

### In Cards
```html
<h4>Card Title</h4>         <!-- 24px, 600 weight -->
<p>Card description</p>     <!-- 16px, 400 weight -->
```

## Common Mistakes to Avoid

❌ **WRONG**: Inline font-family styles
```html
<h3 style="font-family: 'Sarabun'">Title</h3>
<p style="font-family: 'Prompt'">Text</p>
```

✅ **RIGHT**: Use semantic HTML and inherit from typography.css
```html
<h3>Title</h3>  <!-- Automatically uses Kanit -->
<p>Text</p>     <!-- Automatically uses Kanit -->
```

❌ **WRONG**: Inconsistent font sizes
```html
<h3 style="font-size: 36px">Title on desktop</h3>
<!-- Size doesn't change on tablet/mobile -->
```

✅ **RIGHT**: Use rem units that scale with breakpoints
```html
<h3>Title</h3>  <!-- 2rem (32px) desktop, 1.625rem (26px) tablet, 1.375rem (22px) mobile -->
```

❌ **WRONG**: Mix font families in same section
```html
<h3 style="font-family: 'Prompt'">Title</h3>
<p style="font-family: 'Sarabun'">Description</p>
```

✅ **RIGHT**: Use single consistent font stack
```html
<h3>Title</h3>              <!-- Kanit font -->
<p>Description</p>          <!-- Kanit font (same) -->
```

## Accessibility Features

### Responsive Text Sizing
- Font sizes automatically reduce on tablets and mobile devices
- Base font size adjusts: 16px (desktop) → 15px (tablet) → 14px (mobile)
- Ensures readability on all device sizes

### High Contrast Mode Support
- All text colors have sufficient contrast for WCAG AA compliance
- Heading colors automatically increase contrast in high contrast mode

### Print Styles
- Font sizes optimized for printing (12pt base)
- Widow/orphan controls prevent awkward page breaks
- All font families render correctly in print

### Reduced Motion Support
- No animation changes apply to typography
- Smooth transitions disabled for users with motion sensitivity

## Browser Support

The typography system uses CSS variables and modern CSS, supporting:
- Chrome/Edge 49+
- Firefox 31+
- Safari 9.1+
- iOS Safari 9.3+
- Chrome Android 49+

All fonts are served via Google Fonts with fallback to system fonts.

## Font Import Statement

All fonts are imported in `typography.css`:
```css
@import url('https://fonts.googleapis.com/css?family=Montserrat:300,400,500,600,700');
@import url('https://fonts.googleapis.com/css2?family=Sarabun:wght@300;400;500;600;700&display=swap');
@import url('https://fonts.googleapis.com/css2?family=Prompt:wght@300;400;500;600;700&display=swap');
@import url('https://fonts.googleapis.com/css2?family=Kanit:wght@300;400;500;600;700&display=swap');
```

## File Locations

- **Main Typography CSS**: `/wwwroot/css/typography.css`
- **Applied in Layout**: `/Views/Shared/_Layout.cshtml` (line 63)
- **Project-Specific Overrides**: `/Views/Home/Project.cshtml` (inline `<style>` tags)
- **Base Site Styles**: `/wwwroot/css/site.css`
- **Legacy Styles**: `/wwwroot/css/style.css`

## Testing Font Consistency

To verify fonts are displaying consistently across sections:

1. **Browser DevTools Font Inspection**
   - Right-click any text element
   - Select "Inspect" or "Inspect Element"
   - Check the "Computed" tab to verify:
     - Font family is 'Kanit', 'Prompt', or 'Sarabun'
     - Font size matches expected value
     - Font weight is correct (600 for headings, 400 for body)

2. **Visual Comparison Across Devices**
   - Desktop (1920px): Font sizes should match table above
   - Tablet (768px): Font sizes should scale appropriately
   - Mobile (480px): Font sizes should be readable and sized down

3. **Section-by-Section Checklist**
   - [ ] Hero section: h1 appears 56px bold
   - [ ] Concept section: h3 appears 32px bold, p appears 16px normal
   - [ ] Feature cards: h4 appears 24px semibold
   - [ ] Form labels: appear 14px semibold
   - [ ] Body text: appears 16px normal weight
   - [ ] Small text: appears 14px normal weight

## Future Updates

When adding new sections or components:

1. Always use semantic HTML (`<h1>`, `<p>`, `<strong>`, etc.)
2. Inherit typography from `typography.css`
3. Only add inline styles for layout/positioning, never for fonts
4. Use utility classes for size/weight overrides
5. Test on mobile/tablet to ensure responsive scaling works
6. Document any custom typography in this file

---

**Last Updated**: 2024-11-09
**Version**: 1.0
**Status**: Production Ready
