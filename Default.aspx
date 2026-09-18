<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="Default.aspx.cs" Inherits="SmartHealthFitnessAssistant.Default" UnobtrusiveValidationMode="None" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Smart Health &amp; Fitness Assistant</title>
    <link href="Content/site.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="page">
            <div class="device">

                <!-- ============ TOP NAVIGATION ============ -->
                <div class="topbar">
                    <div class="topbar-left">
                        <div class="logo">BF</div>
                        <a href="WellnessDashboard.aspx" class="nav-pill active" style="text-decoration:none;">
                            <span class="nav-ico">&#9632;</span> Dashboard 
                        </a>
                        <div class="nav-ico-btn" title="Reports">&#128202;</div>
                        <div class="nav-ico-btn" title="Plans">&#128196;</div>
                        <div class="nav-ico-btn" title="Messages">&#9993;</div>
                        <div class="nav-ico-btn" title="Settings">&#9881;</div>
                    </div>
                    <div class="topbar-right">
                        <div class="nav-ico-btn">&#128269;</div>
                        <div class="nav-ico-btn">&#128276;<span class="dot"></span></div>
                        <div class="avatar">&#128100;</div>
                    </div>
                </div>

                <!-- ============ PAGE HEADING ============ -->
                <div class="heading-row">
                    <div>
                        <div class="welcome">Welcome back
                            <asp:Literal ID="litWelcomeName" runat="server" Text="Guest" /> &#128075;</div>
                        <h1 class="page-title">
                            Fitness Overview
                            <span class="badge-premium">&#9819; Premium member</span>
                        </h1>
                    </div>
                    <div class="heading-right">
                        <div class="stack-avatars">
                            <span class="mini">&#128104;</span>
                            <span class="mini">&#128105;</span>
                            <span class="mini">&#129333;</span>
                            <span class="mini more">5+</span>
                        </div>
                        <asp:LinkButton ID="btnReset" runat="server" CssClass="btn-dark"
                            CausesValidation="false" OnClick="btnReset_Click">
                            <span class="plus">+</span> Create New
                        </asp:LinkButton>
                    </div>
                </div>

                <!-- ============ VALIDATION SUMMARY ============ -->
                <asp:ValidationSummary ID="vsMain" runat="server" CssClass="alert alert-error"
                    HeaderText="Please correct the following:" DisplayMode="BulletList"
                    ValidationGroup="wellness" />

                <asp:Panel ID="pnlServerErrors" runat="server" CssClass="alert alert-error" Visible="false">
                    <asp:Literal ID="litServerErrors" runat="server" />
                </asp:Panel>

                <!-- ============ ROW 1 : INPUT FORM  +  GOAL CARDS ============ -->
                <div class="grid grid-2">

                    <!-- ---------- Input card ---------- -->
                    <div class="card">
                        <div class="card-head">
                            <h2>Your Wellness Profile</h2>
                            <span class="chip">Step 1</span>
                        </div>

                        <div class="form-grid">

                            <div class="field">
                                <label for="<%= txtName.ClientID %>">Full name</label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="input"
                                    placeholder="Enter your name" MaxLength="50" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName"
                                    ErrorMessage="Name is required." Text="*" CssClass="err"
                                    ValidationGroup="wellness" Display="Dynamic" />
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtName"
                                    ValidationExpression="^[A-Za-z .']{2,50}$"
                                    ErrorMessage="Name may contain only letters and spaces (2-50 characters)."
                                    Text="*" CssClass="err" ValidationGroup="wellness" Display="Dynamic" />
                            </div>

                            <div class="field">
                                <label for="<%= txtAge.ClientID %>">Age (years)</label>
                                <asp:TextBox ID="txtAge" runat="server" CssClass="input"
                                    placeholder="10 - 100" MaxLength="3" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAge"
                                    ErrorMessage="Age is required." Text="*" CssClass="err"
                                    ValidationGroup="wellness" Display="Dynamic" />
                                <asp:RangeValidator runat="server" ControlToValidate="txtAge"
                                    MinimumValue="15" MaximumValue="100" Type="Integer"
                                    ErrorMessage="Age must be a whole number between 15 and 100."
                                    Text="*" CssClass="err" ValidationGroup="wellness" Display="Dynamic" />
                            </div>

                            <div class="field">
                                <label for="<%= ddlGender.ClientID %>">Gender</label>
                                <asp:DropDownList ID="ddlGender" runat="server" CssClass="input">
                                    <asp:ListItem Value="Select your gender" Text="Select your gender" />
                                    <asp:ListItem Value="Male" Text="Male" />
                                    <asp:ListItem Value="Female" Text="Female" />
                                    <asp:ListItem Value="Other" Text="Prefer not to say" />
                                </asp:DropDownList>
                            </div>

                            <div class="field">
                                <label for="<%= txtHeight.ClientID %>">Height (cm)</label>
                                <asp:TextBox ID="txtHeight" runat="server" CssClass="input"
                                    placeholder="100 - 250" MaxLength="6" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtHeight"
                                    ErrorMessage="Height is required." Text="*" CssClass="err"
                                    ValidationGroup="wellness" Display="Dynamic" />
                                <asp:RangeValidator runat="server" ControlToValidate="txtHeight"
                                    MinimumValue="100" MaximumValue="250" Type="Double"
                                    ErrorMessage="Height must be between 100 and 250 cm."
                                    Text="*" CssClass="err" ValidationGroup="wellness" Display="Dynamic" />
                            </div>

                            <div class="field">
                                <label for="<%= txtWeight.ClientID %>">Weight (kg)</label>
                                <asp:TextBox ID="txtWeight" runat="server" CssClass="input"
                                    placeholder="25 - 250" MaxLength="6" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtWeight"
                                    ErrorMessage="Weight is required." Text="*" CssClass="err"
                                    ValidationGroup="wellness" Display="Dynamic" />
                                <asp:RangeValidator runat="server" ControlToValidate="txtWeight"
                                    MinimumValue="25" MaximumValue="250" Type="Double"
                                    ErrorMessage="Weight must be between 25 and 250 kg."
                                    Text="*" CssClass="err" ValidationGroup="wellness" Display="Dynamic" />
                            </div>

                            <div class="field">
                                <label for="<%= ddlActivity.ClientID %>">Activity level</label>
                                <asp:DropDownList ID="ddlActivity" runat="server" CssClass="input">
                                    <asp:ListItem Value="SelectLevel" Text="Select your Level" />
                                    <asp:ListItem Value="Sedentary" Text="Sedentary - mostly sitting" />
                                    <asp:ListItem Value="Light" Text="Light - 1 to 3 days a week" />
                                    <asp:ListItem Value="Moderate" Text="Moderate - 3 to 5 days a week" Selected="True" />
                                    <asp:ListItem Value="Active" Text="Active - 6 to 7 days a week" />
                                    <asp:ListItem Value="VeryActive" Text="Very active - physical job" />
                                </asp:DropDownList>
                            </div>

                            <div class="field">
                                <label for="<%= ddlGoal.ClientID %>">Primary goal</label>
                                <asp:DropDownList ID="ddlGoal" runat="server" CssClass="input">
                                    <asp:ListItem Value="BuildStrength" Text="Build Strength" />
                                    <asp:ListItem Value="LoseBodyFat" Text="Lose Body Fat" />
                                    <asp:ListItem Value="IncreaseFlexibility" Text="Increase Flexibility" />
                                    <asp:ListItem Value="GeneralWellness" Text="General Wellness" Selected="True" />
                                </asp:DropDownList>
                            </div>

                            <div class="field">
                                <label for="<%= ddlDiet.ClientID %>">Diet preference</label>
                                <asp:DropDownList ID="ddlDiet" runat="server" CssClass="input">
                                    <asp:ListItem Value="Vegetarian" Text="Vegetarian" Selected="True" />
                                    <asp:ListItem Value="Vegan" Text="Vegan" />
                                    <asp:ListItem Value="Eggetarian" Text="Eggetarian" />
                                    <asp:ListItem Value="NonVegetarian" Text="Non vegetarian" />
                                </asp:DropDownList>
                            </div>

                            <div class="field">
                                <label for="<%= txtSleep.ClientID %>">Average sleep (hours)</label>
                                <asp:TextBox ID="txtSleep" runat="server" CssClass="input"
                                    placeholder="3 - 14" MaxLength="4" Text="7" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSleep"
                                    ErrorMessage="Sleep hours are required." Text="*" CssClass="err"
                                    ValidationGroup="wellness" Display="Dynamic" />
                                <asp:RangeValidator runat="server" ControlToValidate="txtSleep"
                                    MinimumValue="3" MaximumValue="14" Type="Double"
                                    ErrorMessage="Sleep must be between 3 and 14 hours."
                                    Text="*" CssClass="err" ValidationGroup="wellness" Display="Dynamic" />
                            </div>

                            <div class="field">
                                <label for="<%= txtWorkoutDays.ClientID %>">Training days per week</label>
                                <asp:TextBox ID="txtWorkoutDays" runat="server" CssClass="input"
                                    placeholder="0 - 7" MaxLength="1" Text="3" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtWorkoutDays"
                                    ErrorMessage="Training days are required." Text="*" CssClass="err"
                                    ValidationGroup="wellness" Display="Dynamic" />
                                <asp:RangeValidator runat="server" ControlToValidate="txtWorkoutDays"
                                    MinimumValue="0" MaximumValue="7" Type="Integer"
                                    ErrorMessage="Training days must be a whole number between 0 and 7."
                                    Text="*" CssClass="err" ValidationGroup="wellness" Display="Dynamic" />
                            </div>

                            <div class="field field-wide">
                                <label for="<%= txtNotes.ClientID %>">Anything else we should know? (optional)</label>
                                <asp:TextBox ID="txtNotes" runat="server" CssClass="input textarea"
                                    TextMode="MultiLine" Rows="2" MaxLength="300"
                                    placeholder="e.g. desk job, no gym access, prefer home workouts" />
                                <asp:CustomValidator ID="cvNotes" runat="server" ControlToValidate="txtNotes"
                                    OnServerValidate="cvNotes_ServerValidate" ValidateEmptyText="false"
                                    ErrorMessage="Notes cannot be longer than 300 characters."
                                    Text="*" CssClass="err" ValidationGroup="wellness" Display="Dynamic" />
                            </div>

                        </div>

                        <asp:Button ID="btnGenerate" runat="server" CssClass="btn-primary"
                            Text="Generate my wellness suggestions"
                            ValidationGroup="wellness" OnClick="btnGenerate_Click" />
                        <p class="muted small">
                            Your details are used only to calculate the figures below and to build the AI prompt.
                        </p>
                    </div>

                    <!-- ---------- Goal cards ---------- -->
                    <div class="card">
                        <div class="card-head">
                            <h2>Fitness goal</h2>
                            <span class="chip">
                                <asp:Literal ID="litGoalChip" runat="server" Text="Not set yet" />
                            </span>
                        </div>

                        <div class="goal-grid">
                            <div id="goalStrength" runat="server" class="goal goal-purple">
                                <h3>Build Strength</h3>
                                <p class="goal-sub"><asp:Literal ID="litStrengthSets" runat="server" Text="Progressive overload" /></p>
                                <span class="goal-tag">&#128170; Bravo</span>
                                <div class="goal-wave"></div>
                            </div>

                            <div id="goalFat" runat="server" class="goal goal-yellow">
                                <h3>Lose Body Fat</h3>
                                <p class="goal-sub"><asp:Literal ID="litFatSets" runat="server" Text="Steady calorie deficit" /></p>
                                <span class="goal-tag">&#128077; Well</span>
                                <div class="goal-wave"></div>
                            </div>

                            <div id="goalFlex" runat="server" class="goal goal-dark">
                                <h3>Increase Flexibility</h3>
                                <p class="goal-sub"><asp:Literal ID="litFlexSets" runat="server" Text="Daily mobility work" /></p>
                                <span class="goal-tag">&#128293; Great</span>
                                <div class="goal-wave"></div>
                            </div>
                        </div>

                        <div class="mini-stats">
                            <div class="mini-stat">
                                <span class="mini-label">Weekly active minutes</span>
                                <span class="mini-value"><asp:Literal ID="litActiveMinutes" runat="server" Text="--" /></span>
                            </div>
                            <div class="mini-stat">
                                <span class="mini-label">Daily step target</span>
                                <span class="mini-value"><asp:Literal ID="litSteps" runat="server" Text="--" /></span>
                            </div>
                            <div class="mini-stat">
                                <span class="mini-label">Sleep target</span>
                                <span class="mini-value"><asp:Literal ID="litSleepTarget" runat="server" Text="--" /></span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- ============ ROW 2 : THREE STAT CARDS ============ -->
                <div class="grid grid-3">

                    <!-- BMI -->
                    <div class="card">
                        <div class="card-head">
                            <h2>Body Mass Index</h2>
                            <span class="kebab">&#8942;</span>
                        </div>
                        <div class="curve curve-bmi"></div>
                        <div class="stat-row">
                            <div class="big-number">
                                <asp:Literal ID="litBmi" runat="server" Text="--" /><span class="unit">bmi</span>
                            </div>
                            <asp:Label ID="lblBmiCategory" runat="server" CssClass="badge" Text="Awaiting input" />
                        </div>
                        <p class="muted small">
                            Healthy weight for your height:
                            <strong><asp:Literal ID="litHealthyRange" runat="server" Text="--" /></strong>
                        </p>
                    </div>

                    <!-- Calories -->
                    <div class="card">
                        <div class="card-head">
                            <h2>Calories Goal</h2>
                            <span class="kebab">&#8942;</span>
                        </div>
                        <div class="big-number">
                            <asp:Literal ID="litTargetCalories" runat="server" Text="--" /><span class="unit">/kcal per day</span>
                        </div>
                        <div class="bar-head">
                            <span>0</span>
                            <span><asp:Literal ID="litTdee" runat="server" Text="--" /> maintenance</span>
                        </div>
                        <div class="bars">
                            <asp:Repeater ID="rptBars" runat="server">
                                <ItemTemplate>
                                    <span class='<%# Eval("CssClass") %>'></span>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="card-foot">
                            <span><strong><asp:Literal ID="litProtein" runat="server" Text="--" /></strong> protein target</span>
                            <span class="pill-light"><asp:Literal ID="litWater" runat="server" Text="--" /> water</span>
                        </div>
                    </div>

                    <!-- Heart rate -->
                    <div class="card">
                        <div class="card-head">
                            <h2>Training Heart Rate</h2>
                            <span class="kebab">&#8942;</span>
                        </div>
                        <div class="curve curve-hr"></div>
                        <div class="stat-row">
                            <div class="big-number">
                                <asp:Literal ID="litHrLow" runat="server" Text="--" /><span class="unit">bpm</span>
                            </div>
                            <span class="muted small">
                                <asp:Literal ID="litHrRange" runat="server" Text="zone appears after you submit" />
                            </span>
                        </div>
                        <p class="muted small">
                            Estimated maximum heart rate:
                            <strong><asp:Literal ID="litMaxHr" runat="server" Text="--" /></strong>
                        </p>
                    </div>
                </div>

                <!-- ============ ROW 3 : AI SUGGESTIONS ============ -->
                <div class="grid grid-2b">

                    <div class="card card-ai">
                        <div class="card-head">
                            <h2>&#10024; AI Wellness Suggestions</h2>
                            <asp:Label ID="lblAiStatus" runat="server" CssClass="chip" Text="Ollama - not called yet" />
                        </div>

                        <asp:Panel ID="pnlAiError" runat="server" CssClass="alert alert-warn" Visible="false">
                            <asp:Literal ID="litAiError" runat="server" />
                        </asp:Panel>

                        <div class="ai-body">
                            <asp:Literal ID="litAiResponse" runat="server"
                                Text="Fill in your profile and press Generate. The application will calculate your numbers in C# and then ask the local Ollama model for a general wellness plan." />
                        </div>

                        <asp:Panel ID="pnlPrompt" runat="server" CssClass="prompt-box" Visible="false">
                            <details>
                                <summary>Show the exact prompt sent to Ollama</summary>
                                <pre><asp:Literal ID="litPrompt" runat="server" /></pre>
                            </details>
                        </asp:Panel>
                    </div>

                    <div class="card">
                        <div class="card-head">
                            <h2>Quick Tips</h2>
                            <span class="chip">Rule based</span>
                        </div>
                        <asp:Repeater ID="rptTips" runat="server">
                            <HeaderTemplate><ul class="tips"></HeaderTemplate>
                            <ItemTemplate><li><%# Server.HtmlEncode(Container.DataItem.ToString()) %></li></ItemTemplate>
                            <FooterTemplate></ul></FooterTemplate>
                        </asp:Repeater>
                        <asp:Panel ID="pnlNoTips" runat="server" CssClass="muted small">
                            Tips generated by the C# rules engine will appear here.
                        </asp:Panel>
                    </div>
                </div>

                <p class="disclaimer">
                    This project provides general wellness information for educational purposes only.
                    It is not medical advice. Please speak to a qualified doctor or dietitian before
                    making significant changes to your diet or exercise routine.
                </p>

            </div>
        </div>
    </form>
</body>
</html>
