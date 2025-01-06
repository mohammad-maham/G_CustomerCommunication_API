using System;
using System.Collections.Generic;

namespace G_CustomerCommunication_API.Models;

public partial class NotificationLink
{
    public short Id { get; set; }

    public short NotificationType { get; set; }

    public string Name { get; set; } = null!;

    public short Status { get; set; }

    public string? Config { get; set; }
}
