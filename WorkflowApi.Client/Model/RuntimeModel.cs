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
    /// RuntimeModel
    /// </summary>
    [DataContract(Name = "RuntimeModel")]
    public partial class RuntimeModel : IValidatableObject
    {

        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public RuntimeStatus? Status { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModel" /> class.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="varLock">varLock.</param>
        /// <param name="status">status.</param>
        /// <param name="restorerId">restorerId.</param>
        /// <param name="nextTimerTime">nextTimerTime.</param>
        /// <param name="nextServiceTimerTime">nextServiceTimerTime.</param>
        /// <param name="lastAliveSignal">lastAliveSignal.</param>
        public RuntimeModel(string id = default(string), Guid varLock = default(Guid), RuntimeStatus? status = default(RuntimeStatus?), string restorerId = default(string), DateTimeOffset? nextTimerTime = default(DateTimeOffset?), DateTimeOffset? nextServiceTimerTime = default(DateTimeOffset?), DateTimeOffset? lastAliveSignal = default(DateTimeOffset?))
        {
            this.Id = id;
            this.VarLock = varLock;
            this.Status = status;
            this.RestorerId = restorerId;
            this.NextTimerTime = nextTimerTime;
            this.NextServiceTimerTime = nextServiceTimerTime;
            this.LastAliveSignal = lastAliveSignal;
        }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or Sets VarLock
        /// </summary>
        [DataMember(Name = "lock", EmitDefaultValue = false)]
        public Guid VarLock { get; set; }

        /// <summary>
        /// Gets or Sets RestorerId
        /// </summary>
        [DataMember(Name = "restorerId", EmitDefaultValue = true)]
        public string RestorerId { get; set; }

        /// <summary>
        /// Gets or Sets NextTimerTime
        /// </summary>
        [DataMember(Name = "nextTimerTime", EmitDefaultValue = true)]
        public DateTimeOffset? NextTimerTime { get; set; }

        /// <summary>
        /// Gets or Sets NextServiceTimerTime
        /// </summary>
        [DataMember(Name = "nextServiceTimerTime", EmitDefaultValue = true)]
        public DateTimeOffset? NextServiceTimerTime { get; set; }

        /// <summary>
        /// Gets or Sets LastAliveSignal
        /// </summary>
        [DataMember(Name = "lastAliveSignal", EmitDefaultValue = true)]
        public DateTimeOffset? LastAliveSignal { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class RuntimeModel {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  VarLock: ").Append(VarLock).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  RestorerId: ").Append(RestorerId).Append("\n");
            sb.Append("  NextTimerTime: ").Append(NextTimerTime).Append("\n");
            sb.Append("  NextServiceTimerTime: ").Append(NextServiceTimerTime).Append("\n");
            sb.Append("  LastAliveSignal: ").Append(LastAliveSignal).Append("\n");
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