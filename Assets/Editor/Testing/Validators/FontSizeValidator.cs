// Assets/Test_TieuHoc/Editor/Validators/FontSizeValidator.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Validator kiểm tra kích thước và kiểu font trên tất cả Text elements
    /// </summary>
    public class FontSizeValidator : IValidator
    {
        public string Name => "Kiểm tra cỡ chữ và font";
        public string Description => "Kiểm tra cỡ chữ và kiểu font trên tất cả Text elements, đảm bảo đúng quy định";
        
        // Thiết lập có thể tùy chỉnh
        public float MinUITextSize = 18f;
        public float MinTMProTextSize = 18f;
        public bool CheckInactiveObjects = true;
        public bool GroupByTextType = true;
        public bool CheckFontType = true;  // Mới: Kiểm tra kiểu font
        public string[] AllowedFontNames = new string[] { "Arial", "Roboto", "Noto Sans" };  // Mới: Danh sách font được phép
        
        // Vấn đề font
        public enum FontIssueType
        {
            None,
            SmallSize,
            MissingFont,
            DisallowedFont
        }
        
        // Các loại text được hỗ trợ
        public enum TextType
        {
            UIText,
            TextMeshPro,
            TextMeshProUGUI
        }
        
        // Cấu trúc dữ liệu lưu trữ thông tin text có vấn đề
        public class TextIssueInfo
        {
            public Object textComponent;
            public GameObject gameObject;
            public TextType textType;
            public float fontSize;
            public float minSize;
            public FontIssueType issueType = FontIssueType.None;
            public string fontName = "";

            // Constructor mặc định
            public TextIssueInfo() { }
            
            // Constructor để sao chép
            public TextIssueInfo(TextIssueInfo other)
            {
                this.textComponent = other.textComponent;
                this.gameObject = other.gameObject;
                this.textType = other.textType;
                this.fontSize = other.fontSize;
                this.minSize = other.minSize;
                this.fontName = other.fontName;
            }
        }
        
        private Dictionary<string, string[]> fontSets = new Dictionary<string, string[]>();
        private string selectedFontSet = "Default";
        
        public FontSizeValidator()
        {
            // Initialize default font set
            fontSets["Default"] = new string[] { "Arial", "Times New Roman", "Verdana" };
            fontSets["Vietnamese"] = new string[] { "Arial", "Times New Roman", "Verdana", "Roboto", "Open Sans" };
            fontSets["Custom"] = new string[] { };
        }
        
        public string SelectedFontSet
        {
            get { return selectedFontSet; }
            set { selectedFontSet = value; }
        }
        
        public Dictionary<string, string[]> FontSets
        {
            get { return fontSets; }
        }
        
        public void AddFontSet(string name, string[] fonts)
        {
            fontSets[name] = fonts;
        }
        
        public void RemoveFontSet(string name)
        {
            if (fontSets.ContainsKey(name) && name != "Default")
            {
                fontSets.Remove(name);
                if (selectedFontSet == name)
                {
                    selectedFontSet = "Default";
                }
            }
        }
        
        public string[] GetAllowedFontNames()
        {
            return fontSets[selectedFontSet];
        }
        
        /// <summary>
        /// Kiểm tra tất cả text elements để tìm các text có vấn đề
        /// </summary>
        public List<ValidationIssue> Validate()
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();
            
            // Tìm tất cả text có vấn đề (cỡ chữ nhỏ, font thiếu hoặc không được phép)
            List<TextIssueInfo> textIssues = FindTextIssues();
            
            foreach (var textInfo in textIssues)
            {
                ValidationSeverity severity;
                string message = "";
                
                switch (textInfo.issueType)
                {
                    case FontIssueType.SmallSize:
                        severity = textInfo.fontSize < textInfo.minSize * 0.7f ? 
                            ValidationSeverity.Error : ValidationSeverity.Warning;
                        message = $"có cỡ chữ ({textInfo.fontSize}) nhỏ hơn ngưỡng tối thiểu ({textInfo.minSize})";
                        break;
                        
                    case FontIssueType.MissingFont:
                        severity = ValidationSeverity.Error;
                        message = "thiếu font chữ hoặc font bị null";
                        break;
                        
                    case FontIssueType.DisallowedFont:
                        severity = ValidationSeverity.Warning;
                        message = $"sử dụng font \"{textInfo.fontName}\" không nằm trong danh sách cho phép";
                        break;
                        
                    default:
                        continue;
                }
                
                string typeName = "";
                switch (textInfo.textType)
                {
                    case TextType.UIText: typeName = "UI Text"; break;
                    case TextType.TextMeshPro: typeName = "TextMeshPro"; break;
                    case TextType.TextMeshProUGUI: typeName = "TextMeshPro UI"; break;
                }
                
                string objectPath = ValidatorUtils.GetFullPath(textInfo.gameObject);
                
                ValidationIssue issue = new ValidationIssue()
                {
                    target = textInfo.textComponent,
                    message = $"[{typeName}] {objectPath} {message}",
                    severity = severity,
                    canAutoFix = textInfo.issueType == FontIssueType.SmallSize,  // Chỉ tự động sửa được cỡ chữ
                    fixAction = () => FixTextIssue(textInfo)
                };
                
                issues.Add(issue);
            }
            
            return issues;
        }
        
        /// <summary>
        /// Sửa tất cả các vấn đề về cỡ chữ
        /// </summary>
        public void FixAll()
        {
            List<ValidationIssue> issues = Validate();
            foreach (var issue in issues)
            {
                if (issue.canAutoFix)
                    Fix(issue);
            }
        }
        
        /// <summary>
        /// Sửa một vấn đề cụ thể
        /// </summary>
        public void Fix(ValidationIssue issue)
        {
            if (issue.target is TextMeshProUGUI tmpText)
            {
                // Find the first allowed font that exists in the project
                foreach (string fontName in GetAllowedFontNames())
                {
                    TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>($"Fonts/{fontName}");
                    if (fontAsset != null)
                    {
                        tmpText.font = fontAsset;
                        break;
                    }
                }
            }
            else if (issue.target is TextMeshPro tmpMesh)
            {
                foreach (string fontName in GetAllowedFontNames())
                {
                    TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>($"Fonts/{fontName}");
                    if (fontAsset != null)
                    {
                        tmpMesh.font = fontAsset;
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Tìm tất cả text có vấn đề trong scene
        /// </summary>
        private List<TextIssueInfo> FindTextIssues()
        {
            List<TextIssueInfo> results = new List<TextIssueInfo>();
            
            // Tìm tất cả Unity UI Text
            UnityEngine.UI.Text[] uiTexts = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Text>();
            
            foreach (var text in uiTexts)
            {
                // Bỏ qua objects không active nếu được cấu hình như vậy
                if (!CheckInactiveObjects && !text.gameObject.activeInHierarchy)
                    continue;
                
                // Bỏ qua objects trong prefab mode
                if (IsPartOfPrefabAsset(text.gameObject))
                    continue;
                
                TextIssueInfo issueInfo = new TextIssueInfo
                {
                    textComponent = text,
                    gameObject = text.gameObject,
                    textType = TextType.UIText,
                    fontSize = text.fontSize,
                    minSize = MinUITextSize
                };
                
                // Kiểm tra kích thước font
                if (text.fontSize < MinUITextSize)
                {
                    issueInfo.issueType = FontIssueType.SmallSize;
                    results.Add(issueInfo);
                }
                
                // Kiểm tra font thiếu
                if (CheckFontType && text.font == null)
                {
                    // Tạo bản sao nếu đã thêm do lỗi cỡ chữ
                    if (issueInfo.issueType != FontIssueType.None)
                    {
                        issueInfo = new TextIssueInfo(issueInfo);
                    }
                    
                    issueInfo.issueType = FontIssueType.MissingFont;
                    results.Add(issueInfo);
                }
                
                // Kiểm tra font không được phép
                if (CheckFontType && text.font != null && AllowedFontNames.Length > 0)
                {
                    string fontName = text.font.name;
                    issueInfo.fontName = fontName;
                    
                    bool isAllowed = false;
                    foreach (var allowedFont in AllowedFontNames)
                    {
                        if (fontName.Contains(allowedFont))
                        {
                            isAllowed = true;
                            break;
                        }
                    }
                    
                    if (!isAllowed)
                    {
                        // Tạo bản sao nếu đã thêm do lỗi khác
                        if (issueInfo.issueType != FontIssueType.None)
                        {
                            issueInfo = new TextIssueInfo(issueInfo);
                        }
                        
                        issueInfo.issueType = FontIssueType.DisallowedFont;
                        results.Add(issueInfo);
                    }
                }
            }
            
            // Kiểm tra TextMeshPro nếu có
            CheckTextMeshProComponents(results);
            
            return results;
        }
        
        /// <summary>
        /// Kiểm tra các TextMeshPro components
        /// </summary>
        private void CheckTextMeshProComponents(List<TextIssueInfo> results)
        {
            // Kiểm tra có TextMeshPro trong project không
            System.Type tmProType = System.Type.GetType("TMPro.TextMeshPro, Unity.TextMeshPro");
            System.Type tmProUGUIType = System.Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
            
            if (tmProType != null)
            {
                // Tìm TextMeshPro
                var tmpComponents = Resources.FindObjectsOfTypeAll(tmProType);
                foreach (var tmp in tmpComponents)
                {
                    MonoBehaviour tmpMono = tmp as MonoBehaviour;
                    if (tmpMono != null)
                    {
                        // Bỏ qua objects không active nếu được cấu hình như vậy
                        if (!CheckInactiveObjects && !tmpMono.gameObject.activeInHierarchy)
                            continue;
                        
                        // Bỏ qua objects trong prefab mode
                        if (IsPartOfPrefabAsset(tmpMono.gameObject))
                            continue;
                        
                        // Tạo issue info cơ bản
                        TextIssueInfo issueInfo = new TextIssueInfo
                        {
                            textComponent = tmpMono,
                            gameObject = tmpMono.gameObject,
                            textType = TextType.TextMeshPro,
                            minSize = MinTMProTextSize
                        };
                        
                        // Lấy fontSize bằng reflection
                        System.Reflection.PropertyInfo fontSizeProp = tmProType.GetProperty("fontSize");
                        if (fontSizeProp != null)
                        {
                            float fontSize = (float)fontSizeProp.GetValue(tmp);
                            issueInfo.fontSize = fontSize;
                            
                            // Kiểm tra kích thước font
                            if (fontSize < MinTMProTextSize)
                            {
                                issueInfo.issueType = FontIssueType.SmallSize;
                                results.Add(issueInfo);
                            }
                        }
                        
                        // Kiểm tra font thiếu hoặc không được phép
                        if (CheckFontType)
                        {
                            System.Reflection.PropertyInfo fontProp = tmProType.GetProperty("font");
                            if (fontProp != null)
                            {
                                object font = fontProp.GetValue(tmp);
                                
                                if (font == null)
                                {
                                    // Tạo bản sao nếu đã thêm do lỗi cỡ chữ
                                    if (issueInfo.issueType != FontIssueType.None)
                                    {
                                        issueInfo = new TextIssueInfo(issueInfo);
                                    }
                                    
                                    issueInfo.issueType = FontIssueType.MissingFont;
                                    results.Add(issueInfo);
                                }
                                else if (AllowedFontNames.Length > 0)
                                {
                                    // Lấy tên font từ asset
                                    string fontName = font.ToString();
                                    if (font is Object)
                                    {
                                        fontName = (font as Object).name;
                                    }
                                    
                                    issueInfo.fontName = fontName;
                                    
                                    bool isAllowed = false;
                                    foreach (var allowedFont in AllowedFontNames)
                                    {
                                        if (fontName.Contains(allowedFont))
                                        {
                                            isAllowed = true;
                                            break;
                                        }
                                    }
                                    
                                    if (!isAllowed)
                                    {
                                        // Tạo bản sao nếu đã thêm do lỗi khác
                                        if (issueInfo.issueType != FontIssueType.None)
                                        {
                                            issueInfo = new TextIssueInfo(issueInfo);
                                        }
                                        
                                        issueInfo.issueType = FontIssueType.DisallowedFont;
                                        results.Add(issueInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            if (tmProUGUIType != null)
            {
                // Tìm TextMeshProUGUI
                var tmpUGUIComponents = Resources.FindObjectsOfTypeAll(tmProUGUIType);
                foreach (var tmp in tmpUGUIComponents)
                {
                    MonoBehaviour tmpMono = tmp as MonoBehaviour;
                    if (tmpMono != null)
                    {
                        // Bỏ qua objects không active nếu được cấu hình như vậy
                        if (!CheckInactiveObjects && !tmpMono.gameObject.activeInHierarchy)
                            continue;
                        
                        // Bỏ qua objects trong prefab mode
                        if (IsPartOfPrefabAsset(tmpMono.gameObject))
                            continue;
                        
                        // Tạo issue info cơ bản
                        TextIssueInfo issueInfo = new TextIssueInfo
                        {
                            textComponent = tmpMono,
                            gameObject = tmpMono.gameObject,
                            textType = TextType.TextMeshProUGUI,
                            minSize = MinTMProTextSize
                        };
                        
                        // Lấy fontSize bằng reflection
                        System.Reflection.PropertyInfo fontSizeProp = tmProUGUIType.GetProperty("fontSize");
                        if (fontSizeProp != null)
                        {
                            float fontSize = (float)fontSizeProp.GetValue(tmp);
                            issueInfo.fontSize = fontSize;
                            
                            // Kiểm tra kích thước font
                            if (fontSize < MinTMProTextSize)
                            {
                                issueInfo.issueType = FontIssueType.SmallSize;
                                results.Add(issueInfo);
                            }
                        }
                        
                        // Kiểm tra font thiếu hoặc không được phép
                        if (CheckFontType)
                        {
                            System.Reflection.PropertyInfo fontProp = tmProUGUIType.GetProperty("font");
                            if (fontProp != null)
                            {
                                object font = fontProp.GetValue(tmp);
                                
                                if (font == null)
                                {
                                    // Tạo bản sao nếu đã thêm do lỗi cỡ chữ
                                    if (issueInfo.issueType != FontIssueType.None)
                                    {
                                        issueInfo = new TextIssueInfo(issueInfo);
                                    }
                                    
                                    issueInfo.issueType = FontIssueType.MissingFont;
                                    results.Add(issueInfo);
                                }
                                else if (AllowedFontNames.Length > 0)
                                {
                                    // Lấy tên font từ asset
                                    string fontName = font.ToString();
                                    if (font is Object)
                                    {
                                        fontName = (font as Object).name;
                                    }
                                    
                                    issueInfo.fontName = fontName;
                                    
                                    bool isAllowed = false;
                                    foreach (var allowedFont in AllowedFontNames)
                                    {
                                        if (fontName.Contains(allowedFont))
                                        {
                                            isAllowed = true;
                                            break;
                                        }
                                    }
                                    
                                    if (!isAllowed)
                                    {
                                        // Tạo bản sao nếu đã thêm do lỗi khác
                                        if (issueInfo.issueType != FontIssueType.None)
                                        {
                                            issueInfo = new TextIssueInfo(issueInfo);
                                        }
                                        
                                        issueInfo.issueType = FontIssueType.DisallowedFont;
                                        results.Add(issueInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Kiểm tra object có phải là một phần của prefab asset không
        /// </summary>
        private bool IsPartOfPrefabAsset(GameObject gameObject)
        {
#if UNITY_2018_3_OR_NEWER
            return PrefabUtility.IsPartOfPrefabAsset(gameObject);
#else
            PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
            return prefabType == PrefabType.Prefab;
#endif
        }
        
        /// <summary>
        /// Sửa vấn đề về text
        /// </summary>
        private void FixTextIssue(TextIssueInfo textInfo)
        {
            if (textInfo.issueType == FontIssueType.SmallSize)
            {
                // Sửa cỡ chữ
                switch (textInfo.textType)
                {
                    case TextType.UIText:
                        var uiText = textInfo.textComponent as UnityEngine.UI.Text;
                        if (uiText != null)
                        {
                            Undo.RecordObject(uiText, "Fix Font Size");
                            uiText.fontSize = Mathf.CeilToInt(textInfo.minSize);
                            EditorUtility.SetDirty(uiText);
                        }
                        break;
                    
                    case TextType.TextMeshPro:
                    case TextType.TextMeshProUGUI:
                        // Sử dụng reflection để sửa fontSize
                        System.Type type = textInfo.textComponent.GetType();
                        System.Reflection.PropertyInfo fontSizeProp = type.GetProperty("fontSize");
                        if (fontSizeProp != null)
                        {
                            Undo.RecordObject(textInfo.textComponent as Object, "Fix Font Size");
                            fontSizeProp.SetValue(textInfo.textComponent, textInfo.minSize);
                            EditorUtility.SetDirty(textInfo.textComponent as Object);
                        }
                        break;
                }
                
                Debug.Log($"Đã sửa cỡ chữ của {textInfo.gameObject.name} từ {textInfo.fontSize} thành {textInfo.minSize}");
            }
            // Không tự động sửa vấn đề font
        }
    }
}