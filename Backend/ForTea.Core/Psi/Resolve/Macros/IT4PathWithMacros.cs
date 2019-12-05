using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public interface IT4PathWithMacros
	{
		[NotNull]
		string ResolveString();

		[NotNull, Obsolete("Consider using explicit assembly/file resolver")]
		FileSystemPath ResolvePath();

		[CanBeNull]
		IPsiSourceFile Resolve();

		[NotNull]
		IProjectFile ProjectFile { get; }

		[NotNull]
		string RawPath { get; }

		[NotNull, ItemNotNull]
		IEnumerable<string> RawMacros { get; }
	}
}
