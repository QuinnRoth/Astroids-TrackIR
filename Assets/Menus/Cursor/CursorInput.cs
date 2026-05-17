using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class CursorInput : MonoBehaviour
{
    public RectTransform cursorTransform;
    public UIDocument[] allMenus;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionReference shootAction;

    private UIDocument activeUIDocument;
    private VisualElement lastHovered;
    private VisualElement pickedElement;
    private float nullElementTimer = 0.25f;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("shootRebind"))
        {
            string rebinds = PlayerPrefs.GetString("shootRebind");
            inputActions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    private void OnEnable()
    {
        if (shootAction != null)
            shootAction.action.Enable();
    }

    private void OnDisable()
    {
        if (shootAction != null)
            shootAction.action.Disable();
    }

    void Update()
    {
        // Pick the TOPMOST active menu (important when multiple menus are enabled)
        activeUIDocument = GetTopmostActiveMenu();
        if (activeUIDocument == null || activeUIDocument.rootVisualElement == null)
            return;

        IPanel panel = activeUIDocument.rootVisualElement.panel;
        if (panel == null)
            return; // panel not ready yet

        // UI Toolkit uses top-left as origin
        Vector2 screenPos = cursorTransform.position;
        screenPos.y = Screen.height - screenPos.y;

        Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(panel, screenPos);

        // Find the element under the cursor (and a small radius)
        VisualElement newPickedElement = PickRadius(panel, panelPos, 30f);

        // 0.25 seconds of null elements before clearing selection (helps stability)
        if (newPickedElement != null)
        {
            pickedElement = newPickedElement;
            nullElementTimer = 0.25f;
        }
        else
        {
            nullElementTimer -= Time.unscaledDeltaTime;
            if (nullElementTimer <= 0f)
                pickedElement = null;
        }

        // Hover effects
        HandleHover(pickedElement);

        // Click when hitting spacebar
        if (shootAction != null && shootAction.action.WasPressedThisFrame() && pickedElement != null)
        {
            using (var submitEvt = NavigationSubmitEvent.GetPooled())
            {
                submitEvt.target = pickedElement;
                pickedElement.SendEvent(submitEvt);
            }
        }
    }

    private UIDocument GetTopmostActiveMenu()
    {
        UIDocument best = null;
        float bestOrder = float.NegativeInfinity;

        if (allMenus == null) return null;

        foreach (var menu in allMenus)
        {
            if (menu == null) continue;
            if (!menu.isActiveAndEnabled) continue;
            if (!menu.gameObject.activeInHierarchy) continue;

            // sortingOrder is int in most Unity versions, but float-safe here avoids cast issues
            float order = menu.sortingOrder;

            if (order >= bestOrder)
            {
                bestOrder = order;
                best = menu;
            }
        }

        return best;
    }

    private VisualElement PickRadius(IPanel panel, Vector2 center, float radius)
    {
        Vector2[] pointsToQuery =
        {
            center,
            center + new Vector2(0, radius),
            center + new Vector2(0, -radius),
            center + new Vector2(radius, 0),
            center + new Vector2(-radius, 0),
            center + new Vector2(radius/2f, radius/2f),
            center + new Vector2(radius/2f, -radius/2f),
            center + new Vector2(-radius/2f, radius/2f),
            center + new Vector2(-radius/2f, -radius/2f),
        };

        foreach (var point in pointsToQuery)
        {
            var element = panel.Pick(point);

            // IMPORTANT: Pick() often returns a child (Label / Fill).
            // Walk upward until we find a Button parent.
            element = FindParentButton(element);

            if (element != null)
                return element;
        }

        return null;
    }

    private VisualElement FindParentButton(VisualElement element)
    {
        while (element != null)
        {
            if (element is Button)
                return element;

            element = element.parent;
        }

        return null;
    }

    private void HandleHover(VisualElement currElement)
    {
        if (currElement == lastHovered)
            return;

        if (lastHovered != null)
        {
            using (var leaveEvt = PointerLeaveEvent.GetPooled())
            {
                leaveEvt.target = lastHovered;
                lastHovered.SendEvent(leaveEvt);
            }
        }

        if (currElement != null)
        {
            using (var enterEvt = PointerEnterEvent.GetPooled())
            {
                enterEvt.target = currElement;
                currElement.SendEvent(enterEvt);
            }
        }

        lastHovered = currElement;
    }
}
