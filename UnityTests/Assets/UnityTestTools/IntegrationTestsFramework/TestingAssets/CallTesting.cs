using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class CallTesting : MonoBehaviour
    {
        public enum Functions
        {
            CallAfterSeconds,
            CallAfterFrames,
            Start,
            Update,
            FixedUpdate,
            LateUpdate,
            OnDestroy,
            OnEnable,
            OnDisable,
            OnControllerColliderHit,
            OnParticleCollision,
            OnJointBreak,
            OnBecameInvisible,
            OnBecameVisible,
            OnTriggerEnter,
            OnTriggerExit,
            OnTriggerStay,
            OnCollisionEnter,
            OnCollisionExit,
            OnCollisionStay,
            OnTriggerEnter2D,
            OnTriggerExit2D,
            OnTriggerStay2D,
            OnCollisionEnter2D,
            OnCollisionExit2D,
            OnCollisionStay2D,
        }

        public enum Method
        {
            Pass,
            Fail
        }

        public int afterFrames = 0;
        public float afterSeconds = 0.0f;
        public Functions callOnMethod = Functions.Start;

        public Method methodToCall;
        private int m_StartFrame;
        private float m_StartTime;

        private void TryToCallTesting(Functions invokingMethod)
        {
            if (invokingMethod == callOnMethod)
            {
                if (methodToCall == Method.Pass)
                    IntegrationTest.Pass(gameObject);
                else
                    IntegrationTest.Fail(gameObject);

                afterFrames = 0;
                afterSeconds = 0.0f;
                m_StartTime = float.PositiveInfinity;
                m_StartFrame = int.MinValue;
            }
        }

        public void Start()
        {
            m_StartTime = Time.time;
            m_StartFrame = afterFrames;
            TryToCallTesting(Functions.Start);
        }

        public void Update()
        {
            TryToCallTesting(Functions.Update);
            CallAfterSeconds();
            CallAfterFrames();
        }

        private void CallAfterFrames()
        {
            if (afterFrames > 0 && (m_StartFrame + afterFrames) <= Time.frameCount)
                TryToCallTesting(Functions.CallAfterFrames);
        }

        private void CallAfterSeconds()
        {
            if ((m_StartTime + afterSeconds) <= Time.time)
                TryToCallTesting(Functions.CallAfterSeconds);
        }

        public void OnDisable()
        {
            TryToCallTesting(Functions.OnDisable);
        }

        public void OnEnable()
        {
            TryToCallTesting(Functions.OnEnable);
        }

        public void OnDestroy()
        {
            TryToCallTesting(Functions.OnDestroy);
        }

        public void FixedUpdate()
        {
            TryToCallTesting(Functions.FixedUpdate);
        }

        public void LateUpdate()
        {
            TryToCallTesting(Functions.LateUpdate);
        }

        public void OnControllerColliderHit()
        {
            TryToCallTesting(Functions.OnControllerColliderHit);
        }

        public void OnParticleCollision()
        {
            TryToCallTesting(Functions.OnParticleCollision);
        }

        public void OnJointBreak()
        {
            TryToCallTesting(Functions.OnJointBreak);
        }

        public void OnBecameInvisible()
        {
            TryToCallTesting(Functions.OnBecameInvisible);
        }

        public void OnBecameVisible()
        {
            TryToCallTesting(Functions.OnBecameVisible);
        }

        public void OnTriggerEnter()
        {
            TryToCallTesting(Functions.OnTriggerEnter);
        }

        public void OnTriggerExit()
        {
            TryToCallTesting(Functions.OnTriggerExit);
        }

        public void OnTriggerStay()
        {
            TryToCallTesting(Functions.OnTriggerStay);
        }
        public void OnCollisionEnter()
        {
            TryToCallTesting(Functions.OnCollisionEnter);
        }

        public void OnCollisionExit()
        {
            TryToCallTesting(Functions.OnCollisionExit);
        }

        public void OnCollisionStay()
        {
            TryToCallTesting(Functions.OnCollisionStay);
        }

        public void OnTriggerEnter2D()
        {
            TryToCallTesting(Functions.OnTriggerEnter2D);
        }

        public void OnTriggerExit2D()
        {
            TryToCallTesting(Functions.OnTriggerExit2D);
        }

        public void OnTriggerStay2D()
        {
            TryToCallTesting(Functions.OnTriggerStay2D);
        }

        public void OnCollisionEnter2D()
        {
            TryToCallTesting(Functions.OnCollisionEnter2D);
        }

        public void OnCollisionExit2D()
        {
            TryToCallTesting(Functions.OnCollisionExit2D);
        }

        public void OnCollisionStay2D()
        {
            TryToCallTesting(Functions.OnCollisionStay2D);
        }
    }
}
