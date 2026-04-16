using Oracle.ManagedDataAccess.Client;
using static sto_api_gateway.src.modules.users.UsersDto;

namespace sto_api_gateway.src.modules.users
{
    public class UsersService
    {
        private readonly string _connectionString;

        public UsersService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection")!;
        }

        public UserResponse GetUser(UserRequest request)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            string sql = @"
                SELECT EMAIL, USERNAME, FULL_NAME, CPF, PHONE
                FROM USERS_STO
                WHERE ID = :ID";

            using var command = new OracleCommand(sql, connection);
            command.Parameters.Add(":ID", OracleDbType.Int32).Value = request.Id;

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new UserResponse
                {
                    Email = reader.GetString(0),
                    Username = reader.GetString(1),
                    FullName = reader.GetString(2),
                    Cpf = reader.GetString(3),
                    Phone = reader.GetString(4)
                };
            }
            else
            {
                throw new Exception("Usuário não encontrado.");
            }
        }
    }
}