#!/bin/bash

# ==============================================================================
# PPS Asset Registration Automation Tester (macOS Native AppleScript)
# Generates unique FirstName and TelNo on each run to avoid duplicate validation
# ==============================================================================

# Array of URLs to test (Active projects)
urls=(
  "https://ppsasset.com/singlehouse/thericcoresidenceprime/wongwaenhathairat#register"
  "https://ppsasset.com/singlehouse/thericcoresidenceprime/wongwaenchatuchot#register"
  "https://ppsasset.com/singlehouse/thericcoresidence/ramindrachatuchot#register"
  "https://ppsasset.com/singlehouse/thericcoresidence/ramindrahathairat#register"
  "https://ppsasset.com/townhome/thericcotown/wongwaen_lumlukka#register"
  "https://ppsasset.com/townhome/thericcotown/phahonyothin_saimai53#register"
)

# Test function for a URL
test_url() {
  local url=$1
  
  # Generate unique values
  local timestamp=$(date +%H%M%S)
  local random_phone="08$(nproc 2>/dev/null || echo '12345678' | awk -v min=10000000 -v max=99999999 'BEGIN{srand(); print int(min+rand()*(max-min+1))}')"
  # Let's override phone generation to be strictly numeric and 10 digits
  random_phone="08$((10000000 + RANDOM % 90000000))"
  local test_name="TestAgent$timestamp"
  
  echo "=============================================================================="
  echo "Testing: $url"
  echo "Generating unique credentials:"
  echo "  - FirstName: $test_name"
  echo "  - LastName:  TestAntigravity"
  echo "  - PhoneNo:   $random_phone"
  echo "=============================================================================="

  # Open in Chrome
  osascript -e "tell application \"Google Chrome\" to open location \"$url\""
  
  # Wait for page to load
  echo "Waiting 5 seconds for page load and reCAPTCHA initialization..."
  sleep 5
  
  # Fill the form using AppleScript
  osascript <<EOF
    tell application "Google Chrome"
        tell active tab of first window
            execute javascript "
                (function() {
                    const firstName = document.getElementById('firstName');
                    const lastName = document.getElementById('lastName');
                    const email = document.getElementById('email');
                    const phone = document.getElementById('phone');
                    const province = document.getElementById('province');
                    const district = document.getElementById('district');
                    
                    if (firstName) {
                        firstName.value = '$test_name';
                        firstName.dispatchEvent(new Event('input'));
                    }
                    if (lastName) {
                        lastName.value = 'TestAntigravity';
                        lastName.dispatchEvent(new Event('input'));
                    }
                    if (email) {
                        email.value = 'test-agent@ppsasset.com';
                        email.dispatchEvent(new Event('input'));
                    }
                    if (phone) {
                        phone.value = '$random_phone';
                        phone.dispatchEvent(new Event('input'));
                    }
                    
                    if (province && province.options.length > 1) {
                        province.selectedIndex = 1; 
                        province.dispatchEvent(new Event('change'));
                    }
                    if (district && district.options.length > 1) {
                        district.selectedIndex = 1;
                        district.dispatchEvent(new Event('change'));
                    }
                    console.log('Fields filled by automation script.');
                })();
            "
        end tell
    end tell
EOF
  
  echo "Form has been automatically filled."
  echo "1. Verify the fields in Google Chrome."
  echo "2. Solve reCAPTCHA and click 'Register Now' (Submit) button in Chrome."
  echo "3. Press [Enter] here to close this tab and proceed to the next project..."
  read
  
  # Close active tab
  osascript -e 'tell application "Google Chrome" to close active tab of first window'
}

# Main Execution
echo "Starting PPS Asset Registration Tests..."
echo "Please make sure Google Chrome is open and 'Allow JavaScript from Apple Events' is enabled."
echo "Enable it at: Chrome -> Developer -> Allow JavaScript from Apple Events"
echo "Press [Enter] to begin..."
read

for url in "${urls[@]}"; do
  test_url "$url"
done

echo "All tests complete!"
