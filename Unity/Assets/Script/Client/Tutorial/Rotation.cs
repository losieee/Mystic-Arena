using FullOpaqueVFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private float speed = 5f;
    public AudioSource lobbyBgm;

    private void Start()
    {
        lobbyBgm = GetComponent<AudioSource>();

        if (lobbyBgm != null)
        {
            lobbyBgm.loop = true;
            lobbyBgm.volume = 0.1f;
            lobbyBgm.Play();
        }
    }
    void Update()
    {
        transform.Rotate(0, Time.deltaTime * speed, 0);
    }
}
