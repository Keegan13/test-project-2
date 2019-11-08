using NoSocNet.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSocNet.Core.Extensions
{
    public static class SurveyModelsExtensions
    {
        public static bool IsPublic(this SurveyDto survey)
        {
            return survey.Type == Domain.Models.SurveyType.Public;
        }

        public static bool IsTimeOut(this SurveyInstanceDto survey)
        {
            return survey.EndDate.HasValue && survey.EndDate.Value >= DateTime.Now;
        }

        public static IEnumerable<OptionDto> GetNotSelectedOptions(this QuestionResultDto result)
        {
            if (result.SelectedOptions == null)
            {
                return Enumerable.Empty<OptionDto>();
            }

            return result.SelectedOptions.Where(x => !result.SelectedOptions.Select(so => so.Id).Contains(x.Id)).ToList();
        }
    }
}
