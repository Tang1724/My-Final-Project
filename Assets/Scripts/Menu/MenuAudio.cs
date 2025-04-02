using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    public static MenuAudio instance; // 静态实例，用于全局访问

    public AudioSource[] menuSounds; // 菜单场景的音效资源数组
    public AudioSource menuBackgroundMusic; // 菜单场景的背景音乐

    void Awake()
    {
        // 单例模式初始化
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 确保只有一个实例
        }
    }

    // 播放菜单音效（支持循环）
    public void PlayMenuSound(string name, bool loop = false)
    {
        foreach (AudioSource sound in menuSounds)
        {
            // 检查 sound 和 sound.clip 是否为空
            if (sound != null && sound.clip != null && sound.clip.name == name)
            {
                sound.loop = loop; // 设置是否循环播放
                sound.Play();
                return;
            }
        }
        Debug.LogWarning("Menu sound name not found: " + name);
    }

    // 停止播放特定菜单音效
    public void StopMenuSound(string name)
    {
        foreach (AudioSource sound in menuSounds)
        {
            // 检查 sound 和 sound.clip 是否为空
            if (sound != null && sound.clip != null && sound.clip.name == name)
            {
                sound.Stop();
                return;
            }
        }
        Debug.LogWarning("Menu sound name not found: " + name);
    }

    // 暂停菜单背景音乐
    public void PauseMenuMusic()
    {
        if (menuBackgroundMusic != null && menuBackgroundMusic.isPlaying)
        {
            menuBackgroundMusic.Pause();
        }
    }

    // 恢复菜单背景音乐播放
    public void ResumeMenuMusic()
    {
        if (menuBackgroundMusic != null && !menuBackgroundMusic.isPlaying)
        {
            menuBackgroundMusic.UnPause();
        }
    }

    // 停止菜单背景音乐
    public void StopMenuMusic()
    {
        if (menuBackgroundMusic != null)
        {
            menuBackgroundMusic.Stop();
        }
    }

    // 开始播放菜单背景音乐
    public void StartMenuMusic()
    {
        if (menuBackgroundMusic != null && !menuBackgroundMusic.isPlaying)
        {
            menuBackgroundMusic.Play();
        }
    }

    // 设置菜单背景音乐的音量
    public void SetMenuMusicVolume(float volume)
    {
        if (menuBackgroundMusic != null)
        {
            menuBackgroundMusic.volume = Mathf.Clamp(volume, 0, 1); // 限制音量值在0到1之间
        }
    }

    // 设置所有菜单音效的音量
    public void SetMenuSoundVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1); // 限制音量值在0到1之间
        foreach (AudioSource sound in menuSounds)
        {
            if (sound != null)
            {
                sound.volume = volume;
            }
        }
    }
}
