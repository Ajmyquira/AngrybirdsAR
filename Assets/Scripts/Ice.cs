using UnityEngine;

public class Ice : MonoBehaviour
{
    public GameObject IceShatter;
    public Vector3 destPos = new Vector3(0, 0, 0);
    private Vector3 origin = new Vector3(0, 0, 0);
    private Vector3 finalPos;
    private Vector3 actualPos;
    float minX, maxX, minY, maxY, minZ, maxZ;
    public Vector3 groundPos = new Vector3(0, 0, 0); // Escala del ground: (x,y,z) = (2,1,4)
    public bool kinematic = false;

    void FixedUpdate()
    {
        actualPos = GetComponent<Rigidbody>().position;
        finalPos = GetComponent<Rigidbody>().position + destPos;
        // Delimiting the movement in the ground
        // Escala del ice block: (x,y,z) = (1,3,2) 

        if (destPos != origin)
        {
            minX = groundPos.x - 8.0f;
            maxX = groundPos.x + 9.0f;
            minY = groundPos.y + 1.5f;
            maxY = groundPos.y + 12.0f;
            minZ = groundPos.z - 10.0f;
            maxZ = groundPos.z + 15.0f;

            if (finalPos.x < minX) { finalPos.x = minX; }
            else if (finalPos.x > maxX) { finalPos.x = maxX; }
            if (finalPos.y < minY) { finalPos.y = minY; }
            else if (finalPos.y > maxY) { finalPos.y = maxY; }
            if (finalPos.z < minZ) { finalPos.z = minZ; }
            else if (finalPos.z > maxZ) { finalPos.z = maxZ; }
        }

        if (kinematic)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
        }

        GetComponent<Rigidbody>().position = finalPos;
    }

    private void Destroy()
    {
        GameObject shatter = Instantiate(IceShatter, transform.position, Quaternion.identity);
        GameManager.Instance.AddScore(500, transform.position, Color.white);
        GameManager.Instance.IceDestruction.Play();
        Destroy(shatter, 2);
        Destroy(gameObject);
    }
}
