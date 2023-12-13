using BlogAPI.Configurations;
using BlogAPI.Exceptions;

namespace BlogAPI.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidRefreshToken ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (ForbiddenAccessToClosedCommunityException ex)
        {
            HandleException(context, ex, StatusCodes.Status403Forbidden);
        }
        catch (InvalidTokenException ex)
        {
            HandleException(context, ex, StatusCodes.Status401Unauthorized);
        }
        catch (CommunityUserRoleNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status404NotFound);
        }
        catch (UserCommunityRoleAlreadyExistsException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (AddressElementNotFound ex)
        {
            HandleException(context, ex, StatusCodes.Status404NotFound);
        }
        catch (CommunityNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status404NotFound);
        }
        catch (PostNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status404NotFound);
        }
        catch (InvalidPaginationException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (IncorrectGenderException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (BadImageLinkException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (TagNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }

        catch (NullTokenException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (UserAlreadyExistsException ex)
        {
            HandleException(context, ex, StatusCodes.Status409Conflict);
        }
        catch (UserNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status404NotFound);
        }
        catch (ForbiddenWorkWithCommentException ex)
        {
            HandleException(context, ex, StatusCodes.Status403Forbidden);
        }
        catch(EmptyCommentException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (CommentNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status404NotFound);
        }
        catch (UserCommunityRoleNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status403Forbidden);
        }
        catch (IncorrectPhoneException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (UnauthorizedException ex)
        {
            HandleException(context, ex, StatusCodes.Status401Unauthorized);
        }
        catch (LikeNotFoundException ex)
        {

            HandleException(context, ex, StatusCodes.Status404NotFound);
        }
        catch (LikeAlreadyExistException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        
        catch (Exception ex)
        {
            HandleException(context, ex, StatusCodes.Status500InternalServerError);
        }
        
    }

    private static void HandleException(HttpContext context, Exception exception, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.WriteAsJsonAsync(new Error
        {
            Message = exception.Message,
            StatusCode = context.Response.StatusCode
        });
    }
}   