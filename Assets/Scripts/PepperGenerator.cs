using System.Collections;
using UnityEngine;

public class PepperGenerator : MonoBehaviour
{
    public GameObject Pepper; 
    public int pepperLimit = 1; 
    private int pepperCount = 0; 
    public float spawnInterval = 10f; 
    void Start()
    {
        StartCoroutine(SpawnPeppers());
    }

    IEnumerator SpawnPeppers()
    {
        while (true)
        {
            if (pepperCount < pepperLimit)
            {
                SpawnPepper();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPepper()
    {
        GameObject newPepper = Instantiate(Pepper, transform.position, Quaternion.identity);

        pepperCount++;

        newPepper.GetComponent<Pepper>().OnDestroyed += () => pepperCount--;
    }
}
