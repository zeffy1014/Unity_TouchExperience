using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TouchInfo : MonoBehaviour
{
    public int TouchCount { get; set; } = 0;  //タッチ数
    Touch[] touchInfo = new Touch[CommonSettings.ButtonNum];  //保持しておくタッチ情報
 
    public GameObject touchArea;  //タッチ有効範囲指定用
    Rect touchRect;


    // タッチ情報取得
    public bool GetTouch(int index, ref Touch touch)
    {
        // TouchCount数を超えている、または配列範囲外の場合はfalse
        if (TouchCount - 1 < index || touchInfo.Length -1 < index) return false;

        // タッチ情報を渡す
        touch = touchInfo[index];
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        touchRect = touchArea.GetComponent<RectTransform>().rect;

        // 描画先CanvasのScaleに合わせて位置とサイズを変換
        touchRect.x = touchArea.GetComponentInParent<Canvas>().scaleFactor * touchRect.x + Screen.width / 2;
        touchRect.y = touchArea.GetComponentInParent<Canvas>().scaleFactor * touchRect.y + Screen.height / 2;
        touchRect.width *= touchArea.GetComponentInParent<Canvas>().scaleFactor;
        touchRect.height *= touchArea.GetComponentInParent<Canvas>().scaleFactor;

        Debug.Log("touch area: " + touchRect);
        Debug.Log("screen size: " + Screen.width + ", " + Screen.height);

        //if (!PlatformInfo.IsMobile()) touchPos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        // タッチ情報を保持しておく マウスの左クリックもタッチ情報に変換
        if (PlatformInfo.IsMobile())
        {
            // タッチ情報をそのまま保持(最大でボタン数まで)
            TouchCount = Input.touchCount;
            if (CommonSettings.ButtonNum < TouchCount)
            {
                Debug.Log("Input.touchCount <- too many counts...");
                TouchCount = CommonSettings.ButtonNum;
            }
            for (int count = 0; count < TouchCount; count++)
            {
                touchInfo[count] = Input.GetTouch(count);
                // 範囲外だったら離され扱い
                if (!touchRect.Contains(touchInfo[count].position))
                {
                    touchInfo[count].phase = TouchPhase.Ended;
                }
            }
        }
        else
        {
            TouchCount = 0;
            if (Input.GetMouseButtonDown(0))
            {
                // 左クリックされたら位置を確認し、エリア内だったら情報保持
                if (touchRect.Contains(Input.mousePosition))
                {
                    Debug.Log(Input.mousePosition);
                    // 情報保持
                    TouchCount = 1;
                    touchInfo[0].position = Input.mousePosition;
                    touchInfo[0].phase = TouchPhase.Began;
                }
            }
            if (Input.GetMouseButton(0))
            {
                TouchCount = 1;
                touchInfo[0].position = Input.mousePosition;

                // 左クリックされていたら位置を確認
                if (touchRect.Contains(Input.mousePosition))
                {
                    // エリア内だったら移動情報保持
                    TouchCount = 1;
                    touchInfo[0].position = Input.mousePosition;
                    touchInfo[0].phase = TouchPhase.Moved;
                }
                else
                {
                    // エリア外だったら離され情報を保持
                    touchInfo[0].phase = TouchPhase.Ended;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                // 離された情報を保持
                TouchCount = 1;
                touchInfo[0].position = Input.mousePosition;
                touchInfo[0].phase = TouchPhase.Ended;
            }
        }

    }
}
