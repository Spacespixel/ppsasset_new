# Typography Standards - Project Page

## Current Font Sizes by Section

### Concept Section (Fixed)
| Element | Desktop | Tablet | Mobile | Status |
|---------|---------|--------|--------|--------|
| Title (h3) | 56px (3.5rem) | 26px (1.625rem) | 22px (1.375rem) | ✅ Good |
| Description (p) | **20px (1.25rem)** | **17px (1.0625rem)** | **15px (0.9375rem)** | ✅ Fixed |

### Hero Section
| Element | Desktop | Notes |
|---------|---------|-------|
| Title | ~56px | Same as concept title |
| Subtitle | 20px | Matches concept description |

### Project Details Section
| Element | Desktop | Notes |
|---------|---------|-------|
| Eyebrow | 16px | `1rem` - all caps |
| Heading | 41.6px | `2.6rem` - uppercase |
| Body Text | 16px | `1rem` |

### General Body Text
| Context | Desktop | Notes |
|---------|---------|-------|
| Standard | 20px | `1.25rem` - most common |
| Small | 16px | `1rem` - secondary info |
| Large | 24px | `1.5rem` - callouts |

---

## Font Families

| Type | Font | Fallback | Usage |
|------|------|----------|-------|
| Headings | Montserrat | sans-serif | h1, h2, h3, titles |
| Body | Sarabun | sans-serif | Thai content paragraphs |
| Alternatives | Prompt | sans-serif | Fallback for Thai |

---

## Line Height Standards

| Type | Value | Usage |
|------|-------|-------|
| Headings | 1.2 | Titles, h3 |
| Body Text | 1.6 | Paragraphs |
| Compact | 1.4 | Lists, small text |

---

## Color Standards

| Element | Color | Usage |
|---------|-------|-------|
| Headings | #2d3436 | Dark gray - titles |
| Body Text | #636e72 | Medium gray - descriptions |
| Light Text | #999 | Secondary content |
| Primary CTA | `var(--primary-color)` | Buttons, links |

---

## Spacing Standards

### Concept Section
| Element | Value | Usage |
|---------|-------|-------|
| Title to Description | 20px | `margin: 0 0 20px 0` |
| Row Gap | 40px | `margin-bottom: 40px` |
| Text Section Padding | 50px | `padding: 50px` |
| Line Height | 1.6 | Paragraph readability |

### Desktop Padding
- Sections: 60px (top/bottom)
- Text boxes: 50px
- Containers: 20px side margins

### Tablet Padding
- Text boxes: 40px or 30px
- Reduced from desktop by ~20%

### Mobile Padding
- Adaptive to screen size
- Minimum 20px margins

---

## Consistency Checklist

When adding new text sections, use:

**Desktop Body Text:**
```css
font-size: 1.25rem;     /* 20px */
line-height: 1.6;       /* Space lines */
color: #636e72;         /* Medium gray */
margin: 0;              /* Remove default */
```

**Responsive Update:**
```css
@media (max-width: 768px) {
    font-size: 1.0625rem;   /* 17px */
}

@media (max-width: 480px) {
    font-size: 0.9375rem;   /* 15px */
}
```

---

## Recent Changes

### November 18, 2024
- **Fixed:** Concept section description font size
- **Changed:** 18px → 20px (desktop)
- **File:** `/Views/Home/Project.cshtml` (lines 261, 296, 306)
- **Impact:** Improved readability and consistency

---

## Known Issues / Addressed

| Issue | Status | Resolution |
|-------|--------|-----------|
| Concept section text too small | ✅ Fixed | Increased from 18px to 20px |
| Mobile text too small | ⚠️ Monitor | Increased by 1px on mobile |
| Heading vs body mismatch | ✅ Good | Proper hierarchy maintained |

---

## Testing Guidelines

When verifying typography changes:

1. **Desktop (1920x1080)**
   - Check all headings are properly sized
   - Verify paragraph text is readable
   - Confirm spacing between elements

2. **Tablet (768px)**
   - Verify media query breakpoint works
   - Check responsive font sizes
   - Ensure proper padding adjustments

3. **Mobile (480px)**
   - Verify smallest sizes don't become illegible
   - Check line lengths are appropriate
   - Confirm spacing is sufficient

---

## Tools & Resources

**Typography File Locations:**
- Main typography: `/wwwroot/css/typography.css`
- Project page styles: `/Views/Home/Project.cshtml` (inline `<style>` tag)
- Custom styles: `/wwwroot/css/style-custom.css`

**CSS Units Used:**
- `rem` - relative to root font-size (preferred)
- `px` - absolute pixels (in comments for reference)

---

## Future Improvements

Potential improvements to consider:

1. Create a centralized typography component library
2. Use CSS variables for font sizes:
   ```css
   --font-size-title: 3.5rem;
   --font-size-body: 1.25rem;
   --font-size-small: 1rem;
   ```
3. Implement automatic responsive scaling
4. Create SCSS/LESS variables for reusability

---

## Notes

- All font sizes use `rem` units for better scalability
- Comments in code show pixel equivalents for clarity
- Responsive breakpoints: 768px (tablet), 480px (mobile)
- Typography is inherited from Google Fonts + system fonts
- Thai fonts (Sarabun, Prompt) load from CDN

