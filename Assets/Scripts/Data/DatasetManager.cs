using UnityEngine;

public class DatasetManager : MonoBehaviour
{
    public static DatasetManager Instance;

    public Dataset CurrentDataset;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void LoadDataset(string filePath)
    {
        CurrentDataset =
            CSVImporter.Load(filePath);
    }
}