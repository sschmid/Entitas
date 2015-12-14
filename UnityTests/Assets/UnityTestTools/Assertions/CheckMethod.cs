using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    [Flags]
    public enum CheckMethod
    {
        AfterPeriodOfTime       = 1 << 0,
            Start                   = 1 << 1,
            Update                  = 1 << 2,
            FixedUpdate             = 1 << 3,
            LateUpdate              = 1 << 4,
            OnDestroy               = 1 << 5,
            OnEnable                = 1 << 6,
            OnDisable               = 1 << 7,
            OnControllerColliderHit = 1 << 8,
            OnParticleCollision     = 1 << 9,
            OnJointBreak            = 1 << 10,
            OnBecameInvisible       = 1 << 11,
            OnBecameVisible         = 1 << 12,
            OnTriggerEnter          = 1 << 13,
            OnTriggerExit           = 1 << 14,
            OnTriggerStay           = 1 << 15,
            OnCollisionEnter        = 1 << 16,
            OnCollisionExit         = 1 << 17,
            OnCollisionStay         = 1 << 18,
            OnTriggerEnter2D        = 1 << 19,
            OnTriggerExit2D         = 1 << 20,
            OnTriggerStay2D         = 1 << 21,
            OnCollisionEnter2D      = 1 << 22,
            OnCollisionExit2D       = 1 << 23,
            OnCollisionStay2D       = 1 << 24,
    }
}
