using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Device.Location;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Windows.Storage;
using System.IO.IsolatedStorage;
using Windows.Devices.Geolocation;

namespace Panic
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static string locationURL;
        private IsolatedStorageSettings settings;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        // Load data for the ViewModel Items
        // if the user has saved the data this method will retrieve and show it on text box
        protected override void OnNavigatedTo(NavigationEventArgs e)
            
        {                    
            if (settings.Contains("PhoneNumber") && !(string.IsNullOrEmpty(settings["PhoneNumber"].ToString())))
            {
                txtPhoneNumber.Text = settings["PhoneNumber"].ToString();
            }
        }
        // to send url to user in given specified range by clicking the button
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                locationURL = "http://maps.google.com?q={0}";
                string latandLong = "";
                //Genarte Geo Location

                //SetUp
                Geolocator locationFinder = new Geolocator
                {
                    DesiredAccuracyInMeters = 500,
                    DesiredAccuracy = PositionAccuracy.Default
                };

                // Try to Get The Postion 
                try
                {
                    Geoposition currentLocation = await locationFinder.GetGeopositionAsync(maximumAge: TimeSpan.FromSeconds(120), timeout: TimeSpan.FromSeconds(10));
                    latandLong = currentLocation.Coordinate.Latitude.ToString("0.00000") + "," + currentLocation.Coordinate.Longitude.ToString("0.00000");
                }
                catch
                {

                }
                SmsComposeTask smsComposeTask = new SmsComposeTask();
                smsComposeTask.To = settings["PhoneNumber"].ToString(); // Mention here the phone number to whom the sms is to be sent
                smsComposeTask.Body = string.Format(locationURL, latandLong); // the string containing the sms body
                smsComposeTask.Show(); // this will invoke the native sms edtior
            }
            catch
            {

            }
        }

        
        //to check whether entered phone number is valid or not
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex(@"^([0]|\+91)?[789]\d{9}$");
            if (!regex.IsMatch(txtPhoneNumber.Text))
            {
                MessageBox.Show("Enter a valid phone number", "Invalid phone number", MessageBoxButton.OK);
            }
            else
            {
                if (settings.Contains("PhoneNumber"))
                {
                    settings["PhoneNumber"] = txtPhoneNumber.Text;
                    settings.Save();
                }
                else
                {
                    settings.Add("PhoneNumber", txtPhoneNumber.Text);
                    settings.Save();
                }

                MessageBox.Show("Your location would be shared to this number", "Phone number saved", MessageBoxButton.OK);
            }
        }      
        // to load application page when a valid phone munebr is entered

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (settings.Contains("PhoneNumber") && string.IsNullOrEmpty(settings["PhoneNumber"].ToString()))
            {
                panApp.DefaultItem = panSettings;
            }
        }
    }
}