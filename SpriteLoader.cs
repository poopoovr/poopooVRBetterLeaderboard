using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace poopooVRBetterLeaderboard
{
    public class SpriteLoader : MonoBehaviour
    {
        private static SpriteLoader _instance;
        public static SpriteLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("poopooVR_SpriteLoader");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<SpriteLoader>();
                }
                return _instance;
            }
        }

        private TMP_SpriteAsset _spriteAsset;
        public TMP_SpriteAsset SpriteAsset => _spriteAsset;

        private bool _isLoading = false;
        private bool _isLoaded = false;

        public bool IsLoaded => _isLoaded;

        private const string SERVER_PATH = "https://raw.githubusercontent.com/poopoovr/poopooVRBetterLeaderboard/main/sprites";

        public void LoadSprites()
        {
            if (_isLoading || _isLoaded) return;
            _isLoading = true;
            StartCoroutine(LoadSpritesCoroutine());
        }

        private IEnumerator LoadSpritesCoroutine()
        {
            var textureList = new List<Texture2D>();
            var spriteDataList = new List<(string name, int index)>();

            for (int i = 1; i <= 4; i++)
            {
                int pingLevel = i;
                yield return LoadTexture($"{SERVER_PATH}/ping{pingLevel}.png", tex =>
                {
                    if (tex != null)
                    {
                        spriteDataList.Add(($"Ping{pingLevel}", textureList.Count));
                        textureList.Add(tex);
                    }
                });
            }

            if (textureList.Count > 0)
            {
                CreateSpriteAsset(textureList, spriteDataList);
                _isLoaded = true;
            }

            _isLoading = false;
        }

        private IEnumerator LoadTexture(string url, System.Action<Texture2D> callback)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                    callback?.Invoke(texture);
                }
                else
                {
                    Debug.LogWarning($"Failed to load texture from {url}: {request.error}");
                    callback?.Invoke(null);
                }
            }
        }

        private void CreateSpriteAsset(List<Texture2D> textureList, List<(string name, int index)> spriteDataList)
        {
            _spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            _spriteAsset.name = "poopooVR_Sprites";

            int maxSize = 512;
            Texture2D spriteSheet = new Texture2D(maxSize, maxSize);
            Rect[] rects = spriteSheet.PackTextures(textureList.ToArray(), 2, maxSize);

            _spriteAsset.spriteSheet = spriteSheet;
            _spriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite"))
            {
                mainTexture = spriteSheet
            };

            var versionField = typeof(TMP_SpriteAsset).GetField("m_Version", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (versionField != null)
                versionField.SetValue(_spriteAsset, "1.1.0");

            _spriteAsset.spriteGlyphTable.Clear();
            for (int i = 0; i < spriteDataList.Count; i++)
            {
                var rect = rects[i];

                var glyph = new TMP_SpriteGlyph
                {
                    index = (uint)i,
                    metrics = new UnityEngine.TextCore.GlyphMetrics(
                        width: rect.width * spriteSheet.width,
                        height: rect.height * spriteSheet.height,
                        bearingX: -(rect.width * spriteSheet.width) / 2f,
                        bearingY: rect.height * spriteSheet.height * 0.8f,
                        advance: rect.width * spriteSheet.width
                    ),
                    glyphRect = new UnityEngine.TextCore.GlyphRect(
                        x: (int)(rect.x * spriteSheet.width),
                        y: (int)(rect.y * spriteSheet.height),
                        width: (int)(rect.width * spriteSheet.width),
                        height: (int)(rect.height * spriteSheet.height)
                    ),
                    scale = 1f,
                    atlasIndex = 0
                };
                _spriteAsset.spriteGlyphTable.Add(glyph);
            }

            _spriteAsset.spriteCharacterTable.Clear();
            for (int i = 0; i < spriteDataList.Count; i++)
            {
                var (name, _) = spriteDataList[i];

                var character = new TMP_SpriteCharacter(0xFFFE, _spriteAsset.spriteGlyphTable[i])
                {
                    name = name,
                    scale = 1f,
                    glyphIndex = (uint)i
                };
                _spriteAsset.spriteCharacterTable.Add(character);
            }

            _spriteAsset.UpdateLookupTables();
        }
    }
}
