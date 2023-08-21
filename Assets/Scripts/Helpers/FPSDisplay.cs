using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public Color color = Color.green;
    public bool hideInBuild;

    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        //
        // if (hideInBuild || !Debug.isDebugBuild)
        //     return;

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(25, 25, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperCenter;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = color;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}