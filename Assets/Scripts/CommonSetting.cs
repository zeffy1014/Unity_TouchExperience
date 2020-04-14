using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 共通設定
public static class CommonSettings
{
    // ボタン数
    public static int ButtonNum { get; } = 4;
}

// 色の設定
public static class ColorSettings
{
    // 色の情報
    public enum COLOR
    {
        COLOR_RED,
        COLOR_YELLOW,
        COLOR_BLUE,
        COLOR_GREEN,

        COLOR_NUM
    };

    // 各色の設定値
    public static Color[] colorSet = new Color[(int)COLOR.COLOR_NUM];

    // 静的コンストラクタ
    static ColorSettings()
    {
        // 具体的な値をいれる
        ColorUtility.TryParseHtmlString("#C86464FF", out colorSet[(int)COLOR.COLOR_RED]);
        ColorUtility.TryParseHtmlString("#C8C864FF", out colorSet[(int)COLOR.COLOR_YELLOW]);
        ColorUtility.TryParseHtmlString("#6464C8FF", out colorSet[(int)COLOR.COLOR_BLUE]);
        ColorUtility.TryParseHtmlString("#64C864FF", out colorSet[(int)COLOR.COLOR_GREEN]);


    }

}

// 動作プラットフォーム判断
public class PlatformInfo
{
    static readonly bool isAndroid = Application.platform == RuntimePlatform.Android;
    static readonly bool isIOS = Application.platform == RuntimePlatform.IPhonePlayer;

    public static bool IsMobile()
    {
        // AndroidかiOSか、あるいはUnity RemoteだったらMobile扱いとする
#if UNITY_EDITOR
        bool ret = UnityEditor.EditorApplication.isRemoteConnected;
#else
        bool ret = isAndroid || isIOS;
#endif
        return ret;
    }

}