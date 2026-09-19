<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="SmartHealthFitnessAssistant.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title> Login Page </title>
    <link href="login.css" rel="stylesheet" type="text/css"/>
</head>
<body>
    <div class = "login-container">
        <div class = "animal">
            <div class="ear-left"></div>
            <div class="ear-right"></div>
            <div class="face">
                <div class="eye left-eye"><div class="pupil"></div><div class="cover"></div></div>
                <div class="eye right-eye"><div class="pupil"></div><div class="cover"></div></div>
                <div class="nose"></div>
                <div class="mouth"></div>
            </div>
        </div>
    </div>
    <div class = "login-form">
        <form class = "login" action="">
            <h2>Login</h2>
            <input type="text" placeholder = "Username" class = "username" required />
            <div class = "password-field">
                <input type="password" id = "password" placeholder = "Password" required />
                <span id = "tooglePassword">Show</span>
            </div>
            <button type="submit" class = "button">Login</button>
        </form>
    </div>
    <script src = "login.js"></script>
</body>
</html>
