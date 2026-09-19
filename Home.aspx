<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="SmartHealthFitnessAssistant.Home" UnobtrusiveValidationMode="None" %>

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
        <div class="home-page">
            <div class="home-container">

                <!-- ============ TOP NAV (no Get Started button) ============ -->
                <div class="home-topbar">
                    <div class="home-logo">Smart Health &amp; Fitness</div>
                    <div class="home-nav-links">
                        <a href="#features">Features</a>
                        <a href="#features">How it works</a>
                        <a href="#features">About</a>
                    </div>
                </div>

                <!-- ============ HERO (Option B - split layout) ============ -->
                <div class="hero-split">
                    <div class="hero-copy">
                        <span class="hero-badge">AI-powered</span>
                        <h1>A wellness plan built around your numbers</h1>
                        <p>
                            Answer a few questions. See your BMI, calorie target and training zone
                            instantly, then get an AI-written plan to match.
                        </p>
                        <div class="hero-buttons">
                            <asp:HyperLink ID="lnkCreateProfileHero" runat="server"
                                NavigateUrl="Default.aspx" CssClass="btn-primary-cta" href="login.aspx">
                                Create my profile
                            </asp:HyperLink>
                            <a href="#features" class="btn-outline">See how it works</a>
                        </div>
                    </div>

                    <div class="hero-preview-card">
                        <div class="preview-label">Fitness overview</div>
                        <div class="preview-stats">
                            <div class="preview-stat">
                                <span class="preview-stat-label">BMI</span>
                                <span class="preview-stat-value">23.6</span>
                            </div>
                            <div class="preview-stat">
                                <span class="preview-stat-label">Calories</span>
                                <span class="preview-stat-value">1,693</span>
                            </div>
                        </div>
                        <div class="preview-zone">
                            <span class="preview-zone-label">Heart rate zone</span>
                            <span class="preview-zone-value">106 - 142 bpm</span>
                        </div>
                    </div>
                </div>

                <!-- ============ FEATURES (Option A) ============ -->
                <div class="features-section" id="features">
                    <div class="features-head">
                        <h2>Your wellness, calculated and explained</h2>
                        <p>
                            Enter your goals once. Get instant BMI, calorie and heart-rate targets,
                            plus an AI-generated plan.
                        </p>
                        <asp:HyperLink ID="lnkStartProfile" runat="server"
                            NavigateUrl="Default.aspx" CssClass="btn-primary-cta">
                            Start my wellness profile
                        </asp:HyperLink>
                    </div>

                    <div class="feature-grid">
                        <div class="feature-card feature-purple">
                            <div class="feature-icon">&#128202;</div>
                            <h3>Instant calculations</h3>
                            <p>BMI, calories and heart-rate zones, worked out in seconds.</p>
                        </div>
                        <div class="feature-card feature-teal">
                            <div class="feature-icon">&#10024;</div>
                            <h3>AI wellness plan</h3>
                            <p>A personalised plan generated locally, no paid API needed.</p>
                        </div>
                        <div class="feature-card feature-coral">
                            <div class="feature-icon">&#128190;</div>
                            <h3>Save your progress</h3>
                            <p>Every profile is stored so you can track it over time.</p>
                        </div>
                    </div>
                </div>

                <p class="disclaimer home-disclaimer">
                    This application provides general wellness information for educational purposes only and is not medical advice.
                </p>

            </div>
        </div>
    </form>
</body>
</html>
