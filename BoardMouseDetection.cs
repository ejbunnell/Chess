using UnityEngine;

public class BoardMouseDetection : MonoBehaviour
{
    public bool isClicked;
    private void OnMouseDown()
    {
        isClicked = true;
    }

    public void Reset()
    {
        isClicked = false;
    }
}