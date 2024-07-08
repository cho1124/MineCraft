using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class JsonTest : MonoBehaviour
{
    [System.Serializable]
    public class KeyframeData
    {
        public float[] translate;
        public float[] rotate;
    }

    [System.Serializable]
    public class Part
    {
        public string name;
        public Dictionary<string, KeyframeData> keyframes;
    }

    [System.Serializable]
    public class AnimationData
    {
        public string format_version;
        public int frame_length;
        public List<Part> parts;
    }

    void Start()
    {
        string jsonData = @"
        {
          ""format_version"": ""0.2"",
          ""frame_length"": 10,
          ""parts"": [
            {
              ""name"": ""root"",
              ""keyframes"": {
                ""0"": {
                  ""translate"": [ 0.0, -0.0, -0.0 ],
                  ""rotate"": [ 0.0, 0.0, -0.0 ]
                },
                ""1"": {
                  ""translate"": [ 0.0, -0.0, -0.0 ],
                  ""rotate"": [ 0.1, 1.0, -3.0 ]
                }
              } 
            }
          ]
        }";

        AnimationData animationData = JsonConvert.DeserializeObject<AnimationData>(jsonData);

        if (animationData != null)
        {
            Debug.Log("JSON data parsed successfully.");
            foreach (var part in animationData.parts)
            {
                Debug.Log($"Part Name: {part.name}");
                if (part.keyframes != null)
                {
                    foreach (var keyframe in part.keyframes)
                    {
                        var keyframeData = keyframe.Value;
                        string translation = string.Join(", ", keyframeData.translate);
                        string rotation = string.Join(", ", keyframeData.rotate);
                        Debug.Log($"  Keyframe Time: {keyframe.Key} - Translate: [{translation}], Rotate: [{rotation}]");
                    }
                }
                else
                {
                    Debug.LogWarning($"Keyframes are null for part: {part.name}");
                }
            }
        }
        else
        {
            Debug.LogError("Failed to parse JSON data.");
        }
    }
}
