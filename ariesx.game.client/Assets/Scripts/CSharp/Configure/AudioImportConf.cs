using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Text.RegularExpressions;

namespace Poukoute {
    public class AudioImportConf : BaseConf {
        public string index;
        public string name;
        public string type;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.index = attrDict["index"];
            this.name = attrDict["name"];
            this.type = attrDict["type"];
        }

        public override string GetId() {
            return this.index.ToString();
        }

        static AudioImportConf() {
            ConfigureManager.Instance.LoadConfigure<AudioImportConf>();
        }

#if UNITY_EDITOR
        public void SetAudioClip(AudioImporter clip) {
            string platform = "Android";
            AudioImporterSampleSettings setting = clip.GetOverrideSampleSettings(platform);
            switch (this.type) {
                case "frequently_short":
                    setting.loadType = AudioClipLoadType.DecompressOnLoad;
                    setting.compressionFormat = AudioCompressionFormat.PCM;
                    break;
                case "frequently_medium":
                    setting.loadType = AudioClipLoadType.CompressedInMemory;
                    setting.compressionFormat = AudioCompressionFormat.ADPCM;
                    break;
                case "rarely_short":
                    setting.loadType = AudioClipLoadType.CompressedInMemory;
                    setting.compressionFormat = AudioCompressionFormat.ADPCM;
                    break;
                case "rarely_medium":
                    setting.loadType = AudioClipLoadType.CompressedInMemory;
                    setting.compressionFormat = AudioCompressionFormat.Vorbis;
                    setting.quality = 50;
                    break;
                case "background":
                    setting.loadType = AudioClipLoadType.Streaming;
                    setting.compressionFormat = AudioCompressionFormat.Vorbis;
                    setting.quality = 50;
                    break;
                default:
                    break;
            }
            clip.SetOverrideSampleSettings(platform, setting);
        }
#endif
        public static AudioImportConf GetConf(string id) {
            return ConfigureManager.GetConfById<AudioImportConf>(id);
        }
    }
}
