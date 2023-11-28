using BlogAPI.Configurations;
using BlogAPI.Entities;
using BlogAPI.Exceptions;
using Xunit.Sdk;

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
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new Error
            {
                Message = ex.Message,
                StatusCode = context.Response.StatusCode
            });
        }
        catch (InvalidTokenException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new Error
            {
                Message = ex.Message,
                StatusCode = context.Response.StatusCode
            });
        }
        catch (AddressElementNotFound ex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new Error
            {
                Message = ex.Message,
                StatusCode = context.Response.StatusCode
            });
        }
        catch (NullTokenException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new Error
            {
                Message = ex.Message,
                StatusCode = context.Response.StatusCode
            });
        }
        catch (UserAlreadyExistsException ex)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new Error
            {
                Message = ex.Message,
                StatusCode = context.Response.StatusCode
            });
        }
        catch (UserNotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new Error
            {
                Message = ex.Message,
                StatusCode = context.Response.StatusCode
            });
        }
        catch (IncorrectPhoneException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new Error
            {
                Message = ex.Message,
                StatusCode = context.Response.StatusCode
            });
        }
        catch (UnauthorizedException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new Error
            {
                Message = ex.Message,
                StatusCode =  context.Response.StatusCode
            });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new Error
            {
                Message = ex.Message,
                StatusCode =  context.Response.StatusCode
            });
        }
    } 
    
}   