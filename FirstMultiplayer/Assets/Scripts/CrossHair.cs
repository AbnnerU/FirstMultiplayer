
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [SerializeField] private Transform crossHair;
    [SerializeField] private Transform worldObject;
    [SerializeField] private float distance;

    private InputController inputController;
    private Vector3 worldPosition;
    private Vector3 screenPosition;


    void Awake()
    {
        inputController = FindFirstObjectByType<InputController>();

        if (inputController)
            inputController.OnMousePositionUpdate += MousePositionUpdate;

        //Cursor.visible = false;
    }

    private void MousePositionUpdate(Vector2 vector)
    {
        //screenPosition = new Vector2(1920/2, 1080/2);
        screenPosition = vector;
        screenPosition.z = distance;

        crossHair.position = vector;
    }

    private void FixedUpdate()
    {
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldObject.position = worldPosition;
    }
}
