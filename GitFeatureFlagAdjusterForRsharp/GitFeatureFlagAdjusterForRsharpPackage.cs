using Microsoft.VisualStudio.Shell;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Task = System.Threading.Tasks.Task;

namespace GitFeatureFlagAdjusterForRsharp
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(GitFeatureFlagAdjusterForRsharpPackage.PackageGuidString)]
    public sealed class GitFeatureFlagAdjusterForRsharpPackage : AsyncPackage
    {
        /// <summary>
        /// GitFeatureFlagAdjusterForRsharpPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "bbf9038e-e092-48b4-b033-ecfc1dcc2d95";

        #region Package Members

        private Timer _timer;
        
        private static FieldInfo _ephemeralEditsUseDirtyAttributeOnlyField;
        private static PropertyInfo _cachedValueProperty;

        private static readonly TimeSpan _timerInterval = TimeSpan.FromSeconds(10);

        private static void AdjustGitOperationFeatureFlags()
        {
            _ephemeralEditsUseDirtyAttributeOnlyField = _ephemeralEditsUseDirtyAttributeOnlyField
                                                        ?? GetGitSccFeatureFlagFieldInfo("EphemeralEditsUseDirtyAttributeOnly");
            var featureFlag = _ephemeralEditsUseDirtyAttributeOnlyField?.GetValue(null);
            if (featureFlag == null) return;

            _cachedValueProperty = _cachedValueProperty
                                   ?? GetFeatureFlagCachedValuePropertyInfo(featureFlag.GetType());

            _cachedValueProperty?.SetValue(featureFlag, false);
        }

        private static FieldInfo GetGitSccFeatureFlagFieldInfo(string fieldName)
        {
            var targetAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Equals("Microsoft.TeamFoundation.Git.CoreServices"));
            var featureFlagType = targetAssembly?.GetTypes().SingleOrDefault(t => t.FullName.Equals("Microsoft.TeamFoundation.Git.CoreServices.SccFeatureFlags"));
            return featureFlagType?.GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
        }

        private static PropertyInfo GetFeatureFlagCachedValuePropertyInfo(Type featureFlagType)
        {
            return featureFlagType.GetProperty("CachedValue", BindingFlags.Public | BindingFlags.Instance);
        }

        private void OnTick(object state)
        {
            AdjustGitOperationFeatureFlags();
            _timer.Change(_timerInterval, Timeout.InfiniteTimeSpan);
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            AdjustGitOperationFeatureFlags();
            _timer = new Timer(OnTick, new object(), _timerInterval, Timeout.InfiniteTimeSpan);
        }

        protected override void Dispose(bool disposing)
        {
            _timer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _timer?.Dispose();

            base.Dispose(disposing);
        }

        #endregion
    }
}
