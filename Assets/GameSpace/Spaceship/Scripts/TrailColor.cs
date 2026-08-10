using UnityEngine;

public class TrailColor : MonoBehaviour
{
    void Awake()
    {
        // No longer using this. Ship trail is now a static orange.
        //Color32 newColor = GetShipColor();
        //Color32 currentColor = gameObject.GetComponent<TrailRenderer>().GetComponent<Renderer>().material.color;
        
        //gameObject.GetComponent<TrailRenderer>().GetComponent<Renderer>().material.color = newColor;
    }

    Color32 GetShipColor()
    {
        int ShipColor_R = 255;
        int ShipColor_G = 255;
        int ShipColor_B = 255;

        if(PlayerPrefs.HasKey("ShipColor_R"))
            ShipColor_R = PlayerPrefs.GetInt("ShipColor_R");
        if(PlayerPrefs.HasKey("ShipColor_G"))
            ShipColor_G = PlayerPrefs.GetInt("ShipColor_G");
        if(PlayerPrefs.HasKey("ShipColor_B"))
            ShipColor_B = PlayerPrefs.GetInt("ShipColor_B");
        
        // print($"Changing trail colour to: {ShipColor_R}, {ShipColor_G}, {ShipColor_B}\n");


        return new Color32((byte)Mathf.Clamp(ShipColor_R, 0, 255),
            (byte)Mathf.Clamp(ShipColor_G, 0, 255),
            (byte)Mathf.Clamp(ShipColor_B, 0, 255),
            255);
    }
}
