using System.Text;
using TMPro;
using UnityEngine;

namespace Common
{
    public static class UiUtils
    {
        public static readonly StringBuilder StringBuilder = new();
        
        public static void SetTimerText(TextMeshProUGUI text, float time, ref int currentSeconds)
        {
            int seconds = Mathf.RoundToInt(time);
            if (currentSeconds == seconds || seconds < 0) return;
            currentSeconds = seconds;
            
            SetTimerText(text, time);
        }
        
        public static void SetTimerText(TextMeshProUGUI text, float time)
        {
            int seconds = Mathf.RoundToInt(time);
            int minutes = seconds / 60;
            seconds %= 60;
            
            StringBuilder.Clear();
            StringBuilder.Append(minutes);
            StringBuilder.Append(":");
            if (seconds < 10) StringBuilder.Append("0");
            StringBuilder.Append(seconds);

            text.text = StringBuilder.ToString();
        }
    }
}