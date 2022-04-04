using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Poukoute {
    public class CountDownHourView : MonoBehaviour {
        public TextMeshProUGUI txtHours1;
        public TextMeshProUGUI txtHours2;
        public TextMeshProUGUI txtMinutes1;
        public TextMeshProUGUI txtMinutes2;
        public TextMeshProUGUI txtSeconds1;
        public TextMeshProUGUI txtSeconds2;

        private int Hours {
            set {
                if (this.hours != value) {
                    this.hours = value;
                    this.txtHours1.text = (this.hours / 10).ToString();
                    this.txtHours2.text = (this.hours % 10).ToString();
                }
            }
        }
        private int hours = 0;


        private int Minutes {
            set {
                if (this.minutes != value) {
                    this.minutes = value;
                    this.txtMinutes1.text = (this.minutes / 10).ToString();
                    this.txtMinutes2.text = (this.minutes % 10).ToString();
                }
            }
        }
        private int minutes = 0;


        private int Seconds {
            set {
                if (this.seconds != value) {
                    this.seconds = value;
                    this.txtSeconds1.text = (this.seconds / 10).ToString();
                    this.txtSeconds2.text = (this.seconds % 10).ToString();
                }
            }
        }
        private int seconds = 0;

        public void SetContent(int hours, int minutes, int seconds) {
            this.Hours = hours;
            this.Minutes = minutes;
            this.Seconds = seconds;
        }
    }
}
