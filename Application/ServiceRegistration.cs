using Application.Services;
using Application.Validators.ApprovalValidations;
using Application.Validators.LeaveRequestValidations;
using Application.Validators.UserValidations;
using Core.Interfaces.IServices;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Servisler
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            services.AddScoped<IEmployeeDashboardService, EmployeeDashboardService>();
            services.AddScoped<IApprovalService, ApprovalService>();
            services.AddScoped<IAdminService, AdminService>();

            // FluentValidation            
            services.AddValidatorsFromAssemblyContaining<LoginValidator>();
            services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();
            services.AddValidatorsFromAssemblyContaining<PasswordResetValidation>();
            services.AddValidatorsFromAssemblyContaining<LeaveRequestCreateValidator>();
            services.AddValidatorsFromAssemblyContaining<LeaveRequestUpdateValidator>();
            services.AddValidatorsFromAssemblyContaining<ApprovalCreateValidator>();
        }
    }
}
