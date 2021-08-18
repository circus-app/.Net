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
// A command that is not part of the visual tree.
//
// As opposed to routed commands, relay commands allow to bind a command to
// a method located in a type that is not part of the visual tree.
//
// Let's consider a button and below, a treeview. The treeview data context 
// is bound to a type that derives from DataSource like this:
//
// <Button Command="{CommandBinding TreeViewData, Refresh}" />
// <TreeView DataContext="{DataBinding TreeViewData}" ... >
//
// ... and code:
//
// [ClassCommand("Refresh")]
// public sealed class TreeViewData : DataSource {
//      public TreeViewData() {
//      }
//      private static void Refresh(object sender, ExecutedRelayEventArgs e) {
//          TreeViewData d = (TreeViewData)sender;
//          ... Refresh data ...
//      }
// }
//
// In the above example, we would like the button to invoke the Refresh 
// method of the TreeViewData to perform some action.
//
// This is not possible using routed commands due to TreeViewData not being
// a visual element within the visual tree. Moreover, the command handler is 
// not defined in a control but in a separate type that is bound to a 
// control.
//
// Well, this is where relay commands enter in action.
//
// The previous example uses the self registering pattern declaring a class
// command attribute but we could manually set the whole chain like this:
//
// public sealed class TreeViewData : DataSource {
//      public static readonly RelayCommand Refresh;
//      static TreeViewData() {
//          Refresh = new RelayCommand("Refresh", typeof(TreeViewData));
//          RelayCommandManager.Register(new RelayCommandBinding(Refresh, RefreshInternal)); 
//      }
//      private static void RefreshInternal(object sender, ExecutedRelayEventArgs e) {
//          ...
//      }
// }
//
// ... and xaml:
//
// <Button Command="{x:Static TreeViewData.Refresh}" />
//
// As shown in this second implementation, a static relay command is 
// declared providing a name and the type that owns the method to invoke. 
// The next step is to create a relay command binding that links the 
// command to the desired delegate. Finally, we register the binding to the
// command manager.
//
// Execution uses the context field that corresponds to the DataContext of
// the command source. This is to determine the instance on which the 
// method is invoked. If it is null (our case in the examples) and the
// owner is a shared DataSource (see DataSource for details), it uses the 
// instance provided by the DataSources manager.
//
// These examples use a class derived from DataSource but relay commands 
// will work with any type provided that context is not null.
//
// Most packaged controls (Button, ComboBoxItem, ToggleButton, etc.) 
// support relay commands.
//
// Relay commands do not support input gestures bindings since they do not 
// have a focus scope.


#pragma warning disable IDE0002

using System;
using System.Windows;
using System.Windows.Input;
using Circus.Wpf.Data;
namespace Circus.Wpf.Input {
    /// <summary>Provides a command that is not part of the visual tree.</summary>
    public class RelayCommand : ICommand {
        private object context;
        public event EventHandler CanExecuteChanged {
            add { 
                CommandManager.RequerySuggested += value; 
            }
            remove { 
                CommandManager.RequerySuggested -= value; 
            }
        }
        /// <summary>Returns the name of the command.</summary>
        public string Name { get; private set; }
        /// <summary>Returns the type that owns the method to execute.</summary>
        public Type Owner { get; private set; }
        private RelayCommand() { 
        }
        /// <summary>Constructs a relay command with the specified name and owner.</summary>
        public RelayCommand(string name, Type owner) {
            this.Name = name;
            this.Owner = owner;
        }
        public bool CanExecute(object parameter) {
            return true;
        }
        public void Execute(object parameter) {
            if (RelayCommandManager.Get(this, out RelayCommandBinding b)) {
                if (Assert.Null(this.context)) {
                    if (!RelayCommand.IsDataSource(this.Owner, out DataSource data)) {
                        return;
                    }
                    this.context = data;
                }
                b.OnExecuted(this.context, new ExecutedRelayEventArgs(this, parameter));
            }
        }
        private static bool IsDataSource(Type owner, out DataSource data) {
            data = Assert.Is<DataSource>(owner) ? DataSources.Get(owner) : null;
            return Assert.NotNull(data);
        }
        internal static void RegisterContext(DependencyObject d, object value) {
            if (Assert.Is(value, out RelayCommand command)) {
                command.context = d.GetValue(FrameworkElement.DataContextProperty);
            }
        }
    }
}
