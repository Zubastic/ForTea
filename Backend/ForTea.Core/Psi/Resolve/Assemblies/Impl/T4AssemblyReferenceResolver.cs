using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl
{
	[SolutionComponent]
	public class T4AssemblyReferenceResolver : IT4AssemblyReferenceResolver
	{
		[NotNull]
		private IT4AssemblyNamePreprocessor Preprocessor { get; }

		[NotNull]
		private IModuleReferenceResolveManager ResolveManager { get; }

		public T4AssemblyReferenceResolver(
			[NotNull] IModuleReferenceResolveManager resolveManager,
			[NotNull] IT4AssemblyNamePreprocessor preprocessor
		)
		{
			ResolveManager = resolveManager;
			Preprocessor = preprocessor;
		}

		[CanBeNull]
		private static AssemblyReferenceTarget FindAssemblyReferenceTarget(string assemblyNameOrFile)
		{
			// assembly path
			var path = FileSystemPath.TryParse(assemblyNameOrFile);
			if (!path.IsEmpty && path.IsAbsolute) return path.ToAssemblyReferenceTarget();

			// assembly name
			var nameInfo = AssemblyNameInfo.TryParse(assemblyNameOrFile);
			if (nameInfo == null) return null;
			return nameInfo.ToAssemblyReferenceTarget();
		}

		[CanBeNull]
		private FileSystemPath Resolve(
			AssemblyReferenceTarget target,
			IProject project,
			IModuleReferenceResolveContext resolveContext
		) => ResolveManager.Resolve(target, project, resolveContext);

		public FileSystemPath Resolve(IT4AssemblyDirective directive)
		{
			var projectFile = directive.ResolutionContext;
			using (Preprocessor.Prepare(projectFile))
			{
				var pathWithMacros = directive.Path;
				return Resolve(pathWithMacros);
			}
		}

		public FileSystemPath Resolve(string assemblyNameOrFile, IPsiSourceFile sourceFile)
		{
			var projectFile = sourceFile.ToProjectFile();
			if (projectFile == null) return null;
			using (Preprocessor.Prepare(projectFile))
			{
				var pathWithMacros = new T4PathWithMacros(assemblyNameOrFile, sourceFile, projectFile);
				return Resolve(pathWithMacros);
			}
		}

		public virtual FileSystemPath Resolve([NotNull] IT4PathWithMacros pathWithMacros)
		{
			string resolved = pathWithMacros.ResolveString();
			string path = Preprocessor.Preprocess(pathWithMacros.ProjectFile, resolved);
			var resolveContext = pathWithMacros.ProjectFile.SelectResolveContext();
			var target = FindAssemblyReferenceTarget(path);
			if (target == null) return null;
			return Resolve(target, pathWithMacros.ProjectFile.GetProject().NotNull(), resolveContext);
		}
	}
}
