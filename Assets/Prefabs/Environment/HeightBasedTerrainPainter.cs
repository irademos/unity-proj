using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Terrain))]
public class HeightBasedTerrainPainter : MonoBehaviour
{
    [System.Serializable]
    public class HeightLayer
    {
        [Tooltip("TerrainLayer used for this height band")]
        public TerrainLayer terrainLayer;

        [Tooltip("Minimum world height (Y) where this layer starts to appear")]
        public float minHeight = 0f;

        [Tooltip("Maximum world height (Y) where this layer is strongest")]
        public float maxHeight = 50f;

        [Tooltip("Feather distance (in world units) for smooth blending at edges")]
        public float feather = 5f;
    }

    public HeightLayer[] layers;

    [Tooltip("Apply automatically in editor when something changes")]
    public bool autoApplyInEditor = false;

    private Terrain _terrain;

    private void OnValidate()
    {
        if (autoApplyInEditor && !Application.isPlaying)
        {
            Apply();
        }
    }

    [ContextMenu("Apply Height Texturing")]
    public void Apply()
    {
        if (_terrain == null)
            _terrain = GetComponent<Terrain>();

        if (_terrain == null || _terrain.terrainData == null)
        {
            Debug.LogError("HeightBasedTerrainPainter: No Terrain / TerrainData found.");
            return;
        }

        var data = _terrain.terrainData;
        var terrainLayers = data.terrainLayers;

        if (terrainLayers == null || terrainLayers.Length == 0)
        {
            Debug.LogError("HeightBasedTerrainPainter: Terrain has no TerrainLayers assigned.");
            return;
        }

        if (layers == null || layers.Length == 0)
        {
            Debug.LogError("HeightBasedTerrainPainter: No HeightLayers configured.");
            return;
        }

        int alphaWidth = data.alphamapWidth;
        int alphaHeight = data.alphamapHeight;
        int numLayers = terrainLayers.Length;

        // Map HeightLayer -> terrain layer index
        int[] layerIndices = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            var tl = layers[i].terrainLayer;
            int idx = System.Array.IndexOf(terrainLayers, tl);
            if (idx < 0)
            {
                Debug.LogWarning($"HeightBasedTerrainPainter: TerrainLayer '{tl?.name}' is not assigned to the Terrain. Skipping this band.");
            }
            layerIndices[i] = idx;
        }

        float[,,] alphamaps = new float[alphaHeight, alphaWidth, numLayers];

        Vector3 terrainSize = data.size;

        for (int y = 0; y < alphaHeight; y++)
        {
            float normY = (float)y / (alphaHeight - 1);
            for (int x = 0; x < alphaWidth; x++)
            {
                float normX = (float)x / (alphaWidth - 1);

                // Get world height at this point
                float worldHeight = data.GetInterpolatedHeight(normX, normY);

                // Compute weights for each terrain layer
                float[] weights = new float[numLayers];

                for (int i = 0; i < layers.Length; i++)
                {
                    int layerIndex = layerIndices[i];
                    if (layerIndex < 0 || layerIndex >= numLayers)
                        continue;

                    HeightLayer hl = layers[i];

                    float w = ComputeBandWeight(worldHeight, hl.minHeight, hl.maxHeight, hl.feather);
                    if (w > 0f)
                        weights[layerIndex] += w;
                }

                // If all weights are zero, default to first terrain layer
                float sum = 0f;
                for (int l = 0; l < numLayers; l++)
                    sum += weights[l];

                if (sum <= 0f)
                {
                    weights[0] = 1f;
                    sum = 1f;
                }

                // Normalize and assign
                for (int l = 0; l < numLayers; l++)
                {
                    alphamaps[y, x, l] = weights[l] / sum;
                }
            }
        }

        data.SetAlphamaps(0, 0, alphamaps);
        Debug.Log("HeightBasedTerrainPainter: Applied height-based texturing.");
    }

    /// <summary>
    /// Computes a smooth weight for a height band with feathered edges.
    /// </summary>
    private float ComputeBandWeight(float height, float min, float max, float feather)
    {
        if (feather <= 0.001f)
        {
            // Hard band
            return (height >= min && height <= max) ? 1f : 0f;
        }

        float innerMin = min;
        float innerMax = max;
        float outerMin = min - feather;
        float outerMax = max + feather;

        if (height <= outerMin || height >= outerMax)
            return 0f;

        if (height >= innerMin && height <= innerMax)
            return 1f;

        // Blend in from below
        if (height > outerMin && height < innerMin)
        {
            return Mathf.InverseLerp(outerMin, innerMin, height);
        }

        // Blend out above
        if (height > innerMax && height < outerMax)
        {
            return Mathf.InverseLerp(outerMax, innerMax, height);
        }

        return 0f;
    }
}
