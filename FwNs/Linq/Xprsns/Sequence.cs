Reflector.Disassembler.Translator+ExpressionHandlingException: Expression stack is empty at offset 0025.
   at Reflector.Disassembler.Translator.PopExpression()
   at Reflector.Disassembler.Translator.DecodeDuplicateVariableStatement()
   at Reflector.Disassembler.Translator.DecodeDuplicateStatement(Int32 offset, IEnumerable`1& AdditionalExpressions)
   at Reflector.Disassembler.Translator.DecodeStatement(Int32 end, IEnumerable`1& AdditionalExpressions)
   at Reflector.Disassembler.Translator.DecodeBlockStatement(Int32 offset, Int32 end)
   at Reflector.Disassembler.Translator.TranslateMethodDeclaration(IMethodDeclaration mD, IMethodBody mB, Boolean handleExpressionStack)
   at Reflector.Disassembler.Translator.TranslateMethodDeclaration(IMethodDeclaration mD, IMethodBody mB)
   at Reflector.Disassembler.Disassembler.TransformMethodDeclaration(IMethodDeclaration value)
   at Reflector.CodeModel.Visitor.Transformer.TransformMethodDeclarationCollection(IMethodDeclarationCollection methods)
   at Reflector.Disassembler.Disassembler.TransformTypeDeclaration(ITypeDeclaration value)
   at Reflector.CodeModel.Visitor.Transformer.TransformTypeDeclarationCollection(ITypeDeclarationCollection value)
   at Reflector.Disassembler.Disassembler.TransformTypeDeclaration(ITypeDeclaration value)
   at Reflector.CodeModel.Visitor.Transformer.TransformTypeDeclarationCollection(ITypeDeclarationCollection value)
   at Reflector.Disassembler.Disassembler.TransformTypeDeclaration(ITypeDeclaration value)
   at Reflector.Application.Translator.TranslateTypeDeclaration(ITypeDeclaration value, Boolean memberDeclarationList, Boolean methodDeclarationBody)
   at Reflector.Application.FileDisassembler.WriteTypeDeclaration(ITypeDeclaration typeDeclaration, String sourceFile, ILanguageWriterConfiguration languageWriterConfiguration)
namespace FwNs.Linq.Xprsns
{
}

