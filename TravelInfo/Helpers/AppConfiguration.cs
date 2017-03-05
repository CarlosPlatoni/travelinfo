namespace PlatzInfo.Helpers
{
    /// <summary>
    /// Static class that provides configuration for the whole application
    /// Used like an app.config
    /// </summary>
    public static class AppConfiguration
    {
        // TravelInfo Screen
        public static string TflAppId => "YOUR TFL APP ID";
        public static string TflAppKey => "YOUR TFL APP KEY";
        public static string NrToken => "YOUR NATIONAL RAIL TOKEN";

        // Holding Screen
        public static string HoldingScreenCommentLine1 => "TravelInfo";
        public static string HoldingScreenCommentLine2 => "Line 2";

        // Travel Info Screen
        public static string TravelInfoTitle => "TravelInfo";
    }
}
