using FluentAssertions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public class T4ExpressionDescription : T4AppendableElementDescriptionBase
	{
		[NotNull]
		private IT4Code Source { get; }

		public T4ExpressionDescription([NotNull] IT4Code source) :
			base(source.GetContainingFile().As<IT4File>().NotNull()) => Source = source;

		public override void AppendContent(
			T4CSharpCodeGenerationResult destination,
			IT4ElementAppendFormatProvider provider,
			IT4File context
		)
		{
			if (!provider.ShouldBreakExpressionWithLineDirective) AppendAllContent(destination, provider, context);
			else AppendAllContentBreakingExpression(destination, provider, context);
		}

		private void AppendAllContentBreakingExpression(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4ElementAppendFormatProvider provider,
			[NotNull] IT4File context
		)
		{
			destination.AppendLine(provider.Indent);
			AppendContentPrefix(destination, provider);
			destination.AppendLine();
			AppendOpeningLineDirective(destination, provider);
			AppendMainContent(destination, provider, context);
			destination.AppendLine();
			AppendClosingLineDirective(destination, provider);
			AppendContentSuffix(destination, provider);
		}

		private void AppendAllContent(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4ElementAppendFormatProvider provider,
			[NotNull] IT4File context
		)
		{
			destination.AppendLine(provider.Indent);
			AppendOpeningLineDirective(destination, provider);
			AppendContentPrefix(destination, provider);
			AppendMainContent(destination, provider, context);
			AppendContentSuffix(destination, provider);
			destination.AppendLine(provider.Indent);
			AppendClosingLineDirective(destination, provider);
		}

		private static void AppendClosingLineDirective(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4ElementAppendFormatProvider provider
		)
		{
			destination.Append(provider.Indent);
			destination.AppendLine("#line default");
			destination.Append(provider.Indent);
			destination.AppendLine("#line hidden");
		}

		private static void AppendContentSuffix(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4ElementAppendFormatProvider provider
		)
		{
			destination.Append(provider.ToStringConversionSuffix);
			destination.AppendLine(provider.ExpressionWritingSuffix);
		}

		private void AppendMainContent(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4ElementAppendFormatProvider provider,
			[NotNull] IT4File context
		)
		{
			destination.Append(provider.CodeCommentStart);
			provider.AppendCompilationOffset(destination, GetOffset(Source));
			if (HasSameSource(context)) provider.AppendMappedIfNeeded(destination, Source);
			else destination.Append(Source.GetText());
			destination.Append(provider.CodeCommentEnd);
		}

		private void AppendOpeningLineDirective(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4ElementAppendFormatProvider provider
		)
		{
			destination.Append(provider.Indent);
			destination.AppendLine(GetLineDirectiveText(Source));
		}

		private static void AppendContentPrefix(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4ElementAppendFormatProvider provider
		)
		{
			destination.Append(provider.Indent);
			destination.Append(provider.ExpressionWritingPrefix);
			destination.Append(provider.ToStringConversionPrefix);
		}
	}
}
