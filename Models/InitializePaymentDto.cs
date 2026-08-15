using System.Text.Json.Serialization;

namespace BoredWeb.Models;

// DTO sent from Frontend to Backend
public class InitializePaymentDto
{
    public Guid UserId { get; set; }
    public Guid ActivityId { get; set; }
    public Guid OrderId { get; set; }
}

// Model returned by Paystack API when initializing
public class PaystackInitResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaystackInitData Data { get; set; } = new();
}

public class PaystackInitData
{
    [JsonPropertyName("authorization_url")]
    public string AuthorizationUrl { get; set; } = string.Empty;

    [JsonPropertyName("access_code")]
    public string AccessCode { get; set; } = string.Empty;

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;
}