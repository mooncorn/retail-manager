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

namespace RMDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        
        private readonly IUserEndpoint _userEndpoint;

        private readonly IWindowManager _windowManager;
        private readonly StatusInfoViewModel _statusInfo;

        private UserModel? _selectedUser;
        private string? _selectedUserName;
        private string? _selectedRoleToUnassign;
        private string? _selectedRoleToAssign;

        private BindingList<UserModel> _users;
        private Dictionary<string, string> _roles;

        private BindingList<string> _assignedRoles;
        private BindingList<string> _unassignedRoles;

        public BindingList<UserModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }

        public BindingList<string> AssignedRoles
        {
            get { return _assignedRoles; }
            set
            {
                _assignedRoles = value;
                NotifyOfPropertyChange(() => AssignedRoles);
            }
        }
        public BindingList<string> UnassignedRoles
        {
            get { return _unassignedRoles; }
            set
            {
                _unassignedRoles = value;
                NotifyOfPropertyChange(() => UnassignedRoles);
            }
        }

        public UserModel? SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;

                UpdateAssignedRoles();
                UpdateUnassignedRoles();
                NotifyOfPropertyChange(() => SelectedUser);
            }
        }
        public string? SelectedUserName
        {
            get { return _selectedUserName; }
            set
            {
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }
        public string? SelectedRoleToUnassign
        {
            get { return _selectedRoleToUnassign; }
            set
            {
                _selectedRoleToUnassign = value;
                NotifyOfPropertyChange(() => SelectedRoleToUnassign);
                NotifyOfPropertyChange(() => CanUnassignSelectedRole);
            }
        }
        public string? SelectedRoleToAssign
        {
            get { return _selectedRoleToAssign; }
            set
            {
                _selectedRoleToAssign = value;
                NotifyOfPropertyChange(() => SelectedRoleToAssign);
                NotifyOfPropertyChange(() => CanAssignSelectedRole);
            }
        }

        public bool CanAssignSelectedRole
        {
            get
            {
                return _selectedUser != null && _selectedRoleToAssign != null;
            }
        }

        public bool CanUnassignSelectedRole
        {
            get
            {
                return _selectedUser != null && _selectedRoleToUnassign != null;
            }
        }

        public UserDisplayViewModel(IUserEndpoint userEnpoint,
                                    StatusInfoViewModel statusInfoViewModel,
                                    IWindowManager windowManager)
        {
            _users = new BindingList<UserModel>();
            _roles = new Dictionary<string, string>();

            _assignedRoles = new BindingList<string>();
            _unassignedRoles = new BindingList<string>();

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

        /// <summary>
        /// Clear the list of unassigned roles and fill the binding list with roles that do not belong to currently selected user.
        /// This will compile a list of unassigned roles based on currently assigned roles. 
        /// </summary>
        private void UpdateUnassignedRoles()
        {
            UnassignedRoles.Clear();

            foreach (var role in _roles)
            {
                if (AssignedRoles.IndexOf(role.Value) == -1)
                {
                    UnassignedRoles.Add(role.Value);
                }
            }
        }

        /// <summary>
        /// Clear the list of assigned roles and override the binding list with roles that belong to currently selected user.
        /// </summary>
        private void UpdateAssignedRoles()
        {
            AssignedRoles.Clear();

            if (_selectedUser != null)
            {
                SelectedUserName = _selectedUser.Email;
                AssignedRoles = new BindingList<string>(_selectedUser.Roles.Select(role => role.Value).ToList());
            }
            else
            {
                SelectedUserName = string.Empty;
            }
        }

        /// <summary>
        /// Unassign role from selected user and update the binding lists
        /// </summary>
        public async void UnassignSelectedRole()
        {
            if (_selectedUser != null && _selectedRoleToUnassign != null)
            {
                await _userEndpoint.UnassignRole(_selectedUser.Id, SelectedRoleToUnassign);

                UnassignedRoles.Add(_selectedRoleToUnassign);

                // Update users list by removing _selectedRoleToUnssign from selected user
                var roleKeyToRemove = _roles.Where(keyValuePair => keyValuePair.Value == _selectedRoleToUnassign).FirstOrDefault().Key;
                _selectedUser.Roles.Remove(roleKeyToRemove);
                _users.ResetBindings();

                AssignedRoles.Remove(_selectedRoleToUnassign);
            }
        }

        /// <summary>
        /// Assign role to selected user and update the binding lists
        /// </summary>
        public async void AssignSelectedRole()
        {
            if (_selectedUser != null && _selectedRoleToAssign != null)
            {
                await _userEndpoint.AssignRole(_selectedUser.Id, SelectedRoleToAssign);

                AssignedRoles.Add(_selectedRoleToAssign);

                // Update users list by adding _selectedRoleToAssign to selected user
                var roleToAdd = _roles.Where(keyValuePair => keyValuePair.Value == _selectedRoleToAssign).FirstOrDefault();
                _selectedUser.Roles.Add(roleToAdd.Key, roleToAdd.Value);
                _users.ResetBindings();

                UnassignedRoles.Remove(_selectedRoleToAssign);
            }
        }
    }
}
