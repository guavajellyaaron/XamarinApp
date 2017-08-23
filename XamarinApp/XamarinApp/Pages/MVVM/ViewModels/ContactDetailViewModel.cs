﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApp.Pages.MVVM.Models;

namespace XamarinApp.Pages.MVVM.ViewModels
{
    public class ContactDetailViewModel
    {
        private readonly IContactStore _contactStore;
        private readonly IPageService _pageService;

        public Contact Contact { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ContactDetailViewModel(ContactViewModel viewModel, IContactStore contactStore, IPageService pageService)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            _pageService = pageService;
            _contactStore = contactStore;

            SaveCommand = new Command(async () => await Save());

            // Here we are mapping our view model to a domain model. 
            // This is required because further below where we use ContactStore 
            // to add or update a contact, we need a domain model, not a view model.
            // Read my comment in the constructor of ContactViewModel.

            Contact = new Contact
            {
                Id = viewModel.Id,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Phone = viewModel.Phone,
                Email = viewModel.Email,
                IsBlocked = viewModel.IsBlocked,
            };
        }

        async Task Save()
        {
            if (String.IsNullOrWhiteSpace(Contact.FirstName) &&
                String.IsNullOrWhiteSpace(Contact.LastName))
            {
                await _pageService.DisplayAlert("Error", "Please enter the name.", "OK");
                return;
            }

            if (Contact.Id == 0)
            {
                await _contactStore.AddContact(Contact);

                MessagingCenter.Send(this, Events.ContactAdded, Contact);
            }
            else
            {
                await _contactStore.UpdateContact(Contact);

                MessagingCenter.Send(this, Events.ContactUpdated, Contact);
            }

            await _pageService.PopAsync();
        }
    }
}
