using AutoMapper;
using NoSocNet.Core.Models;
using NoSocNet.Domain.Models;
using NoSocNet.Models;
using System.Linq;


namespace NoSocNet.AutoMapper
{
    public class UIProfile : Profile
    {
        public UIProfile()
        {
            CreateMap<ChatRoomEntity, ChatRoomViewModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(o => o.Id.ToLower()))
                .ForMember(x => x.Owner, opt => opt.MapFrom(o => o.OwnerUser))
                .ForMember(x => x.OwnerId, opt => opt.MapFrom(o => o.OwnerUserId.ToLower()))
                .ForMember(x => x.Messages, opt => opt.MapFrom(o => o.Messages))
                .ForMember(x => x.Participants, opt => opt.MapFrom(o => o.UserRooms.Select(x => x.User).ToList()));

            CreateMap<NewChatUser, ChatJoinViewModel>()
                .ForMember(x=>x.ChatRoom,opt=>opt.MapFrom(o=>o.Room))
                .ForMember(x => x.User, opt => opt.MapFrom(o => o.User));

            CreateMap<ChatRoomDto, ChatRoomViewModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(o => o.Id.ToLower()))
                .ForMember(x => x.OwnerId, opt => opt.MapFrom(o => o.OwnerId.ToLower()));

            CreateMap<UserEntity, UserViewModel>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(o => o.Id.ToLower()))
                    .ForMember(x => x.UserName, opt => opt.MapFrom(o => o.UserName))
                    .ForMember(x => x.Email, opt => opt.MapFrom(o => o.Email))
                    .ForMember(x => x.EmailConfirmed, opt => opt.MapFrom(o => (o as UserEntity) != null ? (o as UserEntity).EmailConfirmed : false))
                    .ForMember(x => x.LockedOut, opt => opt.MapFrom(o => (o as UserEntity) != null ? (o as UserEntity).LockoutEnd.HasValue : false))
                    .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(o => (o as UserEntity) != null ? (o as UserEntity).PhoneNumber : ""));

            CreateMap<UserDto, UserViewModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(o => o.Id.ToLower()))
                .ForMember(x => x.LockedOut, opt => opt.Ignore())
                .ForMember(x => x.PhoneNumber, opt => opt.Ignore())
                .ForMember(x => x.EmailConfirmed, opt => opt.Ignore());



            CreateMap<MessageEntity, MessageViewModel>()
                .ForMember(x => x.ChatRoomId, opt => opt.MapFrom(o => o.ChatRoomId.ToLower()))
                .ForMember(x => x.SenderId, opt => opt.MapFrom(o => o.SenderUserId.ToLower()))
                .ForMember(x => x.SenderUserName, opt => opt.MapFrom(x => string.Format("{0}", x.SenderUser.UserName)));

            CreateMap<MessageDto, MessageViewModel>()
                .ForMember(x => x.ChatRoomId, opt => opt.MapFrom(o => o.ChatRoomId.ToLower()))
                .ForMember(x => x.SenderId, opt => opt.MapFrom(o => o.Sender.Id.ToLower()))
                .ForMember(x => x.SenderUserName, opt => opt.MapFrom(o => o.Sender.UserName));
        }
    }
}
