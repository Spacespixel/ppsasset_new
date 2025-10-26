-- thericco.sy_project definition

CREATE TABLE `sy_project` (
  `ProjectID` varchar(15) NOT NULL,
  `ProjectName` varchar(500) DEFAULT NULL,
  `ProjectAddress` varchar(1000) DEFAULT NULL,
  `ProjectType` varchar(255) DEFAULT NULL,
  `ProjectNameEN` varchar(100) DEFAULT NULL,
  `ProjectEmail` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ProjectID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


-- thericco.tr_transaction definition

CREATE TABLE `tr_transaction` (
  `TransactoinID` varchar(15) CHARACTER SET utf8mb4 NOT NULL,
  `ProjectID` varchar(15) CHARACTER SET utf8mb4 DEFAULT NULL,
  `FirstName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
  `LastName` varchar(255) CHARACTER SET utf8mb4 DEFAULT NULL,
  `Budget` varchar(255) CHARACTER SET utf8mb4 DEFAULT NULL,
  `Province` varchar(255) CHARACTER SET utf8mb4 DEFAULT NULL,
  `ProvinceHome` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Distric` varchar(255) CHARACTER SET utf8mb4 DEFAULT NULL,
  `TelNo` varchar(45) CHARACTER SET utf8mb4 DEFAULT NULL,
  `EMail` varchar(255) CHARACTER SET utf8mb4 DEFAULT NULL,
  `HomeType` varchar(255) CHARACTER SET utf8mb4 DEFAULT NULL,
  `ClientFrom` varchar(500) CHARACTER SET utf8mb4 DEFAULT NULL,
  `TransactionDate` datetime DEFAULT NULL,
  `ProjectName` varchar(255) CHARACTER SET utf8mb4 DEFAULT NULL,
  `TempFields1` varchar(255) CHARACTER SET utf8mb4 DEFAULT NULL,
  `TempFields2` varchar(255) CHARACTER SET utf8mb4 DEFAULT NULL,
  `TempFields3` varchar(255) CHARACTER SET utf8mb4 DEFAULT NULL,
  `Remark` varchar(1000) CHARACTER SET utf8mb4 DEFAULT NULL,
  `FlagEmailSent` bit(1) DEFAULT b'0',
  `utm_source` varchar(255) CHARACTER SET utf8 DEFAULT NULL,
  `utm_medium` varchar(255) CHARACTER SET utf8 DEFAULT NULL,
  `utm_campaign` varchar(255) CHARACTER SET utf8 DEFAULT NULL,
  `utm_term` varchar(255) CHARACTER SET utf8 DEFAULT NULL,
  `utm_content` varchar(255) CHARACTER SET utf8 DEFAULT NULL,
  `AppointmentDate` datetime DEFAULT NULL,
  `AppointmentTime` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `AppointmentType` varchar(20) CHARACTER SET utf8 DEFAULT NULL,
  `ConsentMarketing` bit(1) DEFAULT NULL,
  `AuthenBy` varchar(20) CHARACTER SET utf8 DEFAULT NULL,
  `SocialID` varchar(30) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY (`TransactoinID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- thericco.sy_runningnumber definition

CREATE TABLE `sy_runningnumber` (
  `RunningNumberID` int(11) NOT NULL AUTO_INCREMENT,
  `RunningNumberDocCode` varchar(5) DEFAULT NULL,
  `RunningNumberCurrentYear` varchar(4) DEFAULT NULL,
  `RunningNumber` int(11) DEFAULT NULL,
  PRIMARY KEY (`RunningNumberID`)
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8;