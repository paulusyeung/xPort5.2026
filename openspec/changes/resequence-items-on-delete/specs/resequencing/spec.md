# Spec: Resequencing Logic

## ADDED Requirements

#### Scenario: Delete Item in PreOrder
Given a PreOrder with items having LineNumbers 1, 2, 3
When item 2 is deleted
Then the remaining items should have LineNumbers 1, 2 (formerly 3)
And the database should reflect these changes immediately

#### Scenario: Add Item after Delete
Given a PreOrder with items 1, 2 (re-sequenced from 1, 3)
When a new item is added
Then it should get LineNumber 3
And there should be no duplicate LineNumbers

#### Scenario: Other Modules
The same logic applies to Quotation, SalesContract, PurchaseContract, Sample, and Invoice.
