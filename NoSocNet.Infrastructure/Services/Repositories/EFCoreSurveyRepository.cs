using Microsoft.EntityFrameworkCore;
using NoSocNet.Core.Interfaces.Repositories;
using NoSocNet.Domain.Models;
using NoSocNet.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Repositories
{
    public class EFCoreQuestionRepository : EFCoreRepositoryBase<QuestionEntity, int>, IQuestionRepository
    {
        public EFCoreQuestionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<ICollection<QuestionEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10)
        {
            var data = await this.context.Set<QuestionEntity>()
                .Include(x => x.Options)
                .Include(x => x.Survey)
                .Where(x => x.Text.Contains(keywords))
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return data;
        }
    }
    public class EFCoreQuestionResultRepository : EFCoreRepositoryBase<QuestionResultEntity, int>, IQuestionResultRepository
    {
        public EFCoreQuestionResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<ICollection<QuestionResultEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10)
        {
            var data = await this.context.Set<QuestionResultEntity>()
                .Include(x => x.Question)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return data;
        }
    }
    public class EFCoreSurveyUserResultRepository : EFCoreRepositoryBase<SurveyUserResultEntity, int>, ISurveyUserResultRepository
    {
        public EFCoreSurveyUserResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Task<ICollection<SurveyUserResultEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10)
        {
            throw new NotImplementedException();
        }
    }
    public class EFCoreSurveyInstanceRepository : EFCoreRepositoryBase<SurveyInstanceEntity, int>, ISurveyInstanceRepository
    {
        public EFCoreSurveyInstanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Task<ICollection<SurveyInstanceEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10)
        {
            throw new NotImplementedException();
        }
    }
    public class EFCoreOptionRepository : EFCoreRepositoryBase<OptionEntity, int>, IOptionRepository
    {
        public EFCoreOptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Task<ICollection<OptionEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10)
        {
            throw new NotImplementedException();
        }
    }
    public class EFCoreSurveyRepository : EFCoreRepositoryBase<SurveyEntity, int>, ISurveyRepository
    {
        public EFCoreSurveyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Task<ICollection<SurveyEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10)
        {
            throw new NotImplementedException();
        }
    }
}
