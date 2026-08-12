using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

public class AtlasImporterTool : EditorWindow
{
    private string jsonPath = "";
    private string textureFolder = "";

    [MenuItem("Tools/Import UI Atlas from JSON")]
    public static void ShowWindow()
    {
        GetWindow<AtlasImporterTool>("Import Atlas");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select your atlas JSON file", EditorStyles.boldLabel);

        // JSON 路径选择
        EditorGUILayout.BeginHorizontal();
        jsonPath = EditorGUILayout.TextField("JSON File", jsonPath);
        if (GUILayout.Button("Browse JSON", GUILayout.Width(100)))
        {
            string selected = EditorUtility.OpenFilePanel("Select atlas JSON", Application.dataPath, "json");
            if (!string.IsNullOrEmpty(selected))
            {
                jsonPath = selected;
                // 自动将纹理文件夹设为 JSON 所在目录
                textureFolder = Path.GetDirectoryName(selected);
            }
        }
        EditorGUILayout.EndHorizontal();

        // 纹理文件夹选择
        EditorGUILayout.BeginHorizontal();
        textureFolder = EditorGUILayout.TextField("Texture Folder", textureFolder);
        if (GUILayout.Button("Browse Folder", GUILayout.Width(100)))
        {
            string folder = EditorUtility.OpenFolderPanel("Select texture folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(folder))
            {
                textureFolder = folder;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        if (GUILayout.Button("Import Atlas", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(jsonPath) || !File.Exists(jsonPath))
            {
                EditorUtility.DisplayDialog("Error", "JSON file not found!", "OK");
                return;
            }
            if (string.IsNullOrEmpty(textureFolder) || !Directory.Exists(textureFolder))
            {
                EditorUtility.DisplayDialog("Error", "Texture folder not found!", "OK");
                return;
            }
            ImportAtlas(jsonPath, textureFolder);
        }
    }

    private void ImportAtlas(string jsonFilePath, string textureFolderPath)
    {
        string jsonContent = File.ReadAllText(jsonFilePath);
        AtlasData atlasData = JsonUtility.FromJson<AtlasData>(jsonContent);

        if (atlasData == null || atlasData.textures == null || atlasData.textures.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Invalid JSON format or no textures found.", "OK");
            return;
        }

        int processed = 0;
        int skipped = 0;

        foreach (var texData in atlasData.textures)
        {
            // 尝试在指定文件夹中查找纹理文件
            string imageFileName = texData.image;
            string fullImagePath = Path.Combine(textureFolderPath, imageFileName);

            // 如果文件不存在，尝试在子文件夹中递归查找（可选）
            if (!File.Exists(fullImagePath))
            {
                // 在 textureFolder 下递归搜索（如果用户不想放在根目录）
                string[] foundFiles = Directory.GetFiles(textureFolderPath, imageFileName, SearchOption.AllDirectories);
                if (foundFiles.Length > 0)
                {
                    fullImagePath = foundFiles[0];
                }
                else
                {
                    Debug.LogWarning($"Texture file not found: {imageFileName} in folder {textureFolderPath}");
                    // 让用户手动选择
                    string manualPath = EditorUtility.OpenFilePanel($"Select missing texture: {imageFileName}", textureFolderPath, "png");
                    if (!string.IsNullOrEmpty(manualPath) && File.Exists(manualPath))
                    {
                        fullImagePath = manualPath;
                    }
                    else
                    {
                        skipped++;
                        continue;
                    }
                }
            }

            // 转换为相对项目路径
            string assetPath = GetRelativeAssetPath(fullImagePath);
            if (string.IsNullOrEmpty(assetPath))
            {
                // 如果纹理不在 Assets 内，无法导入
                Debug.LogError($"Texture is outside Assets folder: {fullImagePath}");
                skipped++;
                continue;
            }

            // 设置纹理导入器
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError($"Not a valid texture asset: {assetPath}");
                skipped++;
                continue;
            }

            // 修改导入设置
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Point;   // 可调
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            // 构建 SpriteMetaData 列表
            List<SpriteMetaData> metaList = new List<SpriteMetaData>();
            foreach (var spriteData in texData.sprites)
            {
                int texHeight = texData.size.h; // 从 JSON 中获取纹理高度
                SpriteMetaData meta = new SpriteMetaData
                {
                    name = Path.GetFileNameWithoutExtension(spriteData.filename),

                    rect = new Rect(
    spriteData.region.x,
    texHeight - spriteData.region.y - spriteData.region.h,
    spriteData.region.w,
    spriteData.region.h
),
                    pivot = new Vector2(0.5f, 0.5f),
                    alignment = (int)SpriteAlignment.Center,
                    border = new Vector4(0, 0, 0, 0)
                };
                metaList.Add(meta);
            }

            importer.spritesheet = metaList.ToArray();
            importer.SaveAndReimport();

            processed++;
            Debug.Log($"Processed: {imageFileName} with {metaList.Count} sprites");
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Imported {processed} texture(s), skipped {skipped} due to missing files.", "OK");
    }

    private string GetRelativeAssetPath(string fullPath)
    {
        // 获取项目的 Assets 文件夹完整路径
        string dataPath = Application.dataPath;

        // 规范化路径，消除 .. 和大小写不一致，统一为当前系统的分隔符
        string full = Path.GetFullPath(fullPath);
        string data = Path.GetFullPath(dataPath);

        // 不区分大小写检查是否在 Assets 目录下
        if (full.StartsWith(data, StringComparison.OrdinalIgnoreCase))
        {
            // 截取相对部分（去掉 Assets 目录本身）
            string relative = full.Substring(data.Length);
            // 去除开头可能的分隔符
            if (relative.StartsWith(Path.DirectorySeparatorChar.ToString()))
                relative = relative.Substring(1);
            // Unity 内部路径使用斜杠 '/'
            return "Assets/" + relative.Replace('\\', '/');
        }

        // 如果不在 Assets 内，返回 null
        return null;
    }

    // ---------- JSON 数据结构 ----------
    [System.Serializable]
    public class AtlasData
    {
        public TextureData[] textures;
    }

    [System.Serializable]
    public class TextureData
    {
        public string image;
        public SizeData size;
        public SpriteData[] sprites;
    }

    [System.Serializable]
    public class SizeData
    {
        public int w;
        public int h;
    }

    [System.Serializable]
    public class SpriteData
    {
        public string filename;
        public RegionData region;
        public MarginData margin;
    }

    [System.Serializable]
    public class RegionData
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }

    [System.Serializable]
    public class MarginData
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }
}