namespace whateverthefuck.src.model
{
    /// <summary>
    /// A container for the arguments used when creating a GameEntity.
    /// </summary>
    public class CreationArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreationArgs"/> class.
        /// </summary>
        /// <param name="value">The unencoded value of the CreationArgs.</param>
        public CreationArgs(ulong value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the unencoded value of the CreationArgs.
        /// </summary>
        public ulong Value { get; protected set; }

        /// <summary>
        /// Gets or sets the most significant int.
        /// </summary>
        protected int FirstInt
        {
            get
            {
                return (int)((this.Value & 0xFFFFFFFF00000000) >> 32);
            }

            set
            {
                this.Value &= 0x00000000FFFFFFFF;
                this.Value |= (ulong)value << 32;
            }
        }

        /// <summary>
        /// Gets or sets the least significant int.
        /// </summary>
        protected int SecondInt
        {
            get
            {
                return (int)(this.Value & 0xFFFFFFFF);
            }

            set
            {
                this.Value &= 0xFFFFFFFF00000000;
                this.Value |= (ulong)value << 0;
            }
        }
    }
}
