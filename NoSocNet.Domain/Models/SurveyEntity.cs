using NoSocNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.Domain.Models
{
    public enum SurveyType
    {
        Private,
        Public
    }

    public class SurveyEntity : IIdentifiable<int>
    {
        #region PK

        public int Id { get; set; }
        #endregion

        #region FK
        #endregion

        #region Fields

        public string Name { get; set; }

        public string Description { get; set; }

        public SurveyType Type { get; set; }

        #endregion

        #region Navigational properties

        public ICollection<QuestionResultEntity> Responses { set; get; }

        public ICollection<QuestionEntity> Questions { get; set; }

        public ICollection<SurveyInstanceEntity> Instances { get; set; }
        #endregion
    }

    public class QuestionEntity : IIdentifiable<int>
    {
        #region PK

        public int Id { get; set; }

        #endregion

        #region FK
        public int SurveyId { get; set; }
        #endregion

        #region Fields
        public string Text { get; set; }

        #endregion

        #region Navigational properties
        public SurveyEntity Survey { get; set; }

        public ICollection<VariantEntity> PossibleAnswers { get; set; }

        //public ICollection<ResponseEntity> Responses { get; set; }
        #endregion
    }

    public class VariantEntity : IIdentifiable<int>
    {
        #region PK

        public int Id { get; set; }
        #endregion

        #region FK
        public int SurveyId { get; set; }
        #endregion

        #region Fields
        public string Text { get; set; }

        #endregion

        #region Navigational properties
        public SurveyEntity Survey { get; set; }

        #endregion
    }

    public class SurveyInstanceEntity : IIdentifiable<int>
    {
        #region PK

        public int Id { get; set; }

        #endregion

        #region FK
        public string OwnerUserId { get; set; }
        public int SurveyId { get; set; }
        #endregion

        #region Fields

        public DateTime CreatedDate { get; set; }

        public Nullable<DateTime> StartDate { get; set; }

        public Nullable<DateTime> EndDate { get; set; }
        #endregion

        #region Navigational properties

        public SurveyEntity Survey { get; set; }

        public UserEntity OwnerUser { get; set; }
        public ICollection<SurveyResultEntity> Results { get; set; }
        #endregion
    }


    public class SurveyResultEntity : IIdentifiable<int>
    {
        #region PK
        public int Id { get; set; }
        #endregion

        #region FK
        public int SurveyInstanceId { get; set; }
        public string ResultUserId { get; set; }
        #endregion

        #region Fields

        #endregion

        #region Navigational properties
        public UserEntity ResultUser { get; set; }
        public SurveyInstanceEntity SurveyInstance { get; set; }
        public ICollection<QuestionResultEntity> QuestionResults { get; set; }
        #endregion
        public SurveyResultEntity()
        {
            this.QuestionResults = new HashSet<QuestionResultEntity>();
        }
    }

    public class QuestionResultEntity : IIdentifiable<int>
    {
        #region PK

        public int Id { get; set; }

        #endregion

        #region FK
        public int SurveyResultId { get; set; }
        public int QuestionId { get; set; }
        #endregion

        #region Fields

        public DateTime DateGiven { get; set; }

        #endregion

        #region Navigational properties
        public QuestionEntity Question { get; set; }

        public SurveyResultEntity Result { get; set; }

        public ICollection<VariantResultEntity> Response { get; set; }
        #endregion
    }

    public class VariantResultEntity
    {
        #region PK,FK
        public int QuestionResultId { get; set; }

        public int QuestionVariantId { get; set; }
        #endregion

        #region Fields

        #endregion

        #region Navigational properties

        public VariantEntity QuestionVariant { get; set; }

        public QuestionResultEntity QuestionResult { get; set; }
        #endregion
    }
}

#region PK


#endregion

#region FK
#endregion

#region Fields

#endregion

#region Navigational properties

#endregion