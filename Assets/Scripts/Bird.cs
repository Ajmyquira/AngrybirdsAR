using System.Collections;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public Rigidbody Rb;
    public GameObject Feathers;
    public GameObject FeatherExplosion;
    public AudioSource Slingshot;
    public AudioSource SlingshotRelease;
    public AudioSource Flying;
    public AudioSource BirdCollision;
    public float ReleaseTime = 0.5f;
    public float DestructionTime = 5f;
    public bool _isPressed;
    private bool _isFired;
    public Vector3 destPos;

    private Vector3 refInitPos;
    private bool firstShot = true;

    void FixedUpdate()
    {
        if (_isPressed && !_isFired && !GameManager.Instance.IsLevelCleared)
        {
            // Capture the initial position
            if (firstShot)
            {
                refInitPos = GetComponent<Transform>().position;
                firstShot = false;
            }
            //Debug.Log("Ref: " + refInitPos);

            Vector3 initPos = GetComponent<Transform>().position;
            //Debug.Log("Cur: " + initPos);
            Vector3 finalPos = initPos + destPos;

            // Delimiting the movement
            float x_s = refInitPos.x + 4;
            float x_i = refInitPos.x - 4;
            float y_s = refInitPos.y + 2;
            float y_i = refInitPos.y - 2;
            float z_s = refInitPos.z + 1;
            float z_i = refInitPos.z - 7;
            if (finalPos.x < x_i) { finalPos.x = x_i; }
            if (finalPos.x > x_s) { finalPos.x = x_s; }
            if (finalPos.y < y_i) { finalPos.y = y_i; }
            if (finalPos.y > y_s) { finalPos.y = y_s; }
            if (finalPos.z < z_i) { finalPos.z = z_i; }
            if (finalPos.z > z_s) { finalPos.z = z_s; }

            Rb.position = finalPos;
            Debug.Log("Rb position: " + Rb.position);
        }
    }

    public void HoldEvent()
    {
        Debug.Log("HoldEvent");
        if (_isFired || GameManager.Instance.IsLevelCleared)
        {
            return;
        }

        _isPressed = true;
        Rb.isKinematic = true;
        Slingshot.Play();
    }
    public void ShootEvent()
    {
        Debug.Log("ShootEvent");
        if (_isFired || GameManager.Instance.IsLevelCleared)
        {
            return;
        }

        _isPressed = false;
        Rb.isKinematic = false;

        GameManager.Instance.ActiveTurn = true;

        GetComponent<TrailRenderer>().enabled = true;
        _isFired = true;
        SlingshotRelease.Play();
        Flying.Play();
        StartCoroutine(Release());
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<TrailRenderer>().enabled = false;
        if (!collision.collider.CompareTag("Ground"))
        {
            GameObject feathers = Instantiate(Feathers, transform.position, Quaternion.identity);
            Destroy(feathers, 2);
            if (!BirdCollision.isPlaying)
            {
                BirdCollision.Play();
            }
            GameManager.Instance.AddScore(Random.Range(5, 25) * 10, transform.position, Color.white);
        }
    }

    IEnumerator Release()
    {
        yield return new WaitForSeconds(ReleaseTime);

        Destroy(GetComponent<SpringJoint>());
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(DestructionTime);

        GameManager.Instance.SetNewBird();
        GameManager.Instance.BirdDestroy.Play();
        Instantiate(FeatherExplosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
