<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Sitecore.Shell.Applications.ContentManager.ContentEditorPage" %>

<%@ Import Namespace="Sitecore" %>
<%@ Import Namespace="Sitecore.Globalization" %>
<%@ Import Namespace="Sitecore.Web" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.HtmlControls" Assembly="Sitecore.Kernel" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register src="~/sitecore/shell/Applications/GlobalHeader.ascx" tagName="GlobalHeader" TagPrefix="uc" %>
<asp:placeholder id="DocumentType" runat="server" />
<html>
<head runat="server">
    <script type="text/JavaScript" src="/sitecore/shell/Controls/Lib/jQuery/jquery-1.12.4.min.js"></script>
    <script type="text/javascript">if (!window.$sc) $sc = jQuery.noConflict();</script>
    <title><%= HttpUtility.HtmlEncode(Translate.Text(WebUtil.GetQueryString("he", "Content Editor"))) %></title>
    <link rel="shortcut icon" href="/sitecore/images/favicon.ico" />
    <asp:PlaceHolder ID="BrowserTitle" runat="server" />
    <sc:Stylesheet runat="server" Src="Ribbon.css" DeviceDependant="true" />
    <sc:Stylesheet runat="server" Src="Content Manager.css" DeviceDependant="true" />
    <asp:PlaceHolder ID="Stylesheets" runat="server" />
    <script type="text/JavaScript" src="/sitecore/shell/controls/SitecoreObjects.js"></script>
    <script type="text/JavaScript" src="/sitecore/shell/controls/SitecoreKeyboard.js"></script>
    <script type="text/JavaScript" src="/sitecore/shell/controls/SitecoreVSplitter.js"></script>
    <script type="text/JavaScript" src="/sitecore/shell/controls/SitecoreWindow.js"></script>
    <script type="text/JavaScript" src="/sitecore/shell/Applications/Content Manager/Content Editor.js"></script>
	<script type="text/JavaScript" src="/sitecore/shell/Applications/Content Manager/custom-muti-selection.js"></script>
    <script type="text/JavaScript" src="/sitecore/shell/Applications/Content Manager/Content Editor.Search.js"></script>
    <script type="text/JavaScript" src="/sitecore/shell/controls/TreeviewEx/TreeviewEx.js"></script>
    <script type="text/JavaScript" src="/sitecore/shell/Controls/Lib/Scriptaculous/Scriptaculous.js"></script>
    <script type="text/JavaScript" src="/sitecore/shell/Controls/Lib/Scriptaculous/Effects.js"></script>
    <script type="text/JavaScript" src="/sitecore/shell/Controls/Lib/Scriptaculous/DragDrop.js"></script>
    <script type="text/javascript" src="/sitecore/shell/Controls/Lib/jQuery/jquery-splitter/jquery-splitter.js"></script>
    <script type="text/javascript">
      window.scForm && scForm.enableModifiedHandling();
    </script>
