using CommunityToolkit.Mvvm.ComponentModel;
using DataServices;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class GraphsPageViewModel(DataProjectionService dataProjectionService) : BaseViewModel
    {
        private DataProjectionService _DataProjectionService = dataProjectionService;

        [ObservableProperty]
        public partial ISeries[] Libertadores { get; set; } = [
        new LineSeries<ObservableValue>
        {
            Name = "Estudiantes",
            // 1960, 1970, 1980, 1990, 2000, 2010, 2020, Actualidad
            Values = [ new(0), new(3), new(3), new(3), new(3), new(4), new(4), new(4) ],
            Stroke = new SolidColorPaint(SKColor.Parse("#FA320A")) { StrokeThickness = 4 },
            GeometryFill = new SolidColorPaint(SKColor.Parse("#FA320A")),
            GeometrySize = 10,
            LineSmoothness = 0.5 // Hace que la curva sea un poco más suave
        },

        new LineSeries<ObservableValue>
        {
            Name = "River",
            Values = [ new(0), new(0), new(0), new(1), new(2), new(2), new(4), new(4) ],
            Stroke = new SolidColorPaint(SKColor.Parse("#FA320A")) { StrokeThickness = 4 },
            GeometryFill = new SolidColorPaint(SKColor.Parse("#FA320A")),
            GeometrySize = 10,
            LineSmoothness = 0.5
        },

        new LineSeries<ObservableValue>
        {
            Name = "Boca",
            Values = [ new(0), new(0), new(2), new(2), new(3), new(6), new(6), new(6) ],
            Stroke = new SolidColorPaint(SKColor.Parse("#0A2AFA")) { StrokeThickness = 4 },
            GeometryFill = new SolidColorPaint(SKColor.Parse("#0A2AFA")),
            GeometrySize = 10,
            LineSmoothness = 0.5
        },

        new LineSeries<ObservableValue>
        {
            Name = "Independiente",
            Values = [ new(0), new(2), new(6), new(7), new(7), new(7), new(7), new(7) ],
            Stroke = new SolidColorPaint(SKColor.Parse("#FF0000")) { StrokeThickness = 4 },
            GeometryFill = new SolidColorPaint(SKColor.Parse("#FF0000")),
            GeometrySize = 10,
            LineSmoothness = 0.5
        },

        new LineSeries<ObservableValue>
        {
            Name = "San Muertenzo",
            Values = [ new(0), new(0), new(0), new(0), new(0), new(0), new(1), new(1) ],
            Stroke = new SolidColorPaint(SKColor.Parse("#1416FF")) { StrokeThickness = 4 },
            GeometryFill = new SolidColorPaint(SKColor.Parse("#1416FF")),
            GeometrySize = 10,
            LineSmoothness = 0.5
        },

        new LineSeries<ObservableValue>
        {
            Name = "Racing",
            Values = [ new(0), new(1), new(1), new(1), new(1), new(1), new(1), new(1) ],
            Stroke = new SolidColorPaint(SKColor.Parse("#02ECF7")) { StrokeThickness = 4 },
            GeometryFill = new SolidColorPaint(SKColor.Parse("#02ECF7")),
            GeometrySize = 10,
            LineSmoothness = 0.5
        },

        new LineSeries<ObservableValue>
        {
            Name = "Virgasia",
            Values = [ new(0), new(0), new(0), new(0), new(0), new(0), new(0), new(0) ],
            Stroke = new SolidColorPaint(SKColor.Parse("#000D61")) { StrokeThickness = 4 },
            GeometryFill = new SolidColorPaint(SKColor.Parse("#000D61")),
            GeometrySize = 10,
            LineSmoothness = 0.5
        }
    ];

        [ObservableProperty]
        public partial Axis[] DecadasEjeX { get; set; } = [
        new Axis
        {
            Labels = ["1960", "1970", "1980", "1990", "2000", "2010", "2020", "Hoy"]
        }
    ];

    }
}
