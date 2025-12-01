# Proposal: Resequence Items on Delete

## Background
In `ItemsList.cs` (and similar files in other modules), deleting an item removes it from the database but does not re-sequence the `LineNumber` of the remaining items. When a new item is added, its `LineNumber` is calculated as `Count + 1`, which can result in duplicate LineNumbers if an item was deleted (e.g., deleting item 2 of 3 leaves items 1 and 3; adding a new one gives it LineNumber 3). This causes exceptions when re-opening the order.

## Goal
Ensure that `LineNumber`s are unique and sequential after item deletion to prevent data corruption and runtime errors.

## Scope
- Debug and reproduce the issue in `PreOrder` module.
- Implement re-sequencing logic in `PreOrder`, `Quotation`, `SalesContract`, `PurchaseContract`, `Sample`, and `Invoice` modules.
- Verify the fix.
