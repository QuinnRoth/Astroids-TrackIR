using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    private bool shootPressedThisFrame = false;


    public static CursorInput Instance { get; private set; } 


    private void Awake()
    {   
        // singleton
        if (Instance != null)
        {
            Destroy(transform.root.gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(transform.root.gameObject);

        shootAction.action.performed += OnShootPerformed;

        shootAction.action.Enable();

        if (PlayerPrefs.HasKey("shootRebind"))
        {
            string rebinds = PlayerPrefs.GetString("shootRebind");
            inputActions.LoadBindingOverridesFromJson(rebinds);
        }
        shootAction.action.actionMap.Enable();

        SetCursorVisible(false);
    }

    private void OnEnable()
    {
        if (Instance != this) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (Instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var registry = FindObjectsByType<MenuRegistry>(FindObjectsInactive.Include)[0];
        if (registry != null)
            allMenus = registry.GetMenus();
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        shootAction.action.performed -= OnShootPerformed;
    }


    private void OnShootPerformed(InputAction.CallbackContext ctx)
    {
        shootPressedThisFrame = true;
    }


    public void SetMenus(UIDocument[] menus)
    {
        allMenus = menus;
    }


    public void SetCursorVisible(bool visible)
    {
        cursorTransform.gameObject.SetActive(visible);
        shootPressedThisFrame = false;
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

        // Find the nearest button on the entire menu to the cursor
        VisualElement newPickedElement = PickNearestButton(activeUIDocument.rootVisualElement, panelPos);

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
        if (shootPressedThisFrame && pickedElement != null)
        {
            using (var submitEvt = NavigationSubmitEvent.GetPooled())
            {
                submitEvt.target = pickedElement;
                pickedElement.SendEvent(submitEvt);
            }
        }
        shootPressedThisFrame = false;
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

        // Collect unique candidate buttons found by sampling points inside the radius.
        var candidates = new HashSet<VisualElement>();

        foreach (var point in pointsToQuery)
        {
            var element = panel.Pick(point);
            element = FindParentButton(element);
            if (element != null)
                candidates.Add(element);
        }

        if (candidates.Count == 0)
            return null;

        // Pick the candidate whose worldBound center is closest to the queried center.
        VisualElement best = null;
        float bestDistSq = float.MaxValue;

        foreach (var cand in candidates)
        {
            var bound = cand.worldBound;
            Vector2 candCenter = new Vector2(bound.x + bound.width * 0.5f, bound.y + bound.height * 0.5f);
            float distSq = (candCenter - center).sqrMagnitude;
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                best = cand;
            }
        }

        return best;
    }

    private VisualElement PickNearestButton(VisualElement root, Vector2 center)
    {
        if (root == null)
            return null;

        var buttons = root.Query<Button>().ToList();
        if (buttons == null || buttons.Count == 0)
            return null;

        VisualElement best = null;
        float bestDistSq = float.MaxValue;

        foreach (var b in buttons)
        {
            var bound = b.worldBound;
            Vector2 candCenter = new Vector2(bound.x + bound.width * 0.5f, bound.y + bound.height * 0.5f);
            float distSq = (candCenter - center).sqrMagnitude;
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                best = b;
            }
        }

        return best;
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
