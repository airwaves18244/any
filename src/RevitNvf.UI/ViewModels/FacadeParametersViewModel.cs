using System.Collections.Generic;
using RevitNvf.Core.Model;

namespace RevitNvf.UI.ViewModels
{
    /// <summary>
    /// Параметры фасадной системы для панели ввода (шаг 8). Зависит только от Core;
    /// собирает доменные конфиги (<see cref="FacadeSystem"/>, <see cref="Cladding"/>,
    /// <see cref="PieLayers"/>), которые читают команды раскладки.
    /// </summary>
    public sealed class FacadeParametersViewModel : ViewModelBase
    {
        private double _bracketStepX;
        private double _bracketStepY;
        private double _bracketEdgeOffsetX;
        private double _bracketEdgeOffsetY;
        private double _panelWidth;
        private double _panelHeight;
        private double _jointWidth;
        private PanelStartMode _startMode;
        private bool _includeEdgePanels;
        private double _insulationThickness;
        private double _airGapThickness;
        private double _claddingThickness;
        private FacadePreset _selectedPreset;

        public FacadeParametersViewModel()
        {
            Presets = FacadePreset.Defaults;
            _selectedPreset = Presets[0];
            ApplyPreset(_selectedPreset);
        }

        public IReadOnlyList<FacadePreset> Presets { get; }

        public IReadOnlyList<PanelStartMode> StartModes { get; } =
            new[] { PanelStartMode.Corner, PanelStartMode.Centered };

        public FacadePreset SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                SetField(ref _selectedPreset, value);
                if (value != null)
                {
                    ApplyPreset(value);
                }
            }
        }

        public double BracketStepX { get => _bracketStepX; set => SetField(ref _bracketStepX, value); }
        public double BracketStepY { get => _bracketStepY; set => SetField(ref _bracketStepY, value); }
        public double BracketEdgeOffsetX { get => _bracketEdgeOffsetX; set => SetField(ref _bracketEdgeOffsetX, value); }
        public double BracketEdgeOffsetY { get => _bracketEdgeOffsetY; set => SetField(ref _bracketEdgeOffsetY, value); }
        public double PanelWidth { get => _panelWidth; set => SetField(ref _panelWidth, value); }
        public double PanelHeight { get => _panelHeight; set => SetField(ref _panelHeight, value); }
        public double JointWidth { get => _jointWidth; set => SetField(ref _jointWidth, value); }
        public PanelStartMode StartMode { get => _startMode; set => SetField(ref _startMode, value); }
        public bool IncludeEdgePanels { get => _includeEdgePanels; set => SetField(ref _includeEdgePanels, value); }
        public double InsulationThickness { get => _insulationThickness; set => SetField(ref _insulationThickness, value); }
        public double AirGapThickness { get => _airGapThickness; set => SetField(ref _airGapThickness, value); }
        public double CladdingThickness { get => _claddingThickness; set => SetField(ref _claddingThickness, value); }

        public void ApplyPreset(FacadePreset preset)
        {
            BracketStepX = preset.Bracket.BracketStepX;
            BracketStepY = preset.Bracket.BracketStepY;
            BracketEdgeOffsetX = preset.Bracket.BracketEdgeOffsetX;
            BracketEdgeOffsetY = preset.Bracket.BracketEdgeOffsetY;
            PanelWidth = preset.Cladding.PanelWidth;
            PanelHeight = preset.Cladding.PanelHeight;
            JointWidth = preset.Cladding.JointWidth;
            StartMode = preset.Cladding.StartMode;
            IncludeEdgePanels = preset.Cladding.IncludeEdgePanels;
            InsulationThickness = preset.Layers.InsulationThickness;
            AirGapThickness = preset.Layers.AirGapThickness;
            CladdingThickness = preset.Layers.CladdingThickness;
        }

        public FacadeSystem ToBracketSystem() =>
            new FacadeSystem(BracketStepX, BracketStepY, BracketEdgeOffsetX, BracketEdgeOffsetY);

        public Cladding ToCladding() =>
            new Cladding(PanelWidth, PanelHeight, JointWidth, StartMode, IncludeEdgePanels);

        public PieLayers ToPieLayers() =>
            new PieLayers(InsulationThickness, AirGapThickness, CladdingThickness);
    }
}
