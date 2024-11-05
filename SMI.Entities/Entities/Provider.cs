namespace SMI.Entities.Entities
{  
    public class BaseProviderDetail : BaseEntity
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public virtual string Title { get; set; }
        /// <summary>
        /// Gets or sets the provider code.
        /// </summary>
        /// <value>
        /// The provider code.
        /// </value>
        public virtual string ProviderCode { get; set; }
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public virtual int Priority { get; set; }
        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        /// <value>
        /// The name of the assembly.
        /// </value>
        public virtual string AssemblyName { get; set; }
        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public virtual string TypeName { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public virtual string Status { get; set; }
        /// <summary>
        /// Gets or sets the type of the provider.
        /// </summary>
        /// <value>
        /// The type of the provider.
        /// </value>
        public virtual string ProviderType { get; set; }
        /// <summary>
        /// Gets or sets the name of the settings file.
        /// </summary>
        /// <value>
        /// The name of the settings file.
        /// </value>
        public virtual string SettingsFileName { get; set; }
    }

    public class FacebookProviderDetail
    {
        public virtual ProviderDetail Provider { get; set; }

        public virtual IVisaDirectProvider VisaDirectProvider { get; set; }

        public virtual bool HasError
        {
            get
            {
                if (this.Provider == null)
                {
                    this.ErrorMessage = "Provider not loaded.";
                    return true;
                }

                if (this.VisaDirectProvider == null)
                {
                    this.ErrorMessage = string.Format("Provider not loaded. {0}", this.Provider.AssemblyName);
                    return true;
                }

                if (this.VisaDirectProvider.HasError)
                {
                    this.ErrorMessage = this.VisaDirectProvider.ErrorMessage;
                    return true;
                }

                return false;
            }
        }

        public virtual string ErrorMessage { get; set; }
    }
}
