namespace sto_api_gateway.src.modules.users
{
    public static class UsersDto
    {
        public class UserRequest
        {
            public int Id { get; set; }
        }

        public class UserResponse
        {
            public string Email { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Cpf { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
        }
    }
}