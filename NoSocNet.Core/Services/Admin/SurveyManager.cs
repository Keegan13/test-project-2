using AutoMapper;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Interfaces.Repositories;
using NoSocNet.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.Core.Services.Admin
{
    public class SurveyManager
    {
        private readonly ISurveyRepository surveyRepository;
        private readonly IOptionRepository optionRepository;
        private readonly ISurveyInstanceRepository surveyInstanceRepository;
        private readonly ISurveyUserResultRepository surveyUserResultRepository;
        private readonly IQuestionRepository questionRepository;
        private readonly IQuestionResultRepository questionResultRepository;
        private readonly IMapper mapper;
        private readonly IIdentityService identity;
        public SurveyManager(
            ISurveyRepository surveyRepository,
            IOptionRepository optionRepository,
            ISurveyInstanceRepository surveyInstanceRepository,
            ISurveyUserResultRepository surveyUserResultRepository,
            IQuestionRepository questionRepository,
            IQuestionResultRepository questionResultRepository,
            IMapper mapper,
            IIdentityService identity
            )
        {

            this.surveyRepository = surveyRepository;
            this.optionRepository = optionRepository;
            this.surveyInstanceRepository = surveyInstanceRepository;
            this.surveyUserResultRepository = surveyUserResultRepository;
            this.questionRepository = questionRepository;
            this.questionResultRepository = questionResultRepository;
            this.mapper = mapper;
            this.identity = identity;
        }


    }
}
