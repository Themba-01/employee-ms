using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using ClientLibrary.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using ClientLibrary.Services.Contracts;
using ClientLibrary.Services.Implementatons;
using Blazored.LocalStorage;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor;
using Syncfusion.Licensing; // Add this namespace for licensing
using Client.ApplicationStates;
using ClientLibrary.Services.Implementations;
using BaseLibrary.Entities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register CustomHttpHandler
builder.Services.AddTransient<CustomHttpHandler>();  // Register CustomHttpHandler

// Register HttpClient with CustomHttpHandler
builder.Services.AddHttpClient("SystemApiClient", client =>
{
    //client.BaseAddress = new Uri("http://localhost:5162/");
    client.BaseAddress = new Uri("http://localhost:5001/");
}).AddHttpMessageHandler<CustomHttpHandler>(); // Add CustomHttpHandler to HttpClient

// Register other services
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<GetHttpClient>();
builder.Services.AddScoped<LocalStorageService>();

// Register CustomAuthenticationStateProvider with explicit instantiation
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    new CustomAuthenticationStateProvider(
        provider.GetRequiredService<LocalStorageService>(),
        provider.GetRequiredService<ILogger<CustomAuthenticationStateProvider>>(),
        provider.GetRequiredService<IHttpClientFactory>().CreateClient("SystemApiClient")
    )
);

builder.Services.AddScoped<IUserAccountService, UserAccountService>();

builder.Services.AddScoped<IGenericServiceInterface<GeneralDepartment>,GenericServiceImplementation<GeneralDepartment>>();
builder.Services.AddScoped<IGenericServiceInterface<Department>,GenericServiceImplementation<Department>>();
builder.Services.AddScoped<IGenericServiceInterface<Branch>,GenericServiceImplementation<Branch>>();

builder.Services.AddScoped<IGenericServiceInterface<Country>,GenericServiceImplementation<Country>>();
builder.Services.AddScoped<IGenericServiceInterface<City>,GenericServiceImplementation<City>>();
builder.Services.AddScoped<IGenericServiceInterface<Town>,GenericServiceImplementation<Town>>();

builder.Services.AddScoped<IGenericServiceInterface<Overtime>,GenericServiceImplementation<Overtime>>();
builder.Services.AddScoped<IGenericServiceInterface<OvertimeType>,GenericServiceImplementation<OvertimeType>>();

builder.Services.AddScoped<IGenericServiceInterface<Vacation>,GenericServiceImplementation<Vacation>>();
builder.Services.AddScoped<IGenericServiceInterface<VacationType>,GenericServiceImplementation<VacationType>>();

builder.Services.AddScoped<IGenericServiceInterface<Sanction>,GenericServiceImplementation<Sanction>>();
builder.Services.AddScoped<IGenericServiceInterface<SanctionType>,GenericServiceImplementation<SanctionType>>();

builder.Services.AddScoped<IGenericServiceInterface<Doctor>,GenericServiceImplementation<Doctor>>();

builder.Services.AddScoped<IGenericServiceInterface<Employee>,GenericServiceImplementation<Employee>>();

builder.Services.AddScoped<AllState>();
builder.Services.AddScoped<UserProfileState>();

builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped<SfDialogService>();

// Register Syncfusion License Key here
SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NMaF1cXmhLYVF+WmFZfVtgcl9FYVZSQ2Y/P1ZhSXxWdkRhUH5WdHVVQGRZV0c=");

await builder.Build().RunAsync();