function reInitializeMultiItemsSelector() {

    // check if the item selector checkbox is cheched  
    var scItemCheckboxState = scForm.getCookie('scItemCheckboxState');
    if (scItemCheckboxState && scItemCheckboxState == '1') {
        // get all the tree node gutter
        var allTreeNode = document.querySelectorAll('div.scContentTreeNodeGutter')

        allTreeNode.forEach(element => {
            var checkboxElement = element.parentNode.querySelector('#' + 'item-selector-' + element.id);
            if (!checkboxElement) {
                createCheckBoxSelectorItem(element, false);
            }
        });
        setSelectedTreeItems();
    }
}


function createCheckBoxSelectorItem(element, cheched) {
    checkboxElement = document.createElement('input');
    checkboxElement.type = 'checkbox';
    checkboxElement.className = 'scContentTreeCheckbox';
    checkboxElement.id = 'item-selector-' + ParseCheckboxItemID(element.id);
    checkboxElement.onclick = itemCheckboxSelectEvent;
    checkboxElement.checked = cheched;
    element.parentNode.insertBefore(checkboxElement, element.nextSibling);
}

function removeAllCheckBoxSelector() {
    var allCheckboxItems = document.querySelectorAll('input.scContentTreeCheckbox');
    allCheckboxItems.forEach(input => {
        var parent = input.parentNode;
        parent.removeChild(input);
    });
}

function itemCheckboxSelectEvent() {

    var parent = this.parentNode;
    var element = parent.querySelector('div.scContentTreeNodeGutter');
    createCheckBoxSelectorItem(element, this.checked);
    parent.removeChild(this);
    setSelectedTreeItems();
}

function setSelectedTreeItems() {

    var allCheckedSelector = document.querySelectorAll('input.scContentTreeCheckbox:checked');
    var cookies = '';
    allCheckedSelector.forEach(input => {

        cookies += input.id.replace('item-selector-', '') + ',';
    });

    setCookie('sc_selectedItems', cookies,10);
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays*24*60*60*1000));
    var expires = "expires="+ d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
  }

function ItemSelector_Click() {

    var cookie = scForm.getCookie('scItemCheckboxState');

    if (cookie != '0') {
        scForm.setCookie('scItemCheckboxState', '0');
        removeAllCheckBoxSelector();
    } else {
        scForm.setCookie('scItemCheckboxState', '1');
        reInitializeMultiItemsSelector();
    }

}

function ParseCheckboxItemID(itemId)
{
    return itemId.replace('Gutter', '').replace(/([0-z]{8})([0-z]{4})([0-z]{4})([0-z]{4})([0-z]{12})/,"$1-$2-$3-$4-$5");
}

/////////////////////////////////////////////////

function scContentEditorUpdated() {
    scForm.disableRequests = false;

    var body = $(document.body);
    var editor = $("ContentEditor");
    if (editor) {
        body.fire("sc:contenteditorupdated");

        // obsolete. should only fire on body instead(above).
        editor.fire("sc:contenteditorupdated");
    }

    if (typeof (scGeckoRelayout) != "undefined") {
        scForm.browser.initializeFixsizeElements(true);
        scGeckoRelayout();
    }

    reInitializeMultiItemsSelector();
}

scContentEditor.prototype.expandTreeNode = function (sender, result) {
    var node = null;

    if (typeof (sender) == "string") {
        sender = scForm.browser.getControl(sender);
    }

    if (sender != null) {
        node = sender.parentNode;
    }

    if (node != null) {
        this.collapseTreeNode(sender);

        if (result != "") {
            var container = node.ownerDocument.createElement("div");

            node.appendChild(container);

            container.innerHTML = result;

            scForm.browser.setOuterHtml(node.childNodes[0], scForm.browser.getOuterHtml(node.childNodes[0]).replace("/treemenu_collapsed.png", "/treemenu_expanded.png").replace("/noexpand15x15.gif", "/treemenu_expanded.png").replace("/sc-spinner16.gif", "/treemenu_expanded.png"));

            document.fire("sc:contenttreeupdated", node);
        }
        else {
            scForm.browser.setOuterHtml(node.childNodes[0], scForm.browser.getOuterHtml(node.childNodes[0]).replace("/treemenu_collapsed.png", "/noexpand15x15.gif").replace("/treemenu_expanded.png", "/noexpand15x15.gif").replace("/sc-spinner16.gif", "/noexpand15x15.gif"));
        }
    }
    reInitializeMultiItemsSelector();
}

scContentEditor.prototype.collapseTreeNode = function (sender) {
    if (typeof (sender) == "string") {
        sender = scForm.browser.getControl(sender);
    }

    if (sender != null) {
        node = sender.parentNode;
    }

    var childNumber = 3;
    if (scForm.getCookie('scItemCheckboxState') == '1') {
        childNumber = 4;
    }

    if (node != null) {
        while (node.childNodes.length > childNumber) {
            node.removeChild(node.childNodes[childNumber]);
        }
        scForm.browser.setOuterHtml(node.childNodes[0], scForm.browser.getOuterHtml(node.childNodes[0]).replace("/treemenu_expanded.png", "/treemenu_collapsed.png"));
    }
}
