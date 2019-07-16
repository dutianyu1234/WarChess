using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float near = 20.0f;
    public float far = 100.0f;

    //鼠标滚轮灵敏度
    public float sensitivetyMouseWheel = 2f;

    private BoardManager boradScript;
    private int Height, Width;
    private float ScreenHeight, ScreenWidth;

    private float OSize;
    float x_error, y_error;

    private void Start()
    {
        boradScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>();

        Height = boradScript.Height;
        Width = boradScript.Width;

        ScreenHeight = Screen.height;
        ScreenWidth = Screen.width;

        OSize = GetComponent<Camera>().orthographicSize;

        InitCameraPos();
    }

    void Update()
    {
        x_error = transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        y_error = transform.position.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

        transform.position = new Vector3(
        Mathf.Clamp(transform.position.x, OSize * ScreenWidth / ScreenHeight - 0.5f, Width - (OSize * ScreenWidth / ScreenHeight + 0.5f)),
        Mathf.Clamp(transform.position.y, OSize - 0.5f, Height - (OSize + 0.5f)),
        -10);

        //鼠标在屏幕周围时控制视角移动。
        if (x_error > OSize - 0.5f && x_error <OSize + 0.5f)
        {
            Camera.main.transform.Translate(new Vector3(-0.1f, 0, 0));
        }
        if (x_error < -OSize + 0.5f && x_error > -OSize - 0.5f)
        {
            Camera.main.transform.Translate(new Vector3(0.1f, 0, 0));
        }
        if (y_error > OSize - 0.5f && y_error < OSize + 0.5f)
        {
            Camera.main.transform.Translate(new Vector3(0, -0.1f, 0));
        }
        if (y_error < -OSize + 0.5f && y_error > -OSize - 0.5f)
        {
            Camera.main.transform.Translate(new Vector3(0, 0.1f, 0));
        }


        // 滚轮实现镜头缩进和拉远
        if (Input.GetAxis("Mouse ScrollWheel") != 0 )
        {
            if (OSize * 2 * 4/3 > Width || OSize * 2 > Height)
            {
                OSize = Mathf.Min(Width * 3/4, Height) /2;
            }

            else
            {
                this.GetComponent<Camera>().fieldOfView = this.GetComponent<Camera>().fieldOfView - Input.GetAxis("Mouse ScrollWheel") * sensitivetyMouseWheel;
                this.GetComponent<Camera>().fieldOfView = Mathf.Clamp(this.GetComponent<Camera>().fieldOfView, near, far);

                this.GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * sensitivetyMouseWheel;
            }
        }
    }

    public void InitCameraPos()
    {
        gameObject.transform.position = new Vector3(OSize / ScreenHeight * ScreenWidth -0.5f, OSize / 2 + 0.5f, -10);
    }
}