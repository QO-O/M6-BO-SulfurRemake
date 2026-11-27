using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private bool yAxisOnly = true; 
    private Transform cam;

    void Awake() 
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        Vector3 dir = cam.position - transform.position;
        if (yAxisOnly) dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }
}

