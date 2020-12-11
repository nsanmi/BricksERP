//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hotel.Dal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class admin_hrm_employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public admin_hrm_employee()
        {
            this.HotelEmployees = new HashSet<HotelEmployee>();
            this.Laundry_admin = new HashSet<Laundry_admin>();
            this.product_admin = new HashSet<product_admin>();
            this.collection_register = new HashSet<collection_register>();
            this.Hotel_booking_admin = new HashSet<Hotel_booking_admin>();
            this.Hotel_guest = new HashSet<Hotel_guest>();
        }
    
        public int emp_number { get; set; }
        public string employee_id { get; set; }
        public string emp_lastname { get; set; }
        public string emp_firstname { get; set; }
        public string emp_middle_name { get; set; }
        public string emp_nick_name { get; set; }
        public string ethnic_race_code { get; set; }
        public Nullable<System.DateTime> emp_birthday { get; set; }
        public Nullable<int> nation_code { get; set; }
        public Nullable<byte> emp_gender { get; set; }
        public string emp_marital_status { get; set; }
        public string emp_ssn_num { get; set; }
        public string emp_sin_num { get; set; }
        public string emp_other_id { get; set; }
        public string emp_dri_lice_num { get; set; }
        public Nullable<System.DateTime> emp_dri_lice_exp_date { get; set; }
        public string emp_military_service { get; set; }
        public Nullable<int> emp_status { get; set; }
        public Nullable<int> job_title_code { get; set; }
        public Nullable<int> eeo_cat_code { get; set; }
        public Nullable<int> work_station { get; set; }
        public string emp_street1 { get; set; }
        public string emp_street2 { get; set; }
        public string city_code { get; set; }
        public string coun_code { get; set; }
        public string provin_code { get; set; }
        public string emp_zipcode { get; set; }
        public string emp_hm_telephone { get; set; }
        public string emp_mobile { get; set; }
        public string emp_work_telephone { get; set; }
        public string emp_work_email { get; set; }
        public string sal_grd_code { get; set; }
        public Nullable<System.DateTime> joined_date { get; set; }
        public string emp_oth_email { get; set; }
        public Nullable<int> termination_id { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
        public Nullable<System.DateTime> added_date { get; set; }
        public string emp_about { get; set; }
        public string user_id { get; set; }
        public Nullable<int> active { get; set; }
        public int deleted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelEmployee> HotelEmployees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Laundry_admin> Laundry_admin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<product_admin> product_admin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<collection_register> collection_register { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hotel_booking_admin> Hotel_booking_admin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hotel_guest> Hotel_guest { get; set; }
    }
}
