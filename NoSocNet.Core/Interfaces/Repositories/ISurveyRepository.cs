using NoSocNet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.Core.Interfaces.Repositories
{
    public interface ISurveyRepository : IRepository<SurveyEntity, int>
    {
        Task<IEnumerable<SurveyEntity>> GetAvaliable();


    }

    public interface IQuestionReposiotry : IRepository<QuestionEntity, int>
    {

    }
}
