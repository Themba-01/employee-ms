namespace Client.ApplicationStates
{
    public class AllState
    {
        public Action? Action { get; set; }
        public bool ShowGeneralDepartment {get; set;}
        public void GeneralDepartmentClicked()
        {
            ResetAllDepartments();
            ShowGeneralDepartment = true;
            Action?.Invoke();
        }
        public bool ShowDepartment {get; set;}
        public void DepartmentClicked()
        {
            ResetAllDepartments();
            ShowDepartment = true;
            Action?.Invoke();
        }
        public bool ShowBranch {get; set;}
        public void BranchClicked()
        {
            ResetAllDepartments();
            ShowBranch = true;
            Action?.Invoke();
        }
        public bool ShowCountry {get; set;}
        public void CountryClicked()
        {
            ResetAllDepartments();
            ShowCountry = true;
            Action?.Invoke();
        }
        public bool ShowCity {get; set;}
        public void CityClicked()
        {
            ResetAllDepartments();
            ShowCity = true;
            Action?.Invoke();
        }
        public bool ShowTown {get; set;}
        public void TownClicked()
        {
            ResetAllDepartments();
            ShowTown = true;
            Action?.Invoke();
        }
        public bool ShowUser {get; set;}
        public void UserClicked()
        {
            ResetAllDepartments();
            ShowUser = true;
            Action?.Invoke();
        }
        public bool ShowHealth {get; set;}
        public void HealthClicked()
        {
            ResetAllDepartments();
            ShowHealth = true;
            Action?.Invoke();
        }
        public bool ShowOvertime {get; set;}
        public void OvertimeClicked()
        {
            ResetAllDepartments();
            ShowOvertime = true;
            Action?.Invoke();
        }
        public bool ShowOvertimeType {get; set;}
        public void OvertimeTypeClicked()
        {
            ResetAllDepartments();
            ShowOvertimeType = true;
            Action?.Invoke();
        }
        public bool ShowSanction {get; set;}
        public void SanctionClicked()
        {
            ResetAllDepartments();
            ShowSanction = true;
            Action?.Invoke();
        }
        public bool ShowSanctionType {get; set;}
        public void SanctionTypeClicked()
        {
            ResetAllDepartments();
            ShowSanctionType = true;
            Action?.Invoke();
        }
        public bool ShowVacation {get; set;}
        public void VacationClicked()
        {
            ResetAllDepartments();
            ShowVacation = true;
            Action?.Invoke();
        }
        public bool ShowVacationType {get; set;}
        public void VacationTypeClicked()
        {
            ResetAllDepartments();
            ShowVacationType = true;
            Action?.Invoke();
        }
        public bool ShowEmployee {get; set;} = true;
        public void EmployeeClicked()
        {
            ResetAllDepartments();
            ShowEmployee = true;
            Action?.Invoke();
        }
        public bool ShowUserProfile {get; set;}
        public void UserProfileClicked()
        {
            ResetAllDepartments();
            ShowUserProfile = true;
            Action?.Invoke();
        }

        private void ResetAllDepartments()
        {
            ShowGeneralDepartment = false;
            ShowDepartment = false;
            ShowBranch = false;
            ShowCountry = false;
            ShowCity = false;
            ShowTown = false;
            ShowUser = false;
            ShowEmployee = false;
            ShowHealth= false;
            ShowOvertime= false;
            ShowOvertimeType= false;
            ShowVacation= false;
            ShowVacationType= false;
            ShowSanction= false;
            ShowSanctionType = false;
            
        }
    }
}