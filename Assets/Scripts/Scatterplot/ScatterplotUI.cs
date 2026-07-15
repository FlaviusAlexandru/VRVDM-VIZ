using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DataViz
{
    public class ScatterplotUI : MonoBehaviour
    {
        [Header("References")]
        public MultiplayerScatterplotManager m_Manager;

        [Header("UI Controls")]
        public TMP_Dropdown m_DatasetDropdown;
        public TMP_Dropdown m_XColumnDropdown;
        public TMP_Dropdown m_YColumnDropdown;
        public TMP_Dropdown m_ZColumnDropdown;
        public TMP_Dropdown m_ColorColumnDropdown;
        public Slider m_PointSizeSlider;
        public TextMeshProUGUI m_PointSizeValueText;

        private List<string> m_AvailableDatasets = new();
        private bool m_IsUpdatingUI = false;

        private void Start()
        {
            if (m_Manager == null)
                m_Manager = MultiplayerScatterplotManager.Instance;

            // Discover CSV files in StreamingAssets
            DiscoverDatasets();

            // Setup listeners
            if (m_DatasetDropdown != null)
                m_DatasetDropdown.onValueChanged.AddListener(OnDatasetUIChanged);

            if (m_XColumnDropdown != null)
                m_XColumnDropdown.onValueChanged.AddListener(OnXColumnUIChanged);

            if (m_YColumnDropdown != null)
                m_YColumnDropdown.onValueChanged.AddListener(OnYColumnUIChanged);

            if (m_ZColumnDropdown != null)
                m_ZColumnDropdown.onValueChanged.AddListener(OnZColumnUIChanged);

            if (m_ColorColumnDropdown != null)
                m_ColorColumnDropdown.onValueChanged.AddListener(OnColorColumnUIChanged);

            if (m_PointSizeSlider != null)
                m_PointSizeSlider.onValueChanged.AddListener(OnPointSizeUIChanged);

            // Sync with Manager updates
            if (m_Manager != null)
            {
                m_Manager.OnDatasetLoaded += OnDatasetLoadedFromManager;
                m_Manager.OnPlotSettingsChanged += SyncUIWithManager;
            }

            // Perform initial setup
            OnDatasetLoadedFromManager();
            SyncUIWithManager();
        }

        private void OnDestroy()
        {
            if (m_Manager != null)
            {
                m_Manager.OnDatasetLoaded -= OnDatasetLoadedFromManager;
                m_Manager.OnPlotSettingsChanged -= SyncUIWithManager;
            }
        }

        private void DiscoverDatasets()
        {
            if (m_DatasetDropdown == null) return;

            m_AvailableDatasets.Clear();
            m_DatasetDropdown.ClearOptions();

            string saPath = Application.streamingAssetsPath;
            if (Directory.Exists(saPath))
            {
                string[] files = Directory.GetFiles(saPath, "*.csv");
                foreach (string file in files)
                {
                    m_AvailableDatasets.Add(Path.GetFileName(file));
                }
            }

            // Fallback default in case streaming assets is empty
            if (m_AvailableDatasets.Count == 0)
            {
                m_AvailableDatasets.Add("iris.csv");
                m_AvailableDatasets.Add("cars.csv");
            }

            m_DatasetDropdown.AddOptions(m_AvailableDatasets);
        }

        private void OnDatasetLoadedFromManager()
        {
            if (m_Manager == null || m_Manager.LoadedDataset == null) return;

            // Populate the X, Y, Z, Color column dropdown options
            List<string> columns = new();

            Debug.Log($"[ScatterplotUI] Dataset loaded: {m_Manager.CurrentDatasetName}, Columns: {string.Join(", ", columns)}");

            foreach (var col in m_Manager.LoadedDataset.Columns)
            {

                Debug.Log(col.Name);
                columns.Add(col.Name);
            }

            m_XColumnDropdown.ClearOptions();
            m_XColumnDropdown.AddOptions(columns);

            m_YColumnDropdown.ClearOptions();
            m_YColumnDropdown.AddOptions(columns);

            m_ZColumnDropdown.ClearOptions();
            m_ZColumnDropdown.AddOptions(columns);

            m_ColorColumnDropdown.ClearOptions();
            List<string> colorOptions = new() { "None (Default)" };
            colorOptions.AddRange(columns);
            m_ColorColumnDropdown.AddOptions(colorOptions);

            SyncUIWithManager();


        }

        private void SyncUIWithManager()
        {
            if (m_Manager == null || m_IsUpdatingUI) return;

            m_IsUpdatingUI = true;

            // Sync Dataset
            string currentDataset = m_Manager.CurrentDatasetName.ToString();
            int datasetIdx = m_AvailableDatasets.IndexOf(currentDataset);
            if (datasetIdx >= 0 && m_DatasetDropdown != null)
            {
                m_DatasetDropdown.value = datasetIdx;
            }

            // Sync Column dropdown values
            if (m_XColumnDropdown != null) m_XColumnDropdown.value = m_Manager.XColumnIndex;
            if (m_YColumnDropdown != null) m_YColumnDropdown.value = m_Manager.YColumnIndex;
            if (m_ZColumnDropdown != null) m_ZColumnDropdown.value = m_Manager.ZColumnIndex;
            if (m_ColorColumnDropdown != null) m_ColorColumnDropdown.value = m_Manager.ColorColumnIndex + 1; // +1 due to "None" option

            // Sync Point Size
            if (m_PointSizeSlider != null)
            {
                m_PointSizeSlider.value = m_Manager.PointSize;
            }
            if (m_PointSizeValueText != null)
            {
                m_PointSizeValueText.text = m_Manager.PointSize.ToString("F3");
            }

            m_IsUpdatingUI = false;
        }

        #region UI Change Event Handlers

        private void OnDatasetUIChanged(int idx)
        {
            if (m_IsUpdatingUI || m_Manager == null) return;
            string selectedFile = m_AvailableDatasets[idx];
            m_Manager.RequestLoadDatasetRpc(selectedFile);
        }

        private void OnXColumnUIChanged(int idx)
        {
            if (m_IsUpdatingUI || m_Manager == null) return;
            m_Manager.RequestXColumnRpc(idx);
        }

        private void OnYColumnUIChanged(int idx)
        {
            if (m_IsUpdatingUI || m_Manager == null) return;
            m_Manager.RequestYColumnRpc(idx);
        }

        private void OnZColumnUIChanged(int idx)
        {
            if (m_IsUpdatingUI || m_Manager == null) return;
            m_Manager.RequestZColumnRpc(idx);
        }

        private void OnColorColumnUIChanged(int idx)
        {
            if (m_IsUpdatingUI || m_Manager == null) return;
            m_Manager.RequestColorColumnRpc(idx - 1); // -1 maps 0 ("None") to -1 (disabled)
        }

        private void OnPointSizeUIChanged(float val)
        {
            if (m_IsUpdatingUI || m_Manager == null) return;
            m_Manager.RequestPointSizeRpc(val);
        }

        #endregion
    }
}
