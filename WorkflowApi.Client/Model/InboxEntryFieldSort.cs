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
    /// InboxEntryFieldSort
    /// </summary>
    [DataContract(Name = "InboxEntryFieldSort")]
    public partial class InboxEntryFieldSort : IValidatableObject
    {

        /// <summary>
        /// Gets or Sets Field
        /// </summary>
        [DataMember(Name = "field", EmitDefaultValue = false)]
        public InboxEntryField? Field { get; set; }

        /// <summary>
        /// Gets or Sets Direction
        /// </summary>
        [DataMember(Name = "direction", EmitDefaultValue = false)]
        public Direction? Direction { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="InboxEntryFieldSort" /> class.
        /// </summary>
        /// <param name="field">field.</param>
        /// <param name="direction">direction.</param>
        public InboxEntryFieldSort(InboxEntryField? field = default(InboxEntryField?), Direction? direction = default(Direction?))
        {
            this.Field = field;
            this.Direction = direction;
        }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class InboxEntryFieldSort {\n");
            sb.Append("  Field: ").Append(Field).Append("\n");
            sb.Append("  Direction: ").Append(Direction).Append("\n");
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
