CREATE TABLE `ViewTemplates` (
    `Name` VARCHAR(64) NOT NULL, -- 模板名称，最大长度64个字符
    `Description` TEXT, -- 模板的描述信息（可选）
    PRIMARY KEY (`Name`) -- 模板名称是唯一的主键
);
CREATE TABLE `ViewAutomation` (
    `TemplateName` VARCHAR(64) NOT NULL, -- 模板名称，最大长度64个字符
    `Type` INT NOT NULL, -- 自动化类型，使用`LedgerViewAutomationType`枚举的值（Daily, Weekly, Monthly, Quarterly, Yearly）
    PRIMARY KEY (`TemplateName`, `Type`), -- 使用TemplateName和Type作为联合主键，确保唯一性
    CONSTRAINT `FK_ViewAutomation_ViewTemplates` FOREIGN KEY (`TemplateName`) REFERENCES `ViewTemplates`(`Name`) -- 外键，引用 `ViewTemplates` 表
);
CREATE TABLE `Views` (
    `Name` VARCHAR(64) NOT NULL,  -- 视图名称，最大长度64个字符
    `TemplateName` VARCHAR(64) NOT NULL, -- 模板名称，最大长度64个字符
    `StartTime` DATETIME NOT NULL, -- 视图的开始时间
    `EndTime` DATETIME NOT NULL, -- 视图的结束时间
    `CreateTime` DATETIME NOT NULL, -- 视图的创建时间
    PRIMARY KEY (`Name`), -- 视图名称是唯一标识
    CONSTRAINT `FK_Views_ViewTemplates` FOREIGN KEY (`TemplateName`) REFERENCES `ViewTemplates`(`Name`) -- 外键，引用 `ViewTemplates` 表
);

CREATE INDEX idx_Views_TemplateName ON `Views` (`TemplateName`);
