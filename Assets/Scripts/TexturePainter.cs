using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Texture2DExtensions
{
    /// <summary>
    /// Sets the color of a point on a texture and its
    /// surroundingpoints within a given radius.
    /// </summary>
    public static void DrawTexel(this Texture2D texture, int x, int y, int radius, Color color)
    {
        // Get locations of each surrounding pixel.
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                int x2 = x + i;
                int y2 = y + j;

                // Validate bounds of surrounding pixel.
                // Pixel must be within the given circular radius.
                float rad_ij = Mathf.Sqrt(i * i + j * j);

                if (x2 < 0
                    || y2 < 0
                    || x2 >= texture.width
                    || y2 >= texture.height
                    || rad_ij >= radius)
                    continue;

                texture.SetPixel(x2, y2, color);
            }

            texture.Apply();
        }
    }
}

[RequireComponent(typeof(Renderer))]
public class TexturePainter : MonoBehaviour
{
    public Camera cam;

    [Header("Brush Settings")]
    [SerializeField] private Color brushColor = Color.red;
    [SerializeField] private Color canvasBaseColor = Color.white;
    [SerializeField] private int brushSize = 10;
    [SerializeField] private int brushSizeIncrement = 1;
    [SerializeField] private int brushSizeMin = 1;
    [SerializeField] private int brushSizeMax = 20;
    [SerializeField] private InputActionProperty drawAction;
    [SerializeField] private Transform brushTransform;

    private Texture2D runtimeTexture;


    void OnEnable()
    {
        drawAction.action.Enable();
    }

    void OnDisable()
    {
        drawAction.action.Disable();
    }

    void Start()
    {
        var renderer = GetComponent<Renderer>();

        // Create a copy of the texture.
        runtimeTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        renderer.material.mainTexture = runtimeTexture;

        ClearCanvas();
    }

    void Update()
    {
        float input = drawAction.action.ReadValue<float>();
        if (Mathf.Approximately(input, 0.0f))
            return;

        Ray ray = new(brushTransform.position, brushTransform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        // Calculate the scaled texel coordinate.
        Vector2 texel = hit.textureCoord;
        int x = (int)(texel.x * runtimeTexture.width);
        int y = (int)(texel.y * runtimeTexture.height);

        // Draw the texel.
        runtimeTexture.DrawTexel(x, y, this.brushSize, this.brushColor);

    }

    #region Public Methods

    public void SetBrushSize(int size)
    {
        this.brushSize = math.clamp(size, this.brushSizeMin, this.brushSizeMax);
    }

    public void IncrementBrushSize()
    {
        this.SetBrushSize(this.brushSize + this.brushSizeIncrement);
    }

    public void DecrementBrushSize()
    {
        this.SetBrushSize(this.brushSize - this.brushSizeIncrement);
    }

    public void ClearCanvas()
    {
        for (int x = 0; x < runtimeTexture.height; x++)
        {
            for (int y = 0; y < runtimeTexture.width; y++)
            {
                runtimeTexture.SetPixel(x, y, canvasBaseColor);
            }
        }
    }

    #endregion
}
