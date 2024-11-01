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
    /// StatusModel
    /// </summary>
    [DataContract(Name = "StatusModel")]
    public partial class StatusModel : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusModel" /> class.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="statusCode">statusCode.</param>
        /// <param name="varLock">varLock.</param>
        /// <param name="runtimeId">runtimeId.</param>
        /// <param name="setTime">setTime.</param>
        public StatusModel(Guid id = default(Guid), int statusCode = default(int), Guid varLock = default(Guid), string runtimeId = default(string), DateTimeOffset setTime = default(DateTimeOffset))
        {
            this.Id = id;
            this.StatusCode = statusCode;
            this.VarLock = varLock;
            this.RuntimeId = runtimeId;
            this.SetTime = setTime;
        }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or Sets StatusCode
        /// </summary>
        [DataMember(Name = "statusCode", EmitDefaultValue = false)]
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or Sets VarLock
        /// </summary>
        [DataMember(Name = "lock", EmitDefaultValue = false)]
        public Guid VarLock { get; set; }

        /// <summary>
        /// Gets or Sets RuntimeId
        /// </summary>
        [DataMember(Name = "runtimeId", EmitDefaultValue = true)]
        public string RuntimeId { get; set; }

        /// <summary>
        /// Gets or Sets SetTime
        /// </summary>
        [DataMember(Name = "setTime", EmitDefaultValue = false)]
        public DateTimeOffset SetTime { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class StatusModel {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  StatusCode: ").Append(StatusCode).Append("\n");
            sb.Append("  VarLock: ").Append(VarLock).Append("\n");
            sb.Append("  RuntimeId: ").Append(RuntimeId).Append("\n");
            sb.Append("  SetTime: ").Append(SetTime).Append("\n");
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
