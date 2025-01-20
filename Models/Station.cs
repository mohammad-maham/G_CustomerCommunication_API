using System;
using System.Collections.Generic;

namespace G_CustomerCommunication_API.Models;

public partial class Station
{
    public int Id { get; set; }

    public string Caption { get; set; } = null!;

    public short Status { get; set; }
}
