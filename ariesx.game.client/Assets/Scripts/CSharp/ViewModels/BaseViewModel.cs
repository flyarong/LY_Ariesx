using Protocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public interface IViewModel {
        //void Show();
        //void Hide();
        void HideImmediatly();
    }

    public enum AddOnMap{
        Edit,
        HideAll,
        HideAllWithoutTop
    }

    public class BaseViewModel : MonoBehaviour {
        virtual protected void OnReLogin() {
        }

        void OnEnable() {
            RoleManager.AddLoginAction(this.OnReLogin);
        }

        void OnDisable() {
            RoleManager.RemoveLoginAction(this.OnReLogin);
        }
    }
}
