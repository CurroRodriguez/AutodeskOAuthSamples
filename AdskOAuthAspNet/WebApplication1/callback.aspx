<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="callback.aspx.cs" Inherits="WebApplication1.callback" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        You are authenticated. Now you can close this page. </br>
        Just FYI, the verifier is : <asp:Label ID="lblVerifier" runat="server" Text=""></asp:Label> , you do not have to show this.
    </div>
        <input id="btnClose" type="button" value="Close" onclick="closeThis();" />
    </form>
    <script lang="ja">
        function closeThis() {
          

            window.close();
        }

    </script>
</body>
</html>
