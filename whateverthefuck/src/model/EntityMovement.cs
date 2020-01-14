namespace whateverthefuck.src.model
{
    using whateverthefuck.src.util;

    public class EntityMovement : IEncodable
    {
        private const float NoDirection = float.NaN;

        public EntityMovement()
        {
            this.Direction = float.NaN;
            this.FollowId = null;
        }

        /// <summary>
        /// Gets or sets the direction of movement in radians, or NaN if not moving directionally.
        /// </summary>
        public float Direction { get; set; }

        /// <summary>
        /// Gets or sets the id of the followed entity, or null if not following anything.
        /// </summary>
        public EntityIdentifier FollowId { get; set; }

        public bool IsDirectional => !float.IsNaN(this.Direction);

        public bool IsFollowing => this.FollowId != null;

        /// <summary>
        /// Gets a value indicating whether the GameEntity is moving at all.
        /// </summary>
        public bool IsMoving => this.IsDirectional || this.IsFollowing;

        public void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(this.Direction);
            encoder.Encode(this.FollowId != null ? this.FollowId.Id : 0);
        }

        public void Decode(WhateverDecoder decoder)
        {
            this.Direction = decoder.DecodeFloat();
            var val = decoder.DecodeInt();
            this.FollowId = val == 0 ? null : new EntityIdentifier(val);
        }
    }
}
