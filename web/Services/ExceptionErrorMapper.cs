
namespace HitRefresh.WebLedger.Web.Services;

public static class ExceptionErrorMapper
{
    public static (int statusCode, string errorCode, string message) Map(Exception ex)
    {
        return ex switch
        {
            TypeUndefinedException => (
                StatusCodes.Status400BadRequest,
                "TYPE_UNDEFINED",
                "The specified type does not exist."
            ),
            ViewTemplateUndefinedException => (
                StatusCodes.Status404NotFound,
                "VIEW_TEMPLATE_UNDEFINED",
                "The specified view template does not exist."
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "INTERNAL_SERVER_ERROR",
                "An unexpected error occurred."
            )
        };
    }
}
