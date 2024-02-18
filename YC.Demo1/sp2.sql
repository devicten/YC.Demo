USE [pubs]
GO
/****** Object:  Table [dbo].[account]    Script Date: 2024/2/18 下午 07:44:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[account](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[account] [nvarchar](50) NULL,
	[password] [binary](512) NULL,
	[employee_id] [dbo].[empid] NOT NULL,
 CONSTRAINT [PK_account] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[account]  WITH CHECK ADD  CONSTRAINT [FK_account_employee] FOREIGN KEY([employee_id])
REFERENCES [dbo].[employee] ([emp_id])
GO
ALTER TABLE [dbo].[account] CHECK CONSTRAINT [FK_account_employee]
GO
/****** Object:  StoredProcedure [dbo].[CheckAccount]    Script Date: 2024/2/18 下午 07:44:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CheckAccount]
@Account NVARCHAR(50),		  
@Password NVARCHAR(50) = '' 
AS
BEGIN
-- EXEC [dbo].[CheckAccount] 'user01', '111'
-- EXEC [dbo].[CheckAccount] 'user01', '123456'
	SET NOCOUNT ON;
	DECLARE  @salt NVARCHAR(512) 
	SELECT @salt = dval FROM dbo.const WHERE dkey = 'salt';
	PRINT '00'
	IF NOT EXISTS(SELECT 1 FROM pubs.dbo.account WHERE account = @Account AND  [password] = HASHBYTES('SHA2_512', @Password+@salt)) 
	BEGIN
		SELECT 500 [CODE], '帳號或密碼錯誤' [MESSAGE]
		SELECT -1 [id], null [Name], null [Account]
		RETURN
	END
		
		
	PRINT '01'
	--IF @EMP_PW <> '@58995866@'
	--BEGIN
	--	IF NOT EXISTS(SELECT 1 FROM EZ9HR.DBO.PMAIM201 WHERE   PMAIM2A01=@EMP_NO  AND PMAIM2A11 COLLATE CHINESE_PRC_CS_AS = @EMP_PW)
	--	BEGIN
	--		SELECT '密碼錯誤'FLAG,''EMP_NO,''EMP_NAME,''DPT_NO,''EMAIL,''QUIT,''DPT_NAME,''Ext
	--		RETURN
	--	END
	--END

	--DECLARE @NOW datetime;
	--DECLARE @IsWhite INT;			
	--DECLARE @IsBlack INT;
	--SET @NOW = GETDATE();

	--SET @IsWhite = (SELECT COUNT(*) FROM [project].[dbo].[EMP_WHITE] WHERE EMP_NO = @EMP_NO AND (STAT = '1' OR (STAT = '2' AND @NOW BETWEEN STA_DATE AND END_DATE)));
	--SET @IsBlack = (SELECT COUNT(*) FROM [project].[dbo].[EMP_BLACK] WHERE EMP_NO = @EMP_NO AND (STAT = '1' OR (STAT = '2' AND @NOW BETWEEN STA_DATE AND END_DATE)));

	--PRINT 'IsWhite:' + CONVERT(varchar(10), @IsWhite);
	--PRINT 'IsBlack:' + CONVERT(varchar(10), @IsBlack) ;
	--IF @IsWhite > 0 AND @IsBlack > 0 
	--BEGIN
	--	SELECT '無法燈入，名單設置異常'FLAG,''EMP_NO,''EMP_NAME,''DPT_NO,''EMAIL,''QUIT,''DPT_NAME,''Ext,'' EMP_CLASS,'' HEAD_DPT_NO,'' HEAD_DPT_NAME
	--	RETURN
	--END
	--IF @IsWhite <= 0 AND @IsBlack > 0  
	--BEGIN			
	--	SELECT '無法登入，該帳號已被鎖定，請聯絡系統管理員'FLAG,''EMP_NO,''EMP_NAME,''DPT_NO,''EMAIL,''QUIT,''DPT_NAME,''Ext,'' EMP_CLASS,'' HEAD_DPT_NO,'' HEAD_DPT_NAME	
	--	RETURN
	--END
	--IF @IsBlack = 1
	--BEGIN
	--	SELECT '系統禁止您進入。'FLAG,''EMP_NO,''EMP_NAME,''DPT_NO,''EMAIL,''QUIT,''DPT_NAME,''Ext,'' EMP_CLASS,'' HEAD_DPT_NO,'' HEAD_DPT_NAME	
	--	RETURN
	--END

	--IF @IsWhite = 0 AND NOT EXISTS(SELECT 1 FROM  VIEW_EMP  WHERE EMP_NO = @EMP_NO AND QUIT = 'N')
	--BEGIN
	--	SELECT '員工已離職'FLAG,''EMP_NO,''EMP_NAME,''DPT_NO,''EMAIL,''QUIT,''DPT_NAME,''Ext,'' EMP_CLASS,'' HEAD_DPT_NO,'' HEAD_DPT_NAME	
	--	RETURN
	--END
	
		
	PRINT '02'
	SELECT 200 [CODE], '登入成功' [MESSAGE]
	PRINT '03'
	SELECT acc.id  [id],
		   emp.fname + ' ' + emp.lname [Name],
		   acc.account [Account]
	  FROM pubs.dbo.account acc
 LEFT JOIN pubs.dbo.employee emp ON emp.emp_id = acc.employee_id
     WHERE acc.account = @Account;
	 
	PRINT '04'


END
GO
