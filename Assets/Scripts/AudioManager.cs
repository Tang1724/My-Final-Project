using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public List<AudioClip> soundClips; // 音效资源列表
    public AudioClip backgroundMusicClip; // 背景音乐资源

    private AudioSource backgroundMusicSource; // 动态创建的背景音乐 AudioSource
    private Dictionary<string, AudioSource> soundSources; // 音效 AudioSource 字典

    void Awake()
    {
        // 单例模式初始化
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化背景音乐 AudioSource
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource.clip = backgroundMusicClip;

        // 初始化音效 AudioSource 字典
        soundSources = new Dictionary<string, AudioSource>();
        foreach (AudioClip clip in soundClips)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            soundSources.Add(clip.name, source);
        }
    }

    // 播放音效（支持循环）
    public void PlaySound(string name, bool loop = false)
    {
        if (soundSources.ContainsKey(name))
        {
            AudioSource source = soundSources[name];
            source.loop = loop;
            source.Play();
        }
        else
        {
            Debug.LogWarning("Sound name not found: " + name);
        }
    }

    // 停止播放特定音效
    public void StopSound(string name)
    {
        if (soundSources.ContainsKey(name))
        {
            AudioSource source = soundSources[name];
            source.Stop();
        }
        else
        {
            Debug.LogWarning("Sound name not found: " + name);
        }
    }

    // 暂停背景音乐
    public void PauseMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Pause();
        }
    }

    // 恢复背景音乐播放
    public void ResumeMusic()
    {
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.UnPause();
        }
    }

    // 停止背景音乐
    public void StopMusic()
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }
    }

    // 开始播放背景音乐
    public void StartMusic()
    {
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
        }
    }

    // 设置背景音乐的音量
    public void SetMusicVolume(float volume)
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = Mathf.Clamp(volume, 0, 1);
        }
    }

    // 设置所有音效的音量
    public void SetSoundVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);
        foreach (AudioSource source in soundSources.Values)
        {
            if (source != null)
            {
                source.volume = volume;
            }
        }
    }
}
