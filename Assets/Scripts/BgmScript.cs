using UnityEngine;
using System.Collections;

public class BgmScript : MonoBehaviour {

    public AudioClip BgmInGame;
    public AudioClip BgmLevelClear;
    public AudioClip BgmMenu;
    public AudioSource AudioSourceBgm;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayBgmInGame(bool loop)
    {
        if(AudioSourceBgm.isPlaying)
            AudioSourceBgm.Stop();
        AudioSourceBgm.loop = loop;
        AudioSourceBgm.clip = BgmInGame;
        AudioSourceBgm.Play();
    }

    public void PlayBgmLevelClear(bool loop)
    {
        if(AudioSourceBgm.isPlaying)
            AudioSourceBgm.Stop();
        AudioSourceBgm.loop = loop;
        AudioSourceBgm.clip = BgmLevelClear;
        AudioSourceBgm.Play();
    }

    public void PlayBgmMenu(bool loop)
    {
        if(AudioSourceBgm.isPlaying)
            AudioSourceBgm.Stop();
        AudioSourceBgm.loop = loop;
        AudioSourceBgm.clip = BgmMenu;
        AudioSourceBgm.Play();
    }
}
