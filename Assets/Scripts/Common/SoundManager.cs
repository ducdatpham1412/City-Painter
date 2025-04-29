using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {
    Dictionary<SoundSource, AudioClip> SoundSources = new Dictionary<SoundSource, AudioClip>();
    Dictionary<SF, AudioClip> SFSources = new Dictionary<SF, AudioClip>();

    List<SoundAudio> SoundAudios = new List<SoundAudio>();
    List<AudioSource> SfAudios = new List<AudioSource>();


    void Awake() {
        SoundSources[SoundSource.Kid] = LoadSound("mc_kid");

        SFSources[SF.Pop_01] = LoadSF("sf_pop_01");
        SFSources[SF.Win_01] = LoadSF("sf_win_01");
        SFSources[SF.Whoosh_Transition] = LoadSF("sf_whoosh_transition");
    }

    public AudioSource PlaySF(SF sf, [UnityEngine.Internal.DefaultValue("1.0F")] float volumeScale = 1f) {
        if (SFSources.ContainsKey(sf)) {
            AudioSource sfFree = SfAudios.Find(audio => !audio.isPlaying);
            if (sfFree != null) {
                sfFree.PlayOneShot(SFSources[sf], volumeScale);
                return sfFree;
            }
            AudioSource newAudio = gameObject.AddComponent<AudioSource>();
            newAudio.playOnAwake = false;
            newAudio.loop = false;
            newAudio.PlayOneShot(SFSources[sf], volumeScale);
            SfAudios.Add(newAudio);
            return newAudio;
        }

        return null;
    }

    public void PlayStopBackgroundSound(BackgroundSound sound) {
        SoundSource source = Helper.GetRandomInArr(sound.sources);
        SoundAudio audio = SoundAudios.Find(s => s.sound.id == sound.id);

        if (audio == null) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = true;
            audioSource.loop = true;
            audioSource.clip = SoundSources[source];
            audioSource.Play();
            SoundAudios.Add(new SoundAudio {
                sound = sound,
                audioSource = audioSource,
                coroutine = null,
            });
            if (!GameManager.Instance.profile.backgroundSounds.Contains(sound.id)) {
                GameManager.Instance.profile.backgroundSounds.Add(sound.id);
            }
            return;
        }

        if (audio.coroutine != null) StopCoroutine(audio.coroutine);
        Destroy(audio.audioSource);
        SoundAudios.Remove(audio);
        GameManager.Instance.profile.backgroundSounds.Remove(sound.id);
    }

    public void PlayStopSfxSound(SfxSound sound) {
        SoundAudio audio = SoundAudios.Find(s => s.sound.id == sound.id);

        if (audio == null) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            Coroutine coroutine = StartCoroutine(PlaySfxSoundCoroutine(audioSource, sound));
            SoundAudios.Add(new SoundAudio {
                sound = sound,
                audioSource = audioSource,
                coroutine = coroutine,
            });
            if (!GameManager.Instance.profile.sfxSounds.Contains(sound.id)) {
                GameManager.Instance.profile.sfxSounds.Add(sound.id);
            }
            return;
        }

        if (audio.coroutine != null) StopCoroutine(audio.coroutine);
        Destroy(audio.audioSource);
        SoundAudios.Remove(audio);
        GameManager.Instance.profile.sfxSounds.Remove(sound.id);
    }

    public void Initialize() { }

    IEnumerator PlaySfxSoundCoroutine(AudioSource audio, SfxSound sound) {
        while (true) {
            audio.clip = SFSources[Helper.GetRandomInArr(sound.sources)];
            audio.Play();
            yield return new WaitUntil(() => !audio.isPlaying);
            yield return new WaitForSeconds(Random.Range(sound.minInterval, sound.maxInterval));
        }
    }

    AudioClip LoadSound(string name) {
        return Resources.Load<AudioClip>($"Sounds/Musics/{name}");
    }

    AudioClip LoadSF(string name) {
        return Resources.Load<AudioClip>($"Sounds/SFs/{name}");
    }

    public enum SF {
        None,
        Pop_01,
        Win_01,
        Whoosh_Transition,
    }

    public enum SoundSource {
        Kid,
    }

    [System.Serializable]
    class SoundAudio {
        public BaseSound sound;
        public AudioSource audioSource;
        public Coroutine coroutine;
    }
}
