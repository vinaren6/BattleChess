using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCharachterCollision : MonoBehaviour
{
    public BoxCollider2D playerCollider;
    public BoxCollider2D playerColliderBlocker;
    void Start()
    {
        Physics2D.IgnoreCollision(playerCollider, playerColliderBlocker, true);

    }


}
