using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class VisualDebugger : MonoBehaviour
{
    [HideInInspector]
    public bool isDebuggerActive;
    [HideInInspector]
    public bool isVisible = true;
    private static VisualDebugger instance;
    private readonly Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();
    private GUIStyle categoryLabelStyle;
    private GUIStyle valueLabelStyle;
    private GUIStyle styleInCategory;

    public static VisualDebugger Instance
    {
        get
        {
            if (!instance)
            {
                GameObject gameObject = new GameObject(nameof(VisualDebugger));
                instance = gameObject.AddComponent<VisualDebugger>();
                Debug.Log($"Could not find instance of {nameof(VisualDebugger)}, attaching to a new game object!");
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;

        if (Application.isEditor)
        {
            isDebuggerActive = true;
        }

        categoryLabelStyle = new GUIStyle
        {
            normal = new GUIStyleState {textColor = new Color(0.9f, 0.5f, 0f)},
            wordWrap = true
        };
        valueLabelStyle = new GUIStyle
        {
            normal = new GUIStyleState {textColor = new Color(1f, 1f, 1f)},
            margin = new RectOffset(0, 0, 0, 0),
            wordWrap = true
        };
        styleInCategory = new GUIStyle
        {
            padding = new RectOffset(15, 0, 0, 0)
        };
    }

    private void OnGUI()
    {
        if (!isDebuggerActive || !isVisible) return;

        float width = Screen.width / 4f;
        float height = Screen.height / 2f;
        
        GUILayout.BeginArea(new Rect(10, 10, width, height));
        GUILayout.BeginVertical(GUI.skin.box);
        
        foreach (var section in sections)
        {
            GUILayout.Label(section.Key, categoryLabelStyle);
            GUILayout.BeginVertical(styleInCategory);

            foreach (var value in section.Value)
            {
                GUILayout.Label($"{value.Key} : {value.Value}", valueLabelStyle);
            }
            
            GUILayout.EndVertical();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F3))
        {
            isDebuggerActive = !isDebuggerActive;
        }
    }

    public void Set(object category, object name, object value)
    {
        if (!isDebuggerActive) return;
        
        string categoryName = category.ToString();
        
        if (!sections.ContainsKey(categoryName))
        {
            sections[categoryName] = new Dictionary<string, string>();
        }

        sections[categoryName][name.ToString()] = value.ToString();
    }

    public void Remove(object category)
    {
        if (!isDebuggerActive) return;
        
        sections.Remove(category.ToString());
    }

    public void Remove(object category, object name)
    {
        if (!isDebuggerActive) return;
        string categoryName = category.ToString();
        
        if (sections.ContainsKey(categoryName))
        {
            sections[categoryName].Remove(name.ToString());
        }
    }
}