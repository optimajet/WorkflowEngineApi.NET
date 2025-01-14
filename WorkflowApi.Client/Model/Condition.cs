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
    /// Condition
    /// </summary>
    [DataContract(Name = "Condition")]
    public partial class Condition : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Condition" /> class.
        /// </summary>
        /// <param name="type">type.</param>
        /// <param name="action">action.</param>
        /// <param name="expression">expression.</param>
        /// <param name="resultOnPreExecution">resultOnPreExecution.</param>
        /// <param name="conditionInversion">conditionInversion.</param>
        public Condition(string type = default(string), ActionReference action = default(ActionReference), string expression = default(string), bool? resultOnPreExecution = default(bool?), bool conditionInversion = default(bool))
        {
            this.Type = type;
            this.Action = action;
            this.Expression = expression;
            this.ResultOnPreExecution = resultOnPreExecution;
            this.ConditionInversion = conditionInversion;
        }

        /// <summary>
        /// Gets or Sets Type
        /// </summary>
        [DataMember(Name = "type", EmitDefaultValue = true)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or Sets Action
        /// </summary>
        [DataMember(Name = "action", EmitDefaultValue = false)]
        public ActionReference Action { get; set; }

        /// <summary>
        /// Gets or Sets Expression
        /// </summary>
        [DataMember(Name = "expression", EmitDefaultValue = true)]
        public string Expression { get; set; }

        /// <summary>
        /// Gets or Sets ResultOnPreExecution
        /// </summary>
        [DataMember(Name = "resultOnPreExecution", EmitDefaultValue = true)]
        public bool? ResultOnPreExecution { get; set; }

        /// <summary>
        /// Gets or Sets ConditionInversion
        /// </summary>
        [DataMember(Name = "conditionInversion", EmitDefaultValue = true)]
        public bool ConditionInversion { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Condition {\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  Action: ").Append(Action).Append("\n");
            sb.Append("  Expression: ").Append(Expression).Append("\n");
            sb.Append("  ResultOnPreExecution: ").Append(ResultOnPreExecution).Append("\n");
            sb.Append("  ConditionInversion: ").Append(ConditionInversion).Append("\n");
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
