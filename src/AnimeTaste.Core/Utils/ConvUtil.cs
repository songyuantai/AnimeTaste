namespace AnimeTaste.Core.Utils
{
    public class ConvUtil
    {
        public static int? AsEnumValue<T>(string value, bool ignoreCase = false) where T : struct
        {
            if (!string.IsNullOrEmpty(value) &&
                typeof(T).IsEnum &&
                Enum.TryParse(value, ignoreCase, out T result))
                return Convert.ToInt32(result);
            return null;
        }
    }
}
