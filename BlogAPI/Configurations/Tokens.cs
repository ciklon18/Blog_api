using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.Configurations;

public class Tokens
{
    public SecurityKey? AccessTokenKey { get; private set; }
    public SecurityKey? RefreshTokenKey { get; private set; }
    public TimeSpan AccessTokenExpiration { get; private set; }
    public TimeSpan RefreshTokenExpiration { get; private set; }
    
    private readonly IConfiguration _configuration;

    public Tokens(IConfiguration configuration)
    {
        _configuration = configuration;
        InitKey();
        InitExpirationTimes();
    }



    private void InitKey()
    {
        AccessTokenKey =
            new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetValue<string>("ApiSettings:accessSecret")!)
            );
        RefreshTokenKey =
            new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetValue<string>("ApiSettings:refreshSecret")!)
            );
    }
    
    private void InitExpirationTimes()
    {
        AccessTokenExpiration = TimeSpan.FromMinutes(
            _configuration.GetValue("ApiSettings:accessTokenExpirationMinutes", 15)
        );

        RefreshTokenExpiration = TimeSpan.FromDays(
            _configuration.GetValue("ApiSettings:refreshTokenExpirationDays", 7)
        );
    }
}