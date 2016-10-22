using UnityEngine;
using System.Collections;

public class JFA : MonoBehaviour {
    public int width = 128;
    public int height = 128;

    [SerializeField]
    Shader m_shader;
    Material m_material;

    Texture2D m_demoTex;
    RenderTexture m_voronoiRT;
    RenderTexture m_viewRT;

    float m_jumpSize;

	void Start()
    {
        m_jumpSize = (width % 2 == 0) ? width : width + 1;

        m_material = new Material(m_shader);

        m_demoTex = CreateDemoTex(10);
        m_voronoiRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        m_voronoiRT.filterMode = FilterMode.Point;
        m_viewRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        m_viewRT.filterMode = FilterMode.Point;

        Graphics.Blit(m_demoTex, m_voronoiRT, m_material, 0);
        
    }
	
	void Update()
    {
        m_jumpSize *= 0.5f;
        m_jumpSize = Mathf.Ceil(m_jumpSize);
        m_material.SetFloat("_JumpSize", m_jumpSize);
        var tempRT = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBFloat);
        tempRT.filterMode = FilterMode.Point;
        Graphics.Blit(m_voronoiRT, tempRT);
        m_material.SetTexture("_VoronoiTex", tempRT);
        Graphics.Blit((RenderTexture)null, m_voronoiRT, m_material, 1);
        m_jumpSize = (m_jumpSize % 2 == 0) ? m_jumpSize : m_jumpSize + 1;
        RenderTexture.ReleaseTemporary(tempRT);

        Graphics.Blit(m_voronoiRT, m_viewRT, m_material, 2);
    }

    void OnGUI()
    {
        int texSize = width;
        GUI.DrawTexture(new Rect(0, 0, texSize, texSize), m_demoTex);
        GUI.DrawTexture(new Rect(texSize, 0, texSize, texSize), m_voronoiRT);
        GUI.DrawTexture(new Rect(2 * texSize, 0, texSize, texSize), m_viewRT);
    }

    Texture2D CreateDemoTex(int pointNum)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        tex.filterMode = FilterMode.Point;
        // Fill the texture
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                tex.SetPixel(i, j, Color.black);
            }
        }

        // Set random points
        for (int i = 0; i < pointNum; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            tex.SetPixel(x, y, Random.ColorHSV(0, 1, 0, 1, 0, 1, 1, 1));
        }
        tex.Apply();
        return tex;
    }
}
