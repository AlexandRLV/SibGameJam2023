/*
 * Author: [Bonnate] https://bonnate.tistory.com/
 * License: None (Provided without explicit licensing)
 *
 * Description:
 * This script, "WAVEasyVolumeEditor," is a Unity Editor extension designed for audio professionals and developers working with WAV files. It provides a user-friendly interface to adjust the decibel (dB) levels of WAV audio clips. Key features include:
 *  - Selecting a WAV audio clip from the Unity Project window.
 *  - Setting a target decibel (dB) value to achieve.
 *  - Listening to audio at the specified target dB or processing the audio clip to match the target dB.
 *  - Creating backups of the original audio clips for easy restoration.
 *  - Exporting processed audio clips in WAV format.
 *  - Displaying audio clip details such as maximum dB, sample rate, channels, and length.
 *
 * Notes:
 * - This script assumes that the Unity Editor is being used.
 * - It's provided without explicit licensing, so you are free to use and modify it for your projects.
 * - Please be cautious while adjusting audio levels, as excessive volume adjustments may cause audio clipping or distortion.
 *
 * Usage:
 * 1. Open the Unity Editor.
 * 2. Go to the "Tools" menu and select "Bonnate" > "WAV Easy Volume Editor."
 * 3. In the editor window:
 *    - Choose a WAV audio clip from the Unity Project window (Only .wav files are selectable).
 *    - Set the target decibel (dB) level you want to achieve.
 *    - Click the "Process and Replace" button to adjust the audio clip in place, creating a backup of the original.
 *    - Click the "Process and Export" button to adjust and save the processed audio clip to a new file.
 *    - Use the "Listen" button to preview audio at the specified dB level.
 * 4. The script will display audio clip details, including maximum dB, sample rate, channels, and length.
 *
 * Enjoy enhancing and controlling the audio quality of your WAV files with this tool!
 */

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Bonnate
{

    public class WAVEasyVolumeEditor : EditorWindow
    {
        private AudioClip mAudioClip; // Selected audioclip
        private float mTargetDb = -3.0f; // Target decibel value
        private float mMaxDb; // audioclip's max db

        private Texture2D mListenBtnTexture;

        private string mScriptFoliderPath;
        private string mBackupFolderPath;


        [MenuItem("Tools/Bonnate/WAV Easy Volume Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<WAVEasyVolumeEditor>("WAV Volume Editor");
            window.minSize = new Vector2(300, 210);
            window.maxSize = new Vector2(300, 210);
        }

        private void OnEnable()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            mScriptFoliderPath = Path.GetDirectoryName(scriptPath);

            string imagePath = $"{mScriptFoliderPath}/Images/ListenButtonImage.png";
            mListenBtnTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);

            // Create the backup folder path
            mBackupFolderPath = Path.Combine(mScriptFoliderPath, "_Backups");
        }

        private void OnGUI()
        {
            // Create the backup folder if it doesn't exist
            if (!Directory.Exists(mBackupFolderPath))
            {
                Directory.CreateDirectory(mBackupFolderPath);
                AssetDatabase.Refresh();

                Log($"Backup Folder Created at: {mBackupFolderPath}");
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("WAV File");

            // Find an AudioClip among the selected objects in the Project window
            AudioClip selectedAudioClip = null;
            System.Object[] selectedObjects = Selection.objects;
            if (selectedObjects != null && selectedObjects.Length > 0)
            {
                foreach (var selectedObject in selectedObjects)
                {
                    if (selectedObject is AudioClip)
                    {
                        selectedAudioClip = selectedObject as AudioClip;
                        break; // Use the first found AudioClip
                    }
                }
            }

            if (selectedAudioClip != null)
            {
                // Set to null if the extension is not "wav"
                string assetPath = AssetDatabase.GetAssetPath(selectedAudioClip);
                if (!string.IsNullOrEmpty(assetPath) && !assetPath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                {
                    selectedAudioClip = null;
                }
            }

            // Disable direct object selection in the Project window
            EditorGUI.BeginDisabledGroup(true);
            selectedAudioClip = EditorGUILayout.ObjectField("", selectedAudioClip, typeof(AudioClip), false, GUILayout.Width(180)) as AudioClip;
            EditorGUI.EndDisabledGroup();

            // Update the audioClip
            mAudioClip = selectedAudioClip;

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            // Left alignment
            GUILayout.Label("Target Decibel ");

            // Right alignment
            GUILayout.FlexibleSpace(); // Add right margin
            GUILayout.Label("-");
            mTargetDb = EditorGUILayout.FloatField("", mTargetDb, GUILayout.Width(126));
            mTargetDb = Mathf.Clamp(mTargetDb, 0f, 80f);
            GUILayout.Label("(dB)");

            if (GUILayout.Button(mListenBtnTexture, GUILayout.Width(20), GUILayout.Height(20)))
            {
                ListenToAudio();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Process and Replace"))
            {
                if (mAudioClip == null)
                {
                    Log("Please select a WAV file.");
                }
                else
                {
                    (float[] samples, int channels, int sampleRate) = ProcessAndExport();

                    // Get the AudioClip's path
                    string audioClipPath = AssetDatabase.GetAssetPath(mAudioClip);

                    // Create the backup file path and make a backup
                    if (File.Exists(audioClipPath))
                    {
                        string backupPath = Path.Combine(mScriptFoliderPath, "_Backups", Path.GetFileName(audioClipPath));
                        File.Copy(audioClipPath, $"{backupPath.Replace(".wav", "")}_bak{System.DateTime.Now.ToString("_yyMMdd_HHMMss")}.wav", true);
                    }

                    // Save as a WAV file
                    SaveWav(audioClipPath, samples, channels, sampleRate);

                    // Refresh the asset database
                    AssetDatabase.Refresh();

                    Log(".WAV Asset has been modified.");
                }
            }
            if (GUILayout.Button("Process and Export"))
            {
                if (mAudioClip == null)
                {
                    Log("Please select a WAV file.");
                }
                else
                {
                    (float[] samples, int channels, int sampleRate) = ProcessAndExport();

                    // Get the file path
                    string outputPath = EditorUtility.SaveFilePanel("Save Processed WAV", "", "processed.wav", "wav");
                    if (!string.IsNullOrEmpty(outputPath))
                    {
                        // Save as a WAV file
                        SaveWav(outputPath, samples, channels, sampleRate);

                        Log($".WAV Asset saved at {outputPath}.");                        
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Add Detail Info section
            GUILayout.Label("[Audio Clip Detail Info]");
            if (mAudioClip == null)
            {
                GUILayout.Label($"Select a WAV File from the Project!");
            }
            else
            {
                GUILayout.Label($"Max DB: {GetMaxDB()}");
                GUILayout.Label($"Sample Rate: {mAudioClip.frequency} Hz"); // Sample rate information
                GUILayout.Label($"Channels: {mAudioClip.channels}"); // Channel information
                GUILayout.Label($"Length: {mAudioClip.length:F2} seconds"); // Audio clip length
            }

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Powered by: Bonnate");

            if (GUILayout.Button("Github", GetHyperlinkLabelStyle()))
            {
                OpenURL("https://github.com/bonnate");
            }

            if (GUILayout.Button("Blog", GetHyperlinkLabelStyle()))
            {
                OpenURL("https://bonnate.tistory.com/");
            }

            GUILayout.EndHorizontal();
        }

        private float GetMaxDB()
        {
            if (mAudioClip == null)
            {
                return 0f;
            }

            float[] samples = new float[mAudioClip.samples * mAudioClip.channels];
            mAudioClip.GetData(samples, 0);

            // Find the highest decibel value
            mMaxDb = -Mathf.Infinity;
            for (int i = 0; i < samples.Length; i++)
            {
                float sample = Mathf.Abs(samples[i]);
                float db = 20.0f * Mathf.Log10(sample);
                if (db > mMaxDb)
                {
                    mMaxDb = db;
                }
            }

            return mMaxDb;
        }

        private (float[], int, int) ProcessAndExport()
        {
            int sampleRate = mAudioClip.frequency;
            int channels = mAudioClip.channels; // Get the channel count from the original WAV file

            float[] samples = new float[mAudioClip.samples * channels];
            mAudioClip.GetData(samples, 0);

            // Calculate the multiplier to adjust the decibel value
            float dbDifference = (-mTargetDb) - mMaxDb;
            float multiplier = Mathf.Pow(10.0f, dbDifference / 20.0f);

            // Adjust the decibel value by multiplying the multiplier to all samples
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] *= multiplier;
            }

            // Export as WAV format
            return (samples, channels, sampleRate);
        }

        // Function to save WAV files
        private void SaveWav(string path, float[] samples, int channels, int sampleRate)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                // Write the WAV header
                writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                writer.Write(36 + samples.Length * 2);
                writer.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * 2 * channels);
                writer.Write((ushort)(2 * channels));
                writer.Write((ushort)16);
                writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                writer.Write(samples.Length * 2);

                // Write WAV data
                foreach (float sample in samples)
                {
                    writer.Write((short)(sample * 32767.0f));
                }
            }
        }

        private void ListenToAudio()
        {
            if (mAudioClip == null)
            {
                Log("Please select a WAV file.");
                return;
            }

            AudioSource audioSource = EditorUtility.CreateGameObjectWithHideFlags("AudioSource", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
            audioSource.clip = mAudioClip;

            if (-mTargetDb < mMaxDb)
            {
                // Adjust the volume of the audio source to listen at the specified mTargetDb.
                float maxVolume = Mathf.Pow(10.0f, (-mTargetDb) / 20.0f);
                audioSource.volume = maxVolume;

                audioSource.Play();
            }
            else
            {
                // Retrieve WAV file data using the ProcessAndExport function
                (float[] samples, int channels, int sampleRate) = ProcessAndExport();

                // Convert and play the WAV file as an AudioClip
                AudioClip tempAudioClip = AudioClip.Create("ProcessedAudioClip", samples.Length / channels, channels, sampleRate, false);
                tempAudioClip.SetData(samples, 0);
                audioSource.clip = tempAudioClip;
                audioSource.Play();
            }
        }

        #region _HYPERLINK
        private GUIStyle GetHyperlinkLabelStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = new Color(0f, 0.5f, 1f);
            style.stretchWidth = false;
            style.wordWrap = false;
            return style;
        }

        private void OpenURL(string url)
        {
            EditorUtility.OpenWithDefaultApp(url);
        }
        #endregion

        #region 
        private void Log(string content)
        {
            Debug.Log($"<color=cyan>[WAV Easy Volume Editor]</color> {content}");
        }
        #endregion
    }
}
#endif