using System.Net;

namespace BoredWeb.Models;

public static class CommonResponses
{
    public const string InternalServerErrorResponseMessage = "Something bad happened, try again later";
    private const string FailedDependencyErrorResponseMessage = "An error occured, try again later";
    private const string DefaultOkResponseMessage = "Success";
    private const string DefaultNotFoundResponseMessage = "Not found";
    private const string DefaultCreatedResponseMessage = "Created successfully";
    private const string DefaultBadRequestResponseMessage = "Bad request";

    public static class ErrorResponse
    {
       
    }
}