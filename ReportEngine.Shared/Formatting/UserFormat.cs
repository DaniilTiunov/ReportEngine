using ReportEngine.Domain.Entities;

namespace ReportEngine.Export.Formatting
{
    public class UserFormat
    {
        public static string ToStringFullName(User user)
        {
            return $"{user.Name}, {user.SecondName}";
        }

        public static string ToStringID(User user)
        {
            return user.Id.ToString();
        }
    }
}
