namespace BookCatalog.Domain.Entities
{
    public class User
    {
        public User()
        {
            Role = "User"; // Установка значения по умолчанию в конструкторе
        }

        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
