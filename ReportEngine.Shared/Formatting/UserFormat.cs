using ReportEngine.Domain.Entities;

namespace ReportEngine.Shared.Formatting
{
    public class UserFormat
    {
        public static string ToStringFullName(User user)
        {
            return $"{user.Name}, {user.SecondName}";
        }
    }
}
