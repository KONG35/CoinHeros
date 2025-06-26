using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(GASTagComponent))]
[RequireComponent(typeof(GASAttributeSetComponent))]
[RequireComponent(typeof(GASAbilityComponent))]
[RequireComponent(typeof(GASCueComponent))]
public class CharacterBase : MonoBehaviour
{
    public bool isInit = false;
    public string _name;
    public Texture2D Image;
    protected GASAbilityComponent _ability;
    protected GASAttributeSetComponent _state;
    protected GASCueComponent _cue;
    protected GASTagComponent _tag;

    protected virtual void Start()
    {
        _ability = GetComponent<GASAbilityComponent>();
        _state = GetComponent<GASAttributeSetComponent>();
        _cue = GetComponent<GASCueComponent>();
        _tag = GetComponent<GASTagComponent>();


        isInit = false;

        if (UserData.Instance)
        {
            UserData.Instance.CopyQueue.Enqueue(this);
        }
        StartCoroutine(TextureInit());
    }

    public IEnumerator TextureInit()
    {
        yield return new WaitForEndOfFrame();
        while (UserData.Instance == null)
            yield return new WaitForEndOfFrame();
        while (UserData.Instance.CopyQueue.First() != this)
            yield return new WaitForEndOfFrame();
        transform.localPosition = UserData.Instance.RenderTextureCamera.transform.localPosition - new Vector3(0.97f, 1.06f, 5.5f);
        yield return new WaitForEndOfFrame();
        UserData.Instance.CopyQueue.Dequeue();
        SetTextureCopyToImage();
        transform.localPosition = Vector3.zero;
        isInit = true;
    }

    public float GetState(AttributeDefSO SO)
    {
        return _state.GetValue(SO);
    }
    public void SetModifyState(AttributeDefSO SO, string Key, float value, StackPolicy policy)
    {
        _state.ModifyValue(SO, Key, value, policy);
    }
    public void SetBaseState(AttributeDefSO SO, float value)
    {
        _state.SetBaseValue(SO, value);
    }
    public void SetTextureCopyToImage()
    {
        var texture = UserData.Instance.texture;
        var RenderTextureCamera = UserData.Instance.RenderTextureCamera;
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = texture;
        Image = new Texture2D(texture.width, texture.height, TextureFormat.RGBAFloat, false);
        Image.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;
    }
}
