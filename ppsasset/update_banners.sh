#!/bin/bash

# Check if image path is provided
if [ -z "$1" ]; then
    echo "Usage: ./update_banners.sh <path-to-new-banner-image>"
    echo "Example: ./update_banners.sh /path/to/new-banner.jpg"
    exit 1
fi

INPUT_IMAGE="$1"

# Check if input file exists
if [ ! -f "$INPUT_IMAGE" ]; then
    echo "Error: File '$INPUT_IMAGE' not found!"
    exit 1
fi

PROJECTS_DIR="wwwroot/images/projects"

echo "Updating banner images for all projects..."

# 1. Ricco Residence Chatuchot
cp "$INPUT_IMAGE" "$PROJECTS_DIR/ricco-residence-chatuchot/Ricco-Residence-Ramintra-Chatuchot-banner.jpg"
echo "Updated: ricco-residence-chatuchot banner"

# 2. Ricco Residence Hathairat
cp "$INPUT_IMAGE" "$PROJECTS_DIR/ricco-residence-hathairat/Ricco-Residence-Ramintra-Hathairat-banner.jpg"
echo "Updated: ricco-residence-hathairat banner"

# 3. Ricco Residence Prime Chatuchot
cp "$INPUT_IMAGE" "$PROJECTS_DIR/ricco-residence-prime-chatuchot/ricco-residence-prime-chatuchot-banner.jpg"
echo "Updated: ricco-residence-prime-chatuchot banner"

# 4. Ricco Residence Prime Hathairat
cp "$INPUT_IMAGE" "$PROJECTS_DIR/ricco-residence-prime-hathairat/ricco-residence-prime-hathairat-banner.jpg"
echo "Updated: ricco-residence-prime-hathairat banner"

# 5. Ricco Town Phahonyothin Saimai 53
cp "$INPUT_IMAGE" "$PROJECTS_DIR/ricco-town-phahonyothin-saimai53/ricco-town-phahonyothin-banner.jpg"
echo "Updated: ricco-town-phahonyothin-saimai53 banner"

# 6. Ricco Town Wongwaen Lamlukka
cp "$INPUT_IMAGE" "$PROJECTS_DIR/ricco-town-wongwaen-lamlukka/ricco-town-lamlukka-banner.jpg"
echo "Updated: ricco-town-wongwaen-lamlukka banner"

echo "All banners updated successfully!"
