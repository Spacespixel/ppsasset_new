# Dropdown Menu Quick Reference

## Key CSS File
**Location**: `/wwwroot/css/dropdown-menu.css`

---

## Color Palette

```css
/* Brand */
#365523    Brand Green (Headers, Accents, Hover)

/* Backgrounds */
#ffffff    White (Main content, Cards on hover)
#f8f9fa    Light Gray (Sidebar, Card base)
#e0e0e0    Border Gray (Dividers)

/* Text */
#2c3e50    Dark Gray (Category headers)
#495057    Medium Gray (Body text)
#999999    Light Gray (Close button)
```

---

## Layout Proportions

```css
/* Desktop Layout */
Left Column:   18% (flex: 0 0 18%)
Right Column:  82% (flex: 0 0 82%)

/* Tablet Layout (< 1200px) */
Left Column:   22% (flex: 0 0 22%)
Right Column:  78% (flex: 0 0 78%)
```

---

## Typography Sizes

```css
Column Headers:       22px, weight 700
Category Headers:     18px, weight 600
Left Sidebar Links:   14px, weight 400
Right Content Links:  15px, weight 400
Close Button:         48px, weight 300
```

---

## Spacing Values

```css
/* Padding */
Column:               60px 40px (vertical horizontal)
Headers:              16px 24px
List Items (Left):    10px 12px
List Items (Right):   16px 20px

/* Margins */
Category Top:         32px (first: 0)
List Bottom:          24px
Grid Gap:             16px
```

---

## Key Measurements

```css
/* Positioning */
Top Position:         80px (below nav)
Height:               calc(100vh - 80px)
Border Width:         1px (columns), 2px (sub-headers)

/* Hover Effects */
Transform Left:       translateX(4px)
Transform Right:      translateY(-2px)
Border Left Accent:   3px solid #365523
```

---

## Animation Timings

```css
/* Dropdown */
Open/Close:           0.3s ease
Opacity/Transform:    0.3s ease

/* Hover States */
All Links:            0.25s ease
Arrow Icon:           0.25s ease
Caret Rotation:       0.3s ease

/* Stagger Delays */
Items:                0.05s, 0.08s, 0.11s, 0.14s...
```

---

## Grid System

```css
/* Right Column */
Grid Columns:         2 (repeat(2, 1fr))
Grid Gap:             16px
Column Template:      Equal width columns

/* Tablet */
Grid Columns:         1 (single column)
```

---

## Breakpoints

```css
Desktop:     > 1200px  (Full layout)
Tablet:      991px - 1200px  (Adjusted proportions)
Mobile:      < 991px  (Hidden, mobile menu active)
```

---

## Z-Index Layers

```css
Close Button:         10
Dropdown Container:   (inherits from .navigation z-index: 99)
Column Content:       1 (auto)
```

---

## Common Customizations

### Change Brand Color
**Find**: `#365523`
**Replace with**: Your brand hex color
**Files**: dropdown-menu.css (13 instances)

### Adjust Column Width
```css
/* Line ~70 */
.dropdown-column:first-child {
  flex: 0 0 18%;  /* Change 18% to desired width */
}

/* Line ~75 */
.dropdown-column:last-child {
  flex: 0 0 82%;  /* Adjust to total 100% */
}
```

### Change Grid Columns
```css
/* Line ~260 */
.dropdown-column:last-child .projects-list {
  grid-template-columns: repeat(2, 1fr);  /* Change 2 to 3 or 4 */
}
```

### Modify Header Color
```css
/* Line ~125 */
.column-header {
  background-color: #365523;  /* Change to desired color */
}
```

---

## File Dependencies

### Required Files (Load Order)
1. typography.css (Thai fonts)
2. bootstrap.css (Framework base)
3. style.css (Template styles)
4. **dropdown-menu.css** (This component)
5. site.css (Overrides)

### Font Families Used
- Kanit (Headers, Thai support)
- Prompt (Sub-headers, body text)
- Sarabun (Body text fallback)
- Montserrat (English fallback)

---

## Selector Reference

```css
/* Main Container */
.dropdown-menu.dropdown-menu-columns

/* Columns */
.dropdown-column:first-child   (Left - ACTIVE)
.dropdown-column:last-child    (Right - SOLD OUT)

/* Headers */
.column-header                 (ACTIVE / SOLD OUT)
.dropdown-header.sub-header    (Category headers)

/* Lists */
.projects-list                 (UL container)
.projects-list li a            (Project links)
```

---

## Quick Fixes

### Dropdown Too Tall
```css
/* Line ~54 */
height: calc(100vh - 80px);  /* Increase 80px to 100px or more */
```

### Text Too Large/Small
```css
/* Multiply all font sizes by a factor */
/* Example: 22px → 24px (1.09x) */
```

### Too Much White Space
```css
/* Reduce padding values proportionally */
/* Example: 60px → 40px, 40px → 30px */
```

### Cards Too Close
```css
/* Line ~260 */
gap: 16px;  /* Increase to 20px or 24px */
```

---

## Testing URLs

```
Desktop:    http://localhost:5000/
Hover:      "โครงการ" menu item
Mobile:     < 991px width (should hide)
```

---

## Browser DevTools

### Inspect These Elements
```
.dropdown-menu               (Container)
.dropdown-column            (Columns)
.projects-list li a         (Links)
.column-header              (Headers)
```

### Check These Properties
```
display: flex               (Columns should be side-by-side)
opacity: 1                  (Should be visible on hover)
transform: translateY(0)    (No offset when visible)
```

---

## Accessibility Checklist

- [ ] Tab navigation works
- [ ] Focus visible (2px outline)
- [ ] Color contrast ≥ 4.5:1
- [ ] Reduced motion supported
- [ ] Keyboard accessible
- [ ] Screen reader compatible

---

## Common Console Errors

**None expected** - Pure CSS implementation, no JavaScript required.

If errors appear, check:
1. CSS file loaded (Network tab)
2. No conflicting styles (Computed tab)
3. Proper selector specificity

---

## Performance Metrics

**Target Goals**:
- First Paint: < 16ms
- Animation FPS: 60fps
- Hover Response: < 100ms
- No layout shift (CLS = 0)

**Monitor**: Chrome DevTools Performance tab

---

## Version Info

**Current Version**: 1.0.0
**Last Updated**: 2025-11-25
**CSS File Size**: ~12KB (unminified)
**Selectors**: 85+
**Color Variables**: 8 unique colors

---

## Quick Commands

```bash
# Find file
ls -la /Users/horizon/Documents/dev/ppsasset_new/wwwroot/css/dropdown-menu.css

# Search for color
grep -n "#365523" dropdown-menu.css

# Count selectors
grep -c "{" dropdown-menu.css

# View in browser
open http://localhost:5000
```
