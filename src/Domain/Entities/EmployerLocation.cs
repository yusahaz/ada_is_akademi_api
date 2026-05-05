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

        protected EmployerLocation() { }

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

        protected internal void DeleteEmployerLocation()
            => base.Delete();

        protected internal void SetAddress(Address address)
        {
            Address = address;
        }

        protected internal void SetContact(Contact contact)
        {
            Contact = contact;
        }

        protected internal void SetCoordinate(GeoCoordinate coordinate)
        {
            Coordinate = coordinate;
        }

        protected internal void SetGeofenceRadiusMetres(int geofenceRadiusMetres)
            => GeofenceRadiusMetres = geofenceRadiusMetres.ThrowIfOutOfRange(
                min: 1,
                max: 100000,
                DomainErrorCodes.GeofenceRadiusOutOfRange);

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