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
    public Color brushColor = Color.red;
    public Color canvasBaseColor = Color.white;
    public int brushSize = 10;

    private Texture2D runtimeTexture;

    void Start()
    {
        var renderer = GetComponent<Renderer>();

        // Create a coy2 of the texture.
        runtimeTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        renderer.material.mainTexture = runtimeTexture;

        // Draw each pixel of the texture.
        for (int x = 0; x < runtimeTexture.height; x++)
        {
            for (int y = 0; y < runtimeTexture.width; y++)
            {
                runtimeTexture.SetPixel(x, y, canvasBaseColor);
            }
        }
        runtimeTexture.Apply();
    }

    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Calculate the scaled texel coordinate.
                Vector2 texel = hit.textureCoord;
                int x = (int)(texel.x * runtimeTexture.width);
                int y = (int)(texel.y * runtimeTexture.height);

                // Draw the texel.
                runtimeTexture.DrawTexel(x, y, this.brushSize, this.brushColor);
            }
        }
    }
}
