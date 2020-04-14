using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawMesh : Graphic
{
    public GameObject[] panels = new GameObject[CommonSettings.ButtonNum];
    public Text[] showInfo = new Text[CommonSettings.ButtonNum];
    public TouchInfo touchInfo;

    Touch touch = new Touch();  // タッチ情報保持用

    // uGUIでメッシュ生成する際のコールバック
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        var v1 = new Vector2();
        var v2 = new Vector2();
        var v3 = new Vector2();

        // 改めてタッチ情報を取得して描画
        for (int count = 0; count < touchInfo.TouchCount; count++)
        {
            if (touchInfo.GetTouch(count, ref touch))
            {
                if (TouchPhase.Ended == touch.phase)
                {
                    // 離されだったら何も描画しない(パネルの位置に点をうつだけ)

                    v1 = v2 = v3 = panels[count].transform.position;
                    AddVert(vh, v1, ColorSettings.colorSet[count]);
                    AddVert(vh, v2, ColorSettings.colorSet[count]);
                    AddVert(vh, v3, ColorSettings.colorSet[count]);
                    vh.AddTriangle(count * 3, count * 3 + 1, count * 3 + 2);

                    continue;
                }

                // 描画する三角形：
                // v1:タッチ位置
                // v2:各パネル中心から少し角度ずらした位置
                // v3:v2から反対側に少し移動した位置
                float r = Vector2.Distance(touch.position, panels[count].transform.position);
                float rad = Mathf.Atan2(panels[count].transform.position.y - touch.position.y, panels[count].transform.position.x - touch.position.x);

                v1 = touch.position;
                v2 = new Vector2(touch.position.x + r * (Mathf.Cos(rad + 0.05f)), touch.position.y + r * (Mathf.Sin(rad + 0.05f)));
                v3 = v2 + new Vector2(Screen.width/10.0f * (Mathf.Cos(rad - Mathf.PI/2)), Screen.height/10.0f * (Mathf.Sin(rad - Mathf.PI / 2)));
                //Debug.Log("triangle: (" + v1 + "), (" + v2 + "), (" + v3 + ")");

                AddVert(vh, v1, ColorSettings.colorSet[count]);
                AddVert(vh, v2, ColorSettings.colorSet[count]);
                AddVert(vh, v3, ColorSettings.colorSet[count]);

                vh.AddTriangle(count*3, count*3+1, count*3+2);
                //Debug.Log("TouchCount:" + touchInfo.TouchCount + ", " + "Add Triangle No." + count);

            }

        }

    }

    private void AddVert(VertexHelper vh, Vector2 pos, Color color)
    {
        var vert = UIVertex.simpleVert;
        vert.position = pos;
        vert.color = color;
        vh.AddVert(vert);
    }

    protected override void Start()
    {
        // 描画先CanvasのScaleに合わせてローカルのScaleを変更(こうしないと表示がずれる…)
        float canvasScale = this.GetComponentInParent<Canvas>().scaleFactor;
        this.GetComponent<RectTransform>().localScale = new Vector3(1.0f / canvasScale, 1.0f / canvasScale, 1.0f / canvasScale);
        base.Start();
    }

    private void Update()
    {
        bool needRedraw = false;

        // タッチ情報表示 初期化
        foreach (Text info in showInfo)
        {
            info.text = (
                "Phase: ---" + System.Environment.NewLine +
                "Position: ---.-, ---.-" + System.Environment.NewLine +
                "FingerID: --" + System.Environment.NewLine +
                "DeltaPosition: -.--, -.--" +System.Environment.NewLine +
                "DeltaTime: -.---"
                );
        }

        // タッチ(orマウスクリック)情報を取得し、タッチされていたら再描画のフラグを立てる(範囲はタッチ側で見ている)
        for (int count = 0; count < touchInfo.TouchCount; count++)
        {
            if(touchInfo.GetTouch(count, ref touch) && TouchPhase.Ended != touch.phase)
            {
                // パネルの文字列はここで更新する
                showInfo[count].text = (
                    "Phase: " + touch.phase + System.Environment.NewLine +
                    "Position: " + touch.position.x.ToString("f1") + ", " + touch.position.y.ToString("f1") + System.Environment.NewLine +
                    "FingerID: " + touch.fingerId + System.Environment.NewLine +
                    "DeltaPosition: " + touch.deltaPosition.x.ToString("f2") + ", " + touch.deltaPosition.y.ToString("f2") + System.Environment.NewLine +
                    "DeltaTime: " + touch.deltaTime.ToString("f3")
                );
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                    case TouchPhase.Ended:
                        needRedraw = true;
                        break;
                    case TouchPhase.Canceled:
                    default:
                        break;
                }
            }
        }

        // 再描画が必要
        if (needRedraw) SetVerticesDirty();

    }

}

