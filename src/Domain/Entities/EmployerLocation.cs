namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Represents a physical or operational location of an employer.
    /// </summary>
    public class EmployerLocation :
        CodedNamedEntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected EmployerLocation() { }

        /// <summary>
        /// Creates a location scoped to an employer.
        /// </summary>
        /// <param name="employerId">Owning employer key.</param>
        /// <param name="name">Location name.</param>
        /// <param name="description">Optional description.</param>
        protected internal EmployerLocation(
            int employerId,
            string name,
            string? description = null) :
            base(name, description)
        {
            EmployerId = employerId;
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Soft-deletes this location through the base lifecycle API.
        /// </summary>
        protected internal void DeleteEmployerLocation()
            => base.Delete();

        /// <summary>
        /// Replaces the embedded address value.
        /// </summary>
        protected internal void SetAddress(Address address)
        {
            Address = address;
        }

        /// <summary>
        /// Replaces optional contact details for this location.
        /// </summary>
        protected internal void SetContact(Contact contact)
        {
            Contact = contact;
        }

        /// <summary>
        /// Replaces the geographic coordinate used for distance checks.
        /// </summary>
        protected internal void SetCoordinate(GeoCoordinate coordinate)
        {
            Coordinate = coordinate;
        }

        /// <summary>
        /// Updates geofence radius with platform validation bounds.
        /// </summary>
        protected internal void SetGeofenceRadiusMetres(int geofenceRadiusMetres)
            => GeofenceRadiusMetres = geofenceRadiusMetres.ThrowIfOutOfRange(
                min: 1,
                max: 100000,
                DomainErrorCodes.GeofenceRadiusOutOfRange);

        /// <summary>
        /// Renames the location and optionally updates its description.
        /// </summary>
        protected internal void UpdateEmployerLocation(string name, string? description = null)
        {
            UpdateName(name, description);
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Address details for this location.
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// Optional contact details specific to this location.
        /// </summary>
        public Contact? Contact { get; private set; }

        /// <summary>
        /// Geographic coordinate used for map and distance operations.
        /// </summary>
        public GeoCoordinate Coordinate { get; private set; }

        /// <summary>
        /// Owning employer identifier.
        /// </summary>
        public int EmployerId { get; private set; }

        /// <summary>
        /// Radius in metres used for geofence checks.
        /// </summary>
        public int GeofenceRadiusMetres { get; private set; }

        /// <summary>
        /// Owning employer aggregate.
        /// </summary>
        public virtual Employer Employer { get; private set; }

        #endregion Properties
    }
}
