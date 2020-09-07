using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Nimiq;
using HarmonyLib;
using System;

namespace NimiqClientTest
{
    [HarmonyPatch(typeof(WebClient), "UploadString", new Type[] { typeof(string), typeof(string) })]
    class Patcher
    {
        // test data
        public static string TestData { get; set; }
        public static Dictionary<string, object> LatestRequest { get; set; }
        public static string LatestRequestMethod { get; set; }
        public static object[] LatestRequestParams { get; set; }

        public static void DoPatching()
        {
            var harmony = new Harmony("community.nimiq.patch");
            harmony.PatchAll();
        }

        static bool Prefix(WebClient __instance, ref string address, ref string data, ref string __result)
        {
            // store the request parameters.
            LatestRequest = null;
            LatestRequestMethod = null;
            LatestRequestParams = null;

            LatestRequest = JsonSerializer.Deserialize<Dictionary<string, object>>(data);
            LatestRequestMethod = (string)((JsonElement)LatestRequest["method"]).GetObject();
            LatestRequestParams = (object[])((JsonElement)LatestRequest["params"]).GetObject();

            // return the test data.
            __result = TestData;

            return false;
        }
    }
}
