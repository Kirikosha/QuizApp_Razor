using MySqlConnector;

namespace QuizApp_Razor.Database;
public interface IConnector{
    public MySqlConnection GetConnection();
}