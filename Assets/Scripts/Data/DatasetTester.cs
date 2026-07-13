using UnityEngine;

public class DatasetTester : MonoBehaviour
{
    private void Start()
    {
        string path =
            Application.streamingAssetsPath +
            "/iris.csv";

        Dataset dataset =
            CSVImporter.Load(path);

        Debug.Log(
            $"Rows: {dataset.RowCount}");

        Debug.Log(
            $"Columns: {dataset.ColumnCount}");

        foreach (var column in dataset.Columns)
        {
            Debug.Log(
                $"{column.Name} | " +
                $"{column.Type}");
        }
    }
}