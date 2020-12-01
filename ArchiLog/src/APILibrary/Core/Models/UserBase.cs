using APILibrary.core.Attributes;

namespace APILibrary.core.Models
{
    public class UserBase : ModelBase
    {
        [AuthenticateResponse]
        public string Username { get; set; }

        [NotJson]
        public string Password { get; set; }
    }
}
