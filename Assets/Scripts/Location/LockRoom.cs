using UnityEngine;

public class LockRoom : MonoBehaviour
{
    private int close;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && close !=1)
        {
            close = 1;
            gameObject.transform.localPosition = new Vector3
                (gameObject.transform.localPosition.x, -4, gameObject.transform.localPosition.z);
        }
    }

}
