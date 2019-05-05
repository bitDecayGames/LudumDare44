using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class SpriteSheetPostProcessor : AssetPostprocessor {
    private const string SPRITE_SHEET_FOLDER_CONTAINS = "TestSpriteSheets";
    private const string ANIMATION_FOLDER = "Assets/Anim";
    
    private FileMeta currentFileMeta;

    public void OnPreprocessTexture() {
        if (assetPath.Contains(SPRITE_SHEET_FOLDER_CONTAINS)) {
            currentFileMeta = new FileMeta(assetPath);
            TextureImporter textureImporter = (TextureImporter) assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        }
    }

    public void OnPostprocessTexture(Texture2D texture) {
        if (assetPath.Contains(SPRITE_SHEET_FOLDER_CONTAINS) && currentFileMeta != null) {
            Debug.Log("Process sprite sheet: " + currentFileMeta.filepath);
            var sprites = SliceBySize(texture, currentFileMeta.filepath, currentFileMeta.cellWidth, currentFileMeta.cellHeight);

            if (currentFileMeta.parent != null) {
                if (!AssetDatabase.IsValidFolder(ANIMATION_FOLDER)) {
                    var animFolderSplit = ANIMATION_FOLDER.Split('/');
                    var folderId = AssetDatabase.CreateFolder(animFolderSplit[0], animFolderSplit[1]);
                    Debug.Log("Created root folder " + ANIMATION_FOLDER + " " + folderId);
                }

                AnimatorController ctrl = null;
                string ctrlPath = string.Format("{0}/{1}/{1}.controller", ANIMATION_FOLDER, currentFileMeta.parent);
                if (!AssetDatabase.IsValidFolder(string.Format("{0}/{1}", ANIMATION_FOLDER, currentFileMeta.parent))) {
                    var folderId = AssetDatabase.CreateFolder(ANIMATION_FOLDER, currentFileMeta.parent);
                    Debug.Log(string.Format("Created folder {0}/{1} {2}", ANIMATION_FOLDER, currentFileMeta.parent, folderId));
                    
                    ctrl = CreateAnimationController(ctrlPath);
                }

                if (ctrl == null) {
                    ctrl = GetAnimatorController(ctrlPath);
                }

                CreateClip(string.Format("{0}/{1}/{2}.anim", ANIMATION_FOLDER, currentFileMeta.parent, currentFileMeta.name), currentFileMeta.name, sprites, ctrl);
            }
        }
    }

    private List<Sprite> SliceBySize(Texture2D texture, string filepath, int spriteWidth, int spriteHeight) {
        int colCount = texture.width / spriteWidth;
        int rowCount = texture.height / spriteHeight;

        List<SpriteMetaData> metas = new List<SpriteMetaData>();

        for (int r = 0; r < rowCount; ++r) {
            for (int c = 0; c < colCount; ++c) {
                var x = c * spriteWidth;
                var y = r * spriteHeight;
                if (!IsTextureEmptyAt(texture, x, y, spriteWidth, spriteHeight)) {
                    SpriteMetaData meta = new SpriteMetaData();
                    meta.rect = new Rect(x, y, spriteWidth, spriteHeight);
                    meta.name = c + "-" + r;
                    metas.Add(meta);
                }
            }
        }

        TextureImporter textureImporter = (TextureImporter) assetImporter;
        textureImporter.spritesheet = metas.ToArray();
        
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(filepath).OfType<Sprite>().ToArray();
        List<Sprite> list = new List<Sprite>();
        list.AddRange(sprites);
        return list;
    }

    private bool IsTextureEmptyAt(Texture2D texture, int x, int y, int width, int height) {
        var pixels = texture.GetPixels(x, y, width, height);
        for (int i = 0; i < pixels.Length; i++) {
            var pixel = pixels[i];
            if ((int) (pixel.a * 1000) != 0) {
                return false;
            }
        }

        return true;
    }

    private AnimatorController GetAnimatorController(string filename) {
        return (AnimatorController)AssetDatabase.LoadAssetAtPath(filename, typeof(AnimatorController));
    }
    
    private AnimatorController CreateAnimationController(string filename) {
        // Creates the controller
        var controller = AnimatorController.CreateAnimatorControllerAtPath(filename);

        Debug.Log(string.Format("Created controller asset at {0}", filename));
        return controller;
    }

    private AnimationClip CreateClip(string filename, string assetName, List<Sprite> sprites, AnimatorController ctrl) {
        float frameRate = 10; // FPS
        float secondsBetweenFrames = 1 / frameRate;
        var asset = new AnimationClip();
        asset.name = assetName;
        
        asset.frameRate = frameRate;
        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite"; 
        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Count];
        for(int i = 0; i < spriteKeyFrames.Length; i++) {
            var keyframe = new ObjectReferenceKeyframe();
            keyframe.time = i * secondsBetweenFrames;
            keyframe.value = sprites[i];
            spriteKeyFrames[i] = keyframe;
        }
        AnimationUtility.SetObjectReferenceCurve(asset, spriteBinding, spriteKeyFrames);
        
        AssetDatabase.CreateAsset(asset, filename);
        Debug.Log(string.Format("Created animation asset at {0}", filename));

        if (ctrl != null) {
            var state = ctrl.layers[0].stateMachine.AddState(assetName);
            state.motion = asset;
        } else Debug.LogWarning(string.Format("Failed to add animation clip {0} to controller", assetName));

        return asset;
    }

    private class FileMeta {
        public readonly string filepath;
        public readonly string path;
        public readonly string parent;
        public readonly string name;
        public readonly string extension;
        public readonly int cellWidth;
        public readonly int cellHeight;

        public FileMeta(string filepath) {
            this.filepath = filepath;

            var splitByFolder = splitFilePath();
            if (splitByFolder.Length > 0) {
                var sb = new List<string>();
                for (int i = 0; i < splitByFolder.Length - 1; i++) {
                    sb.Add(splitByFolder[i]);
                }

                if (splitByFolder.Length > 1) {
                    parent = splitByFolder[splitByFolder.Length - 2];
                }
                path = String.Join("/", sb.ToArray());

                var file = splitByFolder[splitByFolder.Length - 1];
                var fileSplit = file.Split('.');

                if (fileSplit.Length > 1) {
                    extension = fileSplit[fileSplit.Length - 1];
                } else {
                    extension = "";
                }

                name = file.Substring(0, file.Length - (extension.Length + 1));

                var nameSplit = name.Split('.');
                if (nameSplit.Length > 1) {
                    name = nameSplit[0];
                    var cellData = nameSplit[nameSplit.Length - 1];
                    var cellDataSplit = cellData.Split('_');
                    if (cellDataSplit.Length == 2) {
                        cellWidth = Convert.ToInt32(cellDataSplit[0]);
                        cellHeight = Convert.ToInt32(cellDataSplit[1]);
                    } else {
                        throw new Exception(string.Format("Failed to process sprite sheet ({0}){1}. Files must be named like [name_of_sprite_sheet].[cell_width]_[cell_height].png", path, name));
                    }
                } else {
                    throw new Exception(string.Format("Failed to process sprite sheet ({0}){1}. Files must be named like [name_of_sprite_sheet].[cell_width]_[cell_height].png", path, name));
                }
            }
        }

        private string[] splitFilePath() {
            var a = filepath.Split('/');
            var b = filepath.Split('\\');
            if (a.Length > b.Length) return a;
            return b;
        }
    }
}