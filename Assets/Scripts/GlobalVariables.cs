using System.Collections.Generic;

public static class GlobalVariables
{
    // Variabel Global untuk Chapter dan Level
    public static string Chapter { get; set; }
    public static string Level { get; set; }
    public static string DialogueData { get; set; }


    // Variabel untuk menyimpan semua data dialog spesifik
    private static List<Dictionary<string, object>> allDialogueDataSpecific = new List<Dictionary<string, object>>();

    // Fungsi untuk menyimpan data dialog
    public static void SetAllDialogueDataSpecific(List<Dictionary<string, object>> data)
    {
        allDialogueDataSpecific = data;
    }

    // Fungsi untuk mengambil data dialog
    public static List<Dictionary<string, object>> GetAllDialogueDataSpecific()
    {
        return allDialogueDataSpecific;
    }
}
