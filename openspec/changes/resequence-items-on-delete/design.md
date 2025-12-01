# Design: Resequence Items on Delete

## Problem
The current implementation of `DeleteItem` in `xPort5.Controls.Utility` only deletes the record. The UI logic in `ItemsList.cs` calculates the new LineNumber based on `ListView.Items.Count + 1`. This leads to collisions.

## Solution Options
1.  **Resequence in UI**: After deletion, iterate through the ListView items and update their LineNumbers in the database.
2.  **Resequence in DAL**: In `Utility.DeleteItem`, after deletion, load all remaining items for the parent order and update their LineNumbers.
3.  **Fix New Item Logic**: Instead of `Count + 1`, find the maximum LineNumber and add 1.

## Preferred Approach
**Option 2 (Resequence in DAL)** is the most robust as it ensures data consistency regardless of the UI state. However, it requires more database operations.
**Option 3** prevents duplicates but leaves gaps (1, 3, 4). This might be acceptable but "re-sequence" implies filling gaps.
The user specifically asked to "re-sequence the remaining items", so Option 1 or 2 is required.
Given the legacy nature and the "Utility" class structure, implementing it in `Utility.DeleteItem` seems appropriate to centralize the logic.

## Implementation Details
Modify `xPort5.Controls.Utility.cs`:
- Update `OrderQT.DeleteItem`
- Update `OrderPL.DeleteItem`
- Update `OrderSC.DeleteItem`
- Update `OrderPC.DeleteItem`
- Update `OrderIN.DeleteItem`
- Update `OrderSP.DeleteItem`

For each, after `orderItem.Delete()`, fetch the remaining items ordered by `LineNumber`, and update them to be 1, 2, 3...
