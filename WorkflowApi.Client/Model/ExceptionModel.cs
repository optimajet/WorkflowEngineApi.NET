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
    /// ExceptionModel
    /// </summary>
    [DataContract(Name = "ExceptionModel")]
    public partial class ExceptionModel : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionModel" /> class.
        /// </summary>
        /// <param name="innerException">innerException.</param>
        public ExceptionModel(ExceptionModel innerException = default(ExceptionModel))
        {
            this.InnerException = innerException;
        }

        /// <summary>
        /// Gets or Sets Type
        /// </summary>
        [DataMember(Name = "type", EmitDefaultValue = true)]
        public string Type { get; private set; }

        /// <summary>
        /// Returns false as Type should not be serialized given that it's read-only.
        /// </summary>
        /// <returns>false (boolean)</returns>
        public bool ShouldSerializeType()
        {
            return false;
        }
        /// <summary>
        /// Gets or Sets Message
        /// </summary>
        [DataMember(Name = "message", EmitDefaultValue = true)]
        public string Message { get; private set; }

        /// <summary>
        /// Returns false as Message should not be serialized given that it's read-only.
        /// </summary>
        /// <returns>false (boolean)</returns>
        public bool ShouldSerializeMessage()
        {
            return false;
        }
        /// <summary>
        /// Gets or Sets Data
        /// </summary>
        [DataMember(Name = "data", EmitDefaultValue = true)]
        public Dictionary<string, Object> Data { get; private set; }

        /// <summary>
        /// Returns false as Data should not be serialized given that it's read-only.
        /// </summary>
        /// <returns>false (boolean)</returns>
        public bool ShouldSerializeData()
        {
            return false;
        }
        /// <summary>
        /// Gets or Sets StackTrace
        /// </summary>
        [DataMember(Name = "stackTrace", EmitDefaultValue = true)]
        public string StackTrace { get; private set; }

        /// <summary>
        /// Returns false as StackTrace should not be serialized given that it's read-only.
        /// </summary>
        /// <returns>false (boolean)</returns>
        public bool ShouldSerializeStackTrace()
        {
            return false;
        }
        /// <summary>
        /// Gets or Sets Source
        /// </summary>
        [DataMember(Name = "source", EmitDefaultValue = true)]
        public string Source { get; private set; }

        /// <summary>
        /// Returns false as Source should not be serialized given that it's read-only.
        /// </summary>
        /// <returns>false (boolean)</returns>
        public bool ShouldSerializeSource()
        {
            return false;
        }
        /// <summary>
        /// Gets or Sets HelpLink
        /// </summary>
        [DataMember(Name = "helpLink", EmitDefaultValue = true)]
        public string HelpLink { get; private set; }

        /// <summary>
        /// Returns false as HelpLink should not be serialized given that it's read-only.
        /// </summary>
        /// <returns>false (boolean)</returns>
        public bool ShouldSerializeHelpLink()
        {
            return false;
        }
        /// <summary>
        /// Gets or Sets HResult
        /// </summary>
        [DataMember(Name = "hResult", EmitDefaultValue = false)]
        public int HResult { get; private set; }

        /// <summary>
        /// Returns false as HResult should not be serialized given that it's read-only.
        /// </summary>
        /// <returns>false (boolean)</returns>
        public bool ShouldSerializeHResult()
        {
            return false;
        }
        /// <summary>
        /// Gets or Sets InnerException
        /// </summary>
        [DataMember(Name = "innerException", EmitDefaultValue = false)]
        public ExceptionModel InnerException { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class ExceptionModel {\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("  Data: ").Append(Data).Append("\n");
            sb.Append("  StackTrace: ").Append(StackTrace).Append("\n");
            sb.Append("  Source: ").Append(Source).Append("\n");
            sb.Append("  HelpLink: ").Append(HelpLink).Append("\n");
            sb.Append("  HResult: ").Append(HResult).Append("\n");
            sb.Append("  InnerException: ").Append(InnerException).Append("\n");
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
