/*
 * Workflow Engine API
 *
 * A Workflow Engine Web API
 *
 * The version of the OpenAPI document: 1.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = WorkflowApi.Client.Client.OpenAPIDateConverter;

namespace WorkflowApi.Client.Model
{
    /// <summary>
    /// CodeActionParameter
    /// </summary>
    [DataContract(Name = "CodeActionParameter")]
    public partial class CodeActionParameter : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeActionParameter" /> class.
        /// </summary>
        /// <param name="title">title.</param>
        /// <param name="name">name.</param>
        /// <param name="type">type.</param>
        /// <param name="isRequired">isRequired.</param>
        /// <param name="dropdownValues">dropdownValues.</param>
        /// <param name="defaultValue">defaultValue.</param>
        /// <param name="comment">comment.</param>
        /// <param name="customName">customName.</param>
        public CodeActionParameter(string title = default(string), string name = default(string), string type = default(string), bool isRequired = default(bool), List<DropdownValue> dropdownValues = default(List<DropdownValue>), string defaultValue = default(string), string comment = default(string), string customName = default(string))
        {
            this.Title = title;
            this.Name = name;
            this.Type = type;
            this.IsRequired = isRequired;
            this.DropdownValues = dropdownValues;
            this.DefaultValue = defaultValue;
            this.Comment = comment;
            this.CustomName = customName;
        }

        /// <summary>
        /// Gets or Sets Title
        /// </summary>
        [DataMember(Name = "title", EmitDefaultValue = true)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Type
        /// </summary>
        [DataMember(Name = "type", EmitDefaultValue = true)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or Sets IsRequired
        /// </summary>
        [DataMember(Name = "isRequired", EmitDefaultValue = true)]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or Sets DropdownValues
        /// </summary>
        [DataMember(Name = "dropdownValues", EmitDefaultValue = true)]
        public List<DropdownValue> DropdownValues { get; set; }

        /// <summary>
        /// Gets or Sets DefaultValue
        /// </summary>
        [DataMember(Name = "defaultValue", EmitDefaultValue = true)]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or Sets Comment
        /// </summary>
        [DataMember(Name = "comment", EmitDefaultValue = true)]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or Sets CustomName
        /// </summary>
        [DataMember(Name = "customName", EmitDefaultValue = true)]
        public string CustomName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class CodeActionParameter {\n");
            sb.Append("  Title: ").Append(Title).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  IsRequired: ").Append(IsRequired).Append("\n");
            sb.Append("  DropdownValues: ").Append(DropdownValues).Append("\n");
            sb.Append("  DefaultValue: ").Append(DefaultValue).Append("\n");
            sb.Append("  Comment: ").Append(Comment).Append("\n");
            sb.Append("  CustomName: ").Append(CustomName).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}