<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutstandingOrderSummary_v5.aspx.cs"  Inherits="xPort5.Admin.Olap.OutstandingOrderSummary_v5" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxPivotGrid.v15.2, Version=15.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPivotGrid" tagprefix="dx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <dx:ASPxMenu ID="ansOlap" runat="server">
        </dx:ASPxMenu>
    </div>
    <div>
        <dx:ASPxPivotGrid ID="pvgOlap" runat="server" OnFieldValueDisplayText="pvgOlap_FieldValueDisplayText">
        </dx:ASPxPivotGrid>
    </div>
    <dx:ASPxPivotGridExporter ID="pvgExporter" runat="server">
    </dx:ASPxPivotGridExporter>
    </form>
</body>
</html>
