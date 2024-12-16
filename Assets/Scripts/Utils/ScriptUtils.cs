using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    public static class ScriptUtils
    {
        public static bool DoesScriptExist(string scriptName)
        {
            // Search for assets with the specified name and a .cs extension
            string[] guids = AssetDatabase.FindAssets(scriptName + " t:script");

            // Check if any result matches the script name exactly
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

                if (fileName == scriptName)
                {
                    return true; // Script exists
                }
            }

            return false; // Script not found
        }
        
        public static bool DoesScriptExist(string scriptName, out string filePath)
        {
            // Search for assets with the specified name and a .cs extension
            string[] guids = AssetDatabase.FindAssets(scriptName + " t:script");

            // Check if any result matches the script name exactly
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            
                if (fileName == scriptName)
                {
                    filePath = path;
                    return true; // Script exists
                }
            }

            filePath = "";
            return false; // Script not found
        }
        
        public static string GetFileContents(string fileName)
        {
            // Find assets with the specified file name (without the extension)
            string[] guids = AssetDatabase.FindAssets(fileName);

            if (guids.Length > 0)
            {
                // Get the first matching asset's path
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);

                // Load the asset as a TextAsset
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

                if (textAsset != null)
                {
                    return textAsset.text;
                }
                else
                {
                    Debug.LogError($"Found file, but it is not a TextAsset: {assetPath}");
                }
            }
            else
            {
                Debug.LogError($"File '{fileName}' not found in the Asset Database.");
            }

            return null;
        }
    }
}
