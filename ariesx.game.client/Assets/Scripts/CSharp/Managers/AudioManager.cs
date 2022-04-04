using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public enum AudioType {
        Default,
        Background,
        Enviroment,
        Action,
        Show
    }

    public enum AudioScene {
        None,
        //     Map,
        Lottery,
        Battle,
        Wave,
        Elf,
        Dwarf,
        Demon,
        Dragon,
        Loading,
        ChoseCountry
    }

    public enum AudioVolumn {
        Wisper = 1,
        Low = 4,
        Medium = 8,
        High = 12
    }

    public class AudioManager : MonoBehaviour {
        private static AudioManager self;
        public static AudioManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("AudioManager is not initialized.");
                }
                return self;
            }
        }

        private Dictionary<AudioType, AudioSource> audioSourceDict =
            new Dictionary<AudioType, AudioSource>();
        private Dictionary<AudioType, float> originAudioVolumDict =
            new Dictionary<AudioType, float>();
        //private AudioScene scene;
        bool isMute = false;
        bool isDefaultMusic = false;
        bool isInit = false;

        void Awake() {
            self = this;
            this.audioSourceDict.Add(AudioType.Default,
                PoolManager.GetObject<AudioSource>(this.transform, "Default"));
            this.audioSourceDict.Add(AudioType.Background,
                PoolManager.GetObject<AudioSource>(this.transform, "Background"));
            this.audioSourceDict.Add(AudioType.Enviroment,
                PoolManager.GetObject<AudioSource>(this.transform, "Enviroment"));
            this.audioSourceDict.Add(AudioType.Action,
                PoolManager.GetObject<AudioSource>(this.transform, "Action"));
            this.audioSourceDict.Add(AudioType.Show,
                PoolManager.GetObject<AudioSource>(this.transform, "Show"));
            //this.scene = AudioScene.None;

            if (PlayerPrefs.HasKey("mute")) {
                this.isMute = PlayerPrefs.GetInt("mute") == 1 ? true : false;
            } else {
                PlayerPrefs.SetInt("mute", 0);
                this.isMute = false;
            }
            Mute(this.isMute);
            UpdateManager.Regist(UpdateInfo.AudioManager, this.UpdateAction);
            foreach (KeyValuePair<AudioType, AudioSource> pair in Instance.audioSourceDict) {
                Instance.originAudioVolumDict.Add(pair.Key, pair.Value.volume);
            }
        }

        public static void Init() {
            Instance.isInit = true;
        }

        private void UpdateAction() {
            if (this.isInit && !this.audioSourceDict[AudioType.Default].isPlaying) {
                PlayDefault();
            }
        }

        public static void PlayDefault(int index = 0) {
            string clip = string.Empty;
            AudioVolumn volumn = AudioVolumn.Low;
            if (Instance.isDefaultMusic) {
                volumn = AudioVolumn.High;
                clip = string.Concat(AudioPath.defPrefix, "interval");
            } else {
                if (index == 0) {
                    index = Random.Range(1, 21) == 1 ? 2 : 1;
                }
                volumn = AudioVolumn.Low;
                clip = string.Concat(AudioPath.defPrefix, index);
            }
            Instance.InnerPlay(clip, AudioType.Default, volumn, false);
            Instance.isDefaultMusic = !Instance.isDefaultMusic;
        }

        // To do: Really need null check?
        public static void Play(string name, AudioType type,
            AudioVolumn volumn, bool loop = false, bool isAdditive = false) {
            if (Instance != null) {
                Instance.InnerPlay(name, type, volumn, loop, isAdditive);
            }
        }

        public static void PlayWithPath(string name, AudioType type,
            AudioVolumn volumn, bool loop = false, bool isAdditive = false) {
            Instance.InnerPlayWithPath(name, type, volumn, loop, isAdditive);
        }

        public static void Play(string name, AudioType type, AudioScene scene,
            AudioVolumn volumn, bool loop = false) {
            Instance.InnerPlay(name, type, volumn, loop);
            //Instance.scene = scene;
        }

        public static void PlayBg(AudioScene scene, int index = 0) {
            string clip = string.Empty;
            bool loop = false;
            //Instance.scene = scene;
            Instance.audioSourceDict[AudioType.Default].volume = 0;
            switch (scene) {
                case AudioScene.Lottery:
                    clip = AudioPath.bgPrefix + "lottery";
                    loop = true;
                    break;
                case AudioScene.Battle:
                    clip = AudioPath.bgPrefix + "battle";
                    loop = true;
                    break;
                default:
                    clip = AudioPath.bgPrefix + GameHelper.LowerFirstCase(scene.ToString());
                    loop = true;
                    break;
            }
            Instance.InnerPlay(clip, AudioType.Background, AudioVolumn.Low, loop);
        }

        public static void StopBg() {
            Instance.audioSourceDict[AudioType.Background].Stop();
            AudioVolumn volumn = Instance.isDefaultMusic ? AudioVolumn.Low : AudioVolumn.High;
            Instance.audioSourceDict[AudioType.Default].volume = ((int)volumn) / 12f;
        }

        public static void Stop(AudioType type) {
            Instance.InnerStop(type);
        }

        public static void Mute(bool mute) {
            Instance.isMute = mute;
            foreach (AudioSource audioSource in Instance.audioSourceDict.Values) {
                audioSource.mute = mute;
            }
        }

        public static void OnVoiceLiveStatusChange(bool liveOn) {
            foreach (KeyValuePair<AudioType, AudioSource> pair in Instance.audioSourceDict) {
                pair.Value.volume = Instance.originAudioVolumDict[pair.Key] * (liveOn ? 0.2f : 1f);
            }
        }

        private void InnerPlayWithPath(string path, AudioType type,
            AudioVolumn volumn, bool loop, bool isAdditive = false) {
            AudioClip clip = PoolManager.GetAudio(path);
            if (clip == null) {
                return;
            }
            if (isAdditive) {
                StartCoroutine(InnerAddPlay(clip));
            } else {
                this.audioSourceDict[type].clip = clip;
                this.audioSourceDict[type].loop = loop;
                this.audioSourceDict[type].volume = ((int)volumn) / 12f;
                this.audioSourceDict[type].Play();
            }
        }

        private void InnerPlay(string name, AudioType type,
            AudioVolumn volumn, bool loop, bool isAdditive = false) {
            AudioClip clip = ArtPrefabConf.GetAudio(name);
            if (clip == null) {
                return;
            }
            if (isAdditive) {
                StartCoroutine(InnerAddPlay(clip));
            } else {
                this.audioSourceDict[type].clip = clip;
                this.audioSourceDict[type].loop = loop;
                this.audioSourceDict[type].volume = ((int)volumn) / 12f;
                this.audioSourceDict[type].Play();
            }
        }

        private IEnumerator InnerAddPlay(AudioClip clip) {
            if (!this.isMute) {
                AudioSource audioSrc = PoolManager.GetObject<AudioSource>(this.transform);
                audioSrc.clip = clip;
                audioSrc.Play();
                yield return YieldManager.GetWaitForSeconds(clip.length);
                Destroy(audioSrc.gameObject);
            } else {
                yield return null;
            }
        }

        private void InnerStop(AudioType type) {
            //  if (this.audioSourceDict != null) {
            this.audioSourceDict[type].Stop();
            //  }
        }

    }
}
