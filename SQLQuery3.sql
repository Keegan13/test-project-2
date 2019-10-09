/****** Script for SelectTopNRows command from SSMS  ******/


INSERT INTO [TestProjectDb].[dbo].[ChatRooms] (Id,IsPrivate) 
SELECT 
	[Id]=NEWID(),
	[IsPrivate]
  FROM [TestProjectDb].[dbo].[ChatRooms]


DECLARE @i INT = 1;
WHILE (@i <= 7800)
 BEGIN
  INSERT INTO [TestProjectDb].[dbo].[UsersRooms] (UserId,ChatRoomId)
(
SELECT top 1 Id as UserId, (SELECT TOP 1 Id as ChatRoomId FROM ChatRooms  as PI 
WHERE PI.Id <> P.Id order by NEWID()) 
as ChatRoomId from AspNetUsers P) 
 SET  @i = @i + 1;
END 





INSERT INTO [TestProjectDb].[dbo].[Messages] (Text, SendDate, ChatRoomId, SenderUserId)
SELECT *
FROM [TestProjectDb].[dbo].[Messages];



SELECT TOP (1000) [Id]
      ,[Text]
      ,[SendDate]
      ,[ChatRoomId]
      ,[SenderUserId]
  FROM [TestProjectDb].[dbo].[Messages]



SELECT Id as UserId, (SELECT TOP 1 Id as ChatRoomId FROM ChatRooms order by NEWID() where Chatroo) 
as ChatRoomId from AspNetUsers



DECLARE @i INT = 1;

WHILE (@i <= 78)
 BEGIN
  

      INSERT INTO Messages (SendDate,SenderUserId,[Text],ChatRoomId)
SELECT GETDATE() as SendDate, UserId as SenderUserId,'message' as [Text],ChatRoomId FROM ChatRooms
  inner join UsersRooms on ChatRoomId=Id
  ORDER BY newid()


 SET  @i = @i + 1;
END 




