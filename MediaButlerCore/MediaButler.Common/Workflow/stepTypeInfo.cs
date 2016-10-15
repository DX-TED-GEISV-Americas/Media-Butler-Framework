namespace MediaButler.Common.Workflow
{
    /// <summary>
    /// Process Step information
    /// </summary>
    public class stepTypeInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public stepTypeInfo()
        { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AssemblyName">Step assembly name</param>
        /// <param name="TypeName">type name</param>
        /// <param name="configKey">configuration step key</param>
        public stepTypeInfo(string AssemblyName, string TypeName, string configKey)
        {
            this.AssemblyName = AssemblyName;
            this.TypeName = TypeName;
            this.ConfigKey = configKey;
        }
        /// <summary>
        /// Step assembly name
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        /// step type name
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// Step cpnfiguration key
        /// </summary>
        public string ConfigKey { get; set; }


    }
}