</head>
<body runat="server" id="Body" class="scWindowBorder1 contentEditor" onmousedown="javascript:scWin.mouseDown(this, event)"
    onmousemove="javascript:scWin.mouseMove(this, event)" onmouseup="javascript:scWin.mouseUp(this, event)">
    <form id="ContentEditorForm" style="overflow: hidden" runat="server">
        <sc:CodeBeside runat="server" Type="Sitecore.Shell.Applications.ContentManager.ContentEditorForm, Sitecore.Client" />
        <sc:DataContext runat="server" ID="ContentEditorDataContext" />
        <sc:RegisterKey runat="server" KeyCode="120" Click="system:publish" />
        <asp:PlaceHolder ID="scLanguage" runat="server" />
        <input type="hidden" id="scActiveRibbonStrip" name="scActiveRibbonStrip" />
        <input type="hidden" id="scEditorTabs" name="scEditorTabs" />
        <input type="hidden" id="scActiveEditorTab" name="scActiveEditorTab" />
        <input type="hidden" id="scPostAction" name="scPostAction" />
        <input type="hidden" id="scShowEditor" name="scShowEditor" />
        <input type="hidden" id="scSections" name="scSections" />
        <div id="outline" class="scOutline" style="display: none">
        </div>
        <span id="scPostActionText" style="display: none">
            <sc:Literal Text="The main window could not be updated due to the current browser security settings. You must click the Refresh button yourself to view the changes."
                runat="server" />
        </span>
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <telerik:RadSpell ID="RadSpell" Skin="Metro" DialogsCssFile="/sitecore/shell/themes/standard/default/Content Manager.css" LocalizationPath="~/sitecore/shell/Applications/Content Manager/Localization/" SpellCheckProvider="EditDistanceProvider" EditDistance="2" runat="server" />
        <iframe id="overlayWindow" src="/sitecore/shell/Controls/Rich Text Editor/EditorWindow.aspx" style="position: absolute; width: 100%; height: 100%; top: 0; left: 0; right: 0; bottom: 0; display: none; z-index: 999; border: none" frameborder="0" allowtransparency="allowtransparency"></iframe>
        <uc:GlobalHeader runat="server" />
        <div class="scFlexColumnContainer scWindowBorder2" style="height: 100%;" onactivate="javascript:scWin.activate(this, event);">
            <div class="scCaption scWindowHandle scDockTop" ondblclick="javascript:scWin.maximizeWindow();" style="min-height: 1px;">
                <div id="CaptionTopLine">
                    <img src="/sitecore/images/blank.gif" width="1" height="1" alt="" />
                </div>
                <div class="scSystemButtons">
                    <asp:PlaceHolder ID="WindowButtonsPlaceholder" runat="server" />
                </div>
                <%--       SystemMenu   moved from line 62 --%>
                <a id="SystemMenu" runat="server" href="#" class="scSystemMenu" onclick="javascript:return scForm.postEvent(this, event, 'SystemMenu_Click')" ondblclick="javascript:return scForm.invoke('contenteditor:close')"></a>
                <div runat="server" id="RibbonPanel" class="RibbonPanel" onclick="javascript:scContent.onEditorClick(this, event);">
                    <asp:PlaceHolder ID="RibbonPlaceholder" runat="server" />
                </div>
            </div>
            <div class="scFlexContent" id="MainPanel" onclick="javascript:scContent.onEditorClick(this, event);">
                <div class="scStretchAbsolute scContentEditorSplitter">
                    <div id="ContentTreePanel">
                        <div class="scFlexColumnContainerWithoutFlexie" style="height: 100%; width: 100%; position: relative">
                            <div id="SearchPanel">
                                <table>
                                    <tr>
                                        <td runat="server">
                                            <input id="TreeSearch" class="scSearchInput scIgnoreModified"
                                                value="<%= Translate.Text(Texts.SEARCH) %>" onkeydown="javascript:if(event.keyCode==13){var result = scForm.postEvent(this,event,'TreeSearch_Click',true);scContent.fixSearchPanelLayout();return result;}"
                                                onfocus="javascript:scContent.watermarkFocus(this,event);" onblur="javascript:scContent.watermarkBlur(this,event);" />
                                        </td>
                                        <td>
                                            <div style="padding: 8px;">
                                                <div class="scElementHover elementImgAlignment">
                                                    <a href="#" class="scSearchButton" onclick="javascript:var result = scForm.postEvent(this,event,'TreeSearch_Click',true);scContent.fixSearchPanelLayout();return result;">
                                                        <sc:ThemedImage runat="server" Src="Office/16x16/magnifying_glass.png" Width="16" Height="16" />
                                                    </a>
                                                </div>
                                            </div>
                                        </td>
                                        <td>
                                            <div style="float: right;">
                                                <div class="scElementHover elementImgAlignment">
                                                    <a href="#" class="scSearchOptionsButton" onclick="javascript:Element.toggle('TreeSearchOptions');scContent.fixSearchPanelLayout(); if (typeof(scGeckoRelayout) != 'undefined') scGeckoRelayout();">
                                                        <sc:ThemedImage runat="server" Src="Images/down_h.png" Width="16" Height="16" />
                                                    </a>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <table id="TreeSearchOptions" width="100%" style="display: none;">
                                    <tr>
                                        <td>
                                            <table id="SearchOptionsList" width="100%" onkeydown="javascript:if(event.keyCode==13){var result = scForm.postEvent(this,event,'TreeSearch_Click',true);scContent.fixSearchPanelLayout(); return result;}">
                                                <col align="right" />
                                                <col width="100%" />
                                                <tr>
                                                    <td class="scSearchOptionsNameContainer" style="min-width: 110px;">
                                                            <div class="scElementHover elementPadding">
                                                                <a id="SearchOptionsControl0" href="#" class="scSearchOptionName searchOptionAlignment" onclick="javascript:return scForm.postEvent(this,event,'TreeSearchOptionName_Click',true);">
                                                                    <sc:Literal Text="Name" runat="server" />
                                                                    <sc:ThemedImage runat="server" Src="Images/down_h.png" Class="arrowDown" Width="16" Height="16" />
                                                                </a>
                                                            </div>
                                                    </td>
                                                    <td class="scSearchOptionsValueContainer">
                                                        <input id="SearchOptionsValue0" class="scSearchOptionsInput scIgnoreModified" />
                                                        <input id="SearchOptionsName0" type="hidden" value="_name" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top" class="criteriaAreaAlignment">
                                                        <div class="scElementHover elementPadding">
                                                            <a href="#" class="scSearchAddCriteria" onclick="javascript:var result = scContent.addSearchCriteria(this,event);scContent.fixSearchPanelLayout();return result;">
                                                                <sc:ThemedImage Src="Office/16x16/add_search.png" Width="16" Height="16" runat="server"
                                                                    Align="absmiddle" Style="margin: 0 4px 0 0" />
                                                                <sc:Literal Text="Add Criteria" runat="server" Class="verticalAlignmentContainer" />
                                                            </a>
                                                        </div>
                                                    </td>
                                                    <td valign="top" class="scSearchOptionsValueContainer scSearchAddCriteriaInput" runat="server">
                                                        <input id="SearchOptionsAddCriteria" class="scSearchOptionsInput scIgnoreModified"
                                                            style="color: #999999" value="<%= Translate.Text(Texts.FIELD1) %>" onkeydown="javascript:if(event.keyCode==13){var result = scContent.addSearchCriteria(this,event);scContent.fixSearchPanelLayout();return result;}"
                                                            onfocus="javascript:scContent.watermarkFocus(this,event);" onblur="javascript:scContent.watermarkBlur(this,event);" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="scFlexContentWithoutFlexie scFlexColumnContainerWithoutFlexie" id="SearchResultHolder" style="display: none">
                                <div id="SearchHeader" class="scSearchHeader">
                                    <div>
                                        <a href="#" style="float: right" onclick="javascript:return scContent.closeSearch(this,event);" class="scElementHover">
                                            <sc:ThemedImage runat="server" Src="Images/tab_close_24x24_h.png" Width="24" Height="24" Margin="0px 0px 0px 0px" RollOver="False" />
                                        </a>
                                    </div>
                                    <div style="padding-top: 5px;">
                                        <sc:ThemedImage runat="server" Src="Office/16x16/magnifying_glass.png" Width="16" Height="16"
                                            Align="absmiddle" />
                                        <span id="SearchResultsHeader"></span>
                                    </div>
                                </div>
                                <div class="scFlexContentWithoutFlexie" id="SearchResult" style="background: white; overflow: auto; padding: 15px;">
                                </div>
                            </div>
                            <div class="scFlexContentWithoutFlexie" id="ContentTreeHolder">
                                <div class="scContentTreeContainer" style="height: 100%">
                                    <asp:PlaceHolder ID="ContentTreePlaceholder" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <sc:Border ID="ContentEditor" runat="server" Class="scEditor" />
                </div>
            </div>
            <asp:PlaceHolder ID="Pager" runat="server" />
            <div class="scWindowBorder3" id="BottomBorder" runat="server"></div>
        </div>
        <sc:KeyMap runat="server" />
    </form>
    <script>
        // Do not move this code to "scContentEditor.prototype.onLoad", because it starts running much faster here.
        var href = window.location.href;
        if ((scForm.getCookie("scContentEditorFolders") != "0") && href.indexOf("mo=mini") < 0 && href.indexOf("mo=popup") < 0
          || href.indexOf("mo=template") >= 0) {
          scContent.turnOnSplitter();
        }
    </script>
</body>
</html>
