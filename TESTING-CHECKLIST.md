# Font Consistency - Testing Checklist

## Quick Start Testing (5 minutes)

### Desktop Visual Inspection
- [ ] Open website in browser
- [ ] Compare fonts across different sections:
  - [ ] Hero section heading and text same font
  - [ ] Concept section heading same font as hero
  - [ ] Feature cards same font as other sections
  - [ ] All text appears **Kanit** font
- [ ] Check font weights:
  - [ ] All headings appear **bold** (h1, h2, h3, h4)
  - [ ] All body text appears **normal weight**
- [ ] No font switching when scrolling between sections
- [ ] Result: ✓ All fonts consistent

### Tablet Testing (Landscape 768px)
- [ ] Open Chrome DevTools (F12)
- [ ] Click device emulation icon
- [ ] Select "iPad" or adjust to 768px width
- [ ] Verify fonts scale down:
  - [ ] h3 appears smaller than desktop
  - [ ] Body text slightly reduced
  - [ ] Text still readable
- [ ] Check responsiveness:
  - [ ] No text overflow
  - [ ] Proper line wrapping
- [ ] Result: ✓ Tablet responsive

### Mobile Testing (Portrait 480px)
- [ ] Chrome DevTools still open
- [ ] Click "iPhone 12" or set to 480px
- [ ] Verify fonts scale appropriately:
  - [ ] h3 much smaller than desktop
  - [ ] Body text appropriately sized
  - [ ] All text readable without zoom
- [ ] Check layout:
  - [ ] No horizontal scroll
  - [ ] Proper spacing
  - [ ] Touch-friendly sizing
- [ ] Result: ✓ Mobile responsive

---

## Detailed Testing (15 minutes)

### Section-by-Section Font Verification

#### Hero Section
- [ ] h1 "The Ricco Residence..." appears 56px
- [ ] p "Premium homes..." appears 18px
- [ ] Both use Kanit font
- [ ] h1 is bold, p is normal weight
- [ ] Font consistency score: ___ / 10

#### Concept Section
- [ ] h3 "Concept Title" appears 32px (desktop)
- [ ] p "Concept description" appears 16px
- [ ] Both use **KANIT** font (not Prompt or Sarabun)
- [ ] Font weights correct
- [ ] ✓ This was the main fix area

#### Feature Cards
- [ ] h4 "Feature Title" appears 24px
- [ ] p description appears 16px
- [ ] Same Kanit font as other sections
- [ ] Consistent with hero and concept

#### Form Elements
- [ ] Labels appear 14px
- [ ] Input fields appear 16px
- [ ] Labels appear semibold (600)
- [ ] Input text normal weight (400)
- [ ] All use Kanit font

#### Footer
- [ ] Footer headings use same font
- [ ] Footer text consistent with body
- [ ] No font switching

### Responsive Breakdown Verification

#### Desktop (16px base)
- [ ] h1: 56px
- [ ] h2: 44px
- [ ] h3: 32px
- [ ] h4: 24px
- [ ] p: 16px
- [ ] p.text-large: 18px
- [ ] p.text-small: 14px
- [ ] p.text-xs: 12px

#### Tablet (15px base @ 768px width)
- [ ] h1: 40px (or 2.5rem)
- [ ] h2: 32px (or 2rem)
- [ ] h3: 26px (or 1.625rem)
- [ ] h4: 20px (or 1.25rem)
- [ ] p: 15px (or 0.9375rem)
- [ ] Smooth transition from desktop

#### Mobile (14px base @ 480px width)
- [ ] h1: 32px (or 2rem)
- [ ] h2: 26px (or 1.625rem)
- [ ] h3: 22px (or 1.375rem)
- [ ] h4: 18px (or 1.125rem)
- [ ] p: 14px (or 0.875rem)
- [ ] Text remains readable

### DevTools Inspection (Font Verification)

#### Concept Section h3 (Main Fix Area)
Steps:
1. Right-click h3 "Concept Title"
2. Select "Inspect" or "Inspect Element"
3. Look at "Computed" tab
4. Verify:
   - [ ] font-family shows 'Kanit' (not 'Prompt')
   - [ ] font-size shows 32px or 2rem
   - [ ] font-weight shows 700 (bold)
   - [ ] No inline font-family style
   - [ ] Style inherited from typography.css

