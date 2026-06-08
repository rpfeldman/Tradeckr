using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.InnerComponents
{
    public sealed partial class CategoryDto(string name, ColorDto color, int id) : ObservableObject
    {
        public readonly int CategoryId = id;

        [ObservableProperty]
        public partial string CategoryName { get; set; } = name;

		[ObservableProperty]
		public partial ColorDto Color { get; set; } = color;
	}
}
