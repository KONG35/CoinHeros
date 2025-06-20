using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class UserData : Singleton<UserData>
{
    public bool isInit = false;
    public List<CharacterData> UnitList;

    public CharacterData[] BattleUnit = new CharacterData[6];

    public Camera RenderTextureCamera;
    public RenderTexture texture;

    public void Start()
    {
        Init();
    }
    public void Init()
    {
        if (isInit)
            return;
        UnitList = new List<CharacterData>();
    }

    [Button]
    public async void AddCharacter()
    {
        var CharacterList = DataTableManager.Instance.characterPrefabList;
        int index = UnityEngine.Random.Range(0,CharacterList.Count);
        var Unit = Instantiate(CharacterList[index], this.transform);
        UnitList.Add(Unit);

        await WaitUntilAsync(() => Unit.isInit);
        await Task.Delay(100);
        var tex = RenderTextureCopy();
        Unit.Image = tex;
        Unit._name = UnityEngine.Random.Range(0,999999).ToString();

        LobbyUI Lobby = FindObjectOfType<LobbyUI>();

        Lobby.UnitListUI.SetListItem();
        Unit.gameObject.SetActive(false);
    }

    public Texture2D RenderTextureCopy()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = texture;
        Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = currentRT;

        return tex;
    }
    public async Task WaitUntilAsync(Func<bool> condition, int checkIntervalMs = 100)
    {
        while (!condition())
        {
            await Task.Delay(checkIntervalMs);
        }
    }
}