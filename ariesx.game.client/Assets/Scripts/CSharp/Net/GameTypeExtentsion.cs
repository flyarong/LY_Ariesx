using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using Poukoute;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.Events;

namespace Protocol {
    public static class GameTypeExtentsion {
        private static readonly Regex sNumber = new Regex(@"\d+");

        public static string GetPassType(this MapTileInfo tileInfo) {
            if (tileInfo.pass == null) {
                return string.Empty;
            }

            string passType = GameConst.PASS_BRIDGE;
            if (MiniMapPassConf.GetMainPassList().Contains(tileInfo.pass)) {
                passType = GameConst.PASS_PASS;
            }

            return passType;
        }

        public static bool IsTilePassBridge(this MapTileInfo tileInfo) {
            return tileInfo.GetPassType().Equals(GameConst.PASS_BRIDGE);
        }

        public static string GetTilePassId(this MapTileInfo tileInfo) {
            string type = tileInfo.GetPassType();
            if (string.IsNullOrEmpty(type)) {
                return string.Empty;
            }
            //Debug.LogError("pass " + string.Concat(tileInfo.pass.level, type));
            return string.Concat(tileInfo.pass.level, type);
        }

        public static void SetTextContent(this Text textObj, string value) {
            // To do : the best way is change UI code, fix the chines new line judge.
            value = GameHelper.ReplaceWhitespace(value, "\xA0");
            textObj.text = value;
        }

        public static void StripLengthWithSuffix(this Text textObj, string input, string suffix = "...") {
            int maxWidth = (int)textObj.GetComponent<RectTransform>().rect.width;
            int len = textObj.CalculateLengthOfText(input);
            if (len > maxWidth) {
                input = string.Concat(textObj.StripLength(input, maxWidth), suffix);
            }
            textObj.text = input;
        }

        public static string StripLength(this Text textObj, string input, int maxWidth) {
            Font myFont = textObj.font;
            myFont.RequestCharactersInTexture(input, textObj.fontSize, textObj.fontStyle);

            CharacterInfo characterInfo = new CharacterInfo();
            char[] arr = input.ToCharArray();
            int i = 0;
            int totalLength = 0;
            foreach (char c in arr) {
                myFont.GetCharacterInfo(c, out characterInfo, textObj.fontSize);
                int newLength = totalLength + characterInfo.advance;
                if (newLength > maxWidth) {
                    if ((newLength - maxWidth.Abs()) > (maxWidth - totalLength).Abs()) {
                        break;
                    } else {
                        totalLength = newLength;
                        i++;
                        break;
                    }
                }
                totalLength += characterInfo.advance;
                i++;
            }
            return input.Substring(0, i - 1);
        }

        public static int CalculateLengthOfText(this Text textObj, string message) {
            int totalLength = 0;
            Font myFont = textObj.font;
            myFont.RequestCharactersInTexture(message, textObj.fontSize, textObj.fontStyle);
            CharacterInfo characterInfo = new CharacterInfo();

            char[] arr = message.ToCharArray();

            foreach (char c in arr) {
                myFont.GetCharacterInfo(c, out characterInfo, textObj.fontSize);
                totalLength += characterInfo.advance;
            }

            return totalLength;
        }

        public static void StripLengthWithSuffix(this TextMeshProUGUI textObj, string input, string suffix = "...") {
            int inputLength = input.Length;
            if (inputLength > 0) {
                RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
                float maxWidth = textRectTransform.rect.width;
                float inputWidth = textObj.GetPreferredValues(input).x;
                if (inputWidth > maxWidth) {
                    while (textObj.GetPreferredValues(input.Substring(0, inputLength)).x > maxWidth) {
                        inputLength--;
                    }
                    input = string.Concat(input.Substring(0, inputLength), suffix);
                }
                textObj.text = input;
            } else {
                textObj.text = string.Empty;
            }
        }

        public static int GetNewEnergy(this Hero hero) {
            int energy = 0;
            long energyUpdateOffset = RoleManager.GetCurrentUtcTime() - hero.EnergyUpdatedAt * 1000;
            int halfHours = (int)(energyUpdateOffset / GameConst.HALF_HOUR_MILLION_SECONDS);
            energy = hero.Energy + halfHours;
            return (energy < GameConst.HERO_ENERGY_MAX) ? energy : GameConst.HERO_ENERGY_MAX;
        }

        public static void SetActiveSafe(this GameObject obj, bool value) {
            if (obj != null && obj.activeSelf != value) {
                obj.SetActive(value);
            }
        }

