
using MySqlConnector;

namespace QuizApp_Razor.Database;
public class Connector : IConnector {
    private IConfiguration _configuration;
    private MySqlConnection _connection;
    public Connector(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("Default");
        _connection = new MySqlConnection(configuration.GetConnectionString("Default"));
    }

    public MySqlConnection GetConnection()
    {
        return _connection;
    }
}