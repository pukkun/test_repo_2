using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundController : Singleton<SoundController>
{

    public static bool LOCK_CHANGE_SOUND_MUSIC = false;
    [SerializeField] private AudioSource soundMusic = null;

    [SerializeField] private AudioSource soundEffect = null;

    [SerializeField] private List<AudioSource> listSoundEffect;

    [SerializeField]
    private Dictionary<string, AudioClip> dicSound;

    public int StatusSoundMusic = 0;//0 mute, 1 sound
    public int StatusSFX = 0;//0 mute, 1 sound
    public const float MAX_VOL = 0.8f;

    private Tweener tweenerMute;

    private AudioClip currentMusic;

    [SerializeField] private List<AudioClip> audioClips = null;

    private System.Action callbackEnd;


    public System.Action<int> HandleChangeSoundState;
    // Use this for initialization
    int timeCountdown;
    int cacheStatus = 0;
    protected override void Awake()
    {
        dicSound = new Dictionary<string, AudioClip>();
        listSoundEffect = new List<AudioSource>();
        if (soundEffect != null) listSoundEffect.Add(soundEffect);
        base.Awake();
        LoadStatusSound();

    }
    public void LoadStatusSound()
    {
        StatusSoundMusic = GameUtils.GetIntPref("STATUS_SOUND_MUSIC", 0, true);
        StatusSFX = GameUtils.GetIntPref("STATUS_SOUND_FX", 1, true);
    }

    public void AddSoundFromBundle(AssetBundle assetBundle)
    {
        AudioClip[] _audioClips = assetBundle.LoadAllAssets<AudioClip>();
        foreach (var item in _audioClips)
        {
            if (!audioClips.Contains(item)) audioClips.Add(item);
        }
    }
    public void AddSound(AudioClip[] _audioClips)
    {
        foreach (var item in _audioClips)
        {
            if (!audioClips.Contains(item)) audioClips.Add(item);
        }
    }

    public void UpdateStatusSound()
    {
        //StatusSound = GameUtils.GetSoundStatus();
        cacheStatus = StatusSoundMusic;
    }

    public void ChangeSoundState()
    {
        if (StatusSoundMusic == 0)
        {
            UnMuteSound();
        }
        else
        {
            MuteSound();
        }
        HandleChangeSoundState?.Invoke(StatusSoundMusic);
        GameUtils.SaveDataPref("STATUS_SOUND_MUSIC", StatusSoundMusic, true);
    }

    public void ChangeSFXState()
    {
        if (StatusSFX == 0)
        {
            UnMuteSFS();
        }
        else
        {
            MuteSFS();
        }
        GameUtils.SaveDataPref("STATUS_SOUND_FX", StatusSFX, true);
    }


    private IEnumerator countdownSecond()
    {
        yield return new WaitForSeconds(1);
        timeCountdown--;
        if (timeCountdown <= 0)
        {
            SaveChangeStatusSound();
        }
        else
        {
            StartCoroutine(countdownSecond());
        }

    }

    private void SaveChangeStatusSound()
    {
        if (cacheStatus != StatusSoundMusic)
        {
            try
            {
                Debug.Log("SaveChangeStatusSound" + StatusSoundMusic);
                cacheStatus = StatusSoundMusic;
                //GameUtils.SaveSetting();
            }
            catch (System.Exception)
            {
            }
        }
    }



    public void InitSound(int sound)
    {
        StatusSoundMusic = sound;
        HandleChangeSoundState?.Invoke(StatusSoundMusic);
    }

    public void DispatchEvent()
    {
        HandleChangeSoundState?.Invoke(StatusSoundMusic);
    }

    public void PlaySoundEffectOneShot(string name)
    {
#if UNITY_EDITOR
        Debug.LogWarning("PlaySoundEffectOneShot " + name);
#endif

        AudioClip sound = getSoundByName(name);
        if (sound == null)
        {
            Debug.LogError("missing sound " + name);
        }
        AudioSource audioSource = getSoundEffectFree();
        if (audioSource == null) return;
        audioSource.bypassEffects = false;
        audioSource.bypassListenerEffects = false;
        audioSource.bypassReverbZones = false;
        audioSource.volume = StatusSFX == 1 ? MAX_VOL : 0;
        audioSource.PlayOneShot(sound);
    }


    // 
    //private IEnumerator playEffectCustomSequentially(string name, float waitTime)
    //{
    //    yield return new WaitForSeconds(waitTime);
    //    if (dicOverplay.GetInt(name) <= 0)
    //    {
    //        if (listRuning.Contains(name)) listRuning.Remove(name);
    //        yield break;
    //    }
    //    else
    //    {

    //        if (listRuning.Contains(name))
    //        {
    //            dicOverplay.Put(name, dicOverplay.GetInt(name) - 1);
    //            StartCoroutine(playEffectCustomSequentially(name, 0.05f));
    //            StopSoundEffect(name);
    //            PlaySoundEffectCustom(name, false);
    //        }
    //    }
    //}


    //Dictionary<string, int> dicOverplay = new Dictionary<string, int>();
    //List<string> listRuning = new List<string>();

    //public void AddPlaySoundEffectCustom(string name)
    //{
    //    if (dicOverplay.GetInt(name) < 4) dicOverplay.Put(name, dicOverplay.GetInt(name) + 1);
    //    Debug.Log("playEffectCustomSequentially add sound " + name + "   " + dicOverplay.GetInt(name));
    //    if (!listRuning.Contains(name))
    //    {
    //        listRuning.Add(name);
    //        StartCoroutine(playEffectCustomSequentially(name, 0));
    //    }
    //}    


    Dictionary<string, int> dicMergeSound = new Dictionary<string, int>();
    IEnumerator iEnumMergeSound;
    public void AddMergeSoundEffect(string name, string namex2)
    {
        dicMergeSound.Put(name, dicMergeSound.GetInt(name) + 1);
        if (iEnumMergeSound != null)
        {
            StopCoroutine(iEnumMergeSound);
        }
        StartCoroutine(playMegerSoundEffect(name, namex2));
    }
    private IEnumerator playMegerSoundEffect(string name, string namex2)
    {
        yield return new WaitForEndOfFrame();
        if (dicMergeSound.ContainsKey(name) && dicMergeSound.GetInt(name) > 1) PlaySoundEffectOneShot(namex2);
        else PlaySoundEffectOneShot(name);
        dicMergeSound.Remove(name);
    }

    /// <summary>
    ///  Chi su dung PlaySoundEffectVer2 khi am thanh can Stop, Pause , UnPause
    /// </summary>
    public void PlaySoundEffectCustom(string name, bool isLoop = false)
    {
#if UNITY_EDITOR
        Debug.LogWarning("PlaySoundEffectCustom " + name);
#endif

        AudioClip sound = getSoundByName(name);
        if (sound == null)
        {
            Debug.LogError("missing sound " + name);
        }
        AudioSource audioSource = getSoundEffectFree();
        audioSource.bypassEffects = false;
        audioSource.bypassListenerEffects = false;
        audioSource.bypassReverbZones = false;
        audioSource.volume = StatusSFX == 1 ? MAX_VOL : 0;
        audioSource.clip = sound;
        audioSource.loop = isLoop;
        audioSource.Play();
    }

    public bool IsPlayingSoundEffect(string name)
    {
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            if (listSoundEffect[i].isPlaying && listSoundEffect[i].clip != null && listSoundEffect[i].clip.name == name)
            {
                return true;
            }
        }
        return false;
    }

    public void StopSoundEffect(string name)
    {
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            if (listSoundEffect[i].isPlaying && listSoundEffect[i].clip != null && listSoundEffect[i].clip.name == name)
            {
                listSoundEffect[i].Stop();
                listSoundEffect[i].clip = null;
            }
        }
    }
    public void PauseSoundEffect(string name)
    {
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            if (listSoundEffect[i].isPlaying && listSoundEffect[i].clip != null && listSoundEffect[i].clip.name == name)
            {
                listSoundEffect[i].Pause();
            }
        }
    }
    public void UnPauseSoundEffect(string name)
    {
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            if (!listSoundEffect[i].isPlaying && listSoundEffect[i].clip != null && listSoundEffect[i].clip.name == name)
            {
                listSoundEffect[i].UnPause();
            }
        }
    }

    private AudioSource getSoundEffectFree()
    {
        if (listSoundEffect.IsNullOrEmpty()) return null;
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            if (!listSoundEffect[i].isPlaying && listSoundEffect[i].clip == null)
            {
                return listSoundEffect[i];
            }
        }
        GameObject go = new GameObject();
        go.name = "Effect_" + (listSoundEffect.Count + 1);
        go.transform.SetParent(transform);
        AudioSource audioSource = go.AddComponent<AudioSource>();
        listSoundEffect.Add(audioSource);

        return audioSource;
    }

    public void PlaySoundMusic(string nameSound, bool loop = true, System.Action callbackEnd = null, float fadeIn = 1)
    {
        if (LOCK_CHANGE_SOUND_MUSIC) return;
        this.callbackEnd = callbackEnd;
        AudioClip audioClip = getSoundByName(nameSound);
        if (audioClip == null)
        {
            //#if !UNITY_EDITOR
            //            StartCoroutine(AssetURLHelper.LoadAudioClip(nameSound, audioResult =>
            //            {
            //                if (audioResult != null)
            //                {
            //                    dicSound.Add(nameSound, audioResult);
            //                    playSound(audioResult);
            //                }
            //            }));
            //#endif

            //string[] assets = new string[] { NameAssetBundle.SOUND_HOME };
            //AssetBundleHelper.Instance.LoadAssetBundle(assets, Enums.TypeLoading.None, false, () =>
            //{
            //    StartCoroutine(loadSoundComplete());
            //});
        }
        else
        {
            playSound(audioClip, loop, fadeIn);
        }
    }
    public void StopSoundMusic(float duration)
    {
        if (StatusSoundMusic == 1)
        {
            soundMusic.DOFade(0, duration);
        }
    }

    private void playSound(AudioClip audioClip, bool loop, float fadeIn)
    {
        bool changeAudio = true;
        if (currentMusic != null)
        {
            changeAudio = currentMusic.name != audioClip.name;
        }
        currentMusic = audioClip;

        if (changeAudio)//
        {
            float length = audioClip.length;
            this.Wait(length, onEndSoundChange);
            soundMusic.volume = 0;
            soundMusic.clip = audioClip;
            soundMusic.loop = loop;
            soundMusic.Play(0);
        }
        else // ko doi~ nhac thi` play tiep
        {

        }
        if (StatusSoundMusic == 1)
        {
            Debug.LogError(" soundMusic volume " + soundMusic.volume);
            DOTween.Kill(soundMusic);
            soundMusic.DOFade(MAX_VOL, fadeIn);
        }

    }
    private void onEndSoundChange()
    {
        callbackEnd?.Invoke();
        callbackEnd = null;
    }

    public void MuteSound()
    {
        StatusSoundMusic = 0;
        if (tweenerMute != null) tweenerMute.Kill();
        tweenerMute = soundMusic.DOFade(0, 0.3f);
    }
    public void UnMuteSound()
    {
        StatusSoundMusic = 1;
        soundMusic.volume = MAX_VOL;
        if (tweenerMute != null) tweenerMute.Kill();
    }

    public void MuteSFS()
    {
        StatusSFX = 0;
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            listSoundEffect[i].volume = 0;
        }
    }
    public void UnMuteSFS()
    {
        StatusSFX = 1;
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            listSoundEffect[i].volume = MAX_VOL;
        }
    }

    public void StopSoundMusic()
    {
        soundMusic.DOFade(0, 1).OnComplete(() => {
            soundMusic.Stop();
        });
    }

    public void ForceStopSound()
    {
        currentMusic = null;
        StopSoundMusic();
    }
    public void FadeOutMuzik(float time)
    {
        soundMusic.DOFade(0, time).SetEase(Ease.Linear);
    }
    public void FadeInMuzik(float time)
    {
        soundMusic.DOFade(getVolumn(), time).SetEase(Ease.Linear);
    }
    private float getVolumn()
    {
        return StatusSoundMusic == 1 ? MAX_VOL : 0;
    }
    private AudioClip getSoundByName(string nameSound)
    {
        if (audioClips == null) return null;
        //if (dicSound.ContainsKey(nameSound)) return dicSound[nameSound];
        for (int i = 0; i < audioClips.Count; i++)
        {
            if (audioClips[i] && audioClips[i].name == nameSound)
            {
                return audioClips[i];
            }
        }
        return null;
    }
    public float GetLengthSound(string nameSound)
    {
        AudioClip audioClip = getSoundByName(nameSound);
        if (audioClip != null) return audioClip.length;
        else return 0;
    }
    public void ForceMuteForRV(bool isMute)
    {
        soundMusic.mute = isMute;
    }
}
