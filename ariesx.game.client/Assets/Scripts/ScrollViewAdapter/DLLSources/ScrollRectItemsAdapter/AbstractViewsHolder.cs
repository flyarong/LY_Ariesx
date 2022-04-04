
using UnityEngine;
using UnityEngine.UI;

namespace frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter {
    /// <summary>
    /// Class representing the concept of a Views Holder, i.e. a class that references some views and the id of the data displayed by those views. 
    /// Usually, the root and its child views, once created, don't change, but <see cref="ItemIndex"/> does, after which the views will change their data.
    /// </summary>
    public abstract class AbstractViewsHolder : MonoBehaviour {
        /// <summary>The root of the view instance (which contains the actual views)</summary>
        public RectTransform root;
        [HideInInspector]
        public CanvasGroup canvasGroup;

        /// <summary> The index of the data model from which this viewsholder's views take their display information </summary>
        public virtual int ItemIndex { get; set; }

        /// <summary>
        /// Make sure to override this when you have children layouts (for example, a [Vertical/Horizontal/Grid]LayoutGroup) and call <see cref="LayoutRebuilder.MarkLayoutForRebuild(RectTransform)"/> for them. Base's implementation should still be called!
        /// </summary>
        public virtual void MarkForRebuild() {
            if (root) LayoutRebuilder.MarkLayoutForRebuild(root);
        }

        private void Awake() {
            this.canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
            if (this.canvasGroup == null) {
                this.canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
            }
        }
    }
}
