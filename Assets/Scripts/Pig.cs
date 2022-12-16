using UnityEngine;

public class Pig : MonoBehaviour
{
    public GameObject Smoke;
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
        // Escala del pig: (x,y,z) = (1,1,1) 
        if (destPos != origin)
        {
            minX = groundPos.x - 8.0f;
            maxX = groundPos.x + 9.0f;
            minY = groundPos.y + 0.5f;
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
        //Debug.Log("PIG POSITION: " + GetComponent<Rigidbody>().position);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 5f)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        GameManager.Instance.PigHit.Play();
        GameManager.Instance.PigDestroy.Play();
        GameObject smoke = Instantiate(Smoke, transform.position, Quaternion.identity);
        GameManager.Instance.AddScore(5000, transform.position, Color.green);
        Destroy(smoke, 3);
        Destroy(gameObject);
    }
}