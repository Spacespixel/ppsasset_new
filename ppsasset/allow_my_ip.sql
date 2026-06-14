-- Run this on your Production Server (103.13.231.222) via MySQL Workbench or CLI
-- This will allow your local IP (49.228.239.91) to connect remotely

GRANT ALL PRIVILEGES ON *.* TO 'root'@'49.228.239.91' IDENTIFIED BY 'Hor$1z0n#25' WITH GRANT OPTION;
FLUSH PRIVILEGES;

-- Verify it worked
SELECT user, host FROM mysql.user WHERE user = 'root';
