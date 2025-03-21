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
    /// Comment
    /// </summary>
    [DataContract(Name = "Comment")]
    public partial class Comment : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Comment" /> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="designerSettings">designerSettings.</param>
        /// <param name="value">value.</param>
        /// <param name="boldText">boldText.</param>
        /// <param name="italicText">italicText.</param>
        /// <param name="underlineText">underlineText.</param>
        /// <param name="lineThroughText">lineThroughText.</param>
        /// <param name="fontSize">fontSize.</param>
        /// <param name="rotation">rotation.</param>
        /// <param name="width">width.</param>
        /// <param name="alignment">alignment.</param>
        public Comment(string name = default(string), Designer designerSettings = default(Designer), string value = default(string), bool boldText = default(bool), bool italicText = default(bool), bool underlineText = default(bool), bool lineThroughText = default(bool), int fontSize = default(int), double rotation = default(double), double width = default(double), string alignment = default(string))
        {
            this.Name = name;
            this.DesignerSettings = designerSettings;
            this.Value = value;
            this.BoldText = boldText;
            this.ItalicText = italicText;
            this.UnderlineText = underlineText;
            this.LineThroughText = lineThroughText;
            this.FontSize = fontSize;
            this.Rotation = rotation;
            this.Width = width;
            this.Alignment = alignment;
        }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets DesignerSettings
        /// </summary>
        [DataMember(Name = "designerSettings", EmitDefaultValue = false)]
        public Designer DesignerSettings { get; set; }

        /// <summary>
        /// Gets or Sets Value
        /// </summary>
        [DataMember(Name = "value", EmitDefaultValue = true)]
        public string Value { get; set; }

        /// <summary>
        /// Gets or Sets BoldText
        /// </summary>
        [DataMember(Name = "boldText", EmitDefaultValue = true)]
        public bool BoldText { get; set; }

        /// <summary>
        /// Gets or Sets ItalicText
        /// </summary>
        [DataMember(Name = "italicText", EmitDefaultValue = true)]
        public bool ItalicText { get; set; }

        /// <summary>
        /// Gets or Sets UnderlineText
        /// </summary>
        [DataMember(Name = "underlineText", EmitDefaultValue = true)]
        public bool UnderlineText { get; set; }

        /// <summary>
        /// Gets or Sets LineThroughText
        /// </summary>
        [DataMember(Name = "lineThroughText", EmitDefaultValue = true)]
        public bool LineThroughText { get; set; }

        /// <summary>
        /// Gets or Sets FontSize
        /// </summary>
        [DataMember(Name = "fontSize", EmitDefaultValue = false)]
        public int FontSize { get; set; }

        /// <summary>
        /// Gets or Sets Rotation
        /// </summary>
        [DataMember(Name = "rotation", EmitDefaultValue = false)]
        public double Rotation { get; set; }

        /// <summary>
        /// Gets or Sets Width
        /// </summary>
        [DataMember(Name = "width", EmitDefaultValue = false)]
        public double Width { get; set; }

        /// <summary>
        /// Gets or Sets Alignment
        /// </summary>
        [DataMember(Name = "alignment", EmitDefaultValue = true)]
        public string Alignment { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Comment {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  DesignerSettings: ").Append(DesignerSettings).Append("\n");
            sb.Append("  Value: ").Append(Value).Append("\n");
            sb.Append("  BoldText: ").Append(BoldText).Append("\n");
            sb.Append("  ItalicText: ").Append(ItalicText).Append("\n");
            sb.Append("  UnderlineText: ").Append(UnderlineText).Append("\n");
            sb.Append("  LineThroughText: ").Append(LineThroughText).Append("\n");
            sb.Append("  FontSize: ").Append(FontSize).Append("\n");
            sb.Append("  Rotation: ").Append(Rotation).Append("\n");
            sb.Append("  Width: ").Append(Width).Append("\n");
            sb.Append("  Alignment: ").Append(Alignment).Append("\n");
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
