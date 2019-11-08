using NoSocNet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.Core.Interfaces.Repositories
{
    public interface ISurveyRepository : IRepository<SurveyEntity, int>
    {

    }

    public interface IQuestionRepository : IRepository<QuestionEntity, int>
    {

    }

    public interface IOptionRepository : IRepository<OptionEntity, int>
    {

    }

    public interface ISurveyInstanceRepository : IRepository<SurveyInstanceEntity, int>
    {

    }

    public interface ISurveyUserResultRepository : IRepository<SurveyUserResultEntity, int>
    {

    }

    public interface IQuestionResultRepository : IRepository<QuestionResultEntity, int>
    {

    }
}
