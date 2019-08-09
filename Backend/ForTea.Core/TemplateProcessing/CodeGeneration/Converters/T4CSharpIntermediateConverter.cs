using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public class T4CSharpIntermediateConverter : T4CSharpIntermediateConverterBase
	{
		protected const string DefaultGeneratedClassName = "GeneratedTransformation";
		private const string AutoGeneratedMessageResource = "GammaJul.ForTea.Core.Resources.AutoGenerated.cs";

		public T4CSharpIntermediateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult,
			[NotNull] IT4File file
		) : base(intermediateResult, file)
		{
		}

		protected sealed override string ResourceName => "GammaJul.ForTea.Core.Resources.TemplateBaseFull.cs";

		protected override string GeneratedClassName =>
			File.GetSourceFile()?.Name.WithoutExtension() ?? DefaultGeneratedClassName;

		protected sealed override string GeneratedBaseClassName => GeneratedClassName + "Base";

		protected sealed override void AppendSyntheticAttribute()
		{
			// Synthetic attribute is only used for avoiding completion.
			// It is not valid during compilation,
			// so it should not be inserted in code
		}

		protected sealed override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions
		)
		{
			AppendIndent();
			Result.AppendLine("if (Errors.HasErrors) return;");
			foreach (var description in descriptions)
			{
				AppendIndent();
				Result.Append("if (Session.ContainsKey(nameof(");
				Result.Append(description.FieldNameString);
				Result.AppendLine(")))");
				AppendIndent();
				Result.AppendLine("{");
				PushIndent();
				AppendIndent();
				Result.Append(description.FieldNameString);
				Result.Append(" = (");
				Result.Append(description.TypeString);
				Result.Append(") Session[nameof(");
				Result.Append(description.FieldNameString);
				Result.AppendLine(")];");
				PopIndent();
				AppendIndent();
				Result.AppendLine("}");
				AppendIndent();
				Result.AppendLine("else");
				AppendIndent();
				Result.AppendLine("{");
				PushIndent();
				AppendIndent();
				Result.Append(
					"object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(nameof(");
				Result.Append(description.FieldNameString);
				Result.AppendLine("));");
				AppendIndent();
				Result.AppendLine("if (data != null)");
				AppendIndent();
				Result.AppendLine("{");
				PushIndent();
				AppendIndent();
				Result.Append(description.FieldNameString);
				Result.Append(" = (");
				Result.Append(description.TypeString);
				Result.AppendLine(") data;");
				PopIndent();
				AppendIndent();
				Result.AppendLine("}");
				PopIndent();
				AppendIndent();
				Result.AppendLine("}");
			}
		}

		protected override void AppendClass()
		{
			AppendIndent();
			Result.AppendLine();
			AppendClassSummary();
			AppendIndent();
			Result.AppendLine();
			AppendIndent();
			Result.AppendLine($"#line 1 \"{File.GetSourceFile().GetLocation()}\"");
			AppendIndent();
			Result.AppendLine(
				"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"JetBrains.ForTea.TextTemplating\", \"42.42.42.42\")]");
			base.AppendClass();
		}

		protected override void AppendTransformMethod()
		{
			Result.AppendLine("#line hidden");
			AppendIndent();
			Result.AppendLine("/// <summary>");
			AppendIndent();
			Result.AppendLine("/// Create the template output");
			AppendIndent();
			Result.AppendLine("/// </summary>");
			base.AppendTransformMethod();
		}

		private void AppendClassSummary()
		{
			AppendIndent();
			Result.AppendLine("/// <summary>");
			AppendIndent();
			Result.AppendLine("/// Class to produce the template output");
			AppendIndent();
			Result.AppendLine("/// </summary>");
		}

		protected override void AppendGeneratedMessage()
		{
			var provider = new T4TemplateResourceProvider(AutoGeneratedMessageResource, this);
			Result.Append(provider.ProcessResource());
		}

		protected override void AppendParameterDeclaration(T4ParameterDescription description)
		{
			// Range maps of this converter are ignored, so it's safe to use Append instead of AppendMapped
			AppendIndent();
			Result.Append("private ");
			Result.Append(description.TypeString);
			Result.Append(" ");
			Result.Append(description.NameString);
			Result.Append(" => ");
			Result.Append(description.FieldNameString);
			Result.AppendLine(";");
		}

		protected override void AppendHost()
		{
			// Host directive does not work for runtime templates
		}

		protected override void AppendIndent(int size)
		{
			// TODO: use user indents?
			for (int index = 0; index < size; index += 1)
			{
				Result.Append("    ");
			}
		}

		// TODO: use user indents?
		protected override IT4ElementAppendFormatProvider Provider =>
			new T4PreprocessCodeFormatProvider(new string(' ', CurrentIndent * 4));
	}
}
