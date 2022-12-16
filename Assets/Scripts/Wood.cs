using UnityEngine;

public class Wood : MonoBehaviour
{
	public GameObject WoodShatter;
    public AudioSource WoodCollision;
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
        // Escala del wood block: (x,y,z) = (1,6,2) 
        if (destPos != origin)
        {
            minX = groundPos.x - 8.0f;
            maxX = groundPos.x + 9.0f;
            minY = groundPos.y + 2.0f;
            maxY = groundPos.y + 13.0f;
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
        GameManager.Instance.WoodDestruction.Play();
        GameObject shatter = Instantiate(WoodShatter, transform.position, Quaternion.identity);
        GameManager.Instance.AddScore(500, transform.position, Color.white);
        Destroy(shatter, 2);
		Destroy(gameObject);
	}
}