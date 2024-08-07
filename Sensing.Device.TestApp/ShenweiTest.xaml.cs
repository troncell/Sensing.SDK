﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.AspNetCore.SignalR.Client;
using Sensing.Device.SDK.SensingHub;
using Sensing.Device.TestApp.Input;
using SensingHub.Sdk;

namespace Sensing.Device.TestApp;

public partial class ShenweiTest : Window
{
    private SensingHubClient _client = new SensingHubClient();
    private SensingHubClient _client1 = new SensingHubClient();
    private SensingHubClient _client2 = new SensingHubClient();
    private SensingHubClient _client3 = new SensingHubClient();
    private SensingHubClient _client4 = new SensingHubClient();
 
    public ShenweiTest()
    {
        InitializeComponent();
        
    }

    private async void connectButton_Click(object sender, RoutedEventArgs e)
    {
        await _client.PadConnect("http://localhost:8080");
        _client.OnMessageReceived<object>( message =>
        {
            this.Dispatcher.Invoke(() =>
            {
                receivedList.Items.Add(message.ToString());
            });
        });

        try
        {
            messagesList.Items.Add("Connection started");
            connectButton.IsEnabled = false;
            sendButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messagesList.Items.Add(ex.Message);
        }
    }
    
    private async void connectButton_Click1(object sender, RoutedEventArgs e)
    {
        await _client1.PadConnect("http://localhost:8080");
        _client1.OnMessageReceived<object>( message =>
        {
            this.Dispatcher.Invoke(() =>
            {
                receivedList1.Items.Add(message.ToString());
            });
        });

        try
        {
            // await connection1.StartAsync();
            messagesList1.Items.Add("Connection started");
            connectButton1.IsEnabled = false;
            sendButton1.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messagesList1.Items.Add(ex.Message);
        }
    }
    
    private async void connectButton_Click2(object sender, RoutedEventArgs e)
    {
        await _client2.DeviceConnect("http://120.26.218.8:8081");
        _client2.OnMessageReceived<object>( message =>
        {
            this.Dispatcher.Invoke(() =>
            {
                receivedList2.Items.Add(message.ToString());
            });
        });

        try
        {
            // await connection2.StartAsync();
            messagesList2.Items.Add("Connection started");
            connectButton2.IsEnabled = false;
            sendButton2.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messagesList2.Items.Add(ex.Message);
        }
    }
    
    private async void connectButton_Click3(object sender, RoutedEventArgs e)
    {
        await _client3.DeviceConnect($"http://localhost:8080");
        _client3.OnMessageReceived<object>( message =>
        {
            this.Dispatcher.Invoke(() =>
            {
                receivedList3.Items.Add(message.ToString());
            });
        });

        try
        {
            // await connection3.StartAsync();
            messagesList3.Items.Add("Connection started");
            connectButton3.IsEnabled = false;
            sendButton3.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messagesList3.Items.Add(ex.Message);
        }
    }

    private async void connectButton_Click4(object sender, RoutedEventArgs e)
    {
        await _client4.DeviceConnect("http://localhost:8080");

        _client4.OnMessageReceived<object>( message =>
        {
            this.Dispatcher.Invoke(() =>
            {
                receivedList4.Items.Add(message.ToString());
            });
        });
        try
        {
            // await connection4.StartAsync();
            messagesList4.Items.Add("Connection started");
            connectButton4.IsEnabled = false;
            sendButton4.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messagesList4.Items.Add(ex.Message);
        }
    }
    private async void loginButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _client.PadLogin( SubKeyTB.Text);
        }
        catch (Exception ex)
        {
            messagesList.Items.Add(ex.Message);
        }
    }
    
    private async void loginButton_Click1(object sender, RoutedEventArgs e)
    {
        try
        {
           
            await _client1.PadLogin(SubKeyTB1.Text);
        }
        catch (Exception ex)
        {
            messagesList1.Items.Add(ex.Message);
        }
    }
    
    
    private async void loginButton_Click2(object sender, RoutedEventArgs e)
    {
        try
        {

            await _client2.DeviceLogin(SDevice1.Text);
        }
        catch (Exception ex)
        {
            messagesList2.Items.Add(ex.Message);
        }
    }
    
    private async void loginButton_Click3(object sender, RoutedEventArgs e)
    {
        try
        {
            await _client3.DeviceLogin(SDevice2.Text);

        }
        catch (Exception ex)
        {
            messagesList3.Items.Add(ex.Message);
        }
    }
    
    private async void loginButton_Click4(object sender, RoutedEventArgs e)
    {
        try
        {
            await _client4.DeviceLogin(SDevice3.Text);

        }
        catch (Exception ex)
        {
            messagesList4.Items.Add(ex.Message);
        }
    }
    
    

    private void AreaID_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {

    }

    private async void lockButton_Click(object sender, RoutedEventArgs e)
    {
        await _client.LockArea(SubKeyTB.Text,long.Parse(LockAreaID.Text));
    }
    
    private async void lockButton_Click1(object sender, RoutedEventArgs e)
    {

        await _client1.LockArea(SubKeyTB1.Text,long.Parse(LockAreaID1.Text));

    }
    
    
    private async void releaseButton_Click(object sender, RoutedEventArgs e)
    {
 
        await _client.ReleaseAreaLock(SubKeyTB.Text,long.Parse(ReleaseAreaID.Text));

    }
    
    private async void releaseButton_Click1(object sender, RoutedEventArgs e)
    {
     
        await _client1.ReleaseAreaLock(SubKeyTB1.Text,long.Parse(ReleaseAreaID1.Text));

    }

    private async void areaControlButton_Click(object sender, RoutedEventArgs e)
    {
        await _client.ControlArea(SubKeyTB.Text,long.Parse(ControlAreaID.Text),long.Parse(AControlID.Text));

    }

    private async void deviceControlButton_Click(object sender, RoutedEventArgs e)
    {
 
        await _client.ControlDevice(ControlDeviceID.Text,long.Parse(DControlID.Text));

    }

    private async void areaControlButton_Click1(object sender, RoutedEventArgs e)
    {
  
        await _client1.ControlArea(SubKeyTB1.Text,long.Parse(ControlAreaID1.Text),long.Parse(AControlID1.Text));

    }

    private async void deviceControlButton_Click1(object sender, RoutedEventArgs e)
    {

        await _client1.ControlDevice(ControlDeviceID1.Text,long.Parse(DControlID1.Text));

    }

    private async void lockDButton_Click(object sender, RoutedEventArgs e)
    {

        await _client.DeviceLock(LockDeviceID.Text);
    }

    private async void releaseDButton_Click(object sender, RoutedEventArgs e)
    {
        await _client.ReleaseDeviceLock(LockDeviceID.Text);
    }

    private async void lockDButton_Click1(object sender, RoutedEventArgs e)
    {
        await _client.DeviceLock(LockDeviceID1.Text);
    }

    private async void releaseDButton_Click1(object sender, RoutedEventArgs e)
    {
        await _client.ReleaseDeviceLock(LockDeviceID1.Text);
    }

    private async void getDeviceButton_Click(object sender, RoutedEventArgs e)
    {
        DeviceLockInput input = new DeviceLockInput();
        input.Type = "DeviceStatus";
        // await connection.InvokeAsync("NotifyDataChanged",
            // input, 1);
    }


    private async void getDeviceTest_Click(object sender, RoutedEventArgs e)
    {
        var deviceControl = await _client.GetDeviceControl(null, "http://120.26.218.8:8081");
        var deviceControlDeviceModelDtos = deviceControl;
    }
}