using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityTest
{
    [Serializable]
    public class NetworkResultsReceiver : EditorWindow
    {
        public static NetworkResultsReceiver Instance;

        private string m_StatusLabel;
        private TcpListener m_Listener;

        [SerializeField]
        private PlatformRunnerConfiguration m_Configuration;

        private List<ITestResult> m_TestResults = new List<ITestResult>();

        #region steering variables
        private bool m_RunFinished;
        private bool m_Repaint;

        private TimeSpan m_TestTimeout = TimeSpan.Zero;
        private DateTime m_LastMessageReceived;
        private bool m_Running;

        public TimeSpan ReceiveMessageTimeout = TimeSpan.FromSeconds(30);
        private readonly TimeSpan m_InitialConnectionTimeout = TimeSpan.FromSeconds(300);
        private bool m_TestFailed;
        #endregion

        private void AcceptCallback(TcpClient client)
        {
            m_Repaint = true;
            ResultDTO dto;
            try
            {
                m_LastMessageReceived = DateTime.Now;
                using (var stream = client.GetStream())
                {
                    var bf = new DTOFormatter();
                    dto = (ResultDTO)bf.Deserialize(stream);
                    stream.Close();
                }
                client.Close();
            }
            catch (ObjectDisposedException e)
            {
                Debug.LogException(e);
                m_StatusLabel = "Got disconnected";
                return;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }

            switch (dto.messageType)
            {
                case ResultDTO.MessageType.TestStarted:
                    m_StatusLabel = dto.testName;
                    m_TestTimeout = TimeSpan.FromSeconds(dto.testTimeout);
                    break;
                case ResultDTO.MessageType.TestFinished:
                    m_TestResults.Add(dto.testResult);
                    m_TestTimeout = TimeSpan.Zero;
                    if (dto.testResult.Executed && dto.testResult.ResultState != TestResultState.Ignored && !dto.testResult.IsSuccess)
                        m_TestFailed = true;
                    break;
                case ResultDTO.MessageType.RunStarted:
                    m_TestResults = new List<ITestResult>();
                    m_StatusLabel = "Run started: " + dto.loadedLevelName;
                    break;
                case ResultDTO.MessageType.RunFinished:
                    WriteResultsToLog(dto, m_TestResults);
                    if (!string.IsNullOrEmpty(m_Configuration.resultsDir))
                    {
                        var platform = m_Configuration.runInEditor ? "Editor" : m_Configuration.buildTarget.ToString();
                        var resultWriter = new XmlResultWriter(dto.loadedLevelName, platform, m_TestResults.ToArray());
                        try
                        {
                            if (!Directory.Exists(m_Configuration.resultsDir))
                            {
                                Directory.CreateDirectory(m_Configuration.resultsDir);
                            }
                            var filePath = Path.Combine(m_Configuration.resultsDir, dto.loadedLevelName + ".xml");
                            File.WriteAllText(filePath, resultWriter.GetTestResult());
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    break;
            case ResultDTO.MessageType.AllScenesFinished:
                m_Running = false;
                m_RunFinished = true;
                break;
            case ResultDTO.MessageType.Ping:
                    break;
            }
        }

        private void WriteResultsToLog(ResultDTO dto, List<ITestResult> list)
        {
            string result = "Run finished for: " + dto.loadedLevelName;
            var failCount = list.Count(t => t.Executed && !t.IsSuccess);
            if (failCount == 0)
                result += "\nAll tests passed";
            else
                result += "\n" + failCount + " tests failed";

            if (failCount == 0)
                Debug.Log(result);
            else
                Debug.LogWarning(result);
        }

        public void Update()
        {
            if (EditorApplication.isCompiling
                && m_Listener != null)
            {
                m_Running = false;
                m_Listener.Stop();
                return;
            }

            if (m_Running)
            {
                try
                {
                    if (m_Listener != null && m_Listener.Pending())
                    {
                        using (var client = m_Listener.AcceptTcpClient())
                        {
                            AcceptCallback(client);
                            client.Close();
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    m_StatusLabel = "Exception happened: " + e.Message;
                    Repaint();
                    Debug.LogException(e);
                }
            }

            if (m_Running)
            {
                var adjustedtestTimeout =  m_TestTimeout.Add(m_TestTimeout);
                var timeout = ReceiveMessageTimeout > adjustedtestTimeout ? ReceiveMessageTimeout : adjustedtestTimeout;
                if ((DateTime.Now - m_LastMessageReceived) > timeout)
                {
                    Debug.LogError("Timeout when waiting for test results");
                    m_RunFinished = true;
                }
            }
            if (m_RunFinished)
            {
                if (InternalEditorUtility.inBatchMode)
                    EditorApplication.Exit(m_TestFailed ? Batch.returnCodeTestsFailed : Batch.returnCodeTestsOk);
                Close();
            }
            if (m_Repaint) Repaint();
        }

        public void OnEnable()
        {
            minSize = new Vector2(300, 100);
            titleContent = new GUIContent("Test run monitor");
            Instance = this;
            m_StatusLabel = "Initializing...";
            if (EditorApplication.isCompiling) return;
            EnableServer();
        }

        private void EnableServer()
        {
            if (m_Configuration == null) throw new Exception("No result receiver server configuration.");

            var ipAddress = IPAddress.Any;
            if (m_Configuration.ipList != null && m_Configuration.ipList.Count == 1)
                ipAddress = IPAddress.Parse(m_Configuration.ipList.Single());

            var ipAddStr = Equals(ipAddress, IPAddress.Any) ? "[All interfaces]" : ipAddress.ToString();
            
            m_Listener = new TcpListener(ipAddress, m_Configuration.port);
            m_StatusLabel = "Waiting for connection on: " + ipAddStr + ":" + m_Configuration.port;
            
            try
            {
                m_Listener.Start(100);
            }
            catch (SocketException e)
            {
                m_StatusLabel = "Exception happened: " + e.Message;
                Repaint();
                Debug.LogException(e);
            }
            m_Running = true;
            m_LastMessageReceived = DateTime.Now + m_InitialConnectionTimeout;
        }

        public void OnDisable()
        {
            Instance = null;
            if (m_Listener != null)
                m_Listener.Stop();
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Status:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(m_StatusLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Stop"))
            {
                StopReceiver();
                if (InternalEditorUtility.inBatchMode)
                    EditorApplication.Exit(Batch.returnCodeRunError);
            }
        }

        public static void StartReceiver(PlatformRunnerConfiguration configuration)
        {
            var w = (NetworkResultsReceiver)GetWindow(typeof(NetworkResultsReceiver), false);
            w.SetConfiguration(configuration);
            if (!EditorApplication.isCompiling)
            {
                w.EnableServer();
            }
            w.Show(true);
        }

        private void SetConfiguration(PlatformRunnerConfiguration configuration)
        {
            m_Configuration = configuration;
        }

        public static void StopReceiver()
        {
            if (Instance == null) return;
			try{
            	Instance.Close();
			}catch(Exception e){
				Debug.LogException(e);
				DestroyImmediate(Instance);
			}
        }
    }
}
