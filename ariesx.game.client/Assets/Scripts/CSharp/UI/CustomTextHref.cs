using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Poukoute {
    public class CustomTextHref : Text, IPointerClickHandler {
        private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

        private static readonly StringBuilder s_TextBuilder = new StringBuilder();

        private static readonly Regex s_HrefRegex =
            new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

        [Serializable]
        public class HrefClickEvent : UnityEvent<string> { }

        [SerializeField]
        private HrefClickEvent m_OnHrefClick = new HrefClickEvent();
        private UnityEvent m_OnTextClick = new UnityEvent();

        public override float preferredWidth {
            get {
                return this.innerPreferredWidth;
            }
        }

        public override float preferredHeight {
            get {
                return this.innerPreferredHeight;
            }
        }

        private float innerPreferredWidth;
        private float innerPreferredHeight;
        private string m_TextForLayout;

        public HrefClickEvent onHrefClick {
            get {
                m_OnHrefClick.RemoveAllListeners();
                return m_OnHrefClick;
            }
        }

        public UnityEvent onTextClick {
            get {
                m_OnTextClick.RemoveAllListeners();
                return m_OnTextClick;
            }
        }

        public override string text {
            get {
                return base.text;
            } set {
                base.text = value;
                this.OnTextChange();
            }
        }

        public float maxWidth;
        
        protected void OnTextChange() {
            s_TextBuilder.Length = 0;
            m_HrefInfos.Clear();
            int indexText = 0;
            foreach (Match match in s_HrefRegex.Matches(text)) {
                s_TextBuilder.Append(text.Substring(indexText, match.Index - indexText));
                Group group = match.Groups[1];
                HrefInfo hrefInfo = new HrefInfo {
                    startIndex = s_TextBuilder.Length * 4,
                    endIndex = (s_TextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3,
                    name = group.Value
                };
                m_HrefInfos.Add(hrefInfo);

                s_TextBuilder.Append(match.Groups[2].Value);
                indexText = match.Index + match.Length;
            }
            s_TextBuilder.Append(text.Substring(indexText, text.Length - indexText));
            this.m_TextForLayout = s_TextBuilder.ToString();
        }

        public void OnPointerClick(PointerEventData eventData) {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);
            foreach (HrefInfo hrefInfo in m_HrefInfos) {
                List<Rect> boxes = hrefInfo.boxes;
                for (int i = 0; i < boxes.Count; ++i) {
                    if (boxes[i].Contains(lp)) {
                        m_OnHrefClick.Invoke(hrefInfo.name);
                        return;
                    }
                }
            }
            this.m_OnTextClick.Invoke();
        }

        protected override void OnPopulateMesh(VertexHelper toFill) {
            string orignText = this.m_Text;
            this.m_Text = this.m_TextForLayout;
            base.OnPopulateMesh(toFill);
            this.m_Text = orignText;
            IList<UIVertex> verts = this.cachedTextGenerator.verts;
            if (verts == null) {
                verts = this.cachedTextGeneratorForLayout.verts;
            }
            foreach (var hrefInfo in m_HrefInfos) {
                hrefInfo.boxes.Clear();
                if (hrefInfo.startIndex >= verts.Count) {
                    continue;
                }
                UIVertex pos = verts[hrefInfo.startIndex];
                var bounds = new Bounds(pos.position, Vector3.zero);
                for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i < m; i++) {
                    if (i >= verts.Count) {
                        break;
                    }
                    pos = verts[i];
                    if (pos.position.x < bounds.min.x) {
                        hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(pos.position, Vector3.zero);
                    } else {
                        bounds.Encapsulate(pos.position);
                    }
                }
                hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
            }
        }

        public override void CalculateLayoutInputHorizontal() {
            TextGenerationSettings setting = this.GetGenerationSettings(cachedTextGenerator.rectExtents.size);
            float width = this.cachedTextGenerator.GetPreferredWidth(this.m_TextForLayout, setting);
            this.innerPreferredWidth = width;
        }

        public override void CalculateLayoutInputVertical() {
            TextGenerationSettings setting = this.GetGenerationSettings(cachedTextGenerator.rectExtents.size);
            setting.generationExtents = new Vector2(this.maxWidth, Mathf.Infinity);
            float height = this.cachedTextGenerator.GetPreferredHeight(this.m_TextForLayout, setting);
            this.innerPreferredHeight = height;
        }

        private class HrefInfo {
            public int startIndex;
            public int endIndex;
            public string name;
            public readonly List<Rect> boxes = new List<Rect>();
        }
    }
}