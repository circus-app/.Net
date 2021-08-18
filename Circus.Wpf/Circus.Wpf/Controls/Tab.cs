// Copyright (c) 2019-2020, Circus.
//
// Licensed under the Apache License, Version 2.0 (the "License");
//
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
//
// A container for selectable tab items.
//
// This is a very basic implementation of tab control with a tabstrip placement 
// at the bottom.
//
// It does not support tabstrip placement, overflow nor drag and drops.


#pragma warning disable IDE0002

using System.Windows;
namespace Circus.Wpf.Controls {
	/// <summary>Provides a container for selectable tab items.</summary>
	public class Tab : Primitives.Tab {
		static Tab() {
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Tab), new FrameworkPropertyMetadata(typeof(Tab)));
		}
		/// <summary>Constructs a tab.</summary>
		public Tab() {
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new TabItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return Assert.Is<TabItem>(item);
		}
	}
}
