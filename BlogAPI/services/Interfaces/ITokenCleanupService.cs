namespace BlogAPI.services.Interfaces;

public interface ITokenCleanupService
{
    void RemoveExpiredRefreshTokensAsync(object? state);
}