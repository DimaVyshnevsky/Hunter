using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [HideInInspector]
    public AudioSource source;
    public string name;
    public AudioClip clip;
    public bool loop;
    [Range(0, 1)]
    public float volume;
}

public enum GameClips
{
    _EngineDriving,
    _EngineIdle,
    _ExplosionRocket,
    _GunMachine,
    _Rocket_Fire,
    _Tank_Explosion,
    _Zombie_1,
    _Zombie_2,
    _Zombie_3
}

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Sound[] allSounds;

    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (var item in allSounds)
        {
            item.name = item.clip.name;
            item.source = gameObject.AddComponent<AudioSource>();

            item.source.clip = item.clip;
            item.source.volume = item.volume;
            item.source.loop = item.loop;
        }
    }

    #region Interface

    public void Play(GameClips clip)
    {
        Play(clip.ToString());
    }

    public void Stop(GameClips clip)
    {
        Stop(clip.ToString());
    }

    public void Pause(GameClips clip, bool pause)
    {
        Pause(clip.ToString(), pause);
    }

    public void Play(string name)
    {
        Sound temp = System.Array.Find(allSounds, clip => clip.name.Equals(name));
        if (temp == null)
        {
            print("Current clip doesn't exists!");
            return;
        }
        temp.source.Play();
    }

    public void Stop(string name)
    {
        Sound temp = System.Array.Find(allSounds, clip => clip.name.Equals(name));
        if (temp == null)
        {
            print("Current clip doesn't exists!");
            return;
        }
        temp.source.Stop();
    }

    public void Pause(string name, bool pause)
    {
        Sound temp = System.Array.Find(allSounds, clip => clip.name.Equals(name));
        if (temp == null)
        {
            print("Current clip doesn't exists!");
            return;
        }

        if (pause && temp.source.isPlaying)
            temp.source.Pause();
        if(!pause && temp.source.isActiveAndEnabled)
            temp.source.UnPause();
    }

    #endregion
}
