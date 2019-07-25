<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValidateCertificateCardForm.aspx.cs" Inherits="PriceCertificateConfirmer.ValidateCertificateCardForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" language="javascript">
        function DisableBackButton() {
            window.history.forward()
        }
        DisableBackButton();
        window.onload = DisableBackButton;
        window.onpageshow = function (evt) { if (evt.persisted) DisableBackButton() }
        window.onunload = function () { void (0) }
 </script>
    <link href="StyleSheet1.css" rel="Stylesheet" />
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 32px;
        }
        .auto-style2 {
            width: 50%;
        }
    </style>
</head>
<body>
    <form id="ValidateSertForm" runat="server">
        <table class="MainTable">
            <tr>
                <td class="auto-style1">
                    <asp:ImageButton ID="ImgWatsonLogo" runat="server" Height="57px" OnClick="ToMainPage_Click" ImageUrl="~/logo.png" Width="284px" align="left" />
                </td>
                <td style="text-align:left;">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="LbStore" align="right" class="righthoralign" runat="server" Font-Size="12"  />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                
                            </td>
                        </tr>
                     </table>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label ID="lbInfo" Style="text-align: center; line-height: 100%;"
                        align="Center"
                        runat="server"
                        Font-Size="20" Text="Text" />
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <asp:Label ID="lblCaption" runat="server" Font-Size="24" Text="Text"></asp:Label>
                    <br />
                    <br />
                </td>
            </tr>

            <tr>
                <td colspan="2" align="right">
                    <table class="MainTable">
                        <tr>
                            <td width="50%" align="center" >
                                <asp:Label ID="lbTask" runat="server" Text="Text"  />
                            </td>
                        </tr>
                        <tr>
                            <td width="50%" align="center" >
                                 <br />
                            </td>
                        </tr>
                        <tr>
                            <td width="50%" align="center" >
                                <asp:Button ID="btRetrive"
                                    Style="margin-left: 0px;"
                                    Text="Очистити екран"
                                    OnClick="btRetrive_Click"
                                    runat="server" Height="31px" Width="186px"/>
                            </td>
                        </tr>
                    </table>
                   
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <table class="MainTable">
                        <tr>
                            <td align="right" class="auto-style2">
                                <table class="specTable">
                                    <tr class="border_bottom">
                                        <td colspan="2">
                                            <label ="left" >Старий</label>Text</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbOldBarСode" runat="server" Text="Text: "></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbOldBarСode" autocomplete="off" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbOldNumber" runat="server" Text="Номер: "></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbOldNumber" autocomplete="off" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbOldValue" runat="server" Text="Text: "></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbOldValue" autocomplete="off" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Button ID="btNext"
                                                Text="Text"
                                                OnClick="btNextStep1_Click"
                                                runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td width="50%" align="left">
                                <table class="specTable">
                                    <tr class="border_bottom">
                                        <td colspan="2">
                                            <label ="left" >Text</label> сертифікат</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbNewBarСode" runat="server" Text="Штрих код: "></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbNewBarСode" autocomplete="off" Enabled="false" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbNewNumber" runat="server" Text="Text: "></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbNewNumber" autocomplete="off" Enabled="false" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbNewValue" runat="server" Text="Text: "></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbNewValue" autocomplete="off" Enabled="false" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Button ID="btStep2"
                                                Text="Text"
                                                OnClick="btNextStep2_Click"
                                                Enabled="false"
                                                runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style2"></td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <div class="wrapper">
                                    <asp:Button ID="btFinish" CssClass="mybtn"
                                        Text="Text"
                                        OnClick="btFinish_Click"
                                        Enabled="false"
                                        runat="server" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <br />
                    <asp:Label ID="lbError" ForeColor="Red" runat="server" Text=""></asp:Label>
                    <br />
                    <asp:Label ID="lbSucssed" ForeColor="Green" runat="server" Text=""></asp:Label>
                    <br />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
