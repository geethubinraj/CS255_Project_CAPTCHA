<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Captcha_HomePage.aspx.cs" Inherits="CS255_TextCAPTCHA.Captcha_Page" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 413px; height: 255px;padding-left:550px">
            <br /><br /><br /><br /><br /><br /><br /><br /><br />
            

            <table id="Table1" 
             style="border-width:12px; border-color:Black; background-color:#f2f0eb;width:400px"
             runat="server">
                <tr>
                    <td>
                        <asp:Image id="img_captcha" runat="server"></asp:Image>

                    </td>
                </tr>
                    <tr>
                    <td style="padding-left:265px">
                        <asp:LinkButton ID="lnk_refresh" Font-Italic="true" runat="server" Text="Re-Generate CAPTCHA" OnClick ="lnk_refresh_Click"></asp:LinkButton>
                        </td>
                     </tr>
               <tr> <td></td></tr><tr> <td>
                </td></tr><tr> <td></td></tr><tr> <td></td></tr><tr> <td></td></tr><tr> <td></td></tr><tr> <td></td></tr>
            
                <tr>
                    <td style="padding-left:50px">
                       
                        <asp:TextBox ID="txtbox_captcha" style="width:200px" CausesValidation="true" runat="server" ></asp:TextBox>
                         <asp:RequiredFieldValidator ID="reqFieldvalidation" ControlToValidate="txtbox_captcha" ValidationGroup="Checknonemptyvalue" ForeColor="red" ErrorMessage="Required*" Display="Dynamic" runat="server"></asp:RequiredFieldValidator>
                    </td>
                </tr>

                 <tr> <td  style="padding-left:280px">
                        <asp:ScriptManager ID="ScriptManager1" runat="server"> </asp:ScriptManager>

           <asp:Timer ID="Timer1" runat="server" Interval="1000"   OnTick="Timer1_Tick"></asp:Timer>

           <asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional">

               <ContentTemplate>
                 
                   <asp:Label ID="lbltimer" Font-Bold="true" Font-Italic="true" ForeColor ="red" runat="server"></asp:Label>
               </ContentTemplate>
                   <Triggers>
                       <asp:AsyncPostBackTrigger ControlID ="Timer1" EventName ="tick" />
                   </Triggers>
           </asp:UpdatePanel>  
                      </td>

                 </tr>
               
                  <tr>
                    <td style="padding-left:130px">
                        <asp:Button ID="btn_CAPTCHAsubmit" Text="Submit" BackColor="#999999" Font-Size="Medium" ValidationGroup="Checknonemptyvalue"  runat ="server" OnClick="btn_CAPTCHAsubmit_Click" Height="26px" Width="70px" />
                    </td>
                </tr>
                 <tr> <td>
                      </td></tr>
                
                <tr> <td>
                    
                     </td></tr><tr> <td></td></tr><tr> <td></td></tr>
                <tr> 
                    <td style="padding-left:50px">
                     <asp:Label runat="server" Font-Bold="true" Font-Italic="true" id="lbl_msgresult"></asp:Label> 
                     </td>
                </tr>
                 <tr> <td></td></tr>           
                <tr> <td></td></tr> 
                <tr> <td></td></tr>
                <tr> <td></td></tr>
                <tr> <td></td></tr>
                <tr> <td></td></tr>
                <tr> <td></td></tr>
                 <tr> <td></td></tr>           
                <tr> <td></td></tr>
               
                <tr> 
                    <td>
                       <b><i> Please submit your feedback based on your experience</i></b></td>
                </tr>
     <tr> <td></td></tr>           <tr> <td></td></tr>
                <tr>
                    <td style="padding-left:50px" >
                <asp:RadioButtonList ID="rdbtn_feedback"  CausesValidation="true" ValidationGroup="valgroup" RepeatDirection="Horizontal" runat="server">
                    <asp:ListItem>Positive</asp:ListItem>
                    <asp:ListItem>Negative</asp:ListItem>
                    <asp:ListItem>Neutral</asp:ListItem>
                </asp:RadioButtonList>
                        <br />
                 <asp:RequiredFieldValidator ID="rqvalidator" ErrorMessage="*" ValidationGroup="valgroup" ControlToValidate="rdbtn_feedback" runat="server" ForeColor="Red" Display="Dynamic" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btn_submitfeedback" CausesValidation="true" Text="Submit" ValidationGroup="valgroup" BackColor="#999999" Font-Size="Medium"  runat ="server" OnClick="btn_feedbacksubmit_Click" Height="26px" Width="70px" />
                    </td>

                </tr>
                </table>
        </div>
    </form>
</body>
</html>
