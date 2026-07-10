using System;

[Serializable]
public enum DataValueType
{
    // Generic Types
    Unknown,

    Numeric,
    Categorical,
    Boolean,
    Text,

    // Temporal
    DateTime,
    Duration,

    // Spatial
    CoordinateX,
    CoordinateY,
    CoordinateZ,

    Latitude,
    Longitude,
    Altitude,

    // Relational / Network Data
    EntityId,
    ParentId,
    TargetId,

    // Visual Overrides
    ColorHex,
    Vector3D
}