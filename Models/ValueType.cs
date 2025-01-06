using System;
using System.Collections.Generic;

namespace G_CustomerCommunication_API.Models;

public partial class ValueType
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Caption { get; set; }
}
