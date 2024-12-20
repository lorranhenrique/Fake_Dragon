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
            //transform.LookAt(Camera.main.transform);
            Vector3 direction = transform.position - Camera.main.transform.position;
            direction.y = 0; // Mantém a rotação apenas nos eixos X e Z
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
