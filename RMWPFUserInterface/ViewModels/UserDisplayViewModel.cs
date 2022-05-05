using Caliburn.Micro;
using RMWPFUserInterface.Library.Api;
using RMWPFUserInterface.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMWPFUserInterface.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private BindingList<UserModel> _users;
        private readonly IUserEndpoint _userEndpoint;
        private readonly IWindowManager _windowManager;
        private readonly StatusInfoViewModel _statusInfo;
        private UserModel _selectedUser;
        private string _selectedUserName;
        private string _selectedRoleToRemove;
        private string _selectedRoleToAdd;
        private BindingList<string> _selectedUserRoles = new BindingList<string>();
        private BindingList<string> _availableRoles = new BindingList<string>();
        private Dictionary<string, string> _roles;

        public BindingList<string> SelectedUserRoles
        {
            get { return _selectedUserRoles; }
            set 
            { 
                _selectedUserRoles = value; 
                NotifyOfPropertyChange(() => SelectedUserRoles);
            }
        }

        public BindingList<string> AvailableRoles
        {
            get { return _availableRoles; }
            set
            {
                _availableRoles = value;
                NotifyOfPropertyChange(() => AvailableRoles);
            }
        }

        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set 
            { 
                _selectedUser = value;
                SelectedUserName = _selectedUser.Email;
                SelectedUserRoles.Clear();
                SelectedUserRoles = new BindingList<string>(_selectedUser.Roles.Select(role => role.Value).ToList());
                UpdateAvailableRoles();
                NotifyOfPropertyChange(() => SelectedUser);
            }
        }

        public string SelectedUserName
        {
            get { return _selectedUserName; }
            set
            {
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }

        public BindingList<UserModel> Users
        {
            get { return _users; }
            set 
            {
                _users = value; 
                NotifyOfPropertyChange(() => Users);
            }
        }

        public string SelectedRoleToRemove
        {
            get { return _selectedRoleToRemove; }
            set
            {
                _selectedRoleToRemove = value;
                NotifyOfPropertyChange(() => SelectedRoleToRemove);
            }
        }

        public string SelectedRoleToAdd
        {
            get { return _selectedRoleToAdd; }
            set
            {
                _selectedRoleToAdd = value;
                NotifyOfPropertyChange(() => SelectedRoleToAdd);
            }
        }

        public UserDisplayViewModel(IUserEndpoint userEnpoint, StatusInfoViewModel statusInfoViewModel, 
            IWindowManager windowManager)
        {
            _userEndpoint = userEnpoint;
            _statusInfo = statusInfoViewModel;
            _windowManager = windowManager;
        }

        protected override async void OnViewLoaded(object view)
        {
            try
            {
                await LoadUsers();
                await LoadRoles();
            }
            catch (Exception e)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (e.Message.Equals("Unauthorized"))
                {
                    _statusInfo.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Users Form.");
                    await _windowManager.ShowDialogAsync(_statusInfo, settings: settings);
                }
                else
                {
                    _statusInfo.UpdateMessage("Fatal Exception", e.Message);
                    await _windowManager.ShowDialogAsync(_statusInfo, settings: settings);
                }


                await TryCloseAsync();
            }
        }

        private async Task LoadUsers()
        {
            Users = new BindingList<UserModel>(await _userEndpoint.GetAll());
        }

        private async Task LoadRoles()
        {
            _roles = await _userEndpoint.GetAllRoles();
        }

        private void UpdateAvailableRoles()
        {
            AvailableRoles.Clear();
            foreach (var role in _roles)
            {
                if (SelectedUserRoles.IndexOf(role.Value) == -1)
                {
                    AvailableRoles.Add(role.Value);
                }
            }
        }

        public async void RemoveSelectedRole()
        {
            try
            {
                await _userEndpoint.UnassignRole(_selectedUser.Id, SelectedRoleToRemove);

                AvailableRoles.Add(SelectedRoleToRemove);
                SelectedUserRoles.Remove(SelectedRoleToRemove);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async void AddSelectedRole()
        {
            try
            {
                await _userEndpoint.AssignRole(_selectedUser.Id, SelectedRoleToAdd);

                SelectedUserRoles.Add(SelectedRoleToAdd);
                AvailableRoles.Remove(SelectedRoleToAdd);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
