﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LostArkBot.Bot.FileObjects.LostMerchants;

public class MerchantGroup
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("server")]
    public string Server { get; set; }

    [JsonPropertyName("merchantName")]
    public string MerchantName { get; set; }

    [JsonPropertyName("activeMerchants")]
    public List<WebsiteMerchant> ActiveMerchants { get; set; }
}