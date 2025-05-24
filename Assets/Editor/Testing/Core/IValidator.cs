// Assets/Test_TieuHoc/Editor/Core/IValidator.cs
using UnityEngine;
using System.Collections.Generic;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Mức độ nghiêm trọng của vấn đề được phát hiện
    /// </summary>
    public enum ValidationSeverity
    {
        Info,       // Thông tin, không phải lỗi
        Warning,    // Cảnh báo, có thể gây vấn đề
        Error       // Lỗi nghiêm trọng, cần sửa
    }

    /// <summary>
    /// Đại diện cho một vấn đề được phát hiện trong quá trình validation
    /// </summary>
    public class ValidationIssue
    {
        /// <summary>
        /// Object liên quan đến vấn đề
        /// </summary>
        public Object target;
        
        /// <summary>
        /// Mô tả vấn đề
        /// </summary>
        public string message;
        
        /// <summary>
        /// Mức độ nghiêm trọng của vấn đề
        /// </summary>
        public ValidationSeverity severity;
        
        /// <summary>
        /// Xác định xem vấn đề có thể tự động sửa không
        /// </summary>
        public bool canAutoFix;
        
        /// <summary>
        /// Action để sửa vấn đề (nếu có thể tự động sửa)
        /// </summary>
        public System.Action fixAction;
    }

    /// <summary>
    /// Interface cho tất cả các validator
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Tên của validator
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Mô tả về chức năng của validator
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Thực hiện kiểm tra và trả về danh sách các vấn đề
        /// </summary>
        /// <returns>Danh sách các vấn đề phát hiện được</returns>
        List<ValidationIssue> Validate();
        
        /// <summary>
        /// Sửa tất cả các vấn đề có thể tự động sửa
        /// </summary>
        void FixAll();
        
        /// <summary>
        /// Sửa một vấn đề cụ thể
        /// </summary>
        /// <param name="issue">Vấn đề cần sửa</param>
        void Fix(ValidationIssue issue);
    }
}