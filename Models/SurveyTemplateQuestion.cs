using System;
using System.Collections.Generic;

namespace G_CustomerCommunication_API.Models;

public partial class SurveyTemplateQuestion
{
    public long Id { get; set; }

    public long SurveyTemplateId { get; set; }

    public short Status { get; set; }

    public string Question { get; set; } = null!;

    public int ValueType { get; set; }

    public List<string>? Values { get; set; }
}
