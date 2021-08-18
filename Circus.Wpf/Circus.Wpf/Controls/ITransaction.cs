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
// A control that supports transactions.
//
// This is intended for a control that forwards logic to another control and
// expects a value back.
//
// A good example is an EditableLabel. The control displays its text content in
// a content presenter. On editing, the content is replaced with an
// EditableItemTextBox. Once editing is completed, the textbox should notify
// the editable label providing its new value.
//
// In the above example, the EditableLabel implements ITransaction and the
// EditableItemTextBox takes an ITransaction as parameter. On editing completed,
// the EditableItemTextBox calls the Commit method providing a transaction result,
// the focus state and its new value.
//
// Focus flag states if the focus should be restored since commitment can happen 
// on key down or selection changed.


namespace Circus.Wpf.Controls {
    /// <summary>Defines a control that supports transactions.</summary>
    public interface ITransaction {
        /// <summary>Commits the transaction using the provided transaction result, focus state and value.</summary>
        void Commit(TransactionResult result, bool focus, object value);
    }
}
