using CommunityToolkit.Mvvm.ComponentModel;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.InnerComponents
{
    public sealed partial class CategoryDto(string name, ColorDto color, int id) : ObservableObject, IEntity
    {
        public int Id { get; set; } = id;

        [ObservableProperty]
        public partial string CategoryName { get; set; } = name;

		[ObservableProperty]
		public partial ColorDto Color { get; set; } = color;
	}
}
