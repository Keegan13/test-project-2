	use TestProjectDb;
	go

  update dbo.UsersRooms set UserId=LOWER(UserId), ChatRoomId=LOWER(ChatRoomId)

  update dbo.MessagesReadBy set UserId=LOWER(UserId);

  update dbo.Messages set SenderUserId=LOWER(SenderUserId),ChatRoomId=LOWER(ChatRoomId)

  update dbo.ChatRooms set OwnerUserId=LOWER(OwnerUserId),Id=LOWER(Id)

  update dbo.AspNetUsers set Id=LOWER(Id)