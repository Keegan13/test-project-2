﻿using AutoMapper;
using NoSocNet.Core.Models;
using NoSocNet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.Infrastructure.AutoMapper
{
    public class InfrastructureProfile : Profile
    {
        public InfrastructureProfile()
        {
            #region Chat
            CreateMap<ChatRoomEntity, ChatRoomDto>()
                .ForMember(x => x.Owner, opt => opt.MapFrom(o => o.OwnerUser))
                .ForMember(x => x.OwnerId, opt => opt.MapFrom(o => o.OwnerUserId))
                .ForMember(x => x.Participants, opt => opt.MapFrom(o => o.UserRooms));

            CreateMap<UsersChatRoomsEntity, UserDto>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(o => o.User.UserName))
                .ForMember(x => x.Id, opt => opt.MapFrom(o => o.User.Id))
                .ForMember(x => x.Email, opt => opt.MapFrom(o => o.User.Email));


            CreateMap<UserEntity, UserDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(o => o.Id))
                    .ForMember(x => x.UserName, opt => opt.MapFrom(o => o.UserName))
                    .ForMember(x => x.Email, opt => opt.MapFrom(o => o.Email));


            CreateMap<MessageEntity, MessageDto>()
                .ForMember(x => x.Sender, opt => opt.MapFrom(o => o.SenderUser))
                .ForMember(x => x.ReadByUsers, opt => opt.MapFrom(x => x.ReadByUsers));

            #endregion

            #region Survey

            CreateMap<SurveyEntity, SurveyDto>()
                .ReverseMap();

            CreateMap<QuestionEntity, QuestionDto>()
                .ReverseMap();


            CreateMap<OptionEntity, OptionDto>()
                .ReverseMap();

            CreateMap<SurveyInstanceEntity, SurveyInstanceDto>()
                .ReverseMap();

            CreateMap<SurveyUserResultEntity, SurveyUserResultDto>()
                .ReverseMap();


            CreateMap<QuestionResultEntity, QuestionResultDto>()
                .ReverseMap()
                .ForMember(x => x.Question, opt => opt.Ignore())
                .ForMember(x => x.SelectedOptions, opt => opt.Ignore())
                .ForMember(x => x.SurveyUserResult, opt => opt.Ignore());

            #endregion
        }

    }
}
