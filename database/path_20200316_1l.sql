USE [xPort3_Newish]
go

-- Standard Alter Table SQL

EXEC sp_rename 'dbo.UserDisplayPreference.PK_UserPreference','PK_UserDisplayPreference','INDEX'
go
EXEC sp_rename 'dbo.FK_UserProfile_UserPreference','FK_UserProfile_UserDisplayPreference'
go
