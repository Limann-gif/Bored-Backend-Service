using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BoredWeb.Data;
using BoredWeb.Models;
using BoredWeb.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BoredWeb.Controllers;

[Route("api/[controller]")]
[ApiController]

public class PaymentController: ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<PaymentController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly BoredDbContext  _boredDbContext;
    private readonly IConfiguration _configuration;

    public PaymentController(IConfiguration config, ILogger<PaymentController> logger, IUserRepository userRepository, BoredDbContext boredDbContext, IConfiguration configuration)
    {
        _config = config;
        _logger = logger;
        _userRepository = userRepository;
        _boredDbContext = boredDbContext;
        _configuration = configuration;
    }

    // Should be removed
    [HttpPost("callback")]
    public async Task<IActionResult> GetPaymentCallback(TransactionDto  transaction)
    {
        var response = await _userRepository.Callback(transaction);
        return Ok(response);
    }
    
    //ADMIN Endpoint
    [HttpGet("paymentHistory")]
    public async Task<IActionResult> FetchPaymentHistory()
    {
        var response = await _userRepository.FetchPaymentHistory();
        return Ok(response);
    }
    
    // Display payment details of user before they pay
    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> PaymentDetailsPage([FromRoute] Guid orderId)
    {
        var response = await _userRepository.PaymentDetailPage(orderId);
        return Ok(response);
    }
    
    //Initialize payment
    // [HttpGet("initialize")]
    // public async Task<IActionResult> InitializePayment(InitializePaymentDto request)
    // {
    //     var response = await _userRepository.InitializePayment(request);
    //     return Ok(response);
    // }
    
    //Paystack calls this Callback url Role: To update your database state securely step 2
    [HttpPost("webhook")]
    [AllowAnonymous] // Paystack server sends requests without authorization tokens
    public async Task<IActionResult> PaystackWebhook()
    {
        // Read raw body stream
        using var reader = new StreamReader(Request.Body);
        var jsonBody = await reader.ReadToEndAsync();

        // 1. Verify Paystack Signature
        var paystackSignature = Request.Headers["x-paystack-signature"].ToString();
        var secretKey = _configuration["Paystack:SecretKey"];

        if (!IsSignatureValid(jsonBody, paystackSignature, secretKey))
        {
            return Unauthorized();
        }

        // 2. Parse Event
        using var doc = JsonDocument.Parse(jsonBody);
        var root = doc.RootElement;
        var eventType = root.GetProperty("event").GetString();

        if (eventType == "charge.success")
        {
            var data = root.GetProperty("data");
            var reference = data.GetProperty("reference").GetString(); // Matches order.Id

            if (Guid.TryParse(reference, out var orderId))
            {
                var order = await _boredDbContext.ActivityBookingOrders.FirstOrDefaultAsync(x => x.Id == orderId);
                if (order != null && order.PaymentStatus != "paid")
                {
                    // Update Order Status
                    order.PaymentStatus = "paid";
                    order.ConfirmationStatus = "confirmed";
                    order.TransactionId = Guid.NewGuid();

                    await _boredDbContext.SaveChangesAsync();
                }
            }
        }

        return Ok(); // Paystack expects a 200 OK
    }

// HMAC-SHA512 Signature verification helper
    private bool IsSignatureValid(string body, string signature, string secretKey)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
        var expectedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
        return expectedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase);
    }
    
    // When the user clicks "Done" Used by Paystack to redirects the user's browser back to bored after they finish or cancel payment. step 3
    [HttpGet("verify/{reference}")]
    public async Task<IActionResult> VerifyPayment(string reference)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _configuration["Paystack:SecretKey"]);

        var response = await client.GetAsync($"https://api.paystack.co/transaction/verify/{reference}");
        var json = await response.Content.ReadAsStringAsync();
     

        using var doc = JsonDocument.Parse(json);
        var status = doc.RootElement.GetProperty("data").GetProperty("status").GetString();

        if (status == "success")
        {
            return Ok(new ApiResponse<string> { Code = 200, Message = "Payment confirmed successfully." });
        }

        return BadRequest(new ApiResponse<string> { Code = 400, Message = "Payment pending or failed." });
    }
    
    //Initialize payment step 1
    [HttpPost("initialize")]
    public async Task<IActionResult> InitializePayment([FromBody] InitializePaymentDto request)
    { // Helper method to get logged in user GUID
        var response = await _userRepository.InitializePaymentAsync(request);

        if (response.Code == 200)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
