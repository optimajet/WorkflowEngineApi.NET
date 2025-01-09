namespace WorkflowApi.Client.Test.Models;

internal record Scheme(
    string Name,
    Scheme.Designer DesignerSettings,
    List<Scheme.Actor> Actors,
    List<Scheme.Parameter> Parameters,
    List<Scheme.Command> Commands,
    List<Scheme.Timer> Timers,
    List<Scheme.Comment> Comments,
    List<Scheme.Activity> Activities,
    List<Scheme.Transition> Transitions,
    List<Scheme.Translation> Localization,
    List<Scheme.CodeAction> CodeActions,
    string CodeActionsCommonUsings,
    Dictionary<string, object> AdditionalParams,
    bool CanBeInlined,
    bool LogEnabled,
    List<string> InlinedSchemes,
    List<string> Tags,
    string DefiningParametersString,
    string RootSchemeCode,
    Guid? RootSchemeId,
    bool IsObsolete,
    Guid Id,
    List<string> AllowedActivities,
    string CalendarName
)
{
    public record Designer(
        string X,
        string Y,
        string Bending,
        string Scale,
        string Color,
        bool AutoTextContrast,
        string Group,
        bool Hidden,
        string OverwriteActivityTo,
        Designer InlineElementSettings
    );
    
    public record Actor(
        string Name,
        Designer DesignerSettings,
        string OriginalName,
        string OriginalSchemeCode,
        bool WasInlined,
        string Rule,
        string? Value,
        bool IsPredefined
    );

    public record Parameter(
        string Name,
        Designer DesignerSettings,
        string Type,
        string Purpose,
        string? InitialValue
    );

    public record Command(
        string Name,
        Designer DesignerSettings,
        List<ParameterReference> InputParameters,
        string Comment
    );

    public record Timer(
        string Name,
        Designer DesignerSettings,
        string Type,
        string Value,
        bool NotOverrideIfExists
    );

    public record Comment(
        string Name,
        Designer DesignerSettings,
        string Value,
        bool BoldText,
        bool ItalicText,
        bool UnderlineText,
        bool LineThroughText,
        int FontSize,
        double Rotation,
        double Width,
        string Alignment
    );
    
    public record Activity(
        string Name,
        Designer DesignerSettings,
        string OriginalName,
        string OriginalSchemeCode,
        string LastTimeInlineName,
        string FirstTimeInlineName,
        bool WasInlined,
        string ActivityType,
        string SchemeCode,
        string State,
        bool IsInitial,
        bool IsFinal,
        bool IsForSetState,
        bool IsAutoSchemeUpdate,
        string? InlinedSchemeCode,
        bool DisablePersistState,
        bool DisablePersistTransitionHistory,
        bool DisablePersistParameters,
        string? UserComment,
        bool HaveImplementation,
        bool HavePreExecutionImplementation,
        List<ActionReference> Implementation,
        List<ActionReference> PreExecutionImplementation,
        List<Annotation> Annotations,
        ActivityTimeout ExecutionTimeout,
        ActivityTimeout IdleTimeout,
        List<ExceptionsHandler> ExceptionsHandlers,
        bool IsState,
        int? NestingLevel
    );

    public record Transition(
        string Name,
        Designer DesignerSettings,
        string InlinedFinalActivityName,
        string OriginalName,
        string OriginalSchemeCode,
        string LastTimeInlineName,
        string FirstTimeInlineName,
        string UserComment,
        bool WasInlined,
        Activity? From,
        Activity? To,
        string Classifier,
        Trigger Trigger,
        List<Condition> Conditions,
        List<Restriction> Restrictions,
        string AllowConcatenationType,
        string RestrictConcatenationType,
        string ConditionsConcatenationType,
        List<Annotation> Annotations,
        bool IsFork,
        bool MergeViaSetState,
        bool DisableParentStateControl,
        string? SubprocessStartupType,
        string? SubprocessInOutDefinition,
        string? SubprocessName,
        string? SubprocessId,
        string? SubprocessStartupParameterCopyStrategy,
        string? SubprocessFinalizeParameterMergeStrategy,
        string? SubprocessSpecifiedParameters,
        bool IsAlwaysTransition,
        bool IsOtherwiseTransition,
        bool IsConditionTransition
    );

    public record Translation(
        string Name,
        Designer DesignerSettings,
        string ObjectName,
        string Value,
        string Type,
        string Culture,
        bool IsDefault
    );

    public record CodeAction(
        string Name,
        Designer DesignerSettings,
        string OriginalName,
        string OriginalSchemeCode,
        bool WasInlined,
        string ActionCode,
        string Type,
        bool IsGlobal,
        bool IsAsync,
        string Usings,
        string ExcludedUsings,
        List<CodeActionParameter> ParameterDefinitions
    );

    public record ParameterReference(
        string Name,
        bool IsRequired,
        string? DefaultValue,
        Parameter Parameter,
        string? Comment
    );

    public record ActionReference(
        string ActionName,
        int Order,
        string? ActionParameter
    );

    public record Annotation(
        string Name,
        string JsonValue
    );

    public record ActivityTimeout(
        string Type,
        string Timer,
        string NameForSet,
        int RetryCount
    );

    public record ExceptionsHandler(
        string Type,
        string NameForSet,
        int RetryCount,
        List<string> Exceptions,
        int Order
    );

    public record Trigger(
        string Type,
        Command? Command,
        Timer? Timer
    );

    public record Condition(
        string Type,
        ActionReference? Action,
        string? Expression,
        bool? ResultOnPreExecution,
        bool ConditionInversion
    );

    public record Restriction(
        string Type,
        Actor Actor
    );

    public record CodeActionParameter(
        string? Title,
        string Name,
        string Type,
        bool IsRequired,
        List<DropdownValue> DropdownValues,
        string? DefaultValue,
        string? Comment,
        string CustomName
    );

    public record DropdownValue(
        string Name,
        string? Value
    );
}
