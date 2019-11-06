CREATE PROCEDURE dbo.recentChats
      @Count int   
    , @User varchar(100) 
	, @Exclude varchar(MAX)
AS  
    SET NOCOUNT ON;  
    SELECT *
      FROM [UsersRooms] AS [c.UserRooms]
      INNER JOIN [AspNetUsers] AS [u.User] ON [c.UserRooms].[UserId] = [u.User].[Id]
      INNER JOIN (
          SELECT DISTINCT [c0].[Id]
          FROM (
              SELECT TOP(@Count) * FROM [ChatRooms] AS CR 
			  left join [UsersRooms] as UR on UR.ChatRoomId = CR.Id  
			  where UR.UserId = @User AND CR.Id NOT IN (SELECT value FROM STRING_SPLIT(@Exclude, ','))
			  ORDER BY	(	SELECT TOP(1) SendDate FROM Messages as M 
							WHERE M.ChatRoomId = CR.Id
							ORDER BY M.Id DESC) DESC
          ) AS [c0]
          LEFT JOIN [AspNetUsers] AS [c.OwnerUser0] ON [c0].[OwnerUserId] = [c.OwnerUser0].[Id]
      ) AS [t] ON [c.UserRooms].[ChatRoomId] = [t].[Id]
GO  
