using AutoMapper;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Interfaces.Repositories;
using NoSocNet.Core.Models;
using NoSocNet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.Core.Services
{
    public class SurveyService
    {
        private readonly ISurveyRepository surveyRepository;
        private readonly IIdentityService identity;
        private readonly IMapper mapper;
        public SurveyService(
            ISurveyRepository surveyRepository,
            IIdentityService identity,
            IMapper mapper
            )
        {
            this.mapper = mapper;
            this.surveyRepository = surveyRepository;
            this.identity = identity;
        }

        public async Task<List<SurveyDto>> List(string keywords, int skip = 0, int take = 25)
        {
            return (await this.surveyRepository.Search(keywords, identity.CurrentUserId, skip, take)).Select(x => mapper.Map<SurveyEntity, SurveyDto>(x)).ToList();
        }
    }
}
