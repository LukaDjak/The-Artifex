using System.Collections.Generic;
using UnityEngine;

public static class SaveAndLoad
{
    //gameData Keys
    private const string GAMES_PLAYED = "games";
    private const string KILLS = "kills";
    private const string ARTIFACTS = "artifacts";
    private const string AURA = "aura";

    //settings Keys
    private const string VOLUME = "volume";
    private const string MUSIC = "music";
    private const string MUTE = "mute";

    //artifact Keys
    private const string COLLECTED_ARTIFACTS = "CollectedArtifacts";

    //gameData
    public static GameData LoadGameData()
    {
        GameData gameData = new()
        {
            total_games_played = PlayerPrefs.GetInt(GAMES_PLAYED, 0),
            total_kills = PlayerPrefs.GetInt(KILLS, 0),
            number_of_artifacts = PlayerPrefs.GetInt(ARTIFACTS, 0),
            total_aura = PlayerPrefs.GetInt(AURA, 0)
        };
        return gameData;
    }

    public static void SaveGameData(GameData gameData)
    {
        PlayerPrefs.SetInt(GAMES_PLAYED, gameData.total_games_played);
        PlayerPrefs.SetInt(KILLS, gameData.total_kills);
        PlayerPrefs.SetInt(ARTIFACTS, gameData.number_of_artifacts);
        PlayerPrefs.SetInt(AURA, gameData.total_aura);
    }

    //settings
    public static Settings LoadSettings()
    {
        Settings settings = new()
        {
            audioVolume = PlayerPrefs.GetFloat(VOLUME, 1),
            musicVolume = PlayerPrefs.GetFloat(MUSIC, 1),
            muteAudio = PlayerPrefs.GetInt(MUTE, 0)
        };
        return settings;
    }

    public static void SaveSettings(Settings settings)
    {
        PlayerPrefs.SetFloat(VOLUME, settings.audioVolume);
        PlayerPrefs.SetFloat(MUSIC, settings.musicVolume);
        PlayerPrefs.SetInt(MUTE, settings.muteAudio);
    }

    //collected Artifacts
    public static List<string> LoadCollectedArtifacts()
    {
        string serializedData = PlayerPrefs.GetString(COLLECTED_ARTIFACTS, string.Empty);
        return string.IsNullOrEmpty(serializedData)
            ? new List<string>()
            : new List<string>(serializedData.Split(','));
    }

    public static void SaveCollectedArtifacts(List<string> collectedArtifacts)
    {
        string serializedData = string.Join(",", collectedArtifacts);
        PlayerPrefs.SetString(COLLECTED_ARTIFACTS, serializedData);
        PlayerPrefs.Save();
    }
}