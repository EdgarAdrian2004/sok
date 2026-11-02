using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SOK.UI.Services;
using SOK.UI.ViewModels;

namespace SOK.UI.Views;

public partial class TutorialView : UserControl
{
    private TutorialViewModel? _viewModel;

    public TutorialView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _viewModel = e.NewValue as TutorialViewModel;

        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        RenderCurrentStep();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(TutorialViewModel.CurrentStep) or nameof(TutorialViewModel.CurrentIndex))
        {
            RenderCurrentStep();
        }
    }

    private void RenderCurrentStep()
    {
        var step = _viewModel?.CurrentStep;

        StepViewer.Document = step is null
            ? null
            : MarkdownFlowDocumentBuilder.Build(step.ContentMarkdown);
    }
}