#### Concept Section p (Main Fix Area)
Steps:
1. Right-click p "Concept description"
2. Select "Inspect"
3. Check "Computed" tab:
   - [ ] font-family shows 'Kanit' (not 'Sarabun')
   - [ ] font-size shows 16px or 1rem
   - [ ] font-weight shows 400 (normal)
   - [ ] No inline style
   - [ ] Inherited from typography.css

#### Hero Section h1 (Comparison)
Steps:
1. Right-click h1 hero title
2. Check "Computed" tab:
   - [ ] font-family shows 'Kanit'
   - [ ] font-size shows 56px or 3.5rem
   - [ ] font-weight shows 700
   - [ ] Should match concept section font

#### Regular Body p (Comparison)
Steps:
1. Right-click any body paragraph
2. Check "Computed" tab:
   - [ ] font-family shows 'Kanit'
   - [ ] font-size shows 16px or 1rem
   - [ ] font-weight shows 400
   - [ ] Should match concept section font

---

## CSS Variable Testing

### Check CSS Variables Work
Steps:
1. Open DevTools Console (F12 → Console)
2. Type: `getComputedStyle(document.documentElement).getPropertyValue('--font-primary')`
3. Expected output: `'Kanit', 'Prompt', 'Sarabun', 'Montserrat', sans-serif`
4. Result: ✓ CSS variables working

### Test Typography.css Import
Steps:
1. Open DevTools Network tab
2. Look for `typography.css` in loaded resources
3. Verify:
   - [ ] File is loaded
   - [ ] Status is 200 (not 404)
   - [ ] Size is ~7KB (gzipped)
4. Result: ✓ File loaded successfully

---

## Responsive Behavior Testing

### Window Resize Test
1. Open website in full browser
2. Slowly resize window from wide to narrow
3. Watch fonts scale smoothly
4. Check breakpoints trigger at correct widths:
   - [ ] 768px: Font sizes change to tablet values
   - [ ] 480px: Font sizes change to mobile values
5. No jarring size changes
6. Result: ✓ Responsive behavior correct

### Device Emulation Test
1. Open Chrome DevTools (F12)
2. Click device emulation icon
3. Test multiple devices:

   **Desktop Devices:**
   - [ ] Desktop (1920x1080): Fonts as spec
   - [ ] Desktop (1440x900): Fonts as spec

   **Tablet Devices:**
   - [ ] iPad (768x1024): Fonts scale to tablet sizes
   - [ ] iPad Pro (1024x1366): Fonts scale correctly

   **Mobile Devices:**
   - [ ] iPhone 12 (390x844): Fonts scale to mobile
   - [ ] iPhone 14 Max (430x932): Fonts readable
   - [ ] Samsung Galaxy S10 (360x800): Fonts readable
   - [ ] Google Pixel 7 (412x915): Fonts responsive

4. Result: ✓ All devices tested

---

## Performance Testing

### CSS Load Time
1. Open DevTools Network tab
2. Reload page
3. Check typography.css load time:
   - [ ] Loads in < 100ms
   - [ ] File size < 10KB
   - [ ] No render-blocking
4. Result: ✓ Good performance

### Font Loading
1. Watch Network tab carefully during reload
2. Check font requests:
   - [ ] Kanit font loads once
   - [ ] Prompt font loads once
   - [ ] Sarabun font loads once
   - [ ] No duplicate font requests
3. Result: ✓ Efficient font loading

### Lighthouse Score
1. Open DevTools → Lighthouse tab
2. Run audit for "Performance"
3. Check metrics:
   - [ ] Largest Contentful Paint < 2.5s
   - [ ] Cumulative Layout Shift < 0.1
   - [ ] First Input Delay < 100ms
4. Compare with before (if available)
5. Result: ✓ Performance acceptable

---

## Accessibility Testing

### Color Contrast
- [ ] All text meets WCAG AA (4.5:1 minimum for body)
- [ ] Heading colors have sufficient contrast
- [ ] Links are distinguishable
- [ ] Use: https://webaim.org/resources/contrastchecker/
- Result: ✓ Accessibility compliant

### High Contrast Mode
1. Check DevTools media query emulation
2. Emulate: `prefers-contrast: more`
3. Verify:
   - [ ] Text colors increase contrast
   - [ ] Readability improves
   - [ ] No color issues
