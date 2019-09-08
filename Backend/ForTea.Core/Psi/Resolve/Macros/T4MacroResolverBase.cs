using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public abstract class T4MacroResolverBase : IT4MacroResolver
	{
		[NotNull]
		private IT4AssemblyNamePreprocessor AssemblyNamePreprocessor { get; }

		protected T4MacroResolverBase([NotNull] IT4AssemblyNamePreprocessor preprocessor) =>
			AssemblyNamePreprocessor = preprocessor;

		public virtual bool IsSupported(IT4Macro macro) => true;
		public abstract IReadOnlyDictionary<string, string> ResolveHeavyMacros(IEnumerable<string> macros, IProjectFile file);
		public abstract IReadOnlyDictionary<string, string> ResolveAllLightMacros(IProjectFile file);

		public void InvalidateAssemblies(
			T4FileDataDiff dataDiff,
			ref bool hasChanges,
			IProjectFile file,
			T4AssemblyReferenceManager referenceManager
		)
		{
			using (AssemblyNamePreprocessor.Prepare(file))
			{
				// removes the assembly references from the old assembly directives
				foreach (string assembly in dataDiff
					.RemovedAssemblies
					.Select(it => AssemblyNamePreprocessor.Preprocess(file, it.ResolveString())))
				{
					bool assemblyExisted = referenceManager.References.TryGetValue(assembly, out var cookie);
					if (!assemblyExisted) continue;
					referenceManager.References.Remove(assembly);
					hasChanges = true;
					cookie.Dispose();
				}

				// adds assembly references from the new assembly directives
				foreach (var _ in dataDiff
					.AddedAssemblies
					.Select(it => AssemblyNamePreprocessor.Preprocess(file, it.ResolveString()))
					.Where(addedAssembly => !referenceManager.References.ContainsKey(addedAssembly))
					.Select(referenceManager.TryAddReference)
					.Where(cookie => cookie != null))
				{
					hasChanges = true;
				}
			}
		}
	}
}
