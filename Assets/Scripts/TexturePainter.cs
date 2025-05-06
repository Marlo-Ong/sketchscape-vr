using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Renderer))]
public class TexturePainter : MonoBehaviour
{
    [Flags]
    public enum Brush
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
    }

    [Header("Brush Settings")]
    [SerializeField] private Color brushColor = Color.red;
    [SerializeField] private Color canvasBaseColor = Color.white;
    [SerializeField] private int brushSize = 10;
    [SerializeField] private int brushSizeIncrement = 1;
    [SerializeField] private int brushSizeMin = 1;
    [SerializeField] private int brushSizeMax = 20;
    [SerializeField] private int canvasResolution = 1080;

    [Header("Controller References")]
    [SerializeField] private Transform leftControllerTransform;
    [SerializeField] private InputActionProperty leftControllerDrawAction;
    [SerializeField] private Transform rightControllerTransform;
    [SerializeField] private InputActionProperty rightControllerDrawAction;

    private Texture2D runtimeTexture;
    private Vector2Int? previousTexel;
    private Dictionary<Vector2Int, Color> texelsToDraw;
    private bool isPainting = false;
    private Brush inputState = Brush.None;

    public static event Action<Brush> OnStartPainting;
    public static event Action<Brush> OnStopPainting;


    void OnEnable()
    {
        leftControllerDrawAction.action.Enable();
        rightControllerDrawAction.action.Enable();
        ColorManager.OnColorChanged += this.SetBrushColor;
        BrushManager.OnSizeChanged += this.SetBrushSize;
    }

    void OnDisable()
    {
        leftControllerDrawAction.action.Disable();
        rightControllerDrawAction.action.Disable();
        ColorManager.OnColorChanged -= this.SetBrushColor;
        BrushManager.OnSizeChanged -= this.SetBrushSize;
    }

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        this.texelsToDraw = new();

        // Create a copy of the texture.
        runtimeTexture = new Texture2D(this.canvasResolution, this.canvasResolution, TextureFormat.RGBA32, false);
        renderer.material.mainTexture = runtimeTexture;

        ClearCanvas();
    }

    void Update()
    {
        // Check input
        bool leftPressed = leftControllerDrawAction.action.IsPressed();
        bool rightPressed = rightControllerDrawAction.action.IsPressed();

        ProcessBrush(Brush.Left, leftPressed, leftControllerTransform);
        ProcessBrush(Brush.Right, rightPressed, rightControllerTransform);

        // Reset previous texel when no brush is drawing
        if (inputState == 0)
            previousTexel = null;
    }

    private void ProcessBrush(Brush brushFlag, bool isPressed, Transform controllerTransform)
    {
        bool wasDrawing = this.inputState.HasFlag(brushFlag);

        if (isPressed)
        {
            Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 texelUV = hit.textureCoord;
                PaintAt(texelUV);

                if (!wasDrawing)
                {
                    this.inputState |= brushFlag;
                    OnStartPainting?.Invoke(inputState);
                }
                return;
            }
        }

        // If here, brush not drawing this frame
        if (wasDrawing)
        {
            this.inputState &= ~brushFlag;
            OnStopPainting?.Invoke(inputState);
        }
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

    private void PaintAt(Vector2 texel)
    {
        // Calculate the scaled texel coordinate.

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