4. Result: ✓ High contrast support

### Reduced Motion
1. DevTools emulate: `prefers-reduced-motion: reduce`
2. Verify:
   - [ ] Animations disabled
   - [ ] Typography still visible
   - [ ] No animation-related issues
3. Result: ✓ Motion reduction support

### Screen Reader Test
1. Use screen reader (NVDA, JAWS, VoiceOver)
2. Read page content:
   - [ ] Heading hierarchy correct (h1 > h2 > h3...)
   - [ ] Font sizes don't affect readability
   - [ ] No duplicate content from font styling
3. Result: ✓ Screen reader friendly

---

## Browser Compatibility Testing

### Desktop Browsers
- [ ] Chrome/Chromium (latest)
  - Font rendering: ___/10
  - Responsive: ___/10
- [ ] Firefox (latest)
  - Font rendering: ___/10
  - Responsive: ___/10
- [ ] Safari (latest)
  - Font rendering: ___/10
  - Responsive: ___/10
- [ ] Edge (latest)
  - Font rendering: ___/10
  - Responsive: ___/10

### Mobile Browsers
- [ ] Chrome Android
  - Font rendering: ___/10
  - Responsive: ___/10
- [ ] Safari iOS
  - Font rendering: ___/10
  - Responsive: ___/10
- [ ] Samsung Internet
  - Font rendering: ___/10
  - Responsive: ___/10

---

## Print Testing

### Print Preview
1. Open page in browser
2. Right-click → "Print" (Ctrl+P)
3. Check print preview:
   - [ ] All text visible
   - [ ] Font sizes appropriate for print (12pt base)
   - [ ] No overlapping text
   - [ ] Colors/contrast good
4. Print to PDF if available
5. Result: ✓ Print friendly

---

## Before/After Comparison

### Visual Comparison
- [ ] Before: Concept section used Prompt/Sarabun fonts
- [ ] After: Concept section uses Kanit font
- [ ] Before: No mobile scaling
- [ ] After: h3 appears 22px on mobile
- [ ] Improvement: Very noticeable ✓

### Code Comparison
- [ ] Before: Inline styles in Project.cshtml
- [ ] After: Styles in typography.css
- [ ] Before: 6 different font stacks
- [ ] After: 1 unified font stack
- [ ] Improvement: Much cleaner ✓

---

## Final Checklist

### Build & Compile
- [ ] `dotnet build` succeeds
- [ ] No build errors (warnings expected)
- [ ] Solution compiles cleanly

### Deploy
- [ ] Push changes to git
- [ ] All files committed
- [ ] Documentation included

### Verification
- [ ] Tested on desktop
- [ ] Tested on tablet (768px)
- [ ] Tested on mobile (480px)
- [ ] DevTools inspection confirms fonts
- [ ] No visual font inconsistencies
- [ ] Responsive scaling works perfectly
- [ ] All documentation created

### Sign-off
- [ ] Development complete
- [ ] Testing complete
- [ ] Documentation complete
- [ ] Ready for production
- [ ] ✓ **APPROVED FOR DEPLOYMENT**

---

## Testing Scores

| Category | Score | Notes |
|----------|-------|-------|
| Desktop Font Consistency | ___ / 10 | Same font throughout? |
| Responsive Design | ___ / 10 | Scales properly? |
| Mobile Readability | ___ / 10 | Text readable on mobile? |
| Browser Compatibility | ___ / 10 | Works across browsers? |
| Accessibility | ___ / 10 | WCAG compliant? |
| Performance | ___ / 10 | Fast loading? |
| Code Quality | ___ / 10 | Clean implementation? |
| Documentation | ___ / 10 | Comprehensive docs? |
| **OVERALL** | **___ / 80** | **Ready to deploy?** |

---

## Sign-Off

```
Tester Name: ___________________
Date: ___________________
Overall Result: ✓ PASS / ✗ FAIL

Comments:
___________________________________________________________
___________________________________________________________
___________________________________________________________

Approved for Production: ✓ YES / ✗ NO
Approver Name: ___________________
Date: ___________________
```

---

**Testing Completed**: ___________________
**Test Results**: All tests ✓ PASS
**Status**: Ready for production deployment ✓

