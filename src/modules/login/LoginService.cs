using Oracle.ManagedDataAccess.Client;
using BCrypt.Net;
using static sto_api_gateway.src.modules.login.LoginDto;

namespace sto_api_gateway.src.modules.login
{
    public class LoginService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public LoginService(IConfiguration configuration, TokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }

        public RegisterResponse Register(RegisterRequest request)
        {
            string connectionString = _configuration["ConnectionStrings:OracleConnection"]
                ?? throw new Exception("Connection string não configurada.");

            using var connection = new OracleConnection(connectionString);
            connection.Open();

            ValidateDuplicateUser(connection, request);

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            string sql = @"
                INSERT INTO USERS_STO (USERNAME, EMAIL, PASSWORD_HASH, FULL_NAME, CPF, PHONE, IS_ACTIVE, CREATED_AT)
                VALUES (:USERNAME, :EMAIL, :PASSWORD_HASH, :FULL_NAME, :CPF, :PHONE, 1, SYSDATE)
                RETURNING ID INTO :ID";

            using var command = new OracleCommand(sql, connection);

            command.Parameters.Add(":USERNAME", OracleDbType.Varchar2).Value = request.Username;
            command.Parameters.Add(":EMAIL", OracleDbType.Varchar2).Value = request.Email;
            command.Parameters.Add(":PASSWORD_HASH", OracleDbType.Varchar2).Value = passwordHash;
            command.Parameters.Add(":FULL_NAME", OracleDbType.Varchar2).Value = request.FullName;
            command.Parameters.Add(":CPF", OracleDbType.Int64).Value = request.Cpf;
            command.Parameters.Add(":PHONE", OracleDbType.Int64).Value = request.Phone;
            command.Parameters.Add(":ID", OracleDbType.Int32).Direction = System.Data.ParameterDirection.Output;

            command.ExecuteNonQuery();

            int id = Convert.ToInt32(command.Parameters[":ID"].Value.ToString());

            return new RegisterResponse
            {
                Id = id,
                Message = "Usuário registrado com sucesso."
            };
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest request)
        {
            string connectionString = _configuration["ConnectionStrings:OracleConnection"]
                ?? throw new Exception("Connection string não configurada.");

            using var connection = new OracleConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT ID, USERNAME, PASSWORD_HASH, IS_ACTIVE
                FROM USERS_STO
                WHERE USERNAME = :USERNAME";

            using var command = new OracleCommand(sql, connection);
            command.Parameters.Add(":USERNAME", OracleDbType.Varchar2).Value = request.Username;

            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                throw new Exception("Usuário ou senha inválidos.");
            }

            int isActive = Convert.ToInt32(reader["IS_ACTIVE"]);

            if (isActive != 1)
            {
                throw new Exception("Usuário inativo.");
            }

            string passwordHash = reader["PASSWORD_HASH"].ToString() ?? string.Empty;

            bool passwordIsValid = BCrypt.Net.BCrypt.Verify(request.Password, passwordHash);

            if (!passwordIsValid)
            {
                throw new Exception("Usuário ou senha inválidos.");
            }

            var response = new AuthenticateResponse
            {
                Id = Convert.ToInt32(reader["ID"]),
                Token = string.Empty
            };

            response.Token = _tokenService.GenerateToken(response);

            return response;
        }

        private void ValidateDuplicateUser(OracleConnection connection, RegisterRequest request)
        {
            string sql = @"
                SELECT COUNT(1)
                FROM USERS_STO
                WHERE USERNAME = :USERNAME
                   OR EMAIL = :EMAIL
                   OR CPF = :CPF";

            using var command = new OracleCommand(sql, connection);
            command.Parameters.Add(":USERNAME", OracleDbType.Varchar2).Value = request.Username;
            command.Parameters.Add(":EMAIL", OracleDbType.Varchar2).Value = request.Email;
            command.Parameters.Add(":CPF", OracleDbType.Int64).Value = request.Cpf;

            int count = Convert.ToInt32(command.ExecuteScalar());

            if (count > 0)
            {
                throw new Exception("Já existe usuário com este nome de usuário ou e-mail.");
            }
        }
    }
}