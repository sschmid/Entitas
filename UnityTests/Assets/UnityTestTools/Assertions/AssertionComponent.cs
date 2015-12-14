using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace UnityTest
{
    [Serializable]
    public class AssertionComponent : MonoBehaviour, IAssertionComponentConfigurator
    {
        [SerializeField] public float checkAfterTime = 1f;
        [SerializeField] public bool repeatCheckTime = true;
        [SerializeField] public float repeatEveryTime = 1f;
        [SerializeField] public int checkAfterFrames = 1;
        [SerializeField] public bool repeatCheckFrame = true;
        [SerializeField] public int repeatEveryFrame = 1;
        [SerializeField] public bool hasFailed;

        [SerializeField] public CheckMethod checkMethods = CheckMethod.Start;
        [SerializeField] private ActionBase m_ActionBase;

        [SerializeField] public int checksPerformed = 0;

        private int m_CheckOnFrame;

        private string m_CreatedInFilePath = "";
        private int m_CreatedInFileLine = -1;

        public ActionBase Action
        {
            get { return m_ActionBase; }
            set
            {
                m_ActionBase = value;
                m_ActionBase.go = gameObject;
            }
        }

        public Object GetFailureReferenceObject()
        {
            #if UNITY_EDITOR
            if (!string.IsNullOrEmpty(m_CreatedInFilePath))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath(m_CreatedInFilePath, typeof(Object));
            }
            #endif
            return this;
        }

        public string GetCreationLocation()
        {
            if (!string.IsNullOrEmpty(m_CreatedInFilePath))
            {
                var idx = m_CreatedInFilePath.LastIndexOf("\\") + 1;
                return string.Format("{0}, line {1} ({2})", m_CreatedInFilePath.Substring(idx), m_CreatedInFileLine, m_CreatedInFilePath);
            }
            return "";
        }

        public void Awake()
        {
            if (!Debug.isDebugBuild)
                Destroy(this);
            OnComponentCopy();
        }

        public void OnValidate()
        {
            if (Application.isEditor)
                OnComponentCopy();
        }

        private void OnComponentCopy()
        {
            if (m_ActionBase == null) return;
            var oldActionList = Resources.FindObjectsOfTypeAll(typeof(AssertionComponent)).Where(o => ((AssertionComponent)o).m_ActionBase == m_ActionBase && o != this);

            // if it's not a copy but a new component don't do anything
            if (!oldActionList.Any()) return;
            if (oldActionList.Count() > 1)
                Debug.LogWarning("More than one refence to comparer found. This shouldn't happen");

            var oldAction = oldActionList.First() as AssertionComponent;
            m_ActionBase = oldAction.m_ActionBase.CreateCopy(oldAction.gameObject, gameObject);
        }

        public void Start()
        {
            CheckAssertionFor(CheckMethod.Start);

            if (IsCheckMethodSelected(CheckMethod.AfterPeriodOfTime))
            {
                StartCoroutine("CheckPeriodically");
            }
            if (IsCheckMethodSelected(CheckMethod.Update))
            {
                m_CheckOnFrame = Time.frameCount + checkAfterFrames;
            }
        }

        public IEnumerator CheckPeriodically()
        {
            yield return new WaitForSeconds(checkAfterTime);
            CheckAssertionFor(CheckMethod.AfterPeriodOfTime);
            while (repeatCheckTime)
            {
                yield return new WaitForSeconds(repeatEveryTime);
                CheckAssertionFor(CheckMethod.AfterPeriodOfTime);
            }
        }

        public bool ShouldCheckOnFrame()
        {
            if (Time.frameCount > m_CheckOnFrame)
            {
                if (repeatCheckFrame)
                    m_CheckOnFrame += repeatEveryFrame;
                else
                    m_CheckOnFrame = Int32.MaxValue;
                return true;
            }
            return false;
        }

        public void OnDisable()
        {
            CheckAssertionFor(CheckMethod.OnDisable);
        }

        public void OnEnable()
        {
            CheckAssertionFor(CheckMethod.OnEnable);
        }

        public void OnDestroy()
        {
            CheckAssertionFor(CheckMethod.OnDestroy);
        }

        public void Update()
        {
            if (IsCheckMethodSelected(CheckMethod.Update) && ShouldCheckOnFrame())
            {
                CheckAssertionFor(CheckMethod.Update);
            }
        }

        public void FixedUpdate()
        {
            CheckAssertionFor(CheckMethod.FixedUpdate);
        }

        public void LateUpdate()
        {
            CheckAssertionFor(CheckMethod.LateUpdate);
        }

        public void OnControllerColliderHit()
        {
            CheckAssertionFor(CheckMethod.OnControllerColliderHit);
        }

        public void OnParticleCollision()
        {
            CheckAssertionFor(CheckMethod.OnParticleCollision);
        }

        public void OnJointBreak()
        {
            CheckAssertionFor(CheckMethod.OnJointBreak);
        }

        public void OnBecameInvisible()
        {
            CheckAssertionFor(CheckMethod.OnBecameInvisible);
        }

        public void OnBecameVisible()
        {
            CheckAssertionFor(CheckMethod.OnBecameVisible);
        }

        public void OnTriggerEnter()
        {
            CheckAssertionFor(CheckMethod.OnTriggerEnter);
        }

        public void OnTriggerExit()
        {
            CheckAssertionFor(CheckMethod.OnTriggerExit);
        }

        public void OnTriggerStay()
        {
            CheckAssertionFor(CheckMethod.OnTriggerStay);
        }

        public void OnCollisionEnter()
        {
            CheckAssertionFor(CheckMethod.OnCollisionEnter);
        }

        public void OnCollisionExit()
        {
            CheckAssertionFor(CheckMethod.OnCollisionExit);
        }

        public void OnCollisionStay()
        {
            CheckAssertionFor(CheckMethod.OnCollisionStay);
        }

        public void OnTriggerEnter2D()
        {
            CheckAssertionFor(CheckMethod.OnTriggerEnter2D);
        }

        public void OnTriggerExit2D()
        {
            CheckAssertionFor(CheckMethod.OnTriggerExit2D);
        }

        public void OnTriggerStay2D()
        {
            CheckAssertionFor(CheckMethod.OnTriggerStay2D);
        }

        public void OnCollisionEnter2D()
        {
            CheckAssertionFor(CheckMethod.OnCollisionEnter2D);
        }

        public void OnCollisionExit2D()
        {
            CheckAssertionFor(CheckMethod.OnCollisionExit2D);
        }

        public void OnCollisionStay2D()
        {
            CheckAssertionFor(CheckMethod.OnCollisionStay2D);
        }

        private void CheckAssertionFor(CheckMethod checkMethod)
        {
            if (IsCheckMethodSelected(checkMethod))
            {
                Assertions.CheckAssertions(this);
            }
        }

        public bool IsCheckMethodSelected(CheckMethod method)
        {
            return method == (checkMethods & method);
        }


        #region Assertion Component create methods

        public static T Create<T>(CheckMethod checkOnMethods, GameObject gameObject, string propertyPath) where T : ActionBase
        {
            IAssertionComponentConfigurator configurator;
            return Create<T>(out configurator, checkOnMethods, gameObject, propertyPath);
        }

        public static T Create<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath) where T : ActionBase
        {
            return CreateAssertionComponent<T>(out configurator, checkOnMethods, gameObject, propertyPath);
        }

        public static T Create<T>(CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, GameObject gameObject2, string propertyPath2) where T : ComparerBase
        {
            IAssertionComponentConfigurator configurator;
            return Create<T>(out configurator, checkOnMethods, gameObject, propertyPath, gameObject2, propertyPath2);
        }

        public static T Create<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, GameObject gameObject2, string propertyPath2) where T : ComparerBase
        {
            var comparer = CreateAssertionComponent<T>(out configurator, checkOnMethods, gameObject, propertyPath);
            comparer.compareToType = ComparerBase.CompareToType.CompareToObject;
            comparer.other = gameObject2;
            comparer.otherPropertyPath = propertyPath2;
            return comparer;
        }

        public static T Create<T>(CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, object constValue) where T : ComparerBase
        {
            IAssertionComponentConfigurator configurator;
            return Create<T>(out configurator, checkOnMethods, gameObject, propertyPath, constValue);
        }

        public static T Create<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath, object constValue) where T : ComparerBase
        {
            var comparer = CreateAssertionComponent<T>(out configurator, checkOnMethods, gameObject, propertyPath);
            if (constValue == null)
            {
                comparer.compareToType = ComparerBase.CompareToType.CompareToNull;
                return comparer;
            }
            comparer.compareToType = ComparerBase.CompareToType.CompareToConstantValue;
            comparer.ConstValue = constValue;
            return comparer;
        }

        private static T CreateAssertionComponent<T>(out IAssertionComponentConfigurator configurator, CheckMethod checkOnMethods, GameObject gameObject, string propertyPath) where T : ActionBase
        {
            var ac = gameObject.AddComponent<AssertionComponent>();
            ac.checkMethods = checkOnMethods;
            var comparer = ScriptableObject.CreateInstance<T>();
            ac.Action = comparer;
            ac.Action.go = gameObject;
            ac.Action.thisPropertyPath = propertyPath;
            configurator = ac;

#if !UNITY_METRO
            var stackTrace = new StackTrace(true);
            var thisFileName = stackTrace.GetFrame(0).GetFileName();
            for (int i = 1; i < stackTrace.FrameCount; i++)
            {
                var stackFrame = stackTrace.GetFrame(i);
                if (stackFrame.GetFileName() != thisFileName)
                {
                    string filePath = stackFrame.GetFileName().Substring(Application.dataPath.Length - "Assets".Length);
                    ac.m_CreatedInFilePath = filePath;
                    ac.m_CreatedInFileLine = stackFrame.GetFileLineNumber();
                    break;
                }
            }
#endif  // if !UNITY_METRO
            return comparer;
        }

        #endregion

        #region AssertionComponentConfigurator
        public int UpdateCheckStartOnFrame { set { checkAfterFrames = value; } }
        public int UpdateCheckRepeatFrequency { set { repeatEveryFrame = value; } }
        public bool UpdateCheckRepeat { set { repeatCheckFrame = value; } }
        public float TimeCheckStartAfter { set { checkAfterTime = value; } }
        public float TimeCheckRepeatFrequency { set { repeatEveryTime = value; } }
        public bool TimeCheckRepeat { set { repeatCheckTime = value; } }
        public AssertionComponent Component { get { return this; } }
        #endregion
    }

    public interface IAssertionComponentConfigurator
    {
        /// <summary>
        /// If the assertion is evaluated in Update, after how many frame should the evaluation start. Deafult is 1 (first frame)
        /// </summary>
        int UpdateCheckStartOnFrame { set; }
        /// <summary>
        /// If the assertion is evaluated in Update and UpdateCheckRepeat is true, how many frame should pass between evaluations
        /// </summary>
        int UpdateCheckRepeatFrequency { set; }
        /// <summary>
        /// If the assertion is evaluated in Update, should the evaluation be repeated after UpdateCheckRepeatFrequency frames
        /// </summary>
        bool UpdateCheckRepeat { set; }

        /// <summary>
        /// If the assertion is evaluated after a period of time, after how many seconds the first evaluation should be done
        /// </summary>
        float TimeCheckStartAfter { set; }
        /// <summary>
        /// If the assertion is evaluated after a period of time and TimeCheckRepeat is true, after how many seconds should the next evaluation happen
        /// </summary>
        float TimeCheckRepeatFrequency { set; }
        /// <summary>
        /// If the assertion is evaluated after a period, should the evaluation happen again after TimeCheckRepeatFrequency seconds
        /// </summary>
        bool TimeCheckRepeat { set; }

        AssertionComponent Component { get; }
    }
}
