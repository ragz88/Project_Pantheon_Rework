
/// <summary>
/// Describes the Acceleration and Decelleration to expect from a specific type of Terrain/Medium.
/// </summary>
[System.Serializable]
public struct EnvironmentAcceleration
{
    public TerrainType terrainType;
    public MediumType mediumType;

    public float moveSpeedModifier;
    public float acceleration;
    public float decceleration;
}
