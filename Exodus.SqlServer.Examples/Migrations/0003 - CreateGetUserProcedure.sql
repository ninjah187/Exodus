CREATE PROCEDURE [dbo].[User]
	@userId INT
AS
BEGIN
	SELECT * FROM Users WHERE Users.Id = @userId
END