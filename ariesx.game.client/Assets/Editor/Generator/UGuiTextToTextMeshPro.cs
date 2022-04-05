using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class UGuiTextToTextMeshPro : Editor {
        [MenuItem("GameObject/CustomUI/ConvertTextToTextMeshPro", false, 51)]
        static bool ConvertTextToTextMeshPro() { 
            GameObject currentGameObject = Selection.activeGameObject;
            Text uiText = currentGameObject.GetComponent<Text>();
            if (uiText == null) {
                EditorUtility.DisplayDialog(
                    "ERROR!", "You must select a Unity UI Text Object to convert.", "OK", "");
                return false;
            }

            DestroyImmediate(uiText);

            LocalizedText uguiLocalText = currentGameObject.GetComponent<LocalizedText>();
            if (uguiLocalText != null) {
                currentGameObject.AddComponent<LocalizedTextMeshPro>();
                LocalizedTextMeshPro textMeshProLocal = currentGameObject.GetComponent<LocalizedTextMeshPro>();
                textMeshProLocal.key = uguiLocalText.key;
                DestroyImmediate(uguiLocalText);
            }

            bool hasOutline = false;
            Outline uguiTextOutline = currentGameObject.GetComponent<Outline>();
            if (uguiTextOutline != null) {
                hasOutline = true;
                Debug.LogError("Has Outline " + ColorUtility.ToHtmlStringRGBA(uguiTextOutline.effectColor));
                DestroyImmediate(uguiTextOutline);
            }

            bool hasShadow = false;
            Shadow uguiTextShadow = currentGameObject.GetComponent<Shadow>();
            if (uguiTextShadow != null) {
                hasShadow = true;
                Debug.LogError("Has Shadow " + ColorUtility.ToHtmlStringRGBA(uguiTextShadow.effectColor));
                DestroyImmediate(uguiTextShadow);
            }

            RectTransform originRectTransform = currentGameObject.GetComponent<RectTransform>();
            Vector2 origionSizeData = originRectTransform.sizeDelta;
            currentGameObject.AddComponent<TextMeshProUGUI>();
            TextMeshProUGUI tmp = currentGameObject.GetComponent<TextMeshProUGUI>();

            if (tmp == null) {
                EditorUtility.DisplayDialog(
                    "ERROR!",
                    "Something went wrong! Text Mesh Pro did not select the newly created object.",
                    "OK",
                    "");
                return false;
            }
            TMP_FontAsset fontAsset = Resources.Load("Fonts/fzcyGBK", typeof(TMP_FontAsset)) as TMP_FontAsset;
            if (fontAsset == null) {
                EditorUtility.DisplayDialog(
                    "ERROR!",
                    "No fzcyGBK found",
                    "OK",
                    "");
                return false;
            }

            tmp.font = fontAsset;
            tmp.fontStyle = GetTmpFontStyle(uiText.fontStyle);
            tmp.fontSize = uiText.fontSize;
            tmp.fontSizeMin = uiText.resizeTextMinSize;
            tmp.fontSizeMax = uiText.resizeTextMaxSize;
            tmp.enableAutoSizing = uiText.resizeTextForBestFit;
            tmp.alignment = GetTmpAlignment(uiText.alignment);
            tmp.text = uiText.text;
            tmp.color = uiText.color;
            tmp.richText = uiText.supportRichText;
            tmp.enableWordWrapping = uiText.horizontalOverflow == HorizontalWrapMode.Wrap;
            tmp.overflowMode = GetOverflowMode(uiText.verticalOverflow);
            tmp.lineSpacing = uiText.lineSpacing;
            tmp.enableKerning = false;
            tmp.raycastTarget = false;

            if (hasOutline || hasShadow) {
                Debug.LogError(hasOutline + " " + hasShadow);
                string materName = "Fonts/fzcyGBK_outline";
                if (hasOutline && hasShadow) {
                    materName = "Fonts/fzcyGBK_outline_shadow";
                }
                Material fontMaterial = Resources.Load(materName, typeof(Material)) as Material;
                if (fontMaterial == null) {
                    EditorUtility.DisplayDialog(
                        "ERROR!",
                        ("No Material " + materName + " found"),
                        "OK",
                        "");
                    return false;
                }
                tmp.fontMaterial = fontMaterial;
            }

            currentGameObject.GetComponent<RectTransform>().sizeDelta = origionSizeData;
            return true;
        }

        private static TextOverflowModes GetOverflowMode(VerticalWrapMode verticalOverflow) {
            if (verticalOverflow == VerticalWrapMode.Truncate)
                return TextOverflowModes.Truncate;

            return TextOverflowModes.Overflow;
        }

        private static FontStyles GetTmpFontStyle(FontStyle uGuiFontStyle) {
            FontStyles tmp = FontStyles.Normal;
            switch (uGuiFontStyle) {
                case FontStyle.Normal:
                default:
                    tmp = FontStyles.Normal;
                    break;
                case FontStyle.Bold:
                    tmp = FontStyles.Bold;
                    break;
                case FontStyle.Italic:
                    tmp = FontStyles.Italic;
                    break;
                case FontStyle.BoldAndItalic:
                    tmp = FontStyles.Bold | FontStyles.Italic;
                    break;
            }

            return tmp;
        }


        private static TextAlignmentOptions GetTmpAlignment(TextAnchor uGuiAlignment) {
            TextAlignmentOptions alignment = TextAlignmentOptions.Left; ;
            switch (uGuiAlignment) {
                case TextAnchor.UpperLeft:
                    alignment = TextAlignmentOptions.TopLeft;
                    break;
                case TextAnchor.UpperCenter:
                    alignment = TextAlignmentOptions.Top;
                    break;
                case TextAnchor.UpperRight:
                    alignment = TextAlignmentOptions.TopRight;
                    break;
                case TextAnchor.MiddleLeft:
                    alignment = TextAlignmentOptions.MidlineLeft;
                    break;
                case TextAnchor.MiddleCenter:
                    alignment = TextAlignmentOptions.Center;
                    break;
                case TextAnchor.MiddleRight:
                    alignment = TextAlignmentOptions.MidlineRight;
                    break;
                case TextAnchor.LowerLeft:
                    alignment = TextAlignmentOptions.BottomLeft;
                    break;
                case TextAnchor.LowerCenter:
                    alignment = TextAlignmentOptions.Bottom;
                    break;
                case TextAnchor.LowerRight:
                    alignment = TextAlignmentOptions.BottomRight;
                    break;
                default:
                    break;
            }

            return alignment;
        }
    }
}