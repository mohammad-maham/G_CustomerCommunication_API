using System;
using System.Collections.Generic;
using NodaTime;

namespace G_CustomerCommunication_API.Models;

public partial class Survey
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long SurveyTemplateId { get; set; }

    public string QuestionValues { get; set; } = null!;

    public DateTime RegDate { get; set; }
}
