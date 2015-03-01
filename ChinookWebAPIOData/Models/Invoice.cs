namespace ChinookWebAPIOData.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Invoice")]
    public partial class Invoice
    {
        public Invoice()
        {
            InvoiceLines = new HashSet<InvoiceLine>();
        }

        private DateTimeWrapper dtw;

        public int InvoiceId { get; set; }

        public int CustomerId { get; set; }

        public DateTimeOffset InvoiceDate
        {
            get { return dtw; }
            set { dtw = value; }
        }

        //[NotMapped]
        //public DateTimeOffset InvoiceDateOffset
        //{
        //    get { return dtw; }
        //    set { dtw = value; }
        //}

        [StringLength(70)]
        public string BillingAddress { get; set; }

        [StringLength(40)]
        public string BillingCity { get; set; }

        [StringLength(40)]
        public string BillingState { get; set; }

        [StringLength(40)]
        public string BillingCountry { get; set; }

        [StringLength(10)]
        public string BillingPostalCode { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Total { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<InvoiceLine> InvoiceLines { get; set; }
    }

    public class DateTimeWrapper
    {
        public static implicit operator DateTimeOffset(DateTimeWrapper p)
        {
            return DateTime.SpecifyKind(p._dt, DateTimeKind.Utc);
        }

        public static implicit operator DateTimeWrapper(DateTimeOffset dto)
        {
            return new DateTimeWrapper(dto.DateTime);
        }

        public static implicit operator DateTime(DateTimeWrapper dtr)
        {
            return dtr._dt;
        }

        public static implicit operator DateTimeWrapper(DateTime dt)
        {
            return new DateTimeWrapper(dt);
        }

        protected DateTimeWrapper(DateTime dt)
        {
            _dt = dt;
        }

        private readonly DateTime _dt;
    }
}
