using UnityEngine;

public class TPCamera : MonoBehaviour
{
    private GameObject player;
    private Vector3 offset;
    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        offset = transform.position - player.transform.position;
    }
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, 6.0f * Time.deltaTime);
    }
}