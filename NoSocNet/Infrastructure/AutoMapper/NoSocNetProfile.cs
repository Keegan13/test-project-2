using AutoMapper;
using NoSocNet.BLL.Models;
using NoSocNet.DAL.Models;
using NoSocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.AutoMapper
{
    public class NoSocNetProfile : Profile
    {
        public NoSocNetProfile()
        {
            CreateMap<ChatRoomDto, ChatRoom<User, string>>()
                .ForMember(x => x.Participants, opt => opt.MapFrom(g => g.UserRooms));

            CreateMap<UsersChatRoomsDto, User>()
                .ConstructUsing(x => x.User);


            CreateMap<Message<User, string>, MessageDto>()
                .ForMember(x => x.Sender, opt => opt.MapFrom(g => g.Sender))
                .ForMember(x => x.ChatRoom, opt => opt.MapFrom(e => e.ChatRoom));

            CreateMap<Message<User, string>, MessageViewModel>()
                .ForMember(x => x.SenderUserName, opt => opt.MapFrom(f => f.Sender.UserName));

            CreateMap<User, UserViewModel>();
            // Use CreateMap... Etc.. here (Profile methods are the same as configuration methods)
        }
    }
}
