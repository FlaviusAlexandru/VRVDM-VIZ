using System;
using System.IO;
using UnityEngine;

namespace DataViz
{
    public class MultiplayerScatterplotManager : MonoBehaviour
    {
        public static MultiplayerScatterplotManager Instance { get; private set; }

        [Header("Dataset Settings")]
        public string CurrentDatasetName = "test.csv";

        [Header("Visualization Settings")]
        public int XColumnIndex = 0;
        public int YColumnIndex = 1;
        public int ZColumnIndex = 2;
        public int ColorColumnIndex = -1;
        public float PointSize = 0.02f;

        [Header("Loaded Dataset")]
        public Dataset LoadedDataset;

        public event Action OnDatasetLoaded;
        public event Action OnPlotSettingsChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnDestroy()
        {
            Debug.Log($"[MultiplayerScatterplotManager] OnDestroy called, clearing Instance {GetEntityId()}");
        }

        private void Start()
        {
            string[] csvFiles = Directory.GetFiles(
                Application.streamingAssetsPath,
                "*.csv"
            );

            if (csvFiles.Length > 0)
            {
                CurrentDatasetName = Path.GetFileName(csvFiles[0]);
                LoadLocalDataset(CurrentDatasetName);
            }
            else
            {
                Debug.LogError("No CSV files found in StreamingAssets.");
            }
        }

        public void LoadLocalDataset(string fileName)
        {
            string path = Path.Combine(Application.streamingAssetsPath, fileName);

            if (!File.Exists(path))
            {
                Debug.LogError($"Dataset file not found: {path}");
                return;
            }

            LoadedDataset = CSVImporter.Load(path);

            if (LoadedDataset != null)
            {
                CurrentDatasetName = fileName;

                Debug.Log(
                    $"Loaded dataset '{LoadedDataset.Name}' " +
                    $"({LoadedDataset.RowCount} rows, {LoadedDataset.ColumnCount} columns)"
                );

                OnDatasetLoaded?.Invoke();
                OnPlotSettingsChanged?.Invoke();
            }
        }

        public void RequestLoadDatasetRpc(string fileName)
        {
            LoadLocalDataset(fileName);

            if (LoadedDataset != null)
            {
                XColumnIndex = 0;
                YColumnIndex = Mathf.Clamp(
                    1,
                    0,
                    LoadedDataset.ColumnCount - 1
                );

                ZColumnIndex = Mathf.Clamp(
                    2,
                    0,
                    LoadedDataset.ColumnCount - 1
                );

                ColorColumnIndex = -1;

                OnPlotSettingsChanged?.Invoke();
            }
        }

        public void RequestXColumnRpc(int columnIndex)
        {
            XColumnIndex = columnIndex;
            OnPlotSettingsChanged?.Invoke();
        }

        public void RequestYColumnRpc(int columnIndex)
        {
            YColumnIndex = columnIndex;
            OnPlotSettingsChanged?.Invoke();
        }

        public void RequestZColumnRpc(int columnIndex)
        {
            ZColumnIndex = columnIndex;
            OnPlotSettingsChanged?.Invoke();
        }

        public void RequestColorColumnRpc(int columnIndex)
        {
            ColorColumnIndex = columnIndex;
            OnPlotSettingsChanged?.Invoke();
        }

        public void RequestPointSizeRpc(float size)
        {
            PointSize = Mathf.Clamp(size, 0.005f, 0.1f);
            OnPlotSettingsChanged?.Invoke();
        }
    }
}