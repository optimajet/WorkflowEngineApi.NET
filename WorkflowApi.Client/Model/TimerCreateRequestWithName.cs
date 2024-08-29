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
    /// TimerCreateRequestWithName
    /// </summary>
    [DataContract(Name = "TimerCreateRequestWithName")]
    public partial class TimerCreateRequestWithName : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerCreateRequestWithName" /> class.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="rootProcessId">rootProcessId.</param>
        /// <param name="nextExecutionDateTime">nextExecutionDateTime.</param>
        /// <param name="ignore">ignore.</param>
        public TimerCreateRequestWithName(string name = default(string), Guid rootProcessId = default(Guid), DateTimeOffset nextExecutionDateTime = default(DateTimeOffset), bool ignore = default(bool))
        {
            this.Name = name;
            this.RootProcessId = rootProcessId;
            this.NextExecutionDateTime = nextExecutionDateTime;
            this.Ignore = ignore;
        }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets RootProcessId
        /// </summary>
        [DataMember(Name = "rootProcessId", EmitDefaultValue = false)]
        public Guid RootProcessId { get; set; }

        /// <summary>
        /// Gets or Sets NextExecutionDateTime
        /// </summary>
        [DataMember(Name = "nextExecutionDateTime", EmitDefaultValue = false)]
        public DateTimeOffset NextExecutionDateTime { get; set; }

        /// <summary>
        /// Gets or Sets Ignore
        /// </summary>
        [DataMember(Name = "ignore", EmitDefaultValue = true)]
        public bool Ignore { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class TimerCreateRequestWithName {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  RootProcessId: ").Append(RootProcessId).Append("\n");
            sb.Append("  NextExecutionDateTime: ").Append(NextExecutionDateTime).Append("\n");
            sb.Append("  Ignore: ").Append(Ignore).Append("\n");
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