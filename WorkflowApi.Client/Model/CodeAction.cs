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
    /// CodeAction
    /// </summary>
    [DataContract(Name = "CodeAction")]
    public partial class CodeAction : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeAction" /> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="designerSettings">designerSettings.</param>
        /// <param name="originalName">originalName.</param>
        /// <param name="originalSchemeCode">originalSchemeCode.</param>
        /// <param name="wasInlined">wasInlined.</param>
        /// <param name="actionCode">actionCode.</param>
        /// <param name="type">type.</param>
        /// <param name="isGlobal">isGlobal.</param>
        /// <param name="isAsync">isAsync.</param>
        /// <param name="usings">usings.</param>
        /// <param name="excludedUsings">excludedUsings.</param>
        /// <param name="parameterDefinitions">parameterDefinitions.</param>
        public CodeAction(string name = default(string), Designer designerSettings = default(Designer), string originalName = default(string), string originalSchemeCode = default(string), bool wasInlined = default(bool), string actionCode = default(string), string type = default(string), bool isGlobal = default(bool), bool isAsync = default(bool), string usings = default(string), string excludedUsings = default(string), List<CodeActionParameter> parameterDefinitions = default(List<CodeActionParameter>))
        {
            this.Name = name;
            this.DesignerSettings = designerSettings;
            this.OriginalName = originalName;
            this.OriginalSchemeCode = originalSchemeCode;
            this.WasInlined = wasInlined;
            this.ActionCode = actionCode;
            this.Type = type;
            this.IsGlobal = isGlobal;
            this.IsAsync = isAsync;
            this.Usings = usings;
            this.ExcludedUsings = excludedUsings;
            this.ParameterDefinitions = parameterDefinitions;
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
        /// Gets or Sets OriginalName
        /// </summary>
        [DataMember(Name = "originalName", EmitDefaultValue = true)]
        public string OriginalName { get; set; }

        /// <summary>
        /// Gets or Sets OriginalSchemeCode
        /// </summary>
        [DataMember(Name = "originalSchemeCode", EmitDefaultValue = true)]
        public string OriginalSchemeCode { get; set; }

        /// <summary>
        /// Gets or Sets WasInlined
        /// </summary>
        [DataMember(Name = "wasInlined", EmitDefaultValue = true)]
        public bool WasInlined { get; set; }

        /// <summary>
        /// Gets or Sets ActionCode
        /// </summary>
        [DataMember(Name = "actionCode", EmitDefaultValue = true)]
        public string ActionCode { get; set; }

        /// <summary>
        /// Gets or Sets Type
        /// </summary>
        [DataMember(Name = "type", EmitDefaultValue = true)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or Sets IsGlobal
        /// </summary>
        [DataMember(Name = "isGlobal", EmitDefaultValue = true)]
        public bool IsGlobal { get; set; }

        /// <summary>
        /// Gets or Sets IsAsync
        /// </summary>
        [DataMember(Name = "isAsync", EmitDefaultValue = true)]
        public bool IsAsync { get; set; }

        /// <summary>
        /// Gets or Sets Usings
        /// </summary>
        [DataMember(Name = "usings", EmitDefaultValue = true)]
        public string Usings { get; set; }

        /// <summary>
        /// Gets or Sets ExcludedUsings
        /// </summary>
        [DataMember(Name = "excludedUsings", EmitDefaultValue = true)]
        public string ExcludedUsings { get; set; }

        /// <summary>
        /// Gets or Sets ParameterDefinitions
        /// </summary>
        [DataMember(Name = "parameterDefinitions", EmitDefaultValue = true)]
        public List<CodeActionParameter> ParameterDefinitions { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class CodeAction {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  DesignerSettings: ").Append(DesignerSettings).Append("\n");
            sb.Append("  OriginalName: ").Append(OriginalName).Append("\n");
            sb.Append("  OriginalSchemeCode: ").Append(OriginalSchemeCode).Append("\n");
            sb.Append("  WasInlined: ").Append(WasInlined).Append("\n");
            sb.Append("  ActionCode: ").Append(ActionCode).Append("\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  IsGlobal: ").Append(IsGlobal).Append("\n");
            sb.Append("  IsAsync: ").Append(IsAsync).Append("\n");
            sb.Append("  Usings: ").Append(Usings).Append("\n");
            sb.Append("  ExcludedUsings: ").Append(ExcludedUsings).Append("\n");
            sb.Append("  ParameterDefinitions: ").Append(ParameterDefinitions).Append("\n");
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