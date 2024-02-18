using System;
using Newtonsoft.Json;
using UnityEngine;

namespace PlayerProgress
{
    public class ProgressManager
    {
        private const string ProgressPrefsKey = "PlayerProgress";
        
        public ProgressData Data { get; private set; }

        public void Initialize()
        {
            if (!PlayerPrefs.HasKey(ProgressPrefsKey))
            {
                CreateDefaultData();
                return;
            }

            try
            {
                string json = PlayerPrefs.GetString(ProgressPrefsKey);
                Data = JsonConvert.DeserializeObject<ProgressData>(json);
            }
            catch
            {
                CreateDefaultData();
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(Data);
            PlayerPrefs.SetString(ProgressPrefsKey, json);
        }

        private void CreateDefaultData()
        {
            Data = new ProgressData();
        }
    }
}