        public static void InvokeSafe(this Action self) {
            if (self != null) {
                self.Invoke();
            }
        }

        public static void InvokeSafe<T0>(this UnityAction<T0> self, T0 param) {
            if (self != null) {
                self.Invoke(param);
            }
        }

        public static void InvokeSafe(this UnityAction self) {
            if (self != null) {
                self.Invoke();
            }
        }

        public static void InvokeSafe(this UnityEvent self) {
            if (self != null) {
                self.Invoke();
            }
        }

        public static void InvokeSafe(this OnButtonClick self) {
            if (self != null) {
                self.Invoke();
            }
        }

        public static void InvokeSafe<T>(this Action<T> self, T obj) {
            if (self != null) {
                self.Invoke(obj);
            }
        }

        public static int Abs(this int self) {
            return self > 0 ? self : -self;
        }

        public static float Abs(this float self) {
            return self > 0 ? self : -self;
        }

        public static double Abs(this double self) {
            return self > 0 ? self : -self;
        }

        #region string extension
        public static bool CustomEquals(this string selfString, string otherString) {
            if (selfString == null) {
                return false;
            } else {
                return selfString.Equals(otherString, StringComparison.Ordinal);
            }
        }


        public static bool CustomIsEmpty(this string str) {
            if ((str + string.Empty).Length == 0) {
                return true;
            }
            return false;
        }

        public static bool CustomStartsWith(this string a, string b) {
            int aLen = a.Length;
            int bLen = b.Length;
            int ap = 0; int bp = 0;

            while (ap < aLen && bp < bLen && a[ap] == b[bp]) {
                ap++;
                bp++;
            }

            return (bp == bLen && aLen >= bLen) ||

                    (ap == aLen && bLen >= aLen);
        }

        public static bool CustomEndsWith(this string a, string b) {
            int ap = a.Length - 1;
            int bp = b.Length - 1;

            while (ap >= 0 && bp >= 0 && a[ap] == b[bp]) {
                ap--;
                bp--;
            }
            return (bp < 0 && a.Length >= b.Length) ||

                    (ap < 0 && b.Length >= a.Length);
        }

        public static int GetNumber(this string str) {
            int result;
            if (int.TryParse(sNumber.Match(str).Value, out result)) {
                return result;
            }
            return -1;
        }

        public static ulong CustomGetHashCode(this string str) {
            ulong hashCode = 0;
            char ch;
            for (int i = 0; i < str.Length; ++i) {
                ch = Char.ToUpperInvariant(str[i]);
                if (str[i] == '\\') { ch = '/'; }
                hashCode = (hashCode << 7) + (hashCode << 1) + hashCode + ch;
            }
            return hashCode;
        }

        public static string[] CustomSplit(this string value, char separator) {
            int bufferLength = value.GetContainCharNum(separator) + 1;
            string[] buffer = new string[bufferLength];
            int valueLength = value.Length;
            int startIndex = 0;
            int resultIndex = 0;
            for (int i = 0; i < valueLength; i++) {
                if (value[i] == separator) {
                    buffer[resultIndex++] = value.Substring(startIndex, i - startIndex);
                    startIndex = i + 1;
                }
            }
            buffer[resultIndex] = value.Substring(startIndex, value.Length - startIndex);
            return buffer;
        }

        private static List<string> buffer = new List<string>();
        public static string[] CustomSplit(this string value, ref string[] separator) {
            int valueLength = value.Length;
            //List<string> buffer = new List<string>();
            buffer.Clear();
            string addString = string.Empty;
            bool isEqual = false;
            int starPoint = 0;
            int targetPoint = 0;
            for (int i = 0; i < valueLength; i++) {
                for (int k = 0; k < separator.Length; k++) {
                    if (value[i] == separator[k][0]) {
                        if (value.Substring(i, separator[k].Length).
                            Equals(separator[k])) {
                            targetPoint = i;
                            i = i + separator[k].Length - 1;
                            isEqual = true;
                            break;
                        }
                    }
                }
                if (isEqual == true) {
                    buffer.Add(value.Substring(starPoint, targetPoint - starPoint));
                    starPoint = i + 1;
                    isEqual = false;
                }
            }

            buffer.Add(value.Substring(starPoint, valueLength - starPoint));
            return buffer.ToArray();
        }

        private static int GetContainCharNum(this string value, char separator) {
            int number = 0;
            int valueLength = value.Length;
            for (int i = 0; i < valueLength; i++) {
                if (separator == value[i]) {
                    ++number;
                }
            }

            return number;
        }
        #endregion

    }
}