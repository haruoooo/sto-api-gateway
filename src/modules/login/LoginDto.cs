namespace sto_api_gateway.src.modules.login
{
    public static class LoginDto
    {
        public class RegisterRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public long Cpf { get; set; }
            public long Phone { get; set; }
        }

        public class RegisterResponse
        {
            public int Id { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        public class AuthenticateRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class AuthenticateResponse
        {
            public int Id { get; set; }
            public string Token { get; set; } = string.Empty;
        }
    }
}