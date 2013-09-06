<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="WebApplication1._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
       <div style="border-style: none; width: 300px; position: absolute; left: 0px; background-color: #FFFFCC; height: 100%; ">
    
        <asp:CheckBox ID="oobCheckBox" runat="server" Text="Out of band" OnCheckedChanged="oobCheckBox_CheckedChanged" AutoPostBack="True" />
        <br />
    
        <asp:Button ID="btnAuthenticate" runat="server" Text="Authenticate" OnClick="btnAuthenticate_Click" />

           <br />

           <asp:Label ID="verifierPinLabel" runat="server" Text="Verifer Pin:" Visible="False"></asp:Label>
           <asp:TextBox ID="txtVerifier" runat="server" Visible="False" OnTextChanged="txtVerifier_TextChanged" AutoPostBack="True" Width="123px"></asp:TextBox>


           <br />
           <asp:Label ID="lblOobHelp" runat="server" Text="Label"><p>For Out Of Band, you need to copy the verifier from the authentication page and paste it here then click "Get Access Token".</p>
           </asp:Label>

           <br />
          
        
    
        <asp:Button ID="btnGetAccessToken" runat="server" Text="Get Access Token" OnClick="btnGetAccessToken_Click" />
    
        <br />
        <asp:Button ID="BtnRefreshToken" runat="server" Text="Refresh Toekn" OnClick="BtnRefreshToken_Click" />
    
        <br />
        <asp:Button ID="btnInvalideToken" runat="server" Text="Invalide Token" OnClick="btnInvalideToken_Click" />
    
        <br />
       
        <br />
    
    </div>
        <div style=" position: absolute; width: 700px; left: 300px; height: 800px; overflow:auto; padding:10px;">
        <asp:Label ID="logOutput" runat="server"></asp:Label>
    
        </div>
    </form>
</body>
</html>
