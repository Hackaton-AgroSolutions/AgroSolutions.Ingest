namespace AgroSolutions.Ingest.Domain.Entitites;

public class SensorClient
{
    public Guid SensorClientId { get; private set; }
    public string ClientId { get; private set; }
    public string ClientSecret { get; private set; }
    public int FieldId { get; private set; }

    public SensorClient(string clientId, string clientSecret, int fieldId)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        FieldId = fieldId;
    }

    public SensorClient(Guid sensorClientId, string clientId, string clientSecret, int fieldId)
    {
        SensorClientId = sensorClientId;
        ClientId = clientId;
        ClientSecret = clientSecret;
        FieldId = fieldId;
    }
}
