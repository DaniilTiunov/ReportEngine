namespace ReportEngine.App.Extensions
{
    public static class ReflectionExtensions
    {
        public static object? GetPropertyValue(this object obj, string propertyName)
        {
            if (obj == null)
                throw new NullReferenceException();

            var property = obj.GetType().GetProperty(propertyName);

            return property?.GetValue(obj);
        }

        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            if(obj == null)
                throw new NullReferenceException();

            var property = obj.GetType().GetProperty(propertyName);

            property?.SetValue(obj, value);
        }
    }
}
