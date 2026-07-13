using System.IO;
using UnityEngine;

public static class CSVImporter
{
    public static Dataset Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"CSV not found: {path}");
            return null;
        }

        string[] lines = File.ReadAllLines(path);
       
        if (lines.Length < 2)
        {
            Debug.LogError("CSV contains no data.");
            return null;
        }

        Dataset dataset =
            new Dataset(
                Path.GetFileNameWithoutExtension(path));

        //-----------------------------------
        // HEADER
        //-----------------------------------

        string[] headers =
            lines[0].Split(',');

        for (int i = 0; i < headers.Length; i++)
        {
            string header =
                headers[i].Trim();

            dataset.Columns.Add(
                new DatasetColumn(header));

            dataset.ColumnMapping[header] = i;
        }

        //-----------------------------------
        // PASS 1
        //-----------------------------------

        for (int lineIndex = 1;
            lineIndex < lines.Length;
            lineIndex++)
        {
            string[] values =
                lines[lineIndex].Split(',');

            DatasetRow row =
                new DatasetRow(headers.Length);

            for (int col = 0;
                col < headers.Length;
                col++)
            {
                string value =
                    col < values.Length
                    ? values[col]
                    : "";

                row.SetRawValue(col, value);

                DatasetColumn column =
                    dataset.Columns[col];

                column.UniqueValues.Add(value);

                if (float.TryParse(value,
                    out float numeric))
                {
                    column.MinValue =
                        Mathf.Min(
                            column.MinValue,
                            numeric);

                    column.MaxValue =
                        Mathf.Max(
                            column.MaxValue,
                            numeric);
                }
            }

            dataset.Rows.Add(row);
        }

        //-----------------------------------
        // DETERMINE TYPES
        //-----------------------------------

        InferColumnTypes(dataset);

        //-----------------------------------
        // PASS 2
        //-----------------------------------

        dataset.FinalizeDatasetMetadata();

        //-----------------------------------
        // PASS 3
        //-----------------------------------

        BakeValues(dataset);

        return dataset;
    }

    private static void InferColumnTypes(
        Dataset dataset)
    {
        foreach (var column in dataset.Columns)
        {
            bool numeric = true;

            int columnIndex =
                dataset.GetColumnIndex(column.Name);

            foreach (var row in dataset.Rows)
            {
                if (!float.TryParse(
                        row.GetRawValue(columnIndex),
                        out _))
                {
                    numeric = false;
                    break;
                }
            }

            column.Type =
                numeric
                ? DataValueType.Numeric
                : DataValueType.Categorical;
        }
    }

    private static void BakeValues(
        Dataset dataset)
    {
        for (int rowIndex = 0;
            rowIndex < dataset.RowCount;
            rowIndex++)
        {
            DatasetRow row =
                dataset.Rows[rowIndex];

            for (int col = 0;
                col < dataset.ColumnCount;
                col++)
            {
                DatasetColumn column =
                    dataset.Columns[col];

                string raw =
                    row.GetRawValue(col);

                if (float.TryParse(raw,
                    out float value))
                {
                    row.SetNumericValue(
                        col,
                        value);
                }

                row.SetNormalizedValue(
                    col,
                    column.GetNormalizedValue(raw));
            }
        }
    }
}