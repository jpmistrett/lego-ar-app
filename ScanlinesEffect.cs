using PixoEvent;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Camera/Scanlines Effect")]
public class ScanlinesEffect : MonoBehaviour, IEventHandler {
  public Shader shader;
  public bool shouldRenderEffect;
  private Material _material;

  [Range(0, 25)]
  public float lineWidth = 2f;

  [Range(0, 1)]
  public float displacementSpeed = 0.1f;

  protected Material material {
    get {
      if (_material == null) {
        _material = new Material(shader);
        _material.hideFlags = HideFlags.HideAndDontSave;
      }
      return _material;
    }
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination) {
    if (shader == null)
      return;
    
    if (shouldRenderEffect)
    {
      material.SetFloat("_LineWidth", lineWidth);
      material.SetFloat("_Speed", displacementSpeed);
      Graphics.Blit(source, destination, material, 0);
    }
    else
    {
      Graphics.Blit(source, destination);
    }
  }

  private void OnEnable()
  {
    EventController.Subscribe(this);
  }

  void OnDisable() {
    if (_material) {
      DestroyImmediate(_material);
    }
    EventController.Unsubscribe(this);
  }

  public void OnEvent(EventPayload p)
  {
    if (p.eventName == "CameraEffect")
    {
      shouldRenderEffect = p.payload == "scanlines";
    }
  }
}