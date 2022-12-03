namespace AvatarController.Infrastructure
{
    public static class Math
    {
        /// <summary>
        /// Normalizes in the bounds specified
        /// </summary>
        public static float Normalize(float input, float maxValue, float minValue, float upperBounds, float lowerBounds)
        {
            return ((upperBounds - lowerBounds) * ((input - minValue) / (maxValue - minValue)) + lowerBounds);
        }

        public static double Normalize(double input, double maxValue, double minValue, double upperBounds, double lowerBounds)
        {
            return ((upperBounds - lowerBounds) * ((input - minValue) / (maxValue - minValue)) + lowerBounds);
        }

        /// <summary>
        /// Normalizes in -1 to 1
        /// </summary>
        public static float Normalize(float input, float maxValue, float minValue)
        {
            return Normalize(input, maxValue, minValue, 1f, -1f);
        }

        public static double Normalize(double input, double maxValue, double minValue)
        {
            return Normalize(input, maxValue, minValue, 1f, -1f);
        }
    }
}
