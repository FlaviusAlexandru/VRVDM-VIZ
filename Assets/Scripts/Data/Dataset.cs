using System;
using System.Collections.Generic;

[Serializable]
public class Dataset
{
    /// <summary>
    /// Dataset name (usually filename).
    /// </summary>
    public string Name;

    /// <summary>
    /// Column metadata.
    /// </summary>
    public List<DatasetColumn> Columns = new();

    /// <summary>
    /// Actual data.
    /// </summary>
    public List<DatasetRow> Rows = new();

    /// <summary>
    /// Fast column lookup.
    /// Example:
    /// "Income" -> 4
    /// </summary>
    public Dictionary<string, int> ColumnMapping = new();

    public int RowCount => Rows.Count;

    public int ColumnCount => Columns.Count;

    public Dataset(string name)
    {
        Name = name;
    }

    public int GetColumnIndex(string columnName)
    {
        return ColumnMapping.TryGetValue(columnName, out int index)
            ? index
            : -1;
    }

    public bool HasColumn(string columnName)
    {
        return ColumnMapping.ContainsKey(columnName);
    }

    public DatasetColumn GetColumn(string columnName)
    {
        int index = GetColumnIndex(columnName);

        if (index < 0)
            return null;

        return Columns[index];
    }

    public DatasetColumn GetColumn(int index)
    {
        if (index < 0 || index >= Columns.Count)
            return null;

        return Columns[index];
    }

    public DatasetRow GetRow(int index)
    {
        if (index < 0 || index >= Rows.Count)
            return null;

        return Rows[index];
    }

    /// <summary>
    /// Builds category lookup tables.
    /// Call after import.
    /// </summary>
    public void FinalizeDatasetMetadata()
    {
        foreach (DatasetColumn column in Columns)
        {
            column.FinalizeMetadata();
        }
    }

    /// <summary>
    /// Returns normalized values for a single column.
    /// Useful for GPU uploads.
    /// </summary>
    public float[] GetNormalizedColumnBuffer(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= ColumnCount)
            return Array.Empty<float>();

        float[] buffer = new float[RowCount];

        for (int i = 0; i < RowCount; i++)
        {
            buffer[i] = Rows[i].GetNormalizedValue(columnIndex);
        }

        return buffer;
    }

    /// <summary>
    /// Generates an interleaved buffer:
    /// X Y Z X Y Z X Y Z...
    /// Perfect for scatterplots.
    /// </summary>
    public float[] GetInterleavedNormalizedBuffer(
        int[] columnIndices)
    {
        int stride = columnIndices.Length;

        float[] buffer =
            new float[RowCount * stride];

        for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
        {
            DatasetRow row = Rows[rowIndex];

            int rowOffset = rowIndex * stride;

            for (int columnSlot = 0;
                columnSlot < stride;
                columnSlot++)
            {
                int columnIndex =
                    columnIndices[columnSlot];

                buffer[rowOffset + columnSlot] =
                    columnIndex >= 0 &&
                    columnIndex < ColumnCount
                        ? row.GetNormalizedValue(columnIndex)
                        : 0f;
            }
        }

        return buffer;
    }
}