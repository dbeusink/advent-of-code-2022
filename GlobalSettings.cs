namespace advent_of_code_2022
{
    public static class GlobalSettings
    {
        public static bool EnableVisualizations { get; private set; }

        public static void LoadFromCommands(Dictionary<char, string> commands)
        {
           EnableVisualizations = commands.ContainsKey('v');
        }
    }
}