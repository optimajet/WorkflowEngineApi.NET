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
    /// Defines ParameterField
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ParameterField
    {
        /// <summary>
        /// Enum Id for value: Id
        /// </summary>
        [EnumMember(Value = "Id")]
        Id = 1,

        /// <summary>
        /// Enum ProcessId for value: ProcessId
        /// </summary>
        [EnumMember(Value = "ProcessId")]
        ProcessId = 2,

        /// <summary>
        /// Enum ParameterName for value: ParameterName
        /// </summary>
        [EnumMember(Value = "ParameterName")]
        ParameterName = 3,

        /// <summary>
        /// Enum Value for value: Value
        /// </summary>
        [EnumMember(Value = "Value")]
        Value = 4
    }

}
