using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StackOverflow.Snippets {
    public class ColorCodeTools : MonoBehaviour
    {
        public static string colorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }
     
        public static Color hexToColor(string hex)
        {
            try
            {
                hex = hex.Replace ("0x", "");   //in case the string is formatted 0xFFFFFF
                hex = hex.Replace ("#", "");    //in case the string is formatted #FFFFFF
                byte a = 255;   //assume fully visible unless specified in hex
                byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            
                //only use alpha if the string has enough characters
                if(hex.Length == 8){
                    a = byte.Parse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
                }
                return new Color32(r,g,b,a);
            } catch {
                Debug.LogError("Invalid hexadeciamal color code detected in input");
                return new Color32(0,0,0,1);
            }
        }
    }
}