using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private Vector2Int? previousTexel;
    private Dictionary<Vector2Int, Color> texelsToDraw;


    void OnEnable()
    {
        drawAction.action.Enable();
        ColorManager.OnColorChanged += this.SetBrushColor;
        BrushManager.OnSizeChanged += this.SetBrushSize;
    }

    void OnDisable()
    {
        drawAction.action.Disable();
        ColorManager.OnColorChanged -= this.SetBrushColor;
        BrushManager.OnSizeChanged -= this.SetBrushSize;
    }

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        this.texelsToDraw = new();

        // Create a copy of the texture.
        runtimeTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        renderer.material.mainTexture = runtimeTexture;

        ClearCanvas();
    }

    void Update()
    {
        // Check if user started drawing.
        float input = drawAction.action.ReadValue<float>();
        if (Mathf.Approximately(input, 0.0f))
            return;

        // Check if brush is pointed at canvas.
        Ray ray = new(brushTransform.position, brushTransform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            this.previousTexel = null;
            return;
        }

        // Calculate the scaled texel coordinate.
        Vector2 texel = hit.textureCoord;
        int x = (int)(texel.x * runtimeTexture.width);
        int y = (int)(texel.y * runtimeTexture.height);
        Vector2Int currentTexel = new(x, y);

        // Did the brush draw on the canvas on the last frame?
        if (previousTexel.HasValue)
        {
            Vector2 prev = previousTexel.Value;
            float distance = Vector2.Distance(prev, currentTexel);
            int steps = Mathf.CeilToInt(distance);

            // Interpolate and draw a line from the previous canvas position.
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                Vector2 interp = Vector2.Lerp(prev, currentTexel, t);
                EnqueueTexel((int)interp.x, (int)interp.y, this.brushSize, this.brushColor);
            }
        }
        else
        {
            // Draw the single texel.
            EnqueueTexel(x, y, this.brushSize, this.brushColor);
        }

        previousTexel = currentTexel;
    }

    void LateUpdate()
    {
        if (this.texelsToDraw == null || this.texelsToDraw.Count == 0)
            return;

        // Draw all enqueued pixels.
        foreach ((var texel, var color) in this.texelsToDraw)
            this.runtimeTexture.SetPixel(texel.x, texel.y, color);

        this.runtimeTexture.Apply();
        this.texelsToDraw.Clear();
    }

    #region Private Methods

    private void EnqueueTexel(int x, int y, int radius, Color color)
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
                    || x2 >= this.runtimeTexture.width
                    || y2 >= this.runtimeTexture.height
                    || rad_ij >= radius)
                    continue;

                Vector2Int texel = new(x2, y2);
                this.texelsToDraw[texel] = color;
            }
        }
    }

    #endregion

    #region Public Methods

    public void SetBrushColor(Color newColor)
    {
        this.brushColor = newColor;
    }

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

    public void SaveCanvasToFile()
    {
        string fileName = "texture" + System.DateTime.Now.ToString("HH-mm-ss") + ".png";
        string filePath = Application.persistentDataPath + fileName;
        var textureBytes = this.runtimeTexture.EncodeToPNG();
        File.WriteAllBytes(filePath, textureBytes);
        Debug.Log($"Canvas successfully written to {filePath}");
    }

    #endregion
}
