$ErrorActionPreference = "Stop"

function CheckExitCode()
{
    if ($LASTEXITCODE -ne 0)
    {
        throw "Build failed. Last exit code $LASTEXITCODE"
    }
}

function RemoveIfExists($Path)
{
    if (Test-Path $Path)
    {
        Remove-Item $Path -Force -Recurse
    }
}

$Path = "$( $PSScriptRoot )/../"
$GeneratedPath = "$( $Path )/.generated"
$SwaggerPath = "$( $Path )/WorkflowApi/.swagger"
$ApiName = "WorkflowApi"
$ProjectName = "$( $ApiName ).Client"

Write-Host "Creating folder for generated content" -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path $GeneratedPath | Out-Null

Write-Host "Clearing generated content" -ForegroundColor Yellow
RemoveIfExists $GeneratedPath

Write-Host "Generating C# client" -ForegroundColor Yellow
docker run --rm -v "$( $SwaggerPath ):/swagger" -v "$( $GeneratedPath ):/output" `
    openapitools/openapi-generator-cli@sha256:3c7deefff276cf383c83069ec7ce4b021063ebf737ff07667a18028e68a95323 `
    generate -i /swagger/swagger.yaml `
    -o /output/csharp -g csharp `
    --additional-properties="apiName=$($ApiName),packageName=$($ProjectName),targetFramework=net8.0,nullableReferenceTypes=true,useDateTimeOffset=true"
CheckExitCode

Write-Host "Remove unnecessary files" -ForegroundColor Yellow
RemoveIfExists "$( $GeneratedPath )/csharp/src/$( $ProjectName )/$( $ProjectName ).csproj"

Write-Host "Remove old client files" -ForegroundColor Yellow
RemoveIfExists "$( $Path )/$( $ProjectName )/Api"
RemoveIfExists "$( $Path )/$( $ProjectName )/Client"
RemoveIfExists "$( $Path )/$( $ProjectName )/Model"

Write-Host "Copying generated client to target project" -ForegroundColor Yellow
Copy-Item "$( $GeneratedPath )/csharp/src/$( $ProjectName )" "$( $Path )" -Recurse -Force

Write-Host "Patching generated files" -ForegroundColor Yellow
$PatchingFilePath = "$( $Path )/$( $ProjectName )/Client/ClientUtils.cs"
$PatchingFragment = @'
        public static string ParameterToString(object obj, IReadableConfiguration configuration = null)
        {
            if (obj is DateTime dateTime)
                // Return a formatted date string - Can be customized with Configuration.DateTimeFormat
                // Defaults to an ISO 8601, using the known as a Round-trip date/time pattern ("o")
                // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8
                // For example: 2009-06-15T13:45:30.0000000
                return dateTime.ToString((configuration ?? GlobalConfiguration.Instance).DateTimeFormat);
            if (obj is DateTimeOffset dateTimeOffset)
                // Return a formatted date string - Can be customized with Configuration.DateTimeFormat
                // Defaults to an ISO 8601, using the known as a Round-trip date/time pattern ("o")
                // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8
                // For example: 2009-06-15T13:45:30.0000000
                return dateTimeOffset.ToString((configuration ?? GlobalConfiguration.Instance).DateTimeFormat);
            if (obj is bool boolean)
                return boolean ? "true" : "false";
            if (obj is ICollection collection) {
                List<string> entries = new List<string>();
                foreach (var entry in collection)
                    entries.Add(ParameterToString(entry, configuration));
                return string.Join(",", entries);
            }
            if (obj is Enum && HasEnumMemberAttrValue(obj))
                return GetEnumMemberAttrValue(obj);

            return Convert.ToString(obj, CultureInfo.InvariantCulture);
        }
'@.Replace("`r`n", "`n")
$TargetFragment = @'
        public static string ParameterToString(object obj, IReadableConfiguration configuration = null)
        {
            if (obj is DateTime dateTime)
                // Return a formatted date string - Can be customized with Configuration.DateTimeFormat
                // Defaults to an ISO 8601, using the known as a Round-trip date/time pattern ("o")
                // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8
                // For example: 2009-06-15T13:45:30.0000000
                return dateTime.ToString((configuration ?? GlobalConfiguration.Instance).DateTimeFormat);
            if (obj is DateTimeOffset dateTimeOffset)
                // Return a formatted date string - Can be customized with Configuration.DateTimeFormat
                // Defaults to an ISO 8601, using the known as a Round-trip date/time pattern ("o")
                // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8
                // For example: 2009-06-15T13:45:30.0000000
                return dateTimeOffset.ToString((configuration ?? GlobalConfiguration.Instance).DateTimeFormat);
            if (obj is bool boolean)
                return boolean ? "true" : "false";
            if (obj is ICollection collection) {
                List<string> entries = new List<string>();
                foreach (var entry in collection)
                    entries.Add(ParameterToString(entry, configuration));
                return string.Join(",", entries);
            }
            if (obj is Enum && HasEnumMemberAttrValue(obj))
                return GetEnumMemberAttrValue(obj);
            if (obj is string str)
                return str;
            if (obj.GetType().IsClass)
                return Serialize(obj);

            return Convert.ToString(obj, CultureInfo.InvariantCulture);
        }
'@

if (Test-Path $PatchingFilePath) {
    $fileContent = Get-Content $PatchingFilePath -Raw

    if ($fileContent -match [regex]::Escape($PatchingFragment)) {
        $newContent = $fileContent -replace [regex]::Escape($PatchingFragment), $TargetFragment
        Set-Content -Path $PatchingFilePath -Value $newContent
    } else {
        throw "Unexpected code fragment in patching file."
    }
} else {
    throw "Patching file '$PatchingFilePath' not found."
}
