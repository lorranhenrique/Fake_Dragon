using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; 
    public Vector3 offset;  

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
            transform.rotation = player.rotation;
            Vector3 direction = transform.position - Camera.main.transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
