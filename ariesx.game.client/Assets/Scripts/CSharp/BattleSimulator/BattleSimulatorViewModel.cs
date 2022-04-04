using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class BattleSimulatorViewModel : BaseViewModel, IViewModel {
        private BattleSimulatorModel model;
        private BattleSimulatorView view;
        /* Model data get set */

        private GameObject configureManager;
        private GameObject poolManager;

        public List<string> HeroList {
            get {
                return this.model.heroList;
            }
        }

        public List<string> SkillList {
            get {
                return this.model.skillList;
            }
        }

        public List<string> ActionList {
            get {
                return this.model.actionList;
            }
        }

        /**********************/

        /* Other members */

        /*****************/

        void Awake() {
            this.InitConfigure();

            this.model = new BattleSimulatorModel();
            this.model.Refresh(null);
            this.view = this.gameObject.AddComponent<BattleSimulatorView>();

            this.view.SetHeroList();
        }

        public void Show() {
            this.gameObject.SetActiveSafe(true);
        }

        public void Hide() {
            this.gameObject.SetActiveSafe(false);
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        /* Add 'NetMessageAck' function here*/

        /***********************************/

        void InitConfigure() {
#if UNITY_EDITOR
            this.configureManager = new GameObject();
            this.configureManager.name = "ConfigureManager";
            this.configureManager.transform.position = UnityEngine.Vector3.zero;
            this.configureManager.AddComponent<ConfigureManager>();
            ConfigureManager.LoadHeroEditorConfigures();

            this.poolManager = new GameObject();
            this.poolManager.name = "PoolManager";
            this.poolManager.transform.position = UnityEngine.Vector3.zero;
            this.poolManager.transform.SetParent(this.transform);
            this.poolManager.AddComponent<PoolManager>();
            Debug.Log("InitPool Complete");

            GameObject updateManager = new GameObject() {
                name = "UpdateManager"
            };
            updateManager.transform.position = UnityEngine.Vector3.zero;
            updateManager.transform.SetParent(this.transform);
            updateManager.AddComponent<UpdateManager>();
            Debug.Log("InitUpdate Complete");

            GameObject audioManager = new GameObject() {
                name = "AudioManager"
            };
            audioManager.transform.position = UnityEngine.Vector3.zero;
            audioManager.transform.SetParent(this.transform);
            audioManager.AddComponent<AudioManager>();
            Debug.Log("InitAudio Complete");

            GameObject animationManager = new GameObject() {
                name = "AnimationManager"
            };
            animationManager.transform.position = UnityEngine.Vector3.zero;
            animationManager.transform.SetParent(this.transform);
            animationManager.AddComponent<AnimationManager>();
            Debug.Log("InitAnimation Complete");

            GameObject roleManager = new GameObject()
            {
                name = "RoleManager"
            };
            roleManager.transform.position = UnityEngine.Vector3.zero;
            roleManager.transform.SetParent(this.transform);
            roleManager.AddComponent<RoleManager>();
            Debug.Log("InitRole Complete");
#endif
        }

        void OnDisable() {
            Destroy(this.configureManager);
            Destroy(this.poolManager);
        }
    }
}
