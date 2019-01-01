using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace SdkSampleExtension.NetFramework
{
    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("Test Suggested Actions")]
    [ContentType("text")]
    internal class TestSuggestedActionsSourceProvider : ISuggestedActionsSourceProvider
    {
        [Import(typeof(ITextStructureNavigatorSelectorService))]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            if (textBuffer == null || textView == null)
            {
                return null;
            }
            return new TestSuggestedActionsSource(this, textView, textBuffer);
        }
    }

    internal class TestSuggestedActionsSource : ISuggestedActionsSource
    {
        private readonly TestSuggestedActionsSourceProvider _mFactory;
        private readonly ITextBuffer _mTextBuffer;
        private readonly ITextView _mTextView;

        public TestSuggestedActionsSource(TestSuggestedActionsSourceProvider testSuggestedActionsSourceProvider, ITextView textView, ITextBuffer textBuffer)
        {
            _mFactory = testSuggestedActionsSourceProvider;
            _mTextBuffer = textBuffer;
            _mTextView = textView;
        }

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range,
            CancellationToken cancellationToken)
        {
            if (TryGetWordUnderCaret(out var extent) && extent.IsSignificant)
            {
                ITrackingSpan trackingSpan = range.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
                var upperAction = new UpperCaseSuggestedAction(trackingSpan);
                return new[] { new SuggestedActionSet(new ISuggestedAction[] { upperAction }) };
            }
            return Enumerable.Empty<SuggestedActionSet>();
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range,
            CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (TryGetWordUnderCaret(out var extent))
                {
                    // don't display the action if the extent has whitespace  
                    return extent.IsSignificant;
                }
                return false;
            }, cancellationToken);
        }

        private bool TryGetWordUnderCaret(out TextExtent wordExtent)
         {
            ITextCaret caret = _mTextView.Caret;
            SnapshotPoint point;

            if (caret.Position.BufferPosition > 0)
            {
                point = caret.Position.BufferPosition - 1;
            }
            else
            {
                wordExtent = default(TextExtent);
                return false;
            }

            ITextStructureNavigator navigator = _mFactory.NavigatorService.GetTextStructureNavigator(_mTextBuffer);

            wordExtent = navigator.GetExtentOfWord(point);
            return true;
        }

        public event EventHandler<EventArgs> SuggestedActionsChanged;
    }

    internal class UpperCaseSuggestedAction : ISuggestedAction
    {
        private readonly ITrackingSpan _mSpan;
        private readonly string _mUpper;
        private readonly string _mDisplay;
        private readonly ITextSnapshot _mSnapshot;

        public UpperCaseSuggestedAction(ITrackingSpan span)
        {
            _mSpan = span;
            _mSnapshot = span.TextBuffer.CurrentSnapshot;
            _mUpper = span.GetText(_mSnapshot).ToUpper();
            _mDisplay = $"Convert '{span.GetText(_mSnapshot)}' to upper case";
        }

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }

        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<SuggestedActionSet>>(null);
        }

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            var textBlock = new TextBlock();
            textBlock.Padding = new Thickness(5);
            textBlock.Inlines.Add(new Run { Text = _mUpper });
            return Task.FromResult<object>(textBlock);
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            _mSpan.TextBuffer.Replace(_mSpan.GetSpan(_mSnapshot), _mUpper);
        }

        public bool HasActionSets => false;

        public string DisplayText => _mDisplay;

        public ImageMoniker IconMoniker => default(ImageMoniker);

        public string IconAutomationText => null;

        public string InputGestureText => null;

        public bool HasPreview => true;
    }
}
