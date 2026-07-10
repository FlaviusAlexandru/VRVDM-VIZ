using System;
using System.Collections.Generic;

[Serializable]
public class DatasetColumn
{
    public string Name;

    public DataValueType Type = DataValueType.Unknown;

    public float MinValue = float.MaxValue;

    public float MaxValue = float.MinValue;

    public HashSet<string> UniqueValues = new();

    private List<string> categoryList;

    private Dictionary<string, int> categoryLookup;

    public int UniqueCount => UniqueValues.Count;

    public bool IsNumeric =>
        Type == DataValueType.Numeric ||
        Type == DataValueType.CoordinateX ||
        Type == DataValueType.CoordinateY ||
        Type == DataValueType.CoordinateZ ||
        Type == DataValueType.Latitude ||
        Type == DataValueType.Longitude ||
        Type == DataValueType.Altitude;

    public bool IsCategorical =>
        Type == DataValueType.Categorical;

    public bool IsTemporal =>
        Type == DataValueType.DateTime ||
        Type == DataValueType.Duration;

    public bool IsSpatial =>
        Type == DataValueType.CoordinateX ||
        Type == DataValueType.CoordinateY ||
        Type == DataValueType.CoordinateZ ||
        Type == DataValueType.Latitude ||
        Type == DataValueType.Longitude ||
        Type == DataValueType.Altitude;

    public DatasetColumn(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Finalizes metadata after import.
    /// Builds lookup tables for category mapping.
    /// </summary>
    public void FinalizeMetadata()
    {
        categoryList = new List<string>(UniqueValues);

        categoryLookup = new Dictionary<string, int>();

        for (int i = 0; i < categoryList.Count; i++)
        {
            categoryLookup[categoryList[i]] = i;
        }
    }

    /// <summary>
    /// Convert a raw value into a normalized 0-1 float.
    /// </summary>
    public float GetNormalizedValue(string rawValue)
    {
        if (IsNumeric)
        {
            if (float.TryParse(rawValue, out float value))
            {
                float range = MaxValue - MinValue;

                if (Math.Abs(range) < 0.00001f)
                    return 0f;

                return (value - MinValue) / range;
            }
        }

        if (IsCategorical)
        {
            if (categoryLookup == null)
                FinalizeMetadata();

            if (!categoryLookup.TryGetValue(rawValue, out int index))
                return 0f;

            if (categoryLookup.Count <= 1)
                return 0f;

            return (float)index / (categoryLookup.Count - 1);
        }

        if (Type == DataValueType.Boolean)
        {
            return rawValue.ToLower() == "true" || rawValue == "1"
                ? 1f
                : 0f;
        }

        return 0f;
    }
}