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

    private float posXcurve_modifier;
    private float posYcurve_modifier;
    private float posZcurve_modifier;

    private float rotXcurve_modifier;
    private float rotYcurve_modifier;
    private float rotZcurve_modifier;
    private float rotWcurve_modifier;

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

        GUILayout.Label("position curve modifier");
        posXcurve_modifier = (float)EditorGUILayout.FloatField(posXcurve_modifier);
        posYcurve_modifier = (float)EditorGUILayout.FloatField(posYcurve_modifier);
        posZcurve_modifier = (float)EditorGUILayout.FloatField(posZcurve_modifier);

        GUILayout.Label("rotation curve modifier (rotWcurve must be -1 or 1)");
        rotXcurve_modifier = (float)EditorGUILayout.FloatField(rotXcurve_modifier);
        rotYcurve_modifier = (float)EditorGUILayout.FloatField(rotYcurve_modifier);
        rotZcurve_modifier = (float)EditorGUILayout.FloatField(rotZcurve_modifier);
        rotWcurve_modifier = (float)EditorGUILayout.FloatField(rotWcurve_modifier);

        if (jsonFile != null && GUILayout.Button("Parse JSON"))
        {
            ParseJsonFile();
        }

        if (animationData != null && model != null)
        {
            if (GUILayout.Button("Create and Apply Animation Clip"))
            {
                Vector3 pos_curve_modifier = new Vector3(posXcurve_modifier, posYcurve_modifier, posZcurve_modifier);
                Vector4 rot_curve_modifier = new Vector4(rotXcurve_modifier, rotYcurve_modifier, rotZcurve_modifier, rotWcurve_modifier);
                AnimationClip clip = AnimationClipUtility.CreateAnimationClip(animationData, model.transform, pos_curve_modifier, rot_curve_modifier);
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
    public static AnimationClip CreateAnimationClip(AnimationData animationData, Transform rootTransform, Vector3 pos_curve_modifier, Vector4 rot_curve_modifier)
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
            Transform partTransform = rootTransform.Find(transformPath);
            if (partTransform == null)
            {
                Debug.LogError($"Transform not found for path: {transformPath}");
                continue;
            }
            Vector3 originalPosition = partTransform.position;
            Quaternion originalRotation = partTransform.rotation;
            if (part.keyframes == null || part.keyframes.Count == 0)
            {
                Debug.LogWarning($"No keyframes found for part: {part.name}");
                continue;
            }

           

            // Create position curves
            AnimationCurve posXCurve = new AnimationCurve();
            AnimationCurve posYCurve = new AnimationCurve();
            AnimationCurve posZCurve = new AnimationCurve();

            // Create rotation curves (Quaternion)
            AnimationCurve rotXCurve = new AnimationCurve();
            AnimationCurve rotYCurve = new AnimationCurve();
            AnimationCurve rotZCurve = new AnimationCurve();
            AnimationCurve rotWCurve = new AnimationCurve();

            foreach (var keyframe in part.keyframes)
            {
                if (keyframe.Value == null)
                {
                    Debug.LogWarning($"Keyframe at time {keyframe.Key} is null for part: {part.name}");
                    continue;
                }

                if (!float.TryParse(keyframe.Key, out float time))
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

                posXCurve.AddKey(time, originalPosition.x + keyframeData.translate[0] * pos_curve_modifier.x);
                posYCurve.AddKey(time, originalPosition.y + keyframeData.translate[1] * pos_curve_modifier.y);
                posZCurve.AddKey(time, originalPosition.z + keyframeData.translate[2] * pos_curve_modifier.z);

                Quaternion rotation = Quaternion.Euler(keyframeData.rotate[0], keyframeData.rotate[1], keyframeData.rotate[2]);

                rotXCurve.AddKey(time, originalRotation.x + rotation.x * rot_curve_modifier.x);
                rotYCurve.AddKey(time, originalRotation.y + rotation.y * rot_curve_modifier.y);
                rotZCurve.AddKey(time, originalRotation.z + rotation.z * rot_curve_modifier.z);
                rotWCurve.AddKey(time, originalRotation.w + rotation.w * rot_curve_modifier.w);

                Debug.Log($"Added keyframe at time {time} for part {part.name}: " +
                          $"translate = [{keyframeData.translate[0]}, {keyframeData.translate[1]}, {keyframeData.translate[2]}], " +
                          $"rotate = [{keyframeData.rotate[0]}, {keyframeData.rotate[1]}, {keyframeData.rotate[2]}]");
            }

            // Ensure correct property paths are used for setting curves
            clip.SetCurve(transformPath, typeof(Transform), "localPosition.x", posXCurve);
            clip.SetCurve(transformPath, typeof(Transform), "localPosition.y", posYCurve);
            clip.SetCurve(transformPath, typeof(Transform), "localPosition.z", posZCurve);

            clip.SetCurve(transformPath, typeof(Transform), "localRotation.x", rotXCurve);
            clip.SetCurve(transformPath, typeof(Transform), "localRotation.y", rotYCurve);
            clip.SetCurve(transformPath, typeof(Transform), "localRotation.z", rotZCurve);
            clip.SetCurve(transformPath, typeof(Transform), "localRotation.w", rotWCurve);

            Debug.Log($"Set animation curves for part {part.name}");
        }

        return clip;
    }
}

