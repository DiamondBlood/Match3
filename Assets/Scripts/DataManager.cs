using UnityEngine;

public static class DataManager
{
    public static void SaveHighestScore(int score)
    {
        PlayerPrefs.SetInt("HighestScore", score);
        PlayerPrefs.Save();
    }

    public static int LoadHighestScore() 
    {
        return PlayerPrefs.GetInt("HighestScore", 0);
    } 
}
