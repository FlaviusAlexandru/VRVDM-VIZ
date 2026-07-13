using UnityEngine;

public class DatasetManager : MonoBehaviour
{
    public static DatasetManager Instance;

    public Dataset CurrentDataset;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadDataset(string filePath)
    {
        CurrentDataset =
            CSVImporter.Load(filePath);
    }
}