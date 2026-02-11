using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Script gắn vào UI Button để xử lý bắn đạn khi click hoặc giữ button.
/// Hỗ trợ cả click (OnClick) và giữ (OnPointerDown/OnPointerUp).
/// </summary>
public class ShootButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Target")]
    [Tooltip("AirplaneController để gọi bắn đạn. Nếu null sẽ tự tìm trong scene.")]
    public AirplaneController airplaneController;

    void Start()
    {
        if (airplaneController == null)
            airplaneController = FindObjectOfType<AirplaneController>();
    }

    // ================= POINTER EVENTS (giữ button) =================

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("IIIIIII");
        if (airplaneController != null)
            airplaneController.StartShooting();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (airplaneController != null)
            airplaneController.StopShooting();
    }

    // ================= CLICK EVENT (click button) =================

    /// <summary>
    /// Gọi từ Button OnClick nếu muốn bắn một lần khi click (không auto-fire).
    /// </summary>
    public void OnShootButtonClick()
    {
        if (airplaneController != null)
            airplaneController.TryShootBullet();
    }
}
