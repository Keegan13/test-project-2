

;WITH cte AS
(  --there are easier ways to build a numbers table
   SELECT
       ROW_NUMBER() OVER (ORDER BY (select 0)) AS rn
   FROM
      sys.columns c1 CROSS JOIN sys.columns c2 CROSS JOIN sys.columns c3
)
INSERT INTO ChatRooms ([Value])
OUTPUT INSERTED.ID INTO T2  -- direct insert to T2
SELECT RAND(CHECKSUM(NEWID()))
FROM cte
WHERE rn <= @N;


INSERT INTO ChatRooms 

INSERT INTO  ( column1, column2, someInt, someVarChar )
SELECT  table2.column1, table2.column2, 8, 'some string etc.'
FROM    table2
WHERE   table2.ID = 7;





select top 15 Id from AspNetUsers order by NEWID();


declare @N int;
declare @row int;
declare @id uniqueidentifier;
set @row = 1
set @N=1000
while (@row <= @N)
begin

set @id= NEWID();
   insert into ChatRooms (id,isPrivate)
   values (CONVERT(varchar(255), @id),1);
   
   set @row = @row + 1;
end
   
   select COUNT(*) from ChatRooms

   
declare @N int;
declare @row int;

set @row = 1
set @N=
while (@row <= @N)
begin

set @id= NEWID();
   insert into ChatRooms (id,isPrivate)
   values (CONVERT(varchar(255), @id),1);
   
   set @row = @row + 1;
end
   
   select COUNT(*) from ChatRooms




insert into  select * from ChatRooms;

insert into UsersRooms select top 2 CONVERT(varchar(255), @id) as ChatRoomId,AspNetUsers.Id as UserId from AspNetUsers order by NEWID();

select top 1 * from ChatRooms

select top 1 * from UsersRooms


declare @id uniqueidentifier
set @id= NEWID();
Print(@id);
Print(CONVERT(varchar(255),@id));

   insert into ChatRooms (id,isPrivate)
   values (CONVERT(varchar(255), @id),1);

   
  insert into UsersRooms select top 2 CONVERT(varchar(255), @id) as ChatRoomId,AspNetUsers.Id as UserId from AspNetUsers order by NEWID();
insert into UsersRooms 

select 
(select top 1 Id from ChatRooms ORDER BY NEWID()) as ChatRoomId,Id as UserId 
from AspNetUsers

select count(*) from UsersRooms ;


select ChatRoomId,count(*) as UserCount from UsersRooms group by UsersRooms.ChatRoomId order by UserCount DESC


select * from Messages


insert into Messages 
(Text,ChatRoomId,SenderUserId,SendDate)




select 'Hellow my firend' as [Text],CC.ChatRoomId,CC.SenderUserId, GETDATE() as SendDate from (select 
CR.Id as ChatRoomId,MIN(U.Id) as SenderUserId
from ChatRooms CR 
inner join UsersRooms UR on CR.Id=UR.ChatRoomId 
inner join AspNetUsers U on UR.UserId=U.Id 
group by CR.Id) as CC inner join AspNetUsers FAKE on 1=1;

