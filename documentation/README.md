# Documentation

**Module Purpose:**

From our experience with our clients, one of the challenges they have faced is being able to select multiple items at the same time in order to perform some actions like publishing or deleting.
In order to enhance Sitecore Content Editor UI, we have extended some functionalities that enables the content author to select multiple items and perform a certain action easily by providing a user-friendly feature to achieve that.

The main purpose of our module is adding a multi-selection feature for items and implementing two main functionalities in which the user can delete or publish those selected items.
In the future, we are planning to extend the multi-selection functionality to include more actions.


## Summary

**Category:** 

Best enhancement to the Sitecore Admin (XP) UI for Content Editors & Marketers

## Pre-requisites

- Sitecore 9.1 Initial Release

## Installation

- Login to your Sitecore instance, and open the Desktop. 

- On the Sitecore menu (lower left), click Development Tools, then Installation Wizard.

- Upload and browse for the following package “DynamicItemSelectionFeature.zip”

- Choose Overwrite, click Apply to all, and hit Next

- You are all set now!

To make sure that you have installed your package successfully, you will be able to see the following extended features in the content editor:

- “Item Selector” checkbox under the View tab.

![ItemSelectorCheckbox](images/ItemSelectorCheckbox.png?raw=true "Item Selector Checkbox")

- “Delete Selected Items” button under the Home tab.

![DeleteSelectedItems](images/DeleteSelectedItems.png?raw=true "Delete Selected Items Button")

- “Publish Selected Items” button under the Publish tab in the publish menu:

![PublishSelectedItems](images/PublishSelectedItems.png?raw=true "Publish Selected Items Button")

On the Files side, the following files, dlls, configurations, and JS files are installed:

1- Hackathon.Feature.DynamicPublish.dll
2- Hackathon.Feature.DynamicPublish.dll
3- Hackathon.Project.Site.dll
4- Hackathon.Feature.DynamicDelete.dll
5- Overwrite to [sitecore instance]\sitecore\shell\Applications\Content Manager\Default.aspx
6- [sitecore instance]\App_Config\Include\Feature\Hackathon.Feature.DynamicDelete.config
7- [sitecore instance]\App_Config\Include\Feature\Hackathon.Feature.DynamicItemsSelection.config
8- [sitecore instance]\App_Config\Include\Feature\Hackathon.Feature.DynamicPublish.config
9- [sitecore instance]\sitecore\shell\Applications\Content Manager\custom-multi-selection.js

## Usage

In order to be able to use our module, install the following package using Sitecore Installation Wizard: “DynamicItemSelectionFeature.zip”.

You can start by navigating to the content editor, click on the “View tab”, and check the “Item Selector” checkbox in order to view the checkboxes next to the items in the content tree:

![ItemSelectorSelected](images/ItemSelectorSelected.png?raw=true "Item Selector Selected")

There are two main functionalities implemented:

- Delete Selected Items:

Select the items that you wish to delete, navigate to the home tab, and click on "Delete Selected Items", a confirmation popup appears, click on ok to delete or cancel to cancel the deletion.

Note: that if you select the parent item to be deleted (i.e. Contacts), it will delete the children as well.

![DeletingSelectedItemsFunctionality](images/DeletingSelectedItemsFunctionality.png?raw=true "Deleting Selected Items Functionality")

If you click on "Delete Selected Items" without selecting any items, the following popup appears asking you to select items and retry:

![PleaseSelectItems](images/PleaseSelectItems.png?raw=true "Please Select Items")

- Publish Selected Items:

Select the items that you wish to publish, navigate to the publish tab, and click on "Publish Selected Items" from the publish menu.

![SelectItemsToPublish](images/SelectItemsToPublish.png?raw=true "Select Items To Publish")

Once the publish is complete, the following popup appears showing the number of items processed (published):

![PublishCompletedPopup](images/PublishCompletedPopup.png?raw=true "Publish Completed Popup")

If you click on "Publish Selected Items" without selecting any items, the following popup appears asking you to select items and retry:

![PleaseSelectItemsToPublish](images/PleaseSelectItemsToPublish.png?raw=true "Please Select Items To Publish")

## Video

Please provide a video highlighing your Hackathon module submission and provide a link to the video. Either a [direct link](https://www.youtube.com/watch?v=EpNhxW4pNKk) to the video, upload it to this documentation folder or maybe upload it to Youtube...

[![Sitecore Hackathon Video Embedding Alt Text](https://img.youtube.com/vi/EpNhxW4pNKk/0.jpg)](https://www.youtube.com/watch?v=EpNhxW4pNKk)
