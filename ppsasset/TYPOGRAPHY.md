# Typography System - PPS Asset Website

## Overview
This document defines the complete typography system for PPS Asset website, ensuring consistent font sizing, weights, and styling across all sections. It serves as the single source of truth for developers and designers.

## Font Stack

The font stack is designed for optimal legibility in both Thai and English.

### Primary Font Family
```css
font-family: 'Kanit', 'Prompt', 'Sarabun', 'Montserrat', sans-serif;
```

**Hierarchy:**
1.  **Kanit** - Primary Thai font, excellent readability.
2.  **Prompt** - Secondary Thai font, elegant alternative.
3.  **Sarabun** - Tertiary Thai font, clean and modern.
4.  **Montserrat** - English/Latin fallback.
5.  **sans-serif** - System fallback.

## Font Size Scale

Responsive font sizing is implemented using `rem` units based on a variable root font size.

### Breakpoints & Base Sizes
-   **Desktop**: 16px base
-   **Tablet** (max-width: 768px): 15px base
-   **Mobile** (max-width: 480px): 14px base

### Type Scale

| Element | Size (rem) | Desktop (16px) | Tablet (15px) | Mobile (14px) | Usage |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `h1` | 3.5 | 56px | 52.5px | 49px | Page Title |
| `h2` | 2.75 | 44px | 41.25px | 38.5px | Section Title |
| `h3` | 2 | 32px | 30px | 28px | Subsection Title |
| `h4` | 1.5 | 24px | 22.5px | 21px | Card Title |
| `h5` | 1.25 | 20px | 18.75px | 17.5px | Small Title |
| `h6` | 1 | 16px | 15px | 14px | Mini Title |
| `p` | 1 | 16px | 15px | 14px | Body Text |
| `.text-large` | 1.125 | 18px | 16.875px | 15.75px | Lead Text |
| `.text-small` | 0.875 | 14px | 13.125px | 12.25px | Helper Text |
| `.text-xs` | 0.75 | 12px | 11.25px | 10.5px | Labels/Captions |

## CSS Variables

Use these variables for consistent styling in your custom CSS.

```css
:root {
    /* Fonts */
    --font-primary: 'Kanit', 'Prompt', 'Sarabun', 'Montserrat', sans-serif;
    
    /* Weights */
    --fw-light: 300;
    --fw-normal: 400;  /* Regular body text */
    --fw-medium: 500;
    --fw-semibold: 600; /* Emphasis, Labels */
    --fw-bold: 700;    /* Headings */

    /* Line Heights */
    --lh-tight: 1.2;   /* Headings */
    --lh-normal: 1.6;  /* Body text */
    --lh-relaxed: 1.8; /* Blockquotes */
    
    /* Letter Spacing */
    --ls-tight: -0.5px;
    --ls-normal: 0;
}
```

## Utility Classes

Directly apply these classes to HTML elements.

### Font Weights
-   `.fw-light` (300)
-   `.fw-normal` (400)
-   `.fw-medium` (500)
-   `.fw-semibold` (600)
-   `.fw-bold` (700)

### Text Sizes
-   `.text-xl` (32px / 2rem)
-   `.text-lg` (24px / 1.5rem)
-   `.text-md` (16px / 1rem)
-   `.text-sm` (14px / 0.875rem)
-   `.text-xs` (12px / 0.75rem)

### Heading Utilities
-   `.heading-xl` (56px) - Forces H1 styling
-   `.heading-lg` (44px) - Forces H2 styling
-   `.heading-md` (32px) - Forces H3 styling
-   `.heading-sm` (24px) - Forces H4 styling

## Implementation Guide

### 1. File Structure
-   **CSS File**: `/wwwroot/css/typography.css`
-   **Imported In**: `/Views/Shared/_Layout.cshtml`

### 2. How to Use
Always prioritize **semantic HTML**. The typography system automatically enhances standard tags.

#### Bad Practice ❌
```html
<!-- Avoid inline styles -->
<div style="font-family: 'Kanit'; font-size: 32px; font-weight: bold;">
    Title
</div>
```

#### Good Practice ✅
```html
<!-- Use Semantic Tags - styles applied automatically -->
<h3>Title</h3>
<p>Content goes here.</p>

<!-- OR Use Utility Classes -->
<div class="heading-md">Title</div>
<p class="text-sm">Small content.</p>
```

### 3. Common Component Patterns

**Hero Section**
```html
<h1>Main Title</h1>
<p class="text-large">Hero subtitle text</p>
```

**Feature Card**
```html
<h4>Feature Title</h4>
<p>Feature description</p>
```

**Form Input**
```html
<label>Full Name</label> <!-- 14px, medium -->
<input type="text" />    <!-- 16px, normal -->
<p class="text-xs">Helper text</p>
```

## Troubleshooting

### Q: Why are fonts not updating?
-   **Browser Cache**: Try `Ctrl+F5` (Windows) or `Cmd+Shift+R` (Mac).
-   **Import Check**: Ensure `<link rel="stylesheet" href="~/css/typography.css" ... />` is in `_Layout.cshtml`.
-   **Specificity**: Check for more specific CSS rules (like IDs or inline styles) overriding your classes.

### Q: Mobile fonts look too small/big.
-   The base font size scales automatically (16px -> 15px -> 14px).
-   If specific adjustments are needed, use valid CSS media queries in your feature-specific stylesheet, using `rem` units.

***
*Last Updated: 2026-01-18*
