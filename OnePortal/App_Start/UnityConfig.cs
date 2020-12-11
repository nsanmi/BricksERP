using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using HRM.DAL.IService;
using HRM.DAL.Service;
using OnePortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnePortal.Controllers;
using WorkFlow.DAL.Service;
using WorkFlow.DAL.IService;
using Hotel.Dal.IService;
using Hotel.Dal.Service;

namespace OnePortal.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();

            container.RegisterType<IEmployeeService, EmployeeService>();
            container.RegisterType<ITimesheetService, TimesheetService>();
            container.RegisterType<ILeaveService, LeaveService>();
            container.RegisterType<IActivityService, ActivityService>();
            container.RegisterType<IProjectService, ProjectService>();
            container.RegisterType<ITaskService, TaskService>();
            container.RegisterType<ICommentService, CommentService>();
            container.RegisterType<IAnnouncementService, AnnouncementService>();
            container.RegisterType<IProcessService, ProcessService>();
            container.RegisterType<IWorkflowService, WorkflowService>();
            container.RegisterType<ISettingsService, SettingsService>();
            container.RegisterType<IVendorService, VendorService>();
            container.RegisterType<ITransactionTokenService, TransactionTokenService>();
            container.RegisterType<IComplainService, ComplainService>();

            container.RegisterType<IPaymentService, PaymentService>();
            container.RegisterType<ILookupService, LookupService>();
            container.RegisterType<ISuggestionService, SuggestionService>();
            container.RegisterType<IInventoryService, InventoryService>();
            container.RegisterType<ILaundryService, LaundryService>();
            container.RegisterType<IBookingsService, BookingsService>();
            container.RegisterType<IGuestService, GuestService>();
            container.RegisterType<IReservationService, ReservationService>();
            container.RegisterType<IRoomService, RoomService>();

            container.RegisterType<IGuestBookingService, GuestBookingService>();
            container.RegisterType<IBookingService, BookingService>();

            container.RegisterType<IAmenityService, AmenityService>();

            container.RegisterType<IAddonService, AddonService>();
            container.RegisterType<IHLaundryService, HLaundryService>();
            container.RegisterType<IHGuestService, HGuestService>();



            container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<ManageController>(new InjectionConstructor());

            

        }
    }
}
