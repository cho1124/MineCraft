using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor.Animations;

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

public class AnimationClipCreatorWindow : EditorWindow
{
    private TextAsset jsonFile;
    private AnimationData animationData;
    private GameObject model;

    [MenuItem("Window/Animation Clip Creator")]
    public static void ShowWindow()
    {
        GetWindow<AnimationClipCreatorWindow>("Animation Clip Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Animation Clip Creator", EditorStyles.boldLabel);

        GUILayout.Label("Drag and drop your JSON file below:");
        jsonFile = (TextAsset)EditorGUILayout.ObjectField(jsonFile, typeof(TextAsset), false);

        GUILayout.Label("Drag and drop your Model below:");
        model = (GameObject)EditorGUILayout.ObjectField(model, typeof(GameObject), true);

        if (jsonFile != null && GUILayout.Button("Parse JSON"))
        {
            ParseJsonFile();
        }

        if (animationData != null && model != null)
        {
            if (GUILayout.Button("Create and Apply Animation Clip"))
            {
                AnimationClip clip = AnimationClipUtility.CreateAnimationClip(animationData);
                if (clip != null)
                {
                    string path = EditorUtility.SaveFilePanelInProject("Save Animation Clip", "New Animation", "anim", "Please enter a file name to save the animation clip to");
                    if (path.Length != 0)
                    {
                        AssetDatabase.CreateAsset(clip, path);
                        AssetDatabase.SaveAssets();

                        ApplyAnimationClipToModel(clip, model);
                    }
                }
                else
                {
                    Debug.LogError("Failed to create Animation Clip. AnimationData is invalid.");
                }
            }
        }
        else
        {
            GUILayout.Label("JSON data has not been parsed or is invalid, or Model is not selected.", EditorStyles.helpBox);
        }
    }

    private void ParseJsonFile()
    {
        if (jsonFile != null)
        {
            string jsonData = jsonFile.text;
            animationData = JsonConvert.DeserializeObject<AnimationData>(jsonData);
            if (animationData != null)
            {
                Debug.Log("JSON data parsed successfully.");
                LogParsedData();
            }
            else
            {
                Debug.LogError("Failed to parse JSON data.");
            }
        }
        else
        {
            Debug.LogWarning("No JSON file selected.");
        }
    }

    private void LogParsedData()
    {
        Debug.Log($"Format Version: {animationData.format_version}");
        Debug.Log($"Frame Length: {animationData.frame_length}");
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

    private void ApplyAnimationClipToModel(AnimationClip clip, GameObject model)
    {
        Animator animator = model.GetComponent<Animator>();
        if (animator == null)
        {
            animator = model.AddComponent<Animator>();
        }

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPathWithClip("Assets/GeneratedController.controller", clip);
        animator.runtimeAnimatorController = controller;
    }
}

public static class AnimationClipUtility
{
    public static AnimationClip CreateAnimationClip(AnimationData animationData)
    {
        if (animationData == null)
        {
            Debug.LogError("AnimationData is null.");
            return null;
        }

        AnimationClip clip = new AnimationClip();
        clip.frameRate = animationData.frame_length;

        foreach (var part in animationData.parts)
        {
            string transformPath = part.name; // Assuming part.name is the path to the transform

            if (part.keyframes == null || part.keyframes.Count == 0)
            {
                Debug.LogWarning($"No keyframes found for part: {part.name}");
                continue;
            }

            // Create position curves
            AnimationCurve posXCurve = new AnimationCurve();
            AnimationCurve posYCurve = new AnimationCurve();
            AnimationCurve posZCurve = new AnimationCurve();

            // Create rotation curves
            AnimationCurve rotXCurve = new AnimationCurve();
            AnimationCurve rotYCurve = new AnimationCurve();
            AnimationCurve rotZCurve = new AnimationCurve();

            foreach (var keyframe in part.keyframes)
            {
                if (keyframe.Value == null)
                {
                    Debug.LogWarning($"Keyframe at time {keyframe.Key} is null for part: {part.name}");
                    continue;
                }

                float time;
                if (!float.TryParse(keyframe.Key, out time))
                {
                    Debug.LogError($"Invalid keyframe time: {keyframe.Key} for part: {part.name}");
                    continue;
                }

                KeyframeData keyframeData = keyframe.Value;

                if (keyframeData.translate == null || keyframeData.rotate == null)
                {
                    Debug.LogWarning($"Translation or rotation data is null for keyframe at time {keyframe.Key} for part: {part.name}");
                    continue;
                }

                posXCurve.AddKey(time, keyframeData.translate[0]);
                posYCurve.AddKey(time, keyframeData.translate[1]);
                posZCurve.AddKey(time, keyframeData.translate[2]);

                rotXCurve.AddKey(time, keyframeData.rotate[0]);
                rotYCurve.AddKey(time, keyframeData.rotate[1]);
                rotZCurve.AddKey(time, keyframeData.rotate[2]);

                Debug.Log($"Added keyframe at time {time} for part {part.name}: " +
                          $"translate = [{keyframeData.translate[0]}, {keyframeData.translate[1]}, {keyframeData.translate[2]}], " +
                          $"rotate = [{keyframeData.rotate[0]}, {keyframeData.rotate[1]}, {keyframeData.rotate[2]}]");
            }

            clip.SetCurve(transformPath, typeof(Transform), "localPosition.x", posXCurve);
            clip.SetCurve(transformPath, typeof(Transform), "localPosition.y", posYCurve);
            clip.SetCurve(transformPath, typeof(Transform), "localPosition.z", posZCurve);

            clip.SetCurve(transformPath, typeof(Transform), "localEulerAnglesRaw.x", rotXCurve);
            clip.SetCurve(transformPath, typeof(Transform), "localEulerAnglesRaw.y", rotYCurve);
            clip.SetCurve(transformPath, typeof(Transform), "localEulerAnglesRaw.z", rotZCurve);

            Debug.Log($"Set animation curves for part {part.name}");
        }

        return clip;
    }
}
