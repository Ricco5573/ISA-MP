using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    public int damage;

    private void Start()
    {
        rb= GetComponent<Rigidbody>();
        StartCoroutine(timerServerRpc());
    }


    private void Update()
    {
        rb.AddForce(this.transform.forward * 900, ForceMode.Force);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Terrain")
        {
            Destroy(gameObject);
        }
        else if (collision.collider.tag == "Player")
        {
            //trigger damage.
            Destroy(gameObject);
        }
    }
    [ServerRpc (RequireOwnership = false)]
    private IEnumerator timerServerRpc()
    {
        yield return new WaitForSecondsRealtime(5);
        this.gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
