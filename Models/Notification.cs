using System;
using System.Collections.Generic;
using NodaTime;

namespace G_CustomerCommunication_API.Models;

public partial class Notification
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long NotificationLinkId { get; set; }

    public long? NotificationTemplateId { get; set; }

    public long SenderUserId { get; set; }

    public string SenderUnit { get; set; } = null!;

    public int Status { get; set; }

    public string DestinationAddress { get; set; } = null!;

    public string Body { get; set; } = null!;

    public DateTime? InsDate { get; set; }

    public string? NotificationResult { get; set; }
}
