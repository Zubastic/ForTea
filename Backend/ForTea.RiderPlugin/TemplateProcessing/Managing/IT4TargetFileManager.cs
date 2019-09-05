using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4TargetFileManager
	{
		[NotNull]
		FileSystemPath GetTemporaryExecutableLocation([NotNull] IT4File file);

		[NotNull]
		FileSystemPath GetExpectedTemporaryTargetFileLocation([NotNull] IT4File file);

		void TryProcessExecutionResults([NotNull] IT4File file);

		[NotNull]
		FileSystemPath SavePreprocessResults([NotNull] IT4File file, [NotNull] string text);

		bool IsGeneratedFrom([NotNull] IProjectFile generated, [NotNull] IProjectFile source);
	}
}
