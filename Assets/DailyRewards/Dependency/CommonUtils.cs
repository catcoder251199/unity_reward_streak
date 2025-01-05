using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace Skylink.GameFramework
{
    public class CommonUtils
    {
        public static IEnumerator LoadTextFileFromStreamingAsset(string filePath, System.Action<bool, string> callbackDone)
        {
            string path = Path.Combine(Application.streamingAssetsPath, filePath);
            Debug.Log("[LoadFile]Read data from file: " + path);
            using (UnityWebRequest req = UnityWebRequest.Get(path))
            {
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    callbackDone?.Invoke(true, req.downloadHandler.text);
                }
                else
                {
                    string error = "[" + req.result + "]" + req.error;
                    callbackDone?.Invoke(false, error);
                }
            }
        }

        public static async UniTask<string> LoadTextFileFromStreamingAssetAsync(string fileName)
        {
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
#if UNITY_ANDROID && !UNITY_EDITOR
            using (UnityWebRequest req = UnityWebRequest.Get(path))
            {
                await req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    return req.downloadHandler.text;
                }
                else
                {
                    return string.Empty;
                }
            }
#else
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            await UniTask.WaitForEndOfFrame();
            return string.Empty;
#endif
        }

        public static string LoadTextFileFromStreamingAsset(string fileName)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return string.Empty;
        }
        public static IEnumerator LoadDataFileFromStreamingAsset(string filePath, System.Action<bool, byte[]> callbackDone)
        {
            string path = Path.Combine(Application.streamingAssetsPath, filePath);
            Debug.Log("[LoadFile]Read data from file: " + path);
            using (UnityWebRequest req = UnityWebRequest.Get(path))
            {
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    callbackDone?.Invoke(true, req.downloadHandler.data);
                }
                else
                {
                    string error = "[" + req.result + "]" + req.error;
                    callbackDone?.Invoke(false, null);
                }
            }
        }
        public static byte[] LoadDataFileFromStreamingAsset(string fileName)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            return null;
        }
        public static int GetNthIndex(string s, char t, int n)
        {
            int count = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public static string GetTermsURL()
        {
            return "https://www.skylinkstudio.com/term-conditions";
        }

        public static string GetPrivacyURL()
        {
            return "https://www.skylinkstudio.com/privacy-policy";
        }

        public static string GetStoreUrl(string appId)
        {
#if UNITY_ANDROID
            return "https://play.google.com/store/apps/details?id=" + appId;
#elif UNITY_IOS
            return "itms-apps://itunes.apple.com/app/id" + appId;
#else
            return "";
#endif
        }
        public static bool is_iOS_14_or_higher()
        {
#if UNITY_IOS && !UNITY_EDITOR
                var (verMajor, verMinor) = get_iOS_version();
                if (verMajor >= 14) return true;
#endif

            return false;
        }

        public static bool is_iOS_14_5_or_higher()
        {
#if UNITY_IOS && !UNITY_EDITOR
                var (verMajor, verMinor) = get_iOS_version();
                if (verMajor * 10 + verMinor >= 145)
                    return true;
#endif

            return false;
        }

#if UNITY_IOS && !UNITY_EDITOR
        private static (int verMajor, int verMinor) get_iOS_version()
        {
            int major = 0;
            int minor = 0;

            string[] v = UnityEngine.iOS.Device.systemVersion.Split('.');

            if (v.Length >= 2)
            {
                major = Int32.Parse(v[0]);
                minor = Int32.Parse(v[1]);
            }

            return (major, minor);
        }
#endif

        public static float Map(float current, float minFrom, float maxFrom, float minTo, float maxTo)
        {
            return minTo + (current - minFrom) / (maxFrom - minFrom) * (maxTo - minTo);
        }

        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            System.Random rand = new System.Random();
            while (n > 1)
            {
                int k = rand.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static Vector3 GetBezierPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float omt = 1 - t;
            float omt2 = omt * omt;
            float t2 = t * t;
            return p0 * (omt2 * omt) +
                    p1 * (3f * omt2 * t) +
                    p2 * (3f * omt * t2) +
                    p3 * (t2 * t);
        }

        public static bool FlipTheCoin() // 50-50 probability
        {
            return UnityEngine.Random.Range(0, 100) >= 50;
        }

        public static string ConvertMoney(int money)
        {
            const int TRILION = 1000000000;
            const int MILION = 1000000;
            const int THOUSAND = 1000;
            const int TENS = 10;
            if (money >= TRILION) return $"{money / TRILION}T";
            else if (money >= MILION * TENS) return $"{money / MILION}M";
            else if (money >= THOUSAND * TENS) return $"{money / THOUSAND}K";
            return $"{money}";
        }
        public static string EncodeBase64(string content)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(content);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string DecodeBase64(string code)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(code);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string ConverDoubleToString(double value)
        {
            return value.ToString("R");
        }

        public static double TryGetDouble(string strValue, double defaultValue = 0d)
        {
            if (!string.IsNullOrEmpty(strValue))
            {
                double result = defaultValue;
                if (double.TryParse(strValue, out result))
                    return result;
            }
            return defaultValue;
        }
        public static string GetTimerString(int totalTimer)
        {
            if (totalTimer < 0) totalTimer = 0;
            int d, h, m, s;
            d = totalTimer / 86400;
            totalTimer %= 86400;
            h = totalTimer / (3600);
            totalTimer %= 3600;
            m = totalTimer / 60;
            s = totalTimer % 60;
            if (d > 0)
            {
                if (h > 0) return string.Format("{0:0}d {1:0}h", d, h);
                else if (m > 0) return string.Format("{0:0}d {1:0}m", d, m);
                else return string.Format("{0:0}d {1:0}s", d, s);
            }
            if (h > 0) return string.Format("{0:00}:{1:00}:{2:00}", h, m, s);
            return string.Format("{0:00}:{1:00}", m, s);
        }
    }
}
