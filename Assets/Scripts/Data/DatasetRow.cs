using System;

[Serializable]
public class DatasetRow
{
    /// <summary>
    /// Original imported values.
    /// Useful for labels, tooltips, details-on-demand.
    /// </summary>
    private readonly string[] rawValues;

    /// <summary>
    /// Parsed numeric values.
    /// Avoids float.Parse during rendering.
    /// </summary>
    private readonly float[] numericValues;

    /// <summary>
    /// Normalized values (0-1).
    /// Used directly by visualization code.
    /// </summary>
    private readonly float[] normalizedValues;

    public DatasetRow(int columnCount)
    {
        rawValues = new string[columnCount];

        numericValues = new float[columnCount];

        normalizedValues = new float[columnCount];
    }

    public void SetRawValue(int index, string value)
    {
        rawValues[index] = value;
    }

    public string GetRawValue(int index)
    {
        return rawValues[index];
    }

    public void SetNumericValue(int index, float value)
    {
        numericValues[index] = value;
    }

    public float GetNumericValue(int index)
    {
        return numericValues[index];
    }

    public void SetNormalizedValue(int index, float value)
    {
        normalizedValues[index] = value;
    }

    public float GetNormalizedValue(int index)
    {
        return normalizedValues[index];
    }

    public string[] RawValues => rawValues;

    public float[] NumericValues => numericValues;

    public float[] NormalizedValues => normalizedValues;
}