using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ZoomInEffect : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float startZoom = 13.0f;
    [SerializeField] private float targetZoom = 11.0f;

    [SerializeField] private UnityEvent endOfZoomEvent;
    private bool inProgress = true;

    private void Start()
    {
        Camera.main.orthographicSize = startZoom;
    }

    void Update()
    {
        if (inProgress)
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetZoom, speed * Time.deltaTime);


            if (Camera.main.orthographicSize - targetZoom < 0.01f)
            {
                inProgress = false;
                endOfZoomEvent.Invoke();
            }
        }
    }
}
