using AgroSolutions.Ingest.Domain.Entitites;
using AgroSolutions.Ingest.Domain.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AgroSolutions.Ingest.Infrastructure.Services;

public class SensorClientAuthService(IConfiguration configuration) : ISensorClientAuthService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(SensorClient sensorClient)
    {
        string issuer = _configuration["Jwt:Issuer"]!;
        string audience = _configuration["Jwt:Audience"]!;
        string key = _configuration["Jwt:Secret"]!;

        Log.Information("Creating the token for the sensor client with ID {SensorClientId}.", sensorClient.SensorClientId);
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(key));
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Audience = audience,
            Issuer = issuer,
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature),
            Subject = new([
                new(nameof(sensorClient.SensorClientId), sensorClient.SensorClientId.ToString()),
                new(nameof(sensorClient.FieldId), sensorClient.FieldId.ToString())
            ])
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
