namespace UserManagementWebApi.Models
{
    public class UserRegistrationResponse
    {
        public UserRegistrationResponse(string userName, string message)
        {
            UserName = userName;
            Message = message;
        }

        public string UserName { get; set; }
        public string Message { get; set; }
    }
}
