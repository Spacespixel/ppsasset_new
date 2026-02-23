# Category Icons - Customization Guide

This directory contains placeholder icons for the location categories section on project pages.

## Icon Files

- **category-transportation.svg** - Transportation/Bus icon
- **category-shopping.svg** - Shopping mall icon
- **category-hospital.svg** - Hospital/Medical icon
- **category-school.svg** - School/Education icon
- **category-park.svg** - Park/Recreation icon
- **category-location.svg** - Default location pin icon

## How to Customize

### Step 1: Create Your Custom Icons
You can replace any of the SVG files with your own designs. Icons should be:
- **Format**: SVG or PNG (both work)
- **Size**: Recommended 100x100px or scalable
- **Color**: Black (#000000) - will be converted to white by CSS filter
- **Style**: Minimal and clean

### Step 2: Replace the Files
Simply replace the icon file in this directory with your custom design:
```
/wwwroot/images/icons/category-[name].svg
```

### Step 3: How Icons Are Applied
The icons are automatically used based on category name matching:

1. If category name contains "การเดินทาง" or "transport" → uses `category-transportation.svg`
2. If category name contains "ห้างสรรพสินค้า" or "shopping" or "mall" → uses `category-shopping.svg`
3. If category name contains "สถานพยาบาล" or "hospital" or "medical" → uses `category-hospital.svg`
4. If category name contains "สถานศึกษา" or "school" or "education" → uses `category-school.svg`
5. If category name contains "สวน" or "park" or "tree" → uses `category-park.svg`
6. If no match → uses `category-location.svg` (default fallback)

### Step 4: CSS Filter Applied
All icons are automatically converted to white using this CSS filter:
```css
filter: brightness(0) invert(1);
```

This means:
- **Upload black icons** (or dark colored icons)
- They will automatically appear **white** on the magenta background
- If you want to control the exact color, modify the filter in `Views/Home/Project.cshtml` around line 2492

## Adding New Categories

To add icons for new categories:

1. Create a new SVG/PNG file (e.g., `category-restaurant.svg`)
2. Add a new case in the `GetCategoryIconPath()` function in `Views/Home/Project.cshtml`:
```csharp
_ when category.Contains("ร้านอาหาร") || category.Contains("restaurant") =>
    "~/images/icons/category-restaurant.svg",
```

## Icon Design Tips

- **Minimal style**: Simple shapes, avoid complex details
- **Clear recognition**: Icon should be immediately understandable
- **Consistent stroke width**: If using outlines, keep consistent
- **Square format**: Design for square aspect ratio (100x100)
- **Padding**: Leave some padding around the icon for breathing room

## Testing Your Icons

After replacing icons:
1. Rebuild the project (dotnet build)
2. Clear browser cache (Ctrl+Shift+Del or Cmd+Shift+Del)
3. Refresh the page
4. Verify icons appear white on magenta circles
