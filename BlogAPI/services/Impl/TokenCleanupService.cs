using BlogAPI.Data;
using BlogAPI.services.Interfaces;

namespace BlogAPI.services.Impl;

public class TokenCleanupService : ITokenCleanupService, IHostedService, IAsyncDisposable
{
    private readonly ApplicationDbContext _db;

    private Timer _timer = null!;

    
    public TokenCleanupService(IServiceProvider serviceProvider)
    {
        _db = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
    
    
    public void RemoveExpiredRefreshTokensAsync(object? state)
    {
        var expiredTokens = _db.RefreshTokens.AsEnumerable().Where(t => t.IsExpired).ToList();

        if (!expiredTokens.Any()) return;
        _db.RefreshTokens.RemoveRange(expiredTokens);
        _db.SaveChanges();

    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(RemoveExpiredRefreshTokensAsync, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _timer.DisposeAsync();
        await _db.DisposeAsync();
    }
}