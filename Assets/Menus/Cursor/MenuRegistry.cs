using UnityEngine;
using UnityEngine.UIElements;

public class MenuRegistry : MonoBehaviour
{
    [SerializeField] private UIDocument[] menus;

    // give the cursor input a list of all the menus
    public UIDocument[] GetMenus() => menus;
}
