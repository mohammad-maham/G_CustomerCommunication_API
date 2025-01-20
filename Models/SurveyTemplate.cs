using System;
using System.Collections.Generic;

namespace G_CustomerCommunication_API.Models;

public partial class SurveyTemplate
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public short Status { get; set; }

    public int NotificationLinkId { get; set; }

    public int? Station { get; set; }
}
