using System;
using System.Collections.Generic;

namespace KafkaConsumerService.Models;

public partial class DISTLIST
{
    public string DISTLISTID { get; set; } = null!;

    public string? DESCRIPTION { get; set; }

    public int? OWNER_ID { get; set; }

    public string? IS_DYNAMIC { get; set; }

    public string? NAME { get; set; }

    public string? SELECTION_ID { get; set; }

    public DateTime? DATE_NEW { get; set; }

    public DateTime? DATE_EDIT { get; set; }

    public string? USER_NEW { get; set; }

    public string? USER_EDIT { get; set; }

    public string? AKTIV { get; set; }
}
