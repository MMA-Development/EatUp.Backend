namespace EatUp.Meals.Extensions
{
    public static class ObjectExtensions
    {
        public static void IfNotNull(this object? value, Action func)
        {
            if (value != null) func();
        }
    }

}
