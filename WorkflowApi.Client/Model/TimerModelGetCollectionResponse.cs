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
    /// TimerModelGetCollectionResponse
    /// </summary>
    [DataContract(Name = "TimerModelGetCollectionResponse")]
    public partial class TimerModelGetCollectionResponse : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerModelGetCollectionResponse" /> class.
        /// </summary>
        /// <param name="collection">collection.</param>
        /// <param name="total">total.</param>
        public TimerModelGetCollectionResponse(List<TimerModel> collection = default(List<TimerModel>), long total = default(long))
        {
            this.Collection = collection;
            this.Total = total;
        }

        /// <summary>
        /// Gets or Sets Collection
        /// </summary>
        [DataMember(Name = "collection", EmitDefaultValue = true)]
        public List<TimerModel> Collection { get; set; }

        /// <summary>
        /// Gets or Sets Total
        /// </summary>
        [DataMember(Name = "total", EmitDefaultValue = false)]
        public long Total { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class TimerModelGetCollectionResponse {\n");
            sb.Append("  Collection: ").Append(Collection).Append("\n");
            sb.Append("  Total: ").Append(Total).Append("\n");
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
