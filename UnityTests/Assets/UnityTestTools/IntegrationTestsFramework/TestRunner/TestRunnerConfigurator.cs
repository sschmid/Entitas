#if !UNITY_METRO && !UNITY_WEBPLAYER && (UNITY_PRO_LICENSE || !(UNITY_ANDROID || UNITY_IPHONE))
#define UTT_SOCKETS_SUPPORTED
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityTest.IntegrationTestRunner;
#if UTT_SOCKETS_SUPPORTED
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
#endif

#if UNITY_EDITOR
using UnityEditorInternal;
#endif

namespace UnityTest
{
    public class TestRunnerConfigurator
    {
        public static string integrationTestsNetwork = "networkconfig.txt";
        public static string batchRunFileMarker = "batchrun.txt";
        public static string testScenesToRun = "testscenes.txt";

        public bool isBatchRun { get; private set; }

        public bool sendResultsOverNetwork { get; private set; }

#if UTT_SOCKETS_SUPPORTED
        private readonly List<IPEndPoint> m_IPEndPointList = new List<IPEndPoint>();
#endif

        public TestRunnerConfigurator()
        {
            CheckForBatchMode();
            CheckForSendingResultsOverNetwork();
        }

        public string GetIntegrationTestScenes(int testSceneNum)
        {
            string text;
            if (Application.isEditor)
                text = GetTextFromTempFile(testScenesToRun);
            else
                text = GetTextFromTextAsset(testScenesToRun);

            List<string> sceneList = new List<string>();
            foreach (var line in text.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries))
            {
                sceneList.Add(line.ToString());
            }

            if (testSceneNum < sceneList.Count)
                return sceneList.ElementAt(testSceneNum);
            else
                return null;
        }

        private void CheckForSendingResultsOverNetwork()
        {
#if UTT_SOCKETS_SUPPORTED
            string text;
            if (Application.isEditor)
                text = GetTextFromTempFile(integrationTestsNetwork);
            else
                text = GetTextFromTextAsset(integrationTestsNetwork);

            if (text == null) return;

            sendResultsOverNetwork = true;

            m_IPEndPointList.Clear();

            foreach (var line in text.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries))
            {
                var idx = line.IndexOf(':');
                if (idx == -1) throw new Exception(line);
                var ip = line.Substring(0, idx);
                var port = line.Substring(idx + 1);
                m_IPEndPointList.Add(new IPEndPoint(IPAddress.Parse(ip), Int32.Parse(port)));
            }
#endif  // if UTT_SOCKETS_SUPPORTED
        }

        private static string GetTextFromTextAsset(string fileName)
        {
            var nameWithoutExtension = fileName.Substring(0, fileName.LastIndexOf('.'));
            var resultpathFile = Resources.Load(nameWithoutExtension) as TextAsset;
            return resultpathFile != null ? resultpathFile.text : null;
        }

        private static string GetTextFromTempFile(string fileName)
        {
            string text = null;
            try
            {
#if UNITY_EDITOR && !UNITY_WEBPLAYER
                text = File.ReadAllText(Path.Combine("Temp", fileName));
#endif
            }
            catch
            {
                return null;
            }
            return text;
        }

        private void CheckForBatchMode()
        {
#if IMITATE_BATCH_MODE
            isBatchRun = true;
#elif UNITY_EDITOR
            if (Application.isEditor && InternalEditorUtility.inBatchMode)
                isBatchRun = true;
#else
            if (GetTextFromTextAsset(batchRunFileMarker) != null) isBatchRun = true;
#endif
        }

        public static List<string> GetAvailableNetworkIPs()
        {
#if UTT_SOCKETS_SUPPORTED
            if (!NetworkInterface.GetIsNetworkAvailable()) 
                return new List<String>{IPAddress.Loopback.ToString()};

            var ipList = new List<UnicastIPAddressInformation>();
            var allIpsList = new List<UnicastIPAddressInformation>();

            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                    netInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
                    continue;

                var ipAdresses = netInterface.GetIPProperties().UnicastAddresses
                    .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork);
                allIpsList.AddRange(ipAdresses);

                if (netInterface.OperationalStatus != OperationalStatus.Up) continue;

                ipList.AddRange(ipAdresses);
            }

            //On Mac 10.10 all interfaces return OperationalStatus.Unknown, thus this workaround
            if(!ipList.Any()) return allIpsList.Select(i => i.Address.ToString()).ToList();

            // sort ip list by their masks to predict which ip belongs to lan network
            ipList.Sort((ip1, ip2) =>
                        {
                            var mask1 = BitConverter.ToInt32(ip1.IPv4Mask.GetAddressBytes().Reverse().ToArray(), 0);
                            var mask2 = BitConverter.ToInt32(ip2.IPv4Mask.GetAddressBytes().Reverse().ToArray(), 0);
                            return mask2.CompareTo(mask1);
                        });
            if (ipList.Count == 0)
                return new List<String> { IPAddress.Loopback.ToString() };
            return ipList.Select(i => i.Address.ToString()).ToList();
#else
            return new List<string>();
#endif  // if UTT_SOCKETS_SUPPORTED
        }

        public ITestRunnerCallback ResolveNetworkConnection()
        {
#if UTT_SOCKETS_SUPPORTED
            var nrsList = m_IPEndPointList.Select(ipEndPoint => new NetworkResultSender(ipEndPoint.Address.ToString(), ipEndPoint.Port)).ToList();

            var timeout = TimeSpan.FromSeconds(30);
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime) < timeout)
            {
                foreach (var networkResultSender in nrsList)
                {
                    try
                    {
                        if (!networkResultSender.Ping()) continue;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        sendResultsOverNetwork = false;
                        return null;
                    }
                    return networkResultSender;
                }
                Thread.Sleep(500);
            }
            Debug.LogError("Couldn't connect to the server: " + string.Join(", ", m_IPEndPointList.Select(ipep => ipep.Address + ":" + ipep.Port).ToArray()));
            sendResultsOverNetwork = false;
#endif  // if UTT_SOCKETS_SUPPORTED
            return null;
        }
    }
}
