using UnityEngine;
using static GameManager;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
/*
public static class LoadSave
{
    static string path = Application.persistentDataPath;

    [System.Serializable]
    public class Progress
    {
        public int level;
        public int millState;//0:nothing, 1: knocking, 2: building, 3: nothing(done), 4: knocking(done), 5: building(done)
        public bool huntDone;
        public bool brewDone;
        public bool potatoDone;
        public bool marketDone;
        public bool startSeqDone;

        public int jokeCount;

        public Progress()
        {
            level = 0;
            millState = 0;
            huntDone = false;
            potatoDone = false;
            marketDone = false;
            startSeqDone = false;
            jokeCount = 0;
        }
    }

    [System.Serializable]
    public class CollectedHats
    {
        public bool hat1;
        //...

        public CollectedHats()
        {
            hat1 = false;
        }
    }

    [System.Serializable]
    public class Soundfile
    {
        public float[] data;

        public Soundfile() => data = new float[1];
    }

    public static void DeleteSaveFile()
    {
        if(File.Exists(path + "/ekafd/savefile.game")) File.Delete(path + "/ekafd/savefile.game");
        if(File.Exists(path + "/ekafd/savefile_hats.game")) File.Delete(path + "/ekafd/savefile_hats.game");
        if(File.Exists(path + "/ekafd/savefile_sound.game")) File.Delete(path + "/ekafd/savefile_sound.game");
    }

    public static bool SaveFileExists() => File.Exists(path + "/ekafd/savefile.game");

    public static void LoadProgress()
    {
        if (File.Exists(path + "/ekafd/savefile.game"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path + "/ekafd/savefile.game", FileMode.Open);
            progress = (Progress)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            if (!Directory.Exists(path + "/ekafd"))
                Directory.CreateDirectory(path + "/ekafd");
            progress = new Progress();
        }
    }

    public static void LoadHats()
    {
        if (File.Exists(path + "/ekafd/savefile_hats.game"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path + "/ekafd/savefile_hats.game", FileMode.Open);
            hatProgress = (CollectedHats)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            if (!Directory.Exists(path + "/ekafd"))
                Directory.CreateDirectory(path + "/ekafd");
            hatProgress = new CollectedHats();
        }
    }

    public static void LoadSound()
    {
        if (File.Exists(path + "/ekafd/savefile_sound.game"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path + "/ekafd/savefile_sound.game", FileMode.Open);
            playerCry = (Soundfile)formatter.Deserialize(stream);
            stream.Close();

            //player.GetComponent<AudioSource>().clip = AudioClip.Create("playerName", playerCry.data.Length, 1, 44100, false);
            //player.GetComponent<AudioSource>().clip.SetData(playerCry.data, 0);
        }
        else
        {
            if (!Directory.Exists(path + "/ekafd"))
                Directory.CreateDirectory(path + "/ekafd");
            playerCry = new Soundfile();
        }
    }



    public static void SaveProgress()
    {
        if (progress == null) { Debug.Log("Error: progress is nullptr"); return; }
        progress.level = difficulty;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + "/ekafd/savefile.game", FileMode.Create);
        formatter.Serialize(stream, progress);
        stream.Close();
    }

    public static void SaveHats()
    {
        if (hatProgress == null) { Debug.Log("Error: hatProgress is nullptr"); return; }
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + "/ekafd/savefile_hats.game", FileMode.Create);
        formatter.Serialize(stream, hatProgress);
        stream.Close();
    }

    public static void SaveSound()
    {
        if (playerCry == null) { Debug.Log("Error: playerCry is nullptr"); return; }
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + "/ekafd/savefile_sound.game", FileMode.Create);
        formatter.Serialize(stream, playerCry);
        stream.Close();
    }
}
*/