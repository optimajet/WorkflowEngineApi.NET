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
    /// Defines FilterType
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FilterType
    {
        /// <summary>
        /// Enum And for value: And
        /// </summary>
        [EnumMember(Value = "And")]
        And = 1,

        /// <summary>
        /// Enum Or for value: Or
        /// </summary>
        [EnumMember(Value = "Or")]
        Or = 2,

        /// <summary>
        /// Enum Not for value: Not
        /// </summary>
        [EnumMember(Value = "Not")]
        Not = 3,

        /// <summary>
        /// Enum Equal for value: Equal
        /// </summary>
        [EnumMember(Value = "Equal")]
        Equal = 4,

        /// <summary>
        /// Enum NotEqual for value: NotEqual
        /// </summary>
        [EnumMember(Value = "NotEqual")]
        NotEqual = 5,

        /// <summary>
        /// Enum Greater for value: Greater
        /// </summary>
        [EnumMember(Value = "Greater")]
        Greater = 6,

        /// <summary>
        /// Enum GreaterEqual for value: GreaterEqual
        /// </summary>
        [EnumMember(Value = "GreaterEqual")]
        GreaterEqual = 7,

        /// <summary>
        /// Enum Less for value: Less
        /// </summary>
        [EnumMember(Value = "Less")]
        Less = 8,

        /// <summary>
        /// Enum LessEqual for value: LessEqual
        /// </summary>
        [EnumMember(Value = "LessEqual")]
        LessEqual = 9,

        /// <summary>
        /// Enum Contains for value: Contains
        /// </summary>
        [EnumMember(Value = "Contains")]
        Contains = 10,

        /// <summary>
        /// Enum StartsWith for value: StartsWith
        /// </summary>
        [EnumMember(Value = "StartsWith")]
        StartsWith = 11,

        /// <summary>
        /// Enum EndsWith for value: EndsWith
        /// </summary>
        [EnumMember(Value = "EndsWith")]
        EndsWith = 12,

        /// <summary>
        /// Enum In for value: In
        /// </summary>
        [EnumMember(Value = "In")]
        In = 13
    }

}
