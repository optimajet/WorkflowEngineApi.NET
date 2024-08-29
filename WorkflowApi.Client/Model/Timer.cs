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
    /// Timer
    /// </summary>
    [DataContract(Name = "Timer")]
    public partial class Timer : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Timer" /> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="designerSettings">designerSettings.</param>
        /// <param name="type">type.</param>
        /// <param name="value">value.</param>
        /// <param name="notOverrideIfExists">notOverrideIfExists.</param>
        public Timer(string name = default(string), Designer designerSettings = default(Designer), string type = default(string), string value = default(string), bool notOverrideIfExists = default(bool))
        {
            this.Name = name;
            this.DesignerSettings = designerSettings;
            this.Type = type;
            this.Value = value;
            this.NotOverrideIfExists = notOverrideIfExists;
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
        /// Gets or Sets Type
        /// </summary>
        [DataMember(Name = "type", EmitDefaultValue = true)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or Sets Value
        /// </summary>
        [DataMember(Name = "value", EmitDefaultValue = true)]
        public string Value { get; set; }

        /// <summary>
        /// Gets or Sets NotOverrideIfExists
        /// </summary>
        [DataMember(Name = "notOverrideIfExists", EmitDefaultValue = true)]
        public bool NotOverrideIfExists { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Timer {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  DesignerSettings: ").Append(DesignerSettings).Append("\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  Value: ").Append(Value).Append("\n");
            sb.Append("  NotOverrideIfExists: ").Append(NotOverrideIfExists).Append("\n");
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