using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;
using UnityEngine.UI;
using System.IO;
using Unity.Burst;

public class CreateSprite : MonoBehaviour
{
    [Header("카메라 설정")]
    public Camera renderCamera;
    public RenderTexture renderTexture;
    
    [Header("스프라이트 설정")]
    public string spriteName = "GeneratedSprite";
    public string savePath = "Assets/2.Art/Sprites/UI/";
    
    [Button("Test Capture")]
    void Test()
    {
        Camera cam = renderCamera;
        Texture2D tex = CaptureUtil.CaptureCameraToTexture2D(cam, renderTexture.width, renderTexture.height);

        // 원하는 경로에 저장
        string path = savePath + spriteName+".png";
        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
        Debug.Log("저장 완료: " + path);

        // 저장 후 Asset DB 갱신
        AssetDatabase.Refresh();
    }
}
