using Microsoft.CodeAnalysis;

namespace SourceGeneratorSupplement.Factory;

static class Format
{
    //include readonly modifier
    public static SymbolDisplayFormat TypeDeclaration { get; } = new SymbolDisplayFormat(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
        kindOptions: SymbolDisplayKindOptions.IncludeTypeKeyword,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters |
                         SymbolDisplayGenericsOptions.IncludeVariance
    );

    //exclude readonly modifier
    public static SymbolDisplayFormat PartialTypeDeclaration { get; } = new SymbolDisplayFormat(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters |
                         SymbolDisplayGenericsOptions.IncludeVariance
    );

    public static SymbolDisplayFormat MethodDeclarationExcludeModifiers { get; } = new SymbolDisplayFormat(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        kindOptions: SymbolDisplayKindOptions.IncludeMemberKeyword,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters |
                         SymbolDisplayGenericsOptions.IncludeVariance,
        parameterOptions: SymbolDisplayParameterOptions.IncludeName |
                          SymbolDisplayParameterOptions.IncludeType |
                          SymbolDisplayParameterOptions.IncludeDefaultValue |
                          SymbolDisplayParameterOptions.IncludeExtensionThis |
                          SymbolDisplayParameterOptions.IncludeParamsRefOut,
        memberOptions: SymbolDisplayMemberOptions.IncludeType |
                       SymbolDisplayMemberOptions.IncludeParameters |
                       SymbolDisplayMemberOptions.IncludeConstantValue |
                       SymbolDisplayMemberOptions.IncludeExplicitInterface |
                       SymbolDisplayMemberOptions.IncludeRef,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                              SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
    );

    public static SymbolDisplayFormat MethodDeclaration { get; } = new SymbolDisplayFormat(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        kindOptions: SymbolDisplayKindOptions.IncludeMemberKeyword,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters |
                         SymbolDisplayGenericsOptions.IncludeVariance,
        parameterOptions: SymbolDisplayParameterOptions.IncludeName |
                          SymbolDisplayParameterOptions.IncludeType |
                          SymbolDisplayParameterOptions.IncludeDefaultValue |
                          SymbolDisplayParameterOptions.IncludeExtensionThis |
                          SymbolDisplayParameterOptions.IncludeParamsRefOut,
        memberOptions: SymbolDisplayMemberOptions.IncludeType |
                       SymbolDisplayMemberOptions.IncludeParameters |
                       SymbolDisplayMemberOptions.IncludeConstantValue |
                       SymbolDisplayMemberOptions.IncludeExplicitInterface |
                       SymbolDisplayMemberOptions.IncludeModifiers |
                       SymbolDisplayMemberOptions.IncludeAccessibility |
                       SymbolDisplayMemberOptions.IncludeRef,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                              SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
    );

    public static SymbolDisplayFormat NamespaceDeclaration { get; } = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        kindOptions: SymbolDisplayKindOptions.IncludeNamespaceKeyword |
                     SymbolDisplayKindOptions.IncludeTypeKeyword |
                     SymbolDisplayKindOptions.IncludeMemberKeyword
    );
}
