namespace SmartHealthFitnessAssistant.Models
{
    /// <summary>
    /// One segment of the calorie progress graphic on the dashboard.
    /// A named public class is used (instead of an anonymous type) because
    /// ASP.NET data binding cannot read properties of internal types.
    /// </summary>
    public class CalorieBar
    {
        public CalorieBar() { }

        public CalorieBar(bool filled)
        {
            Filled = filled;
        }

        public bool Filled { get; set; }

        /// <summary>CSS class used directly by the Repeater item template.</summary>
        public string CssClass
        {
            get { return Filled ? "bar on" : "bar"; }
        }
    }
}
