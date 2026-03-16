using System;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MWUtilityScripts
{
    public static class NetworkUtil
    {
        private struct IpAddressResponse
        {
            public string IP;
        }

        public static async Task<IPAddress> GetPublicIp()
        {
            var ipText = JsonUtility.FromJson<IpAddressResponse>(await GetText("https://api.ipify.org?format=json")).IP;
            if (string.IsNullOrWhiteSpace(ipText) || !IPAddress.TryParse(ipText, out var address))
            {
                throw new Exception("Unable to determine IP address");
            }
            return address;
        }

        public static async Task<string> GetText(string uri)
        {
            var uwr = UnityWebRequest.Get(uri);
            uwr.timeout = 5;
            await uwr.SendWebRequest();

            return uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError ? throw new Exception(uwr.error) : uwr.downloadHandler.text;
        }
    }
}